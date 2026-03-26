using WordGame.Models;

namespace WordGame.Services
{
    public class GameValidator
    {
        private readonly LocalizationService localization;

        public GameValidator(LocalizationService localization)
        {
            this.localization = localization;
        }

        public bool ValidateBaseWord(string word, out string error)
        {
            error = "";

            if (word.Length < 8 || word.Length > 30)
            {
                error = localization.GetString("WordLengthError");
                return false;
            }

            for (int i = 0; i < word.Length; i++)
            {
                if (!char.IsLetter(word[i]))
                {
                    error = localization.GetString("LettersOnlyError");
                    return false;
                }
            }

            return true;
        }

        public bool ValidatePlayerWord(string word, GameSession session, out string error)
        {
            error = "";

            if (string.IsNullOrEmpty(word))
            {
                error = localization.GetString("EmptyWordError");
                return false;
            }

            word = word.ToLower().Trim();

            if (session.AcceptedWords.Contains(word))
            {
                error = localization.GetString("WordUsed", word);
                return false;
            }

            if (!session.IsWordValid(word))
            {
                error = localization.GetString("InvalidLetters", word);
                return false;
            }

            return true;
        }

        public bool ValidatePlayerName(string name, out string error)
        {
            error = "";

            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            if (name.Length > 20)
            {
                error = localization.GetString("NameTooLong");
                return false;
            }

            return true;
        }
    }
}