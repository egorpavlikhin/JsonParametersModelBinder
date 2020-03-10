using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace JsonBinder
{
    public class JsonBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

            var context = bindingContext.HttpContext;
            if (context.Request.ContentType != "application/json")
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }

#if (NETSTANDARD2_1 || NETCOREAPP3_0)
            context?.Request.EnableBuffering();
#else
            context?.Request.EnableRewind();
#endif

            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8,
                false,
                1024,
                true); // so body can be re-read next time

            var body = await reader.ReadToEndAsync();
            var json = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(body);
            if (json.TryGetValue(bindingContext.FieldName, out var value))
            {
                if (bindingContext.ModelType == typeof(string))
                {
                    bindingContext.Result = ModelBindingResult.Success(value.GetString());
                }
                else if (bindingContext.ModelType == typeof(short))
                {
                    bindingContext.Result = ModelBindingResult.Success(value.GetInt16());
                }
                else if (bindingContext.ModelType == typeof(int))
                {
                    bindingContext.Result = ModelBindingResult.Success(value.GetInt32());
                }
                else if (bindingContext.ModelType == typeof(long))
                {
                    bindingContext.Result = ModelBindingResult.Success(value.GetInt64());
                }
                else if (bindingContext.ModelType == typeof(bool))
                {
                    bindingContext.Result = ModelBindingResult.Success(value.GetBoolean());
                }
                else if (bindingContext.ModelType.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                {
                    bindingContext.Result = ModelBindingResult.Success(
                        ConvertList(DynamicJsonConverter.ReadList(value), bindingContext.ModelType, true));
                }
                else if (bindingContext.ModelType == typeof(object))
                {
                    var serializerOptions = new JsonSerializerOptions
                    {
                        Converters = {new DynamicJsonConverter()}
                    };
                    var val = JsonSerializer.Deserialize<dynamic>(value.ToString(), serializerOptions);
                    bindingContext.Result = ModelBindingResult.Success(val);
                }
                else
                {
                    bindingContext.Result = ModelBindingResult.Failed();
                }
            }

            context.Request.Body.Position = 0; // rewind
        }
        
        public static object ConvertList(IList<object> items, Type type, bool performConversion = false)
        {
            var containedType = type.GenericTypeArguments.First();
            var enumerableType = typeof(System.Linq.Enumerable);
            var castMethod = enumerableType.GetMethod(nameof(System.Linq.Enumerable.Cast)).MakeGenericMethod(containedType);
            var toListMethod = enumerableType.GetMethod(nameof(System.Linq.Enumerable.ToList)).MakeGenericMethod(containedType);

            IEnumerable<object> itemsToCast;

            if(performConversion)
            {
                itemsToCast = items.Select(item => Convert.ChangeType(item, containedType));
            }
            else 
            {
                itemsToCast = items;
            }

            var castedItems = castMethod.Invoke(null, new[] { itemsToCast });

            return toListMethod.Invoke(null, new[] { castedItems });
        }
    }
}