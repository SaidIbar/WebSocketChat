namespace WebSocketChat
{
    using SocketIO.Core;
    using SocketIOClient;
    using System;
    using System.Net.Sockets;
    using System.Text.Json;
   

    public class ClientManager
    {
        private static SocketIO? _client;
        private static List<Message> messages = new List<Message>();
        private const string ServerUrl = "wss://api.leetcode.se";
        private const string ServerPath = "/sys25d";
        private const string messageEventName = "myMessage";
        private static string userName = "";


        public static async Task Connect()
        {
            var message = new Message(); Animations spin = new Animations();

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

            _client.On("typing", response =>
            {
                string typingUser = response.GetValue<string>();
                if (typingUser != userName)
                    Console.WriteLine($"✍️  {typingUser} is typing...");
            });


            SpinAnimation.Start("waiting for the connection  ");
            //Console.WriteLine($"Waiting connection ... ");

            //System.Threading.Thread.Sleep(3000);
            _client.OnConnected += async (sender, eventArgs) =>
            {
                
                SpinAnimation.Stop();
                Console.SetCursorPosition(0, Console.CursorTop + 1);
                Console.WriteLine("Connected to the server.");
                //Console.SetCursorPosition(0, Console.CursorTop - 0);
              
            };

            _client.OnReconnectAttempt += (sender, attempt) =>
            {
                Console.WriteLine($"Reconnecting... Attempt {attempt}");
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
           // _client.EmitAsync("joinRoom", "rum1"); // exempel: gå med i rum1
            await _client.EmitAsync(messageEventName, myInput);
           
            DisplayMessage(myInput, "send");
        }

        public static void DisplayMessage(Message message, string messageType)
        {
           
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

            string messageSent = ClientManagerHelpers.ChatDateTimeUtils(message.Timestamp);


            Console.WriteLine($"{message.Sender}: {message.Content}  {messageSent}");
            Console.ResetColor();
            messages.Add(message);
            Console.WriteLine(messages.Count);
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

        public static void DisplayMessageHistory()
        {
            Console.WriteLine("---- Meddelandehistorik ----");
            foreach (var msg in messages)
            {
                string messageSent = ClientManagerHelpers.ChatDateTimeUtils(msg.Timestamp);
                Console.WriteLine($"{msg.Sender}: {msg.Content}  {messageSent}");
            }
           
        }


    }
}
