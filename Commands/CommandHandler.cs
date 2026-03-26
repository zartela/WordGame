using System;
using System.Collections.Generic;
using WordGame.Models;
using WordGame.Services;

namespace WordGame.Commands
{
    public class CommandHandler
    {
        private readonly Dictionary<string, ICommand> commands = new Dictionary<string, ICommand>();
        private readonly LocalizationService localization;

        public CommandHandler(LocalizationService localization)
        {
            this.localization = localization;
            RegisterCommands();
        }

        private void RegisterCommands()
        {
            AddCommand(new ShowWordsCommand());
            AddCommand(new ScoreCommand());
            AddCommand(new TotalScoreCommand());
        }

        private void AddCommand(ICommand command)
        {
            commands[command.Name.ToLower()] = command;
        }

        public bool IsCommand(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            string normalized = input.Trim().ToLower();
            string commandKey = normalized.Split(' ')[0];

            return commands.ContainsKey(commandKey) ||
                   commands.ContainsKey(commandKey.Replace('_', '-')) ||
                   commands.ContainsKey(commandKey.Replace('-', '_'));
        }

        public void Execute(string input, GameSession session, StatisticsService statistics)
        {
            if (string.IsNullOrEmpty(input))
            {
                return;
            }

            string normalized = input.Trim().ToLower();
            string commandKey = normalized.Split(' ')[0];

            ICommand command = null;

            if (commands.ContainsKey(commandKey))
            {
                command = commands[commandKey];
            }
            else if (commands.ContainsKey(commandKey.Replace('_', '-')))
            {
                command = commands[commandKey.Replace('_', '-')];
            }
            else if (commands.ContainsKey(commandKey.Replace('-', '_')))
            {
                command = commands[commandKey.Replace('-', '_')];
            }

            if (command != null)
            {
                command.Execute(session, statistics, localization);
            }
            else
            {
                localization.WriteLine("InvalidCommand");
            }
        }

        public void ShowHelp()
        {
            localization.WriteLine("Commands");
            foreach (var command in commands.Values)
            {
                string descKey = command.Name.Replace("/", "");
                Console.WriteLine($"  {command.Name} - {localization.GetString(descKey)}");
            }
        }
    }
}