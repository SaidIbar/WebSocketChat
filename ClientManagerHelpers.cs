using System.Runtime.CompilerServices;
using WebSocketChat;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebSocketChat
{
    internal static class ClientManagerHelpers
    {

        public static string ChatDateTimeUtils(DateTime timestamp)
        {
            string newDate = "";
            var month1 = int.Parse(timestamp.Date.ToString("MM"));
            var month2 = int.Parse(DateTime.Now.Date.ToString("MM"));
            var day1 = int.Parse(timestamp.Date.ToString("dd"));
            var day2 = int.Parse(DateTime.Now.Date.ToString("dd"));
            var diff = day2 - day1;

            if (diff == 0 && month1 == month2)  //DateTime.Now.ToString("MM/dd/yyyy")
            {
                return newDate = "  ---> idag " + timestamp.ToString("hh:mm tt");
            }
            if (diff == 1 && month1 == month2)
            {
                return newDate = "  ---> igår " + timestamp.ToString("hh:mm tt"); ;
            }
            else
            {
                
                return newDate = " ---> " + timestamp.ToString("dd MMMM"); //DateTime.Now.ToString("yyyy MMMM")
            }

            //return newDate;
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