using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using WordGame.Models;

namespace WordGame.Services
{
    public class StatisticsService
    {
        private const string STATS_FILE = "game_stats.json";
        private const string HISTORY_FILE = "game_history.json";

        private List<PlayerStats> players = new List<PlayerStats>();
        private List<GameResult> gameHistory = new List<GameResult>();
        private readonly LocalizationService localization;

        public StatisticsService(LocalizationService localization)
        {
            this.localization = localization;
            Load();
        }

        public void Load()
        {
            try
            {
                if (File.Exists(STATS_FILE))
                {
                    string json = File.ReadAllText(STATS_FILE);
                    players = JsonSerializer.Deserialize<List<PlayerStats>>(json) ?? new List<PlayerStats>();
                }

                if (File.Exists(HISTORY_FILE))
                {
                    string json = File.ReadAllText(HISTORY_FILE);
                    gameHistory = JsonSerializer.Deserialize<List<GameResult>>(json) ?? new List<GameResult>();
                }
            }
            catch
            {
                players = new List<PlayerStats>();
                gameHistory = new List<GameResult>();
            }
        }

        public void Save()
        {
            try
            {
                string statsJson = JsonSerializer.Serialize(players, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(STATS_FILE, statsJson);

                string historyJson = JsonSerializer.Serialize(gameHistory, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(HISTORY_FILE, historyJson);
            }
            catch
            {
            }
        }

        public void RecordGameResult(GameSession session, string winner, string loser)
        {
            var winnerStats = GetOrCreatePlayer(winner);
            winnerStats.Wins++;

            var loserStats = GetOrCreatePlayer(loser);
            loserStats.Losses++;

            var gameResult = new GameResult(winner, loser, session.BaseWord, session.AcceptedWords, session.AllAttempts);
            gameHistory.Add(gameResult);

            Save();
        }

        public void RecordGameResult(string winner, string loser)
        {
            var winnerStats = GetOrCreatePlayer(winner);
            winnerStats.Wins++;

            var loserStats = GetOrCreatePlayer(loser);
            loserStats.Losses++;

            Save();
        }

        private PlayerStats GetOrCreatePlayer(string name)
        {
            var player = players.FirstOrDefault(p => p.Name == name);
            if (player == null)
            {
                player = new PlayerStats { Name = name };
                players.Add(player);
            }
            return player;
        }

        public PlayerStats GetPlayerStats(string name)
        {
            return players.FirstOrDefault(p => p.Name == name);
        }

        public List<PlayerStats> GetAllStats()
        {
            return players.OrderByDescending(p => p.Wins).ToList();
        }

        public List<GameResult> GetGameHistory()
        {
            return gameHistory.OrderByDescending(g => g.Date).ToList();
        }

        public List<GameResult> GetPlayerGameHistory(string playerName)
        {
            return gameHistory.Where(g => g.Winner == playerName || g.Loser == playerName).OrderByDescending(g => g.Date).ToList();
        }

        public void ShowPlayerScore(string playerName)
        {
            var stats = GetPlayerStats(playerName);

            if (stats != null)
            {
                Console.WriteLine(localization.GetString("StatisticsFor") + " " + playerName);
                Console.WriteLine(localization.GetString("Wins") + ": " + stats.Wins);
                Console.WriteLine(localization.GetString("Losses") + ": " + stats.Losses);
                Console.WriteLine(localization.GetString("TotalGames") + ": " + stats.TotalGames);

                if (stats.TotalGames > 0)
                {
                    Console.WriteLine(localization.GetString("WinRate") + ": " + stats.WinRate.ToString("F1") + "%");
                }

                var playerGames = GetPlayerGameHistory(playerName);
                if (playerGames.Count > 0)
                {
                    Console.WriteLine();
                    Console.WriteLine(localization.GetString("RecentGames"));
                    for (int i = 0; i < Math.Min(3, playerGames.Count); i++)
                    {
                        var game = playerGames[i];
                        string result = game.Winner == playerName ? localization.GetString("Accepted") : localization.GetString("Rejected");
                        Console.WriteLine($"  {result} {game.Date:dd.MM.yyyy} - {game.BaseWord}");
                    }
                }
            }
            else
            {
                Console.WriteLine(localization.GetString("NoStatisticsFor") + " " + playerName);
            }
        }

        public void ShowTotalScore()
        {
            Console.WriteLine(localization.GetString("TotalStatistics"));

            if (players.Count == 0)
            {
                Console.WriteLine(localization.GetString("NoGamesPlayed"));
                return;
            }

            var sorted = players.OrderByDescending(p => p.Wins).ToList();

            foreach (var player in sorted)
            {
                Console.WriteLine($"{player.Name}: {player.Wins} {localization.GetString("Wins")}, {player.Losses} {localization.GetString("Losses")}");
            }

            Console.WriteLine();
            Console.WriteLine(localization.GetString("TotalGamesPlayed") + ": " + gameHistory.Count);
        }

        public void ShowScoreBetweenPlayers(string player1, string player2)
        {
            int player1Wins = 0;
            int player2Wins = 0;

            foreach (var game in gameHistory)
            {
                if (game.Winner == player1 && game.Loser == player2)
                {
                    player1Wins++;
                }
                else if (game.Winner == player2 && game.Loser == player1)
                {
                    player2Wins++;
                }
            }

            Console.WriteLine(localization.GetString("ScoreBetweenPlayers", player1, player2));
            Console.WriteLine(localization.GetString("PlayerStatsLine", player1, player1Wins, localization.GetString("Wins"), player2Wins, localization.GetString("Losses")));
            Console.WriteLine(localization.GetString("TotalGames") + ": " + (player1Wins + player2Wins));
        }

        public void ShowGameHistory(int limit = 10)
        {
            Console.WriteLine(localization.GetString("GameHistory"));

            if (gameHistory.Count == 0)
            {
                Console.WriteLine(localization.GetString("NoGamesPlayed"));
                return;
            }

            var recent = gameHistory.OrderByDescending(g => g.Date).Take(limit).ToList();

            for (int i = 0; i < recent.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {recent[i].GetSummary()}");
            }
        }
    }
}