[![NuGet](https://img.shields.io/nuget/v/JsonParametersModelBinder)](https://www.nuget.org/packages/JsonParametersModelBinder/1.0.0)
# JsonParametersModelBinder
Allows you to map JSON object directly to controller action parameters.
Convert JSON model 
```
{ "a": "a", "b": { "c": "value" } }
``` 
directly to action method
```C#
public ... Method(string a, dynamic b)
```

## Warning ##
Only string and dynamic types (with limitations) are supported for now.

## Usage
**Step 1.**
Add attribute `JsonParameters` to your action
```C#
[HttpPost("two")]
[JsonParameters]
public async Task<IActionResult> TwoParameters(string a, dynamic b)
{
    Console.WriteLine(b.c);
    return Ok();
}
```

**Step 2.**
Add `JsonBinderProvider` to your `ModelBinderProviders`
```C#
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();

    services.AddControllers(options =>
    {
        options.ModelBinderProviders.Insert(0, new JsonBinderProvider());
    });
}
```

**Step 3.**
Enhoy simplified workflow.

## Thanks
Thanks to [tchivs](https://github.com/tchivs) for providing [code to map to dynamic types](https://github.com/dotnet/runtime/issues/29690#issuecomment-571969037)
