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
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost5173",
        builder => builder.WithOrigins("http://localhost:5173")
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

var app = builder.Build();

app.UseCors("AllowLocalhost5173");

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var english = new Language { LanguageCode = "en", LanguageName = "English", Enabled = true };
    var french = new Language { LanguageCode = "fr", LanguageName = "French", Enabled = true };
    var spanish = new Language { LanguageCode = "es", LanguageName = "Spanish" };
    var german = new Language { LanguageCode = "de", LanguageName = "German" };
    var italian = new Language { LanguageCode = "it", LanguageName = "Italian" };
    var portuguese = new Language { LanguageCode = "pt", LanguageName = "Portuguese" };
    var dutch = new Language { LanguageCode = "nl", LanguageName = "Dutch" };
    var russian = new Language { LanguageCode = "ru", LanguageName = "Russian" };
    var chinese = new Language { LanguageCode = "zh", LanguageName = "Chinese" };
    var japanese = new Language { LanguageCode = "ja", LanguageName = "Japanese" };

    context.Languages.AddRange(english, french, spanish, german, italian, portuguese, dutch, russian,
                                chinese, japanese);

    context.SaveChanges();

    var languages = context.Languages.ToList();

    Console.WriteLine("Languages:");
    foreach (var language in languages)
    {
        Console.WriteLine($"Code: {language.LanguageCode}, Name: {language.LanguageName}");
    }
}

app.Run();


