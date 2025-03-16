using Microsoft.Extensions.Options;
using QuickTranslate.Configurations;
using QuickTranslate.Middlewares;
using QuickTranslate.Services.Business;
using QuickTranslate.Services.Validation;

var builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment.EnvironmentName;

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.Configure<TranslationAPI>(builder.Configuration.GetSection("translation"));
builder.Services.Configure<LanguageSupport>(builder.Configuration.GetSection("language"));
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<TranslationAPI>>().Value);
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<LanguageSupport>>().Value);
builder.Services.AddScoped<IValidationService, ValidationService>();
builder.Services.AddScoped<ITranslatorService, TranslatorService>();
builder.Services.AddHttpClient();


if (environment == "Development")
{
    builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);

}
else if (environment == "Production")
{
    builder.Configuration.AddJsonFile("appsettings.Production.json", optional: true, reloadOnChange: true);
}

builder.Services.AddControllers();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.MapControllers();

app.Run();
