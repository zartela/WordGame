using WordGame.Models;
using WordGame.Services;

namespace WordGame.Commands
{
    public class ScoreCommand : ICommand
    {
        public string Name => "/score";

        public void Execute(GameSession session, StatisticsService statistics, LocalizationService localization)
        {
            string playerName = session.GetCurrentPlayerName();
            string otherPlayerName = session.GetOtherPlayerName();

            statistics.ShowScoreBetweenPlayers(playerName, otherPlayerName);
        }
    }
}