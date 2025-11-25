using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using WordGame;

class Program
{
    private const int MIN_WORD_LENGTH = 8;
    private const int MAX_WORD_LENGTH = 30;
    private const int TURN_TIMEOUT_MS = 30000;
    private const int CHECK_INTERVAL_MS = 100;

    static string baseWord = "";
    static List<string> usedWords = new List<string>();
    static int currentPlayer = 1;
    static string currentLanguage = "ru";
    static bool gameOver = false;
    static Timer timer;

    static void Main(string[] args)
    {
        StartGame();
    }

    static void StartGame()
    {
        ChooseLanguage();
        GetBaseWord();
        GameLoop();
    }


    static string GetString(string key, params object[] args)
    {
        System.Globalization.CultureInfo culture;
        if (currentLanguage == "en")
        {
            culture = new System.Globalization.CultureInfo("en-US");
        }
        else
        {
            culture = new System.Globalization.CultureInfo("ru-RU");
        }

        string resourceString = Resources.ResourceManager.GetString(key, culture);

        if (resourceString == null)
        {
            return key;
        }

        if (args.Length > 0)
        {
            return string.Format(resourceString, args);
        }

        return resourceString;
    }


        static void ChooseLanguage()
    {
        Console.WriteLine(Resources.ChooseLanguage);
        Console.WriteLine(Resources.Russian);
        Console.WriteLine(Resources.English);

        string choice = Console.ReadLine();

        if (choice == null)
        {
            choice = "1";
        }

        if (choice == "2")
        {
            currentLanguage = "en";
        }
        else
        {
            if (choice != "1")
            {
                Console.WriteLine(Resources.InvalidChoice);
            }
            currentLanguage = "ru";
        }
    }

    static void GetBaseWord()
    {
        string word = "";
        bool validWord = false;

        while (!validWord)
        {
            Console.Write(GetString("EnterBaseWord") + " ");
            word = Console.ReadLine();
            if (word == null)
            {
                word = "";
            }

            if (word.Length < MIN_WORD_LENGTH || word.Length > MAX_WORD_LENGTH)
            {
                Console.WriteLine(GetString("WordLengthError"));
                continue;
            }

            bool hasOnlyLetters = true;
            for (int i = 0; i < word.Length; i++)
            {
                char c = word[i];
                if (!char.IsLetter(c))
                {
                    hasOnlyLetters = false;
                    break;
                }
            }

            if (!hasOnlyLetters)
            {
                Console.WriteLine(GetString("LettersOnlyError"));
                continue;
            }

            validWord = true;
        }

        baseWord = word;
    }

    static void GameLoop()
    {
        Console.WriteLine(GetString("GameStarts", baseWord));

        while (!gameOver)
        {
            PlayTurn();

            if (!gameOver)
            {
                if (currentPlayer == 1)
                {
                    currentPlayer = 2;
                }
                else
                {
                    currentPlayer = 1;
                }
            }
        }

        Console.WriteLine(GetString("GameOver"));
        Console.ReadLine();
    }

    static void PlayTurn()
    {
        Console.WriteLine("\n" + GetString("PlayerTurn", currentPlayer));
        Console.WriteLine(GetString("UsedWords", string.Join(", ", usedWords)));

        string inputWord = GetPlayerInputWithTimer();
        CheckInput(inputWord);
    }

    static string GetPlayerInputWithTimer()
    {
        string inputWord = "";
        bool timeExpired = false;
        ManualResetEvent inputCompleted = new ManualResetEvent(false);

        timer = new Timer(_ =>
        {
            timeExpired = true;
            Console.WriteLine("\n" + GetString("TimeExpired"));
            inputCompleted.Set();
        }, null, TURN_TIMEOUT_MS, Timeout.Infinite);

        Console.Write(GetString("EnterWord") + " ");

        Thread inputThread = new Thread(() =>
        {
            inputWord = Console.ReadLine();
            if (inputWord == null)
            {
                inputWord = " ";
            }
            if (!timeExpired)
            {
                inputCompleted.Set();
            }
        })
        {
            IsBackground = true
        };

        inputThread.Start();
        inputCompleted.WaitOne();

        timer.Dispose();

        if (timeExpired)
        {
            Thread.Sleep(CHECK_INTERVAL_MS);
            return "";
        }

        return inputWord;
    }

    static void CheckInput(string inputWord)
    {
        if (string.IsNullOrEmpty(inputWord))
        {
            Console.WriteLine(GetString("PlayerLostTime", currentPlayer));
            gameOver = true;
            return;
        }

        if (usedWords.Contains(inputWord))
        {
            Console.WriteLine(GetString("WordUsed", inputWord));
            Console.WriteLine(GetString("PlayerLostUsed", currentPlayer));
            gameOver = true;
            return;
        }

        if (!IsWordValid(inputWord))
        {
            Console.WriteLine(GetString("InvalidLetters", inputWord));
            Console.WriteLine(GetString("PlayerLostInvalid", currentPlayer));
            gameOver = true;
            return;
        }

        Console.WriteLine(GetString("WordAccepted", inputWord));
        usedWords.Add(inputWord);
    }

    static bool IsWordValid(string word)
    {
        List<char> availableLetters = new List<char>();
        for (int i = 0; i < baseWord.Length; i++)
        {
            availableLetters.Add(baseWord[i]);
        }

        for (int i = 0; i < word.Length; i++)
        {
            char letter = word[i];
            bool letterFound = false;

            for (int j = 0; j < availableLetters.Count; j++)
            {
                if (availableLetters[j] == letter)
                {
                    availableLetters.RemoveAt(j);
                    letterFound = true;
                    break;
                }
            }

            if (!letterFound)
            {
                return false;
            }
        }

        return true;
    }
}