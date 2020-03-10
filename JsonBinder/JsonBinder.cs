using System;
using System.Collections.Generic;
using System.IO;
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
                else if (bindingContext.ModelType == typeof(object))
                {
                    var serializerOptions = new JsonSerializerOptions
                    {
                        Converters = {new DynamicJsonConverter()}
                    };
                    var val = JsonSerializer.Deserialize<dynamic>(value.ToString(), serializerOptions);
                    bindingContext.Result = ModelBindingResult.Success(val);
                }
            }

            context.Request.Body.Position = 0; // rewind
        }
    }
}