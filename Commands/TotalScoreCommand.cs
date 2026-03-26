using WordGame.Models;
using WordGame.Services;

namespace WordGame.Commands
{
    public class TotalScoreCommand : ICommand
    {
        public string Name => "/total-score";

        public void Execute(GameSession session, StatisticsService statistics, LocalizationService localization)
        {
            statistics.ShowTotalScore();
        }
    }
}