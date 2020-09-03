# **Mist**

Mist é um projeto que tem o objetivo de servir exemplos de como aplicar certas funcionalidades e frameworks a um projeto .NET

### Estrutura do Projeto

```
├───containers // Projetos WebApi
│   └───api-auth
│       └───src
│           └───Auth.Api.csproj
├───dotnet-packages
│   ├───src // Packages de biblioteca de classes
│   └───test // Packages de teste unitário
└───dotnet-solutions // Solutions
    └───Auth.sln
```

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

Com as configurações feitas, podemos aplicar o versionamento nas Controllers, da seguinte forma.

###### XptoController.cs

```
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class XptoController : ControllerBase
{
    [HttpGet()]
    public ActionResult Get()
    {
        return Ok();
    }
}
```

Seu endpoint será "https://host:porta/api/v1/xpto".

### AppSettings para cada ambiente

Podemos criar um arquivo appsettings.json para cada ambiente, no caso teriamos um arquivo appsettings.json geral e outro arquivo appsettings.Development.json para o ambiente de desenvolvimento. As configurações de mesma chave do appsettings.Development.json vão sobrescrever as da appsettings.json.

###### appsettings.json

```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Debug",
      "Microsoft.Hosting.Lifetime": "Debug"
    }
  }
}
```

###### appsettings.Development.json

```
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  },
  "AllowedHosts": "*"
}
```

###### Startup.cs

```
public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IHostEnvironment hostEnvironment)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(hostEnvironment.ContentRootPath)
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{hostEnvironment.EnvironmentName}.json", true, true)
            .AddEnvironmentVariables();

        Configuration = builder.Build();
    }
}
```

Neste caso quando obtermos as configurações do IConfiguration, teremos o seguinte.

```
Logging:LogLevel:Default = "Debug"
Logging:LogLevel:Microsoft = "Debug"
Logging:LogLevel:Microsoft.Hosting.Lifetime = "Debug"
AllowedHosts = "*"
```

Podemos ver que a key "Logging:LogLevel:Default" foi sobrescrita, a key "AllowedHost" foi criada e o restante das keys foram mantidas como estavam no appsettings.json.

### Health Check

Health Check é um endpoint com um Get simples para checar se a aplicação está em pé. Basta criar uma controller com um método Get que retorna Ok().

###### HealthCheckController.cs

```
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}")]
public class HealthCheckController : ControllerBase
{
    [HttpGet()]
    public ActionResult Get()
    {
        return Ok();
    }
}
```

Pode chamar por "https://host:porta/api/v1", se a resposta por 200, significa que a aplicação está em pé.
