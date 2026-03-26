using System;

namespace WordGame.Models
{
    public class PlayerStats
    {
        public string Name { get; set; } = "";
        public int Wins { get; set; }
        public int Losses { get; set; }

        public int TotalGames => Wins + Losses;

        public double WinRate
        {
            get
            {
                if (TotalGames == 0) return 0;
                return (double)Wins / TotalGames * 100;
            }
        }
    }
}