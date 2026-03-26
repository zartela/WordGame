using System;
using System.Globalization;
using WordGame.Properties;

namespace WordGame.Services
{
    public class LocalizationService
    {
        private string currentLanguage = "ru";

        public void SetLanguage(string language)
        {
            currentLanguage = language == "en" ? "en" : "ru";
        }

        public string GetCurrentLanguage()
        {
            return currentLanguage;
        }

        public string GetString(string key, params object[] args)
        {
            CultureInfo culture = currentLanguage == "en" ? new CultureInfo("en-US") : new CultureInfo("ru-RU");
            string resourceString = Resources.ResourceManager.GetString(key, culture);

            if (resourceString == null)
            {
                return key;
            }

            if (args.Length > 0)
            {
                return string.Format(resourceString, args);
            }

            return resourceString;
        }

        public void WriteLine(string key, params object[] args)
        {
            Console.WriteLine(GetString(key, args));
        }

        public void Write(string key, params object[] args)
        {
            Console.Write(GetString(key, args));
        }

        public void WritePrompt(string key, params object[] args)
        {
            Console.Write(GetString(key, args) + " ");
        }
    }
}