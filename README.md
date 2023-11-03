# HttpContextCatcher

The `HttpContextCatcher` is an ASP.NET Core middleware that provides an easy and flexible way to intercept and work with the HTTP context asynchronously. It is designed to integrate seamlessly with the ASP.NET Core pipeline, offering a straightforward approach to access request and response data for logging, analysis, or any custom processing needs.



## Features

- **Asynchronous Context Processing**: Handle your HTTP context without affecting the request-response lifecycle.
- **Dependency Injection Friendly**: Works with ASP.NET Core's built-in dependency injection. Inject any registered service into your custom catcher services.
- **Configurable**: Choose to ignore certain requests or responses based on custom logic.
- **Easy Integration**: Minimal setup required to add to your existing ASP.NET Core applications.
- **Customizable Catcher Services**: Implement the `IAsyncCatcherService` interface to define your own logic for what happens when the HTTP context is captured.



## Getting Started

To use `HttpContextCatcher` in your project, follow these simple steps:



### Installation

```sh
dotnet add package HttpContextCatcher
```



### Configuration

In your `Program.cs`, register `HttpContextCatcher` services:

```
var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddHttpContextCatcher(options =>
{
    options.SetCatcher<MyCatcherService>();
    // options.IgnoreRequest(); (Optional)
    // options.IgnoreResponse(); (Optional)
});

builder.Services.AddSingleton<DatabaseAccessor>(); // If your catcher requires it

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseHttpContextCatcher(); // Use the HttpContextCatcher middleware

// Other middleware configurations
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

Implement your custom catcher service by inheriting from `IAsyncCatcherService`:

```
using HttpContextCatcher.CatcherManager;

// In your custom catcher service, you can inject any service that's registered with the DI container
public class MyCatcherService : IAsyncCatcherService
{
    private readonly DatabaseAccessor _databaseAccessor;

    public MyCatcherService(DatabaseAccessor databaseAccessor)
    {
        _databaseAccessor = databaseAccessor;
    }

    public async Task OnCatchAsync(ContextCatcher contextCatcher)
    {
        // Your logic to handle the caught context
        _databaseAccessor.Write(contextCatcher);
    }
}
```



## Support or Contact

Having trouble with `HttpContextCatcher`? Please email us at [r05221017@gmail.com](mailto:r05221017@gmail.com) and we'll help you sort it out.

## FAQ

Q: How do I ignore specific requests? A: Use the `IgnoreRequest` method in the options while configuring the service to specify the conditions under which a request should be ignored.

Q: Is it possible to use `HttpContextCatcher` for response logging? A: Absolutely! Implement the `OnCatchAsync` method in your catcher service to process the response context as required.

