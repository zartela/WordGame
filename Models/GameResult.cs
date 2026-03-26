using System;
using System.Collections.Generic;

namespace WordGame.Models
{
    public class GameResult
    {
        public DateTime Date { get; set; }
        public string Winner { get; set; } = "";
        public string Loser { get; set; } = "";
        public string BaseWord { get; set; } = "";
        public List<string> AcceptedWords { get; set; } = new List<string>();
        public List<string> AllAttempts { get; set; } = new List<string>();
        public int TotalTurns { get; set; }

        public GameResult()
        {
            Date = DateTime.Now;
        }

        public GameResult(string winner, string loser, string baseWord, List<string> acceptedWords, List<string> allAttempts)
        {
            Date = DateTime.Now;
            Winner = winner;
            Loser = loser;
            BaseWord = baseWord;
            AcceptedWords = new List<string>(acceptedWords);
            AllAttempts = new List<string>(allAttempts);
            TotalTurns = allAttempts.Count;
        }

        public string GetSummary()
        {
            return $"{Date:yyyy-MM-dd HH:mm} | {Winner} vs {Loser} | {BaseWord} | {TotalTurns} ходов";
        }
    }
}