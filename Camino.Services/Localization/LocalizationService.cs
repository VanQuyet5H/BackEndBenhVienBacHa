using System.Linq;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.Localization;
using Camino.Data;
using Camino.Services.Helpers;

namespace Camino.Services.Localization
{
    [ScopedDependency(ServiceType = typeof(ILocalizationService))]
    public class LocalizationService : ILocalizationService
    {
        private readonly IRepository<LocaleStringResource> _lsrRepository;
        public LocalizationService(IRepository<LocaleStringResource> lsrRepository, IUserAgentHelper userAgentHelper)
        {
            _lsrRepository = lsrRepository;
            CurrentUserLanguage = userAgentHelper.GetUserLanguage();
        }

        public Enums.LanguageType? CurrentUserLanguage { get; set; }

        public string GetResource(string resourceKey)
        {
            return GetResource(resourceKey, (int) (CurrentUserLanguage ?? Enums.LanguageType.VietNam));
        }

     
        private string GetResource(string resourceKey, int languageId, string defaultValue = "")
        {
            var result = string.Empty;
            if (resourceKey == null)
                resourceKey = string.Empty;
            resourceKey = resourceKey.Trim().ToLowerInvariant();

             var lsr = _lsrRepository.TableNoTracking.FirstOrDefault(o => o.ResourceName == resourceKey && o.Language == languageId);
            if (lsr != null)
            {
                result = lsr.ResourceValue;
            }
            if (!string.IsNullOrEmpty(result))
                return result;
            if (!string.IsNullOrEmpty(defaultValue))
            {
                return defaultValue;
            }
            return resourceKey;
        }
    }
}
