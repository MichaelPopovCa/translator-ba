using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QuickTranslate.Configurations;
using QuickTranslate.Entities;
using QuickTranslate.Middlewares;
using QuickTranslate.Repositories.DBContext;
using QuickTranslate.Services.Business;
using QuickTranslate.Services.Validation;

var builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment.EnvironmentName;

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.Configure<TranslationAPI>(builder.Configuration.GetSection("translation"));
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<TranslationAPI>>().Value);
builder.Services.AddScoped<IValidationService, ValidationService>();
builder.Services.AddScoped<ITranslatorService, TranslatorService>();
builder.Services.AddHttpClient();

builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("QuickTranslateDB"));


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

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var english = new Language { LanguageCode = "en", LanguageName = "English" };
    var french = new Language { LanguageCode = "fr", LanguageName = "French" };
    context.Languages.AddRange(english, french);
    context.SaveChanges(); 

    var englishSupport = new LanguageSupport { ForeignKeyLanguageId = english.Id, Language = english };
    var frenchSupport = new LanguageSupport { ForeignKeyLanguageId = french.Id, Language = french };
    context.LanguageSupports.AddRange(englishSupport, frenchSupport);
    context.SaveChanges(); 

    var languages = context.Languages.ToList();
    var languageSupports = context.LanguageSupports.ToList();

    Console.WriteLine("Languages:");
    foreach (var language in languages)
    {
        Console.WriteLine($"Code: {language.LanguageCode}, Name: {language.LanguageName}");
    }

    Console.WriteLine("Language Supports:");
    foreach (var support in languageSupports)
    {
        Console.WriteLine($"ForeignKey: {support.ForeignKeyLanguageId}, Language: {support.Language.LanguageName}");
    }
}

app.Run();


