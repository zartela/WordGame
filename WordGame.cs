using System;
using WordGame.Models;
using WordGame.Services;
using WordGame.Commands;

namespace WordGame
{
    public class WordGame
    {
        private const int TURN_TIMEOUT_MS = 30000;

        private GameSession session;
        private readonly LocalizationService localization;
        private readonly StatisticsService statistics;
        private readonly TimerService timer;
        private readonly GameValidator validator;
        private readonly PlayerInputService input;
        private readonly CommandHandler commandHandler;

        public WordGame()
        {
            localization = new LocalizationService();
            statistics = new StatisticsService(localization);
            timer = new TimerService();
            validator = new GameValidator(localization);
            input = new PlayerInputService(timer, localization);
            commandHandler = new CommandHandler(localization);
            session = new GameSession();
        }

        public void Start()
        {
            try
            {
                ChooseLanguage();
                GetPlayerNames();
                GetBaseWord();
                RunGameLoop();
            }
            catch (Exception ex)
            {
                Console.WriteLine(localization.GetString("ErrorOccurred", ex.Message));
            }
            finally
            {
                Console.WriteLine(localization.GetString("GameOver"));
                Console.ReadLine();
            }
        }

        private void ChooseLanguage()
        {
            string choice = input.GetLanguageChoice();

            if (choice == "2")
            {
                localization.SetLanguage("en");
            }
            else
            {
                if (choice != "1")
                {
                    localization.WriteLine("InvalidChoice");
                }
                localization.SetLanguage("ru");
            }
        }

        private void GetPlayerNames()
        {
            string name1 = input.GetPlayerName("1");
            if (!string.IsNullOrEmpty(name1))
            {
                session.Player1Name = name1;
            }

            string name2 = input.GetPlayerName("2");
            if (!string.IsNullOrEmpty(name2))
            {
                session.Player2Name = name2;
            }
        }

        private void GetBaseWord()
        {
            bool valid = false;

            while (!valid)
            {
                string word = input.GetBaseWord();

                if (validator.ValidateBaseWord(word, out string error))
                {
                    session.BaseWord = word.ToLower();
                    valid = true;
                }
                else
                {
                    Console.WriteLine(error);
                }
            }
        }

        private void RunGameLoop()
        {
            localization.WriteLine("GameStarts", session.BaseWord);
            commandHandler.ShowHelp();

            while (!session.IsGameOver)
            {
                bool wordWasEntered = PlayTurn();  

                if (!session.IsGameOver && wordWasEntered)  
                {
                    session.SwitchPlayer();
                }
            }
        }

        private bool PlayTurn()
        {
            string playerName = session.GetCurrentPlayerName();
            localization.WriteLine("PlayerTurn", session.CurrentPlayer, playerName);

            string inputWord = input.GetInputWithTimer(TURN_TIMEOUT_MS);

            if (commandHandler.IsCommand(inputWord))
            {
                commandHandler.Execute(inputWord, session, statistics);
                return false; 
            }

            if (validator.ValidatePlayerWord(inputWord, session, out string error))
            {
                session.AddAttempt(inputWord, true);
                localization.WriteLine("WordAccepted", inputWord);
                return true;  
            }
            else
            {
                session.AddAttempt(inputWord, false);
                localization.WriteLine(error);
                EndGame(playerName);
                return false;  
            }
        }

        public void OnApplicationClosing()
        {
            if (!session.IsGameOver)
            {
                string loserName = session.GetCurrentPlayerName();
                string winnerName = session.GetOtherPlayerName();

                Console.WriteLine(localization.GetString("ApplicationClosed") + " " + loserName);
                statistics.RecordGameResult(session, winnerName, loserName);

                session.IsGameOver = true;
            }
        }

        private void EndGame(string loserName)
        {
            string winnerName = session.GetOtherPlayerName();

            localization.WriteLine("PlayerLost", loserName);

            statistics.RecordGameResult(session, winnerName, loserName);

            session.IsGameOver = true;
        }
    }
}