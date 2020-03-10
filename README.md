[![NuGet](https://img.shields.io/nuget/v/JsonParametersModelBinder)](https://www.nuget.org/packages/JsonParametersModelBinder)
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
Nested objects are supported as dynamic types (with limitations).

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
Enhoy simplified workflow.

## Thanks
Thanks to [tchivs](https://github.com/tchivs) for providing [code to map to dynamic types](https://github.com/dotnet/runtime/issues/29690#issuecomment-571969037)
