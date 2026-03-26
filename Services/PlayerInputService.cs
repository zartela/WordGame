using System;

namespace WordGame.Services
{
    public class PlayerInputService
    {
        private readonly TimerService timerService;
        private readonly LocalizationService localization;

        public PlayerInputService(TimerService timerService, LocalizationService localization)
        {
            this.timerService = timerService;
            this.localization = localization;
        }

        public string GetInputWithTimer(int timeoutMs)
        {
            localization.WritePrompt("EnterWord");

            bool timeExpired = timerService.WaitForInput(timeoutMs, out string input);

            if (timeExpired)
            {
                localization.WriteLine("TimeExpired");
                return "";
            }

            return input?.Trim() ?? "";
        }

        public string GetPlayerName(string playerNumber)
        {
            localization.WritePrompt("EnterPlayerName", playerNumber);
            string name = Console.ReadLine();
            return name?.Trim() ?? "";
        }

        public string GetBaseWord()
        {
            localization.WritePrompt("EnterBaseWord");
            string word = Console.ReadLine();
            return word?.Trim() ?? "";
        }

        public string GetLanguageChoice()
        {
            localization.WriteLine("ChooseLanguage");
            localization.WriteLine("Russian");
            localization.WriteLine("English");

            string choice = Console.ReadLine();
            return choice ?? "1";
        }
    }
}