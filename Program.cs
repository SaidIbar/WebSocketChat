namespace WebSocketChat
{
    public class Program
    {
        public static string? _sender;
        public static int menuCount = 0;
        public static Message _message = new Message();

        private static void Main()
        {
        

            Console.WriteLine("Welcom to online chat, you can at any time go back to the menu by pressing 00 and enter");
            ChatMenu();
            while (true)
            {
                if (!string.IsNullOrEmpty(_sender))
                {
                    _message.UserName = _sender;
                    StartChat(_message!).Wait();
                }
                else
                {
                    Console.WriteLine($"UserName name is not set. Please enter your name.");
                    _sender = Console.ReadLine();
                    if (!string.IsNullOrEmpty(_sender))
                    {
                        _message.UserName = _sender;
                        StartChat(_message!).Wait();
                    }
                }
            }
          
        }

        private static async Task StartChat(Message myInput)
        {
            try
            {
                
                //myInput.UserName = sender;
                string input = Console.ReadLine();

                if (input == "00")
                {
                    Console.SetCursorPosition(0, Console.CursorTop - 2);
                    //Console.SetCursorPosition(0, Console.CursorTop + 1);
                    ChatMenu();
                }
                else
                {
                    myInput.Content = input;
                    var checkMessage = ClientManagerHelpers.IsValidMessageInput(myInput.Content, "message");
                    if (!checkMessage)
                    {
                        while (!checkMessage)
                        {
                            Console.Write("Enter you message: ");
                            myInput.Content = Console.ReadLine();
                            checkMessage = ClientManagerHelpers.IsValidMessageInput(myInput.Content!, "message"); //!string.IsNullOrEmpty(_sender) && ClientManagerHelpers.IsValidNameInput(_sender!);
                        }
                    }
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    ClientManagerHelpers.ClearCurrentConsoleLine();
                    myInput.Timestamp = DateTime.Now;
                   // myInput.Room = ClientManager.messageEventName;
                    await ClientManager.SendMessage(myInput);
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
                "Connect and Start chating",
                "Disconnect",
                "List message history",
                "List events list",
                "List connected users",
                "List of informated messages",
                "Change Event name and connect",
                "Quit\n"
            };
            menuCount = menuArray.Length;
            if (Console.CursorTop > 0) { 
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.WriteLine("\n");
            }
            
          
           for (int i = 0; i < menuArray.Length; i++)
            {
                if(i == menuArray.Length -1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                   
                }else
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }

                Console.WriteLine($" {i + 1} . {menuArray[i]}");
                

            }
            Console.ResetColor();
            bool disconnectResult = false;
            var input = Console.ReadLine();
            switch (input)
            {
              
                case "1":
                    _message.Status = "join";
                    bool connectInRoom = ClientManager.ConnectToServerAsync(_message).Result;
                    if (connectInRoom)
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
                        _message.UserName = _sender;
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                        Console.WriteLine("Enter the room name you want to join");
                        var roomName = Console.ReadLine();
                        var romNameValid = ClientManagerHelpers.IsValidMessageInput(roomName!, "room name");
                        if (!romNameValid)
                        {
                            while (!romNameValid)
                            {
                                Console.Write("Enter the room name you want to join: ");
                                roomName = Console.ReadLine();
                                romNameValid = ClientManagerHelpers.IsValidMessageInput(roomName!, "room name");
                            }
                        }
                        _message.Room = roomName;
                        ClientManager.JoinRoomAsync(_message).Wait();
                        _message.Status = "join";
                        Console.WriteLine($"{_message.UserName} type your message: ");
                        StartChat(_message).Wait();
                    }
                    break;
                case "2":
                    _message.Status = "leave";
                    disconnectResult = ClientManager.ConnectToServerAsync(_message).Result;
                    if (!disconnectResult)
                    {
                        Console.WriteLine("You are disconnected from the server.");
                    }
                    ChatMenu();
                    break;

                case "3":
                    ClientManager.DisplayMessageHistory();
                    ChatMenu();
                    break;
                case "4":
                    ClientManager.GetEventList();
                    ChatMenu();
                    break;
                case "5":
                    ClientManager.ConnectedUsers();
                    ChatMenu();
                    break;
                case "7":
                    Console.WriteLine($"Changing event name {ClientManager.messageEventName}");
                    Console.Write("Enter new event name: > ");
                    var eventInput = Console.ReadLine();
                    var newEventName = ClientManager.EventName(eventInput);
                    Console.WriteLine($"Event name changed to: {newEventName}");
                    ChatMenu();
                    break;
                case "6":
                    ClientManager.GetInformatedMessage();
                    ChatMenu();
                    break;
                
                case "8":
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
                        _message.Status = "leave";
                        disconnectResult = ClientManager.ConnectToServerAsync(_message).Result;
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
