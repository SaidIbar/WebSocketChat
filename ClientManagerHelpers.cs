using System.Runtime.CompilerServices;
using WebSocketChat;

namespace WebSocketChat
{
    internal static class ClientManagerHelpers
    {

        public static string ChatUtils(DateTime timestamp)
        {
            string newDate = "";
            if (timestamp.Date == DateTime.Now.Date)
            {
                newDate = "nu";
            }
            else
            {
                newDate = timestamp.ToString();
            }

            return newDate;
        }

        public static bool IsValidInput(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
               return true;    
            }
            else
            {
                Console.WriteLine("Sender name is not set. Please enter your name.");
                return false;

            }
            
        }

        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
    }
}