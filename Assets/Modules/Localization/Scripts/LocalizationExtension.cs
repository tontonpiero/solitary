using System.Linq;

namespace Modules.Localization
{
    static class LocalizationExtension
    {
        static public string Localize(this string key, params object[] parameters)
        {
            return LocalizationManager.Instance.Localize(key, parameters);
        }

        static public string LocalizeOrDefault(this string key, string defaultValue, object[] parameters = null)
        {
            if (!LocalizationManager.Instance.HasLocalization(key))
            {
                return defaultValue;
            }
            string result = LocalizationManager.Instance.Localize(key, parameters);
            if (string.IsNullOrWhiteSpace(result))
                return defaultValue;
            return result;
        }

        public static string FirstCharToUpper(this string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            return input.First().ToString().ToUpper() + input.Substring(1);
        }
    }
}
