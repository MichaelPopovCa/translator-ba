using Microsoft.EntityFrameworkCore;
using QuickTranslate.Entities;
using QuickTranslate.Enums;
using QuickTranslate.Exceptions;
using QuickTranslate.Models.Response;
using QuickTranslate.Repositories.DBContext;

namespace QuickTranslate.Repositories.LanguageRepository
{
    public class LanguageRepository : ILanguageRepository
    {
        private readonly AppDbContext _appDbContext;

        public LanguageRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<IEnumerable<LanguageResponse>> GetAllAppLanguagesAsync()
        {
            return await _appDbContext.Languages
                            .Select(l => new LanguageResponse
                            {
                                LanguageCode = l.LanguageCode,
                                LanguageName = l.LanguageName,
                                Enabled = l.Enabled,
                            })
                            .ToListAsync();
        }

        public async Task FindLanguageByLanguageCodeAndUpdate(string languageCode, bool enable)
        {
            var existingLanguage = await _appDbContext.Languages
              .FirstOrDefaultAsync(l => l.LanguageCode == languageCode);

            if (existingLanguage == null)
            {
                throw new InvalidLanguageException($"The language with code {languageCode} is not available", TranslationErrorCode.InvalidLanguageException);
            }

            existingLanguage.Enabled = enable;

            await _appDbContext.SaveChangesAsync();
        }
    }
}
