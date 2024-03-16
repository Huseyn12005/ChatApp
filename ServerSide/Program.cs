using ServerSide;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

var ip = IPAddress.Parse("127.0.0.1");
var port = 27001;

var listenerEP = new IPEndPoint(ip, port);

TcpListener listener = new TcpListener(listenerEP);

listener.Start();

while (true)
{
    var client = listener.AcceptTcpClient();
    Console.WriteLine($"{client.Client.RemoteEndPoint} is connected ...");

    _ = Task.Run(async () =>
    {
        var clientData = client.GetStream();
        var clientReader = new BinaryReader(clientData);
        var clinentWriter = new BinaryWriter(clientData);

        bool isCheck = true;
        var username = "";
        while (true)
        {
            if(isCheck)
            {
                username = clientReader.ReadString();
                DataBase.Users.Add(new User()
                {
                    UserName = username.ToLower(),
                    TcpClient = client
                });


                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{username} is connected ...");
                Console.ForegroundColor = ConsoleColor.White;

                isCheck = false;
            }

            if (!isCheck)
            {
                var message = clientReader.ReadString();
                MessageDTO? msg = JsonSerializer.Deserialize<MessageDTO>(message);


                if(msg is not null)
                {
                    var user = DataBase.Users.FirstOrDefault(x => x.UserName.ToLower() == msg.UserName.ToLower());

                    if (user is not null)
                    {
                        BinaryWriter? userWriter = new BinaryWriter(user.TcpClient.GetStream());
                        MessageDTO? sendMsg = new MessageDTO()
                        {
                            UserName = username,
                            Message = msg.Message
                        };
                        var send = JsonSerializer.Serialize<MessageDTO>(sendMsg);
                        userWriter.Write(send);
                    }
                }
            }
        }
    });
}

static class DataBase
{
    public static List<User> Users = new List<User>();
}