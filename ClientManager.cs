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
        public static string messageEventName = "myMessage";
        private static string userName = "";
        private static List<string> connectedUsers = new List<string>();
        private static List<string> EventList = new List<string>();
        private static List<object> InformatedMessage = new List<object>();
        private static List<string> ErroMessage = new List<string>();


        public static async Task Connect()
        {
            var message = new Message();
           // messageEventName = EventName("");

            _client = new SocketIO(ServerUrl, new SocketIOOptions
            {
                Path = ServerPath,
                Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
            });

            /*_client.On(messageEventName, static response =>
            {
                var receivedMessage = response.GetValue<string>();
                //object receivedMessage2 = response.GetValue<object>();
               
                    Message? inMessage = JsonSerializer.Deserialize<Message>(receivedMessage);
                    if (inMessage != null)
                    {
                        DisplayMessage(inMessage, "receive");
                    }  
                
                
            });*/
            _client.OnAny((eventName, response) =>
            {
                //Console.WriteLine($"\n [Event: {eventName}]");
                EventList.Add(eventName);
               

                try
                {
                   
                    //var element = response.GetValue<JsonElement>();
                    var element = response.GetValue<System.Text.Json.JsonElement>();

                    switch (element.ValueKind)
                    {
                        case JsonValueKind.String:
                            var inMessage = JsonSerializer.Deserialize<Message>(element.GetString()!);
                            DisplayMessage(inMessage, "receive");
                            break;

                        case JsonValueKind.True:
                        case JsonValueKind.False:
                            Console.WriteLine($"Boolean: {element.GetBoolean()}");
                            break;

                        case JsonValueKind.Object:
                        case JsonValueKind.Array:
                            string json = JsonSerializer.Serialize(
                            element, new JsonSerializerOptions { WriteIndented = true });
                            InformatedMessage.Add(json);
                           // Console.WriteLine($"String: {element.GetString()}");
                            break;
                        case JsonValueKind.Null:
                            ErroMessage.Add("Null value");
                            break;

                        default:
                            InformatedMessage.Add(element.ValueKind);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    // Console.WriteLine($"Error reading message: {ex.Message}");
                    ErroMessage.Add(ex.Message);
                }
               
            });

            _client.On("typing", response =>
            {
                string typingUser = response.GetValue<string>();
                //if (typingUser != userName)
                Console.WriteLine($" {typingUser} is typing...");
            });

          
            _client.On("join", response =>
            {
                var users = response.GetValue<string[]>();
                
                connectedUsers.Add(users.ToString());
                Console.WriteLine($"\n User joined: {users[users.Length -1]}");
            });
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            SpinAnimation.Start("waiting for the connection  ");
            //Console.WriteLine($"Waiting connection ... ");

            //System.Threading.Thread.Sleep(3000);
            _client.OnConnected += async (sender, eventArgs) =>
            {
                 message.Channel = messageEventName;
                SpinAnimation.Stop();
                Console.SetCursorPosition(0, Console.CursorTop + 1);
                Console.WriteLine($"Connected to the server in chennel {message.Channel}");
               
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
            //await Task.Delay(2000); // Keep the connection alive

            //Console.WriteLine($"Connected... {_client.Connected}");
        }

        public static async Task SendMessage(Message myInput, string user)
        {
            await _client.EmitAsync("typing", user);
            myInput.Channel = messageEventName;
            await _client.EmitAsync(messageEventName, myInput);
           
            DisplayMessage(myInput, "send");
        }

        public static void DisplayMessage(Message message, string messageType)
        {           
            string messageDis = "";
            if(messageType == "send")
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                //messageDis = "Skickat";
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                //messageDis = "motaget";
            }

            string messageSent = ClientManagerHelpers.ChatDateTimeUtils(message.Timestamp);
            Console.WriteLine($">{message.UserName}: {message.Content}  {messageSent}");
            Console.ResetColor();
            messages.Add(message);
            
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
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.WriteLine("---- Meddelandehistorik ----\n");
            var inMessage = new Message();
            
            foreach (var msg in messages)
            {
                string messageSent = ClientManagerHelpers.ChatDateTimeUtils(msg.Timestamp);
                //DisplayMessage(msg, "receive");
                Console.WriteLine($"{msg.UserName}: {msg.Content}  {messageSent}");
            }
            Console.WriteLine("\n----------------------------\n");

        }

        public static string EventName(string eventName)
        {
            if (!string.IsNullOrEmpty(eventName))
            {
                return messageEventName = eventName;
            }
            else
            {
                return messageEventName = "myMessage";
            }
        }

        internal static void ConnectedUsers()
        {
            foreach (var user in connectedUsers)
            {
               // Console.WriteLine($" - {user.UserName}");
            }
        }

        public static void GetInformatedMessage()
        {
            if(InformatedMessage.Count == 0)
            {
                Console.WriteLine("No informated messages received yet.");
                return;
            }
            foreach (var infoMessage in InformatedMessage)
            {
                Console.WriteLine($" - {infoMessage}");
            }
        }

        public static void GetEventList()
        {
            if(EventList.Count == 0)
            {
                Console.WriteLine("No events received yet.");
                return;
            }
            foreach (var eventName in EventList)
            {
                Console.WriteLine($" - {eventName}");
            }
        }
    }
}
