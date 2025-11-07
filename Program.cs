
namespace WebSocketChat
{
   
    public class Program
    {
       
        static async Task Main(string[] args)
        {
            // ClientManager clientManager = new ClientManager();

            try
            {
                Message myInput = new Message();
                await ClientManager.Connect();
                string text = Console.ReadLine();
                while (true)
                {
                  
                    if (text.ToLower() == "exit")
                    {
                        break;
                    }
                   
                    myInput.Sender = "Said";
                    myInput.Content = Console.ReadLine();
                    myInput.Timestamp = DateTime.Now;
                    await ClientManager.SendMessage(myInput);
                   // Console.WriteLine();
                }
           
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

           
        }
    

    }
}
