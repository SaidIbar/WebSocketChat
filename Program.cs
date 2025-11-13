namespace WebSocketChat
{
    public class Program
    {
        public static string? _sender;
        public static int menuCount = 0;


        private static void Main()
        {
           
            Console.WriteLine("Welcom to online chat, you alwas get back to the menu by pressing 00 an enter");
            ChatMenu();
            while (true)
            {
                if (!string.IsNullOrEmpty(_sender))
                {
                    StartChat(_sender!).Wait();
                }
                else
                {
                    Console.WriteLine($"UserName name is not set. Please enter your name.");
                    _sender = Console.ReadLine();
                    if (!string.IsNullOrEmpty(_sender))
                    {
                        StartChat(_sender!).Wait();
                    }
                }
            }
          
        }

        private static async Task StartChat(string sender)
        {
            try
            {
                Message myInput = new Message();
                myInput.UserName = sender;
                string input = Console.ReadLine();

                if (input == "00")
                {
                    Console.SetCursorPosition(0, Console.CursorTop - 2);
                    //Console.SetCursorPosition(0, Console.CursorTop + 1);
                    ChatMenu();
                }
                else if (input == "exit")
                {
                    string disconnectResult = ClientManager.ConnectToServerAsync("disconnect").Result;
                    if (disconnectResult.Equals("disconnected"))
                    {
                        Console.WriteLine("You are disconnected from the server.");
                    }
                    ChatMenu();
                }
                else
                {
                    myInput.Content = input;
                    var checkMessage = ClientManagerHelpers.IsValidMessageInput(myInput.Content);
                    if (!checkMessage)
                    {
                        while (!checkMessage)
                        {
                            Console.Write("Enter you message: ");
                            myInput.Content = Console.ReadLine();
                            checkMessage = ClientManagerHelpers.IsValidMessageInput(myInput.Content!); //!string.IsNullOrEmpty(_sender) && ClientManagerHelpers.IsValidNameInput(_sender!);
                        }
                    }
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    ClientManagerHelpers.ClearCurrentConsoleLine();
                    myInput.Timestamp = DateTime.Now;
                    await ClientManager.SendMessage(myInput, _sender);
                }
                   
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }



        private static void ChatMenu()
        {
            var menuArray = new string[]
            {
                "Connect",
                "Change Event name",
                "List of informated messages",
                "Disconnect",
                "See message history",
                "See event list",
                "Quit\n"
            };
            menuCount = menuArray.Length;
            if (Console.CursorTop > 0) { 
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.WriteLine("\n");
            }
            
          
           for (int i = 0; i < menuArray.Length; i++)
            {
                Console.WriteLine($" {i + 1} . {menuArray[i]}");
                

            }
            string disconnectResult = "";
            var input = Console.ReadLine();
            switch (input)
            {
                
                case "1":
                    string connect = ClientManager.ConnectToServerAsync("connect").Result;
                    if (connect.Equals("connected"))
                    {
                        Console.Write("Enter you name: > ");
                        _sender = Console.ReadLine();
                        var checkName = ClientManagerHelpers.IsValidNameInput(_sender!);
                        if (!checkName)
                        {
                            while (!checkName)
                            {
                                Console.Write("Enter you name: ");
                                _sender = Console.ReadLine();
                                checkName = ClientManagerHelpers.IsValidNameInput(_sender!);
                            }
                        }
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                        Console.WriteLine($"{_sender} type your message: ");
                        StartChat(_sender).Wait();
                    }
                    break;
                case "2":
                    Console.WriteLine($"Changing event name {ClientManager.messageEventName}");
                    Console.Write("Enter new event name: > ");
                    var eventInput = Console.ReadLine();
                    var newEventName = ClientManager.EventName(eventInput);
                    Console.WriteLine($"Event name changed to: {newEventName}");
                    ChatMenu();
                    break;
                case "3":
                    ClientManager.GetInformatedMessage();
                    ChatMenu();
                    break;
                case "4":
                    disconnectResult = ClientManager.ConnectToServerAsync("disconnect").Result;
                    if (disconnectResult.Equals("disconnected"))
                    {
                        Console.WriteLine("You are disconnected from the server.");
                    }
                    ChatMenu();
                    break;
                
                case "5":
                    ClientManager.DisplayMessageHistory();
                    ChatMenu();
                    break;
                case "6":
                    ClientManager.GetEventList();
                    ChatMenu();
                    break;
                case "7":
                    Console.WriteLine("Are you sure you want to quit y/n");
                    var exitInput = Console.ReadLine();


                    if (string.IsNullOrEmpty(exitInput))
                    {
                        Console.WriteLine("Invalid input.must be y/n");
                        exitInput = Console.ReadLine();
                    }
                    var answer = ClientManagerHelpers.IsAnswerYes(exitInput!);
                    if (answer)
                    {
                        disconnectResult = ClientManager.ConnectToServerAsync("disconnect").Result;
                        if (disconnectResult.Equals("disconnected"))
                        {
                            Console.WriteLine("You are disconnected from the server.");
                        }
                        Console.WriteLine("Exiting the chat application. Goodbye!");
                        Environment.Exit(0);
                    }
                    else
                    {
                        ChatMenu();
                    }
                    break;
                default:
                    Console.WriteLine($"Invalid option, select from 1 to {menuCount}");
                    ChatMenu();
                    break;
                }

        }

    }
}
