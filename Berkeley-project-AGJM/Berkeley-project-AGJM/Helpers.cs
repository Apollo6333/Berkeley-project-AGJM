namespace Berkeley_project_AGJM
{
    internal class Helpers
    {
        private static readonly object _defaultLogLock = new();

        public static bool IsPortAlreadyInUse(int port)
        {
            return false; // TO-DO
        }

        public static void Log(DateTime time, string log, bool coordinator = false, bool error = false)
        {
            Log(_defaultLogLock, -1, time, log, coordinator, error);
        }

        public static void Log(int id, DateTime time, string log, bool coordinator = false, bool error = false)
        {
            Log(_defaultLogLock, id, time, log, coordinator, error);
        }

        public static void Log(object threadLock, int id, DateTime time, string log, bool coordinator = false, bool error = false)
        {
            lock (threadLock)
            {
                string timeFormated = time.ToString("HH:mm:ss:fff");
                string logToSend = (error ? "ERROR: " : "") + log;
                string fullLog = (id != -1 ? $"[{id}] @ " : "") + $"{timeFormated} | {logToSend}";

                if (error) Console.ForegroundColor = ConsoleColor.Red;
                else if (coordinator) Console.ForegroundColor = ConsoleColor.Green;
                else Console.ResetColor();

                Console.WriteLine(fullLog);
            
                Console.ResetColor();
            }
        }
    }
}