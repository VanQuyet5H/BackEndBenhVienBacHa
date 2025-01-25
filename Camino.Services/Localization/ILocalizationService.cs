using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain;

namespace Camino.Services.Localization
{
    public interface ILocalizationService
    {
        string GetResource(string resourceKey);
        Enums.LanguageType? CurrentUserLanguage { get; set; }

      
    }
}
