namespace WebSocketChat
{
    using SocketIOClient;
    using System;
    using System.Runtime.CompilerServices;
    using System.Text.Json;
    using WebSocketChat;

    public class ClientManager
    {
        private static SocketIO? _client;
        private const string ServerUrl = "wss://api.leetcode.se";
        private const string ServerPath = "/sys25d";
        private const string messageEventName = "myMessage";
        ClientManager clientManager = new ClientManager();
        public static async Task Connect()
        {
            var message = new Message(); 
            _client = new SocketIO(ServerUrl, new SocketIOOptions
            {
                Path = ServerPath,
            });

            _client.On(messageEventName, static response =>
            {
                var receivedMessage = response.GetValue<string>();
                if (receivedMessage != null)
                {
                    Message? inMessage = JsonSerializer.Deserialize<Message>(receivedMessage);
                    if (inMessage != null)
                    {
                        DisplayMessage(inMessage, "receive");
                    }
                    // else: handle deserialization failure if needed
                }
                // else: handle null receivedMessage if needed
            });
            Console.WriteLine("Waiting connection ... ");

            _client.OnConnected += async (sender, eventArgs) =>
            {
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.WriteLine("Connected to the server."); 
            };

            _client.OnDisconnected += (sender, eventArgs) =>
            {
                Console.WriteLine("Disconnected from the server.");
            };
            await _client.ConnectAsync();
            await Task.Delay(2000); // Keep the connection alive

            //Console.WriteLine($"Connected... {_client.Connected}");
        }

        public static async Task SendMessage(Message myInput)
        {
            await _client.EmitAsync(messageEventName, myInput);
            DisplayMessage(myInput, "send");
        }

        public static void DisplayMessage(Message message, string messageType)
        {
            //ChatUtils chatUtils = new ChatUtils();
            string messageDis = "";
            if(messageType == "send")
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                messageDis = "Skickat";
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                messageDis = "motaget";
            }

            string messageSent = ClientManagerHelpers.ChatUtils(message.Timestamp);


            Console.WriteLine($"{message.Sender}: {message.Content} {messageDis} : {messageSent}");
            Console.ResetColor();
        }

        public static async Task<string> ConnectToServerAsync(string status)
        {
            if (status == "disconnect" && _client != null && _client.Connected)
            {
                await _client.DisconnectAsync();
                return "disconnected";
            } else if (status == "disconnect" && (_client == null || !_client.Connected))
            {
                return "disconnected";
            }
            else
            {
                await ClientManager.Connect();
                return "connected";
            }
            
        }
    }
}
