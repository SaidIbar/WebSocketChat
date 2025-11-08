
using static System.Net.Mime.MediaTypeNames;

namespace WebSocketChat
{
   
    public class Program
    {
       
        static async Task Main(string[] args)
        {
             

            try
            {

                //Message myInput = new Message();
                // await ClientManager.Connect();
                //string text = Console.ReadLine();
                ClientManager clientManager = new ClientManager();
                await ClientManager.Connect();
                Console.WriteLine("Type your message (or 'exit' to quit): ");
                ChatMenu();
                while (Console.ReadKey().Key != ConsoleKey.Enter) { }
                while (true)
                {

                    /*if (text.ToLower() == "exit")
                    {
                       break;
                    }*/
                    StartChat();

                  /*  myInput.Sender = "Said";
                    myInput.Content = Console.ReadLine();
                    myInput.Timestamp = DateTime.Now;
                    await ClientManager.SendMessage(myInput);*/
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

          //  ChatMenu();
        }
        private static async Task StartChat()
        {
            try
            {
               
                Message myInput = new Message();

                myInput.Sender = "Said";
                myInput.Content = Console.ReadLine();
                //Console.WriteLine("Test");
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                ClearCurrentConsoleLine();
                myInput.Timestamp = DateTime.Now;
                await ClientManager.SendMessage(myInput);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
               
        }

        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        private static void ChatMenu()
        {
            Console.WriteLine("Welcom to online chat\n");
            Console.WriteLine("To strat chatting press 1");
            Console.WriteLine("To change Event name press 2");
            Console.WriteLine("To exit press 3");
            var input = Console.ReadLine();
            switch (input)
            {
                case "exit":
                    Console.WriteLine();
                    break;
                case "1":
                    StartChat();
                    break;
                case "2":
                    Console.WriteLine();
                    break;
                case "3":
                    Console.WriteLine();
                    break;
            }
        }
    

    }
}
