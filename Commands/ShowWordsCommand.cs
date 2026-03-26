using System;
using WordGame.Models;
using WordGame.Services;

namespace WordGame.Commands
{
    public class ShowWordsCommand : ICommand
    {
        public string Name => "/show-words";

        public void Execute(GameSession session, StatisticsService statistics, LocalizationService localization)
        {
            localization.WriteLine("ShowWords");

            if (session.AllAttempts.Count == 0)
            {
                localization.WriteLine("NoWords");
            }
            else
            {
                for (int i = 0; i < session.AllAttempts.Count; i++)
                {
                    string word = session.AllAttempts[i];
                    bool accepted = session.AcceptedWords.Contains(word);
                    string status = accepted ? localization.GetString("Accepted") : localization.GetString("Rejected");
                    Console.WriteLine($"{i + 1}. {word} {status}");
                }
                Console.WriteLine(localization.GetString("TotalAttempts") + ": " + session.AllAttempts.Count);
            }
        }
    }
}