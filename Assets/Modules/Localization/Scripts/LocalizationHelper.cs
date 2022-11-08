using System.Collections.Generic;
using UnityEngine;

namespace Modules.Localization
{

    public class LanguageHelper
    {

        /// <summary>
        /// Helps to convert Unity's Application.systemLanguage to a 
        /// 2 letter ISO language code. There is unfortunately not more
        /// countries available as Unity's enum does not enclose all
        /// countries.
        /// </summary>
        /// <returns>The 2-letter ISO code from system language.</returns>
        public static string GetLanguageCode(SystemLanguage name)
        {
            string res = "en";
            switch (name)
            {
                case SystemLanguage.Afrikaans: res = "af"; break;
                case SystemLanguage.Arabic: res = "ar"; break;
                case SystemLanguage.Basque: res = "eu"; break;
                case SystemLanguage.Belarusian: res = "by"; break;
                case SystemLanguage.Bulgarian: res = "bg"; break;
                case SystemLanguage.Catalan: res = "ca"; break;
                case SystemLanguage.Chinese: res = "zh"; break;
                case SystemLanguage.Czech: res = "cs"; break;
                case SystemLanguage.Danish: res = "da"; break;
                case SystemLanguage.Dutch: res = "nl"; break;
                case SystemLanguage.English: res = "en"; break;
                case SystemLanguage.Estonian: res = "et"; break;
                case SystemLanguage.Faroese: res = "fo"; break;
                case SystemLanguage.Finnish: res = "fi"; break;
                case SystemLanguage.French: res = "fr"; break;
                case SystemLanguage.German: res = "de"; break;
                case SystemLanguage.Greek: res = "el"; break;
                case SystemLanguage.Hebrew: res = "iw"; break;
                case SystemLanguage.Hungarian: res = "hu"; break;
                case SystemLanguage.Icelandic: res = "is"; break;
                case SystemLanguage.Indonesian: res = "in"; break;
                case SystemLanguage.Italian: res = "it"; break;
                case SystemLanguage.Japanese: res = "ja"; break;
                case SystemLanguage.Korean: res = "ko"; break;
                case SystemLanguage.Latvian: res = "lv"; break;
                case SystemLanguage.Lithuanian: res = "lt"; break;
                case SystemLanguage.Norwegian: res = "no"; break;
                case SystemLanguage.Polish: res = "pl"; break;
                case SystemLanguage.Portuguese: res = "pt"; break;
                case SystemLanguage.Romanian: res = "ro"; break;
                case SystemLanguage.Russian: res = "ru"; break;
                case SystemLanguage.SerboCroatian: res = "sh"; break;
                case SystemLanguage.Slovak: res = "sk"; break;
                case SystemLanguage.Slovenian: res = "sl"; break;
                case SystemLanguage.Spanish: res = "es"; break;
                case SystemLanguage.Swedish: res = "sv"; break;
                case SystemLanguage.Thai: res = "th"; break;
                case SystemLanguage.Turkish: res = "tr"; break;
                case SystemLanguage.Ukrainian: res = "uk"; break;
                case SystemLanguage.Unknown: res = "en"; break;
                case SystemLanguage.Vietnamese: res = "vi"; break;
            }
            return res;
        }

        /// <summary>
        /// Return internal Id for language
        /// </summary>
        /// <param name="languageName">SystemLanguage</param>
        /// <returns>The Id</returns>
        public static int GetLanguageId(SystemLanguage languageName, string countryCode = null)
        {
            int res = 0;
            switch (GetLocale(languageName, countryCode))
            {
                case "fr": res = 1; break;
                case "en": case "en-UK": res = 2; break;
                case "es": res = 3; break;
                case "de": res = 6; break;
                case "nl": res = 7; break;
                case "en-US": res = 8; break;
                case "en-AU": res = 9; break;
                case "en-CA": res = 10; break;
                case "fr-CA": res = 11; break;
            }
            return res;
        }

        public static string GetLocale(SystemLanguage languageName, string countryCode = null)
        {
            return GetLocale(GetLanguageCode(languageName), countryCode);
        }

        public static string GetLocale(string languageCode, string countryCode = null)
        {
            if (string.IsNullOrWhiteSpace(countryCode) || countryCode == "All") return languageCode;
            return string.Format("{0}-{1}", languageCode, countryCode);
        }

        public static string[] GetCountryCodes(SystemLanguage languageName)
        {
            string languageCode = GetLanguageCode(languageName);
            List<string> countries = new List<string>();
            if (LanguageCountries.ContainsKey(languageCode)) countries.AddRange(LanguageCountries[languageCode]);
            countries.Add("All");
            return countries.ToArray();
        }

        public static Dictionary<string, string[]> LanguageCountries = new Dictionary<string, string[]>()
        {
            {"af", new[] {"ZA"} },
            {"ar", new[] {"AE", "BH", "DZ", "EG", "IQ", "JO", "KW", "LB", "LY", "MA", "OM", "QA", "SA", "SY", "TN", "YE"} },
            {"as", new[] {"IN"} },
            {"az", new[] {"AZ"} },
            {"bg", new[] {"BG"} },
            {"ca", new[] {"ES"} },
            {"cs", new[] {"CZ"} },
            {"da", new[] {"DK"} },
            {"de", new[] {"AT", "CH", "DE", "LI", "LU"} },
            {"el", new[] {"GR"} },
            {"en", new[] {"GB", "AU", "BZ", "CA", "IE", "IN", "JM", "MY", "NZ", "PH", "SG", "TT", "US", "ZA", "ZW"} },
            {"es", new[] {"AR", "BO", "CL", "CO", "CR", "DO", "EC", "ES", "GT", "HN", "MX", "NI", "PA", "PE", "PR", "PY", "SV", "US", "UY", "VE"} },
            {"et", new[] {"EE"} },
            {"eu", new[] {"ES"} },
            {"fi", new[] {"FI"} },
            {"fo", new[] {"FO"} },
            {"fr", new[] {"FR", "BE", "CA", "CH", "LU", "MC"} },
            {"fy", new[] {"NL"} },
            {"hu", new[] {"HU"} },
            {"in", new[] {"ID"} },
            {"is", new[] {"IS"} },
            {"it", new[] {"IT", "CH"} },
            {"iw", new[] {"IL"} },
            {"ja", new[] {"JP"} },
            {"ko", new[] {"KR"} },
            {"lt", new[] {"LT"} },
            {"lv", new[] {"LV"} },
            {"no", new[] {"NO"} },
            {"pl", new[] {"PL"} },
            {"pt", new[] {"PT", "BR"} },
            {"ro", new[] {"RO"} },
            {"ru", new[] {"RU"} },
            {"sh", new[] {"CS", "HR"} },
            {"sk", new[] {"SK"} },
            {"sl", new[] {"SI"} },
            {"sv", new[] {"FI", "SE"} },
            {"th", new[] {"TH"} },
            {"tr", new[] {"TR"} },
            {"uk", new[] {"UA"} },
            {"vi", new[] {"VN"} },
            {"zh", new[] {"ZA", "CN", "HK", "MO", "SG", "TW"} }
        };
    }
}