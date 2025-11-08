

namespace WebSocketChat
{
    using SocketIOClient;
    using System;
    using System.Text.Json;

    public class ClientManager
    {
        private static SocketIO _client;
        private const string ServerUrl = "wss://api.leetcode.se";
        private const string ServerPath = "/sys25d";
        private const string messageEventName = "myMessage";
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
              //  string jsonString = JsonSerializer.Deserialize(receivedMessage);
                Message inMessage = JsonSerializer.Deserialize<Message>(receivedMessage);
                DisplayMessage(inMessage, "receive");
                // Console.WriteLine(jsonString);
               // Console.WriteLine($"{inMessage.Sender}: {inMessage.Content} {"Skickat: "} {inMessage.Timestamp}");

            });

            _client.OnConnected += async (sender, eventArgs) =>
            {
                Console.WriteLine("Connected to the server.");
                
               // await _client.EmitAsync(messageEventName, "lets talk"); 
            };

            _client.OnDisconnected += (sender, eventArgs) =>
            {
                Console.WriteLine("Disconnected from the server.");
            };
            await _client.ConnectAsync();
            await Task.Delay(2000); // Keep the connection alive

            Console.WriteLine($"Connected... {_client.Connected}");
        }

        public static async Task SendMessage(Message myInput)
        {
            //message.Timestamp = DateTime.ParseExact(dateString, "d", CultureInfo.InvariantCulture);
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

            string messageSent = ChatUtils(message.Timestamp);


            Console.WriteLine($"{message.Sender}: {message.Content} {messageDis} : {messageSent}");
            Console.ResetColor();
        }

        private static string ChatUtils(DateTime timestamp)
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
    }
}
