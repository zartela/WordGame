using System.Collections.Generic;

namespace WordGame.Models
{
    public class GameSession
    {
        public string BaseWord { get; set; } = "";
        public List<string> AcceptedWords { get; set; } = new List<string>();
        public List<string> AllAttempts { get; set; } = new List<string>();
        public string Player1Name { get; set; } = "Player1";
        public string Player2Name { get; set; } = "Player2";
        public int CurrentPlayer { get; set; } = 1;
        public bool IsGameOver { get; set; }

        public string GetCurrentPlayerName()
        {
            return CurrentPlayer == 1 ? Player1Name : Player2Name;
        }

        public string GetOtherPlayerName()
        {
            return CurrentPlayer == 1 ? Player2Name : Player1Name;
        }

        public void SwitchPlayer()
        {
            CurrentPlayer = CurrentPlayer == 1 ? 2 : 1;
        }

        public void AddAttempt(string word, bool accepted)
        {
            AllAttempts.Add(word);
            if (accepted)
            {
                AcceptedWords.Add(word);
            }
        }

        public bool IsWordValid(string word)
        {
            List<char> availableLetters = new List<char>();
            for (int i = 0; i < BaseWord.Length; i++)
            {
                availableLetters.Add(BaseWord[i]);
            }

            for (int i = 0; i < word.Length; i++)
            {
                char letter = word[i];
                bool letterFound = false;

                for (int j = 0; j < availableLetters.Count; j++)
                {
                    if (availableLetters[j] == letter)
                    {
                        availableLetters.RemoveAt(j);
                        letterFound = true;
                        break;
                    }
                }

                if (!letterFound)
                {
                    return false;
                }
            }

            return true;
        }
    }
}