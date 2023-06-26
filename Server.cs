using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer2
{
    enum PacketType
    {
        SimpleMessage = 0,
        CharState = 1
    }
    internal class Server
    {
        private static TcpListener? listener;
        public static Dictionary<string, Client> ClientDic = new();
        public static Dictionary<string, Match> MatchDic = new();
        public static Client? QueuedClient;
        // in a 1v1 game only one player of a particular league should be waiting at a time...technically, logically,...what could go wrong right

        public static  void Start(int _port)
        {
            Console.WriteLine("starting server...");
            Console.WriteLine(DateTime.Now.ToString());

            listener = new TcpListener(IPAddress.Any, _port);
            listener.Start();

            while (true)
            {
                Console.WriteLine("waiting for connection...");
                TcpClient? _sock = listener.AcceptTcpClient();
                if (_sock.Client.RemoteEndPoint != null ) 
                {
                    string _tempId = _sock.Client.RemoteEndPoint.ToString();
                    ClientDic.Add(_tempId, new Client(_tempId));
                    ClientDic[_tempId].Connect(_sock);      // handle match as well
                }
            }
        }

    }
}
