using System;
using WordGame.Services;

namespace WordGame
{
    class Program
    {
        private static bool isClosing = false;
        private static WordGame game;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            LocalizationService localization = new LocalizationService();

            Console.CancelKeyPress += OnCancelKeyPress;
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

            try
            {
                game = new WordGame();
                game.Start();
            }
            catch (Exception ex)
            {
                localization.WriteLine("CriticalError", ex.Message);
                Console.ReadLine();
            }
        }

        private static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            if (!isClosing)
            {
                Console.WriteLine("\nApplication is closing...");

                game?.OnApplicationClosing();
                isClosing = true;

                e.Cancel = true;
                Environment.Exit(0);
            }
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            if (!isClosing)
            {
                isClosing = true;
                game?.OnApplicationClosing();
            }
        }
    }
}