# **Mist**

Mist é um projeto que tem o objetivo de servir exemplos de como aplicar certas funcionalidades e frameworks a um projeto .NET

### Comandos .NET

###### Solution
```
dotnet new sln -o <folder> -n <name>
```

###### Package
```
dotnet new classlib -o <folder> -n <name> -f <framework>
```

###### Api
```
dotnet new webapi -o <folder> -n <name>
```

### Injeção de Dependência

Para não acumular todas as configurações no Startup.cs, podemos criar uma classe que aplicará as configurações para organizar melhor.

###### ApiConfig.cs

```
public static class ApiConfig
{
    public static IServiceCollection WebApiConfig(this IServiceCollection services)
    {
        // Aqui você aplica as configurações ao services

        return services;
    }

    public static IApplicationBuilder UseMvcConfiguration(this IApplicationBuilder app)
    {
        // Aqui você aplica as configurações ao app 

        return app;
    }
}
```

###### Startup.cs

```
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.WebApiConfig(); // Aqui você chama o método da classe ApiConfig
    }
    
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
    {
        app.UseMvcConfiguration(); // Aqui você chama o método da classe ApiConfig
    }
}
```

### Versionamento da API

Para utilizar versionamento na API, é necessário aplicar algumas configurações. Criaremos uma classe ApiConfig para aplicar as configurações.

###### Packages

```
Microsoft.AspNetCore.Mvc.Versioning
Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer
```

###### ApiConfig.cs

```
public static class ApiConfig
{
    public static IServiceCollection WebApiConfig(this IServiceCollection services)
    {
        services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);

        services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ReportApiVersions = true;
        });

        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        return services;
    }

    public static IApplicationBuilder UseMvcConfiguration(this IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        return app;
    }
}
```

###### Startup.cs

```
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.WebApiConfig();
    }
    
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
    {
        app.UseMvcConfiguration();
    }
}
```
