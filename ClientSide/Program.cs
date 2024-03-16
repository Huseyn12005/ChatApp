using ClientSide;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

var ip = IPAddress.Parse("127.0.0.1");
var port = 27001;

var serverEP =  new IPEndPoint(ip, port);

TcpClient client = new TcpClient();

client.Connect(serverEP);

NetworkStream? clientData = client.GetStream();

BinaryReader? reader = new BinaryReader(clientData);
BinaryWriter? writer = new BinaryWriter(clientData);

_ = Task.Run(() =>
{
    var message = reader.ReadString();
    var msg = JsonSerializer.Deserialize<MessageDTO>(message);
    Console.WriteLine(msg);
});


bool isCheck = true;

while(true)
{
    if(isCheck)
    {
        Console.Write("Enter Your Name: ");
        var name = Console.ReadLine();
        writer.Write(name);
        isCheck = false;
    }
    else
    {
        Console.Write("Sender Name: ");
        var name = Console.ReadLine();
        Console.Write("Sender Message: ");
        var message = Console.ReadLine();

        var msg = new MessageDTO()
        {
            UserName = name,
            Message = message
        };

        var msgJson = JsonSerializer.Serialize(msg);
        writer.Write(msgJson);

    }
}