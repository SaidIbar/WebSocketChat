namespace WebSocketChat
{
    using SocketIO.Core;
    using SocketIOClient;
    using System;
    using System.Diagnostics.Metrics;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Reflection.Metadata;
    using System.Text.Json;
    using System.Text.RegularExpressions;

    public class ClientManager
    {
        private static SocketIO? _client;
        private static List<Message> messages = new List<Message>();
        private const string ServerUrl = "wss://api.leetcode.se";
        private const string ServerPath = "/sys25d";
        public static string messageEventName = "message";
        private static string userName = "";
        private static List<string> connectedUsers = new List<string>();
        private static List<string> EventList = new List<string>();
        private static List<object> InformatedMessage = new List<object>();
        private static List<string> ErroMessage = new List<string>();
       // static string currentRoom = "general";


        public static async Task Connect()
        {
            var message = new Message();
           // messageEventName = EventName("");

            _client = new SocketIO(ServerUrl, new SocketIOOptions
            {
                Path = ServerPath,
                Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
            });

           
            _client.OnAny((eventName, response) =>
            {
                //Console.WriteLine($"\n [Event: {eventName}]");
               
                try
                {
                    EventList.Add(eventName);
                    
                    //var element = response.GetValue<JsonElement>();
                    var element = response.GetValue<System.Text.Json.JsonElement>();

                    switch (element.ValueKind)
                    {
                        case JsonValueKind.String:
                            var inMessage = JsonSerializer.Deserialize<Message>(element.GetString()!);
                            connectedUsers.Add(inMessage.UserName);
                            DisplayMessage(inMessage, "receive");
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
                string typingUser = response.GetValue<JsonElement>().GetProperty("UserName").ToString();
                if (typingUser != userName)
                Console.WriteLine($" {typingUser} is typing...");
            });



            Console.SetCursorPosition(0, Console.CursorTop - 1);
            SpinAnimation.Start("waiting for the connection  ");
            //Console.WriteLine($"Waiting connection ... ");

            //System.Threading.Thread.Sleep(3000);
            _client.OnConnected += async (sender, eventArgs) =>
            {
                //await _client.EmitAsync("joinRoom", "room1");
                //message.Room = messageEventName;
                SpinAnimation.Stop();
                Console.SetCursorPosition(0, Console.CursorTop + 1);
                Console.WriteLine($"Connected to the server in chennel {message.Room}");
               
            };

            _client.OnReconnectAttempt += (sender, attempt) =>
            {
                Console.WriteLine($"Reconnecting... Attempt {attempt}");
            };


            _client.OnDisconnected += async (sender, eventArgs) =>
            {
                
                Console.WriteLine("Disconnected from the server.");
            };
            await _client.ConnectAsync();
            //await Task.Delay(2000); // Keep the connection alive

            //Console.WriteLine($"Connected... {_client.Connected}");
        }

        public static async Task SendMessage(Message myInput)
        {
          
            await _client.EmitAsync(myInput.Room, new { UserName = myInput.UserName, Message = myInput.Content, room = myInput.Room, Timestamp = myInput.Timestamp });
            
            DisplayMessage(myInput, "send");
        }

        public static async Task JoinRoomAsync(Message message)
        {
            var timeStamp = ClientManagerHelpers.ChatDateTimeUtils(message.Timestamp);
            await _client.EmitAsync(message.Room, $"{message.UserName} joined {message.Room} at {timeStamp}");
           
        }

        public static async Task LeaveRoomAsync(Message message)
        {
            var timeStamp = ClientManagerHelpers.ChatDateTimeUtils(message.Timestamp);
            await _client.EmitAsync(message.Room, $"{message.UserName} left {message.Room} at {timeStamp}");

        }

        public static void DisplayMessage(Message message, string messageType)
        {           
           
            if(messageType == "send")
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
              
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
              
            }

            string messageSent = ClientManagerHelpers.ChatDateTimeUtils(message.Timestamp);
            Console.WriteLine($">{message.UserName}: {message.Content}  {messageSent}");
            Console.ResetColor();
            messages.Add(message);
            
        }

        public static async Task<bool> ConnectToServerAsync(Message message)
        {
            if (message.Status == "leave" && _client != null && _client.Connected)
            {
                ClientManager.LeaveRoomAsync(message).Wait();
                await _client.DisconnectAsync();
                message.Connected = false;
                return false;
            } else if (message.Status == "leave" && (_client == null || !_client.Connected))
            {
                message.Connected = false;
                return false;
            }
            else
            {
                await ClientManager.Connect();
                message.Connected = true;
                return true;
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
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.WriteLine("----Event list-----\n");
            if (!string.IsNullOrEmpty(eventName))
            {
                return messageEventName = eventName;
            }
            else
            {
                return messageEventName = "message";
            }
        }

        public static void ConnectedUsers()
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.WriteLine("---- Connected users ----\n");

            if (connectedUsers.Count == 0)
            {
                Console.WriteLine("No users received yet.");
                return;
            }
            foreach (var members in connectedUsers)
            {
               
                Console.WriteLine($" - {members}");
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
                string result = Regex.Replace((string)infoMessage, @"\s+", " ").Trim().ToLower();
                Console.WriteLine($" - {result}");
            }
        }

        public static void GetEventList()
        {
            if(EventList.Count == 0)
            {
                Console.WriteLine("No events received yet.");
                return;
            }
            foreach (var eventName in EventList.Distinct())
            {
                Console.WriteLine($" - {eventName}");
            }
        }
    }
}
