using System;
using System.Threading;

namespace WordGame.Services
{
    public class TimerService
    {
        private Timer timer;
        private bool timeExpired;
        private ManualResetEvent inputCompleted;
        private const int CHECK_INTERVAL_MS = 100;

        public bool WaitForInput(int timeoutMs, out string input)
        {
            string result = "";
            timeExpired = false;
            inputCompleted = new ManualResetEvent(false);

            timer = new Timer(OnTimerTick, null, timeoutMs, Timeout.Infinite);

            Thread inputThread = new Thread(() =>
            {
                string readResult = Console.ReadLine() ?? "";
                if (!timeExpired)
                {
                    result = readResult;
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
                input = "";
                return true;
            }

            input = result;
            return false;
        }

        private void OnTimerTick(object state)
        {
            timeExpired = true;
            inputCompleted.Set();
        }

        public void Stop()
        {
            timer?.Dispose();
        }
    }
}