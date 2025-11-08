using System.Runtime.CompilerServices;

namespace WebSocketChat
{
    public class Program
    {
        public static string? _sender;

        static void Main()
        {
           
            try
            {
               
                ChatMenu();
               
                while (true)
                {
                   
                    if (!string.IsNullOrEmpty(_sender))
                    {
                        StartChat(_sender!).Wait();
                    }
                    else
                    {
                        Console.WriteLine($"Sender name is not set. Please enter your name.");
                        _sender = Console.ReadLine();
                        if (!string.IsNullOrEmpty(_sender))
                        {
                            StartChat(_sender!).Wait();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private static async Task StartChat(string sender)
        {
            try
            {
                Message myInput = new Message();
                myInput.Sender = sender;
                myInput.Content = Console.ReadLine();
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                ClientManagerHelpers.ClearCurrentConsoleLine();
                myInput.Timestamp = DateTime.Now;
                await ClientManager.SendMessage(myInput);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private static void ChatMenu()
        {
            Console.WriteLine("Welcom to online chat\n");
            Console.WriteLine("To strat chatting press 1");
            Console.WriteLine("To change Event name press 2");
            Console.WriteLine("To disconnect press 3");
            Console.WriteLine("To quit press q");
            var input = Console.ReadLine();
            switch (input)
            {
                case "q":
                    Console.WriteLine("Are you sure you want to quit y/n");
                    var exitInput = Console.ReadLine();
                    string disconnectResult = "";


                    if (exitInput.ToLower() == "y")
                    {
                        disconnectResult = ClientManager.ConnectToServerAsync("disconnect").Result;
                        if (disconnectResult.Equals("disconnected"))
                        {
                            Console.WriteLine("You are disconnected from the server.");
                        }
                        Console.WriteLine("Exiting the chat application. Goodbye!");
                        Environment.Exit(0);

                    }
                   
                    break;
                case "1":
                    string connect = ClientManager.ConnectToServerAsync("connect").Result;
                    if (connect.Equals("connected"))
                    {
                        Console.Write("Enter you name: ");
                        _sender = Console.ReadLine();
                        var checkName = !string.IsNullOrEmpty(_sender) && ClientManagerHelpers.IsValidInput(_sender!);
                        if (!checkName)
                        {
                            while (!checkName)
                            {
                                Console.Write("Enter you name: ");
                                _sender = Console.ReadLine();
                                checkName = !string.IsNullOrEmpty(_sender) && ClientManagerHelpers.IsValidInput(_sender!);
                            }
                        }
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                        Console.WriteLine($"{_sender} type your message: ");
                        StartChat(_sender).Wait();
                    }
                    break;
                case "2":
                    Console.WriteLine();
                    break;
                case "3":
                     disconnectResult = ClientManager.ConnectToServerAsync("disconnect").Result;
                    if (disconnectResult.Equals("disconnected"))
                    {
                        Console.WriteLine("You are disconnected from the server.");
                    }
                    ChatMenu();
                    break;
            }
        }
    }
}
