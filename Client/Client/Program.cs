using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            //Setting up the client
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            int port = 5000;
            TcpClient client = new TcpClient();
            client.Connect(ip, port);
            Console.WriteLine("Connected to server");
            Console.WriteLine("Type \"!ready\" when you are ready to start a game.\nWhen eveyone is ready, the game starts.\nType \"!dc\" to disconnect from the server.\nOther than that, feel free to chat with the other players connected to the server.");
            NetworkStream ns = client.GetStream();
            Thread thread = new Thread(o => ReceiveData((TcpClient)o));

            thread.Start(client);

            //Registers what is written in the console by the client and stores it in inputString
            string inputString;
            while ((inputString = Console.ReadLine()) != null)
            {
                if (inputString == "!dc") break; //If "!dc" is types in the consoles, the while-loop is exited and client is shut closed
                byte[] buffer = Encoding.ASCII.GetBytes(inputString);
                ns.Write(buffer, 0, buffer.Length);
            }

            //Shutdowns client
            client.Client.Shutdown(SocketShutdown.Send);
            thread.Join();
            ns.Close();
            client.Close();
            Console.WriteLine("Disconnected from server");
            Console.ReadKey();
        }

        static void ReceiveData(TcpClient client) //Method for receiving data from the broadcast method in the server
        {
            NetworkStream ns = client.GetStream();
            byte[] receivedBytes = new byte[1024];
            int byte_count;

            while ((byte_count = ns.Read(receivedBytes, 0, receivedBytes.Length)) > 0)
            {
                Console.Write(Encoding.ASCII.GetString(receivedBytes, 0, byte_count));
            }
        }
    }
}