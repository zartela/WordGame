using WordGame.Models;
using WordGame.Services;

namespace WordGame.Commands
{
    public interface ICommand
    {
        string Name { get; }
        void Execute(GameSession session, StatisticsService statistics, LocalizationService localization);
    }
}