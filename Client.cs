using GameServer2.PacketTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GameServer2
{
    internal class Client
    {
        public string Id;       // rn just dateTime

        // network stuff
        public int BufSize = ushort.MaxValue;
        public TcpClient? Sock;
        public NetworkStream? Stream;



        // match stuff
        public int? SelfIndex;     // 0 if added first or 1 if added second in Match.Player array
        public int? OppIndex;
        public string? MatchId;

        public Client(string _id) { Id = _id; }

        public static byte[] ObjToArr(Packet _pkt)
        {
            using(MemoryStream _ms = new())
            {
                var _serializer = new DataContractSerializer(typeof(Packet));
                _serializer.WriteObject(_ms, _pkt);
                return _ms.ToArray();
            }
        }

        public static Packet? ArrToObj(byte[] _data, int _len)
        {
            using (MemoryStream _ms = new(_data, 0, _len))
            {
                if (_len < 0) { return null; }
                var _serializer = new DataContractSerializer(typeof(Packet));
                var _temp = _serializer.ReadObject(_ms);
                if (_temp != null ) { return (Packet)_temp;  }
                return null;
            }
        }

        public async void Connect(TcpClient _clientSock)
        {
            Sock = _clientSock;
            Stream = _clientSock.GetStream();
            Console.WriteLine($"Connected to :{Sock.Client.RemoteEndPoint}");
            int _count = 0;
            char lastChar = 'a';
            while (true)
            {
                try
                {
                    // read 1st 2 bytes for length of the incoming packets
                    // then read again for with the length specified in a loop until the entire length is read and added to the complete array
                    


                    var _buffer = new byte[BufSize];
                    await Stream.ReadAsync(_buffer.AsMemory(0, 2));
                    int _dataLength = BitConverter.ToUInt16(_buffer, 0);
                    int offset = 0;

                    while (offset < _dataLength)
                    {
                        int received = await Stream.ReadAsync(_buffer.AsMemory(offset, _dataLength - offset));
                        if (received == 0)
                        {
                            Console.WriteLine($"Client {Sock.Client.RemoteEndPoint} disconnected.");
                            throw new Exception("disconnected");
                        }
                        offset += received;
                    }




                    // handle data
                    // after getting the packet obj (ArrToObj) check the type of object recieved and call the mapped function and send the obj as parameter to it...actions/funcs c#
                    // create an enum for it

                    var _rcvdObj = ArrToObj(_buffer, _dataLength);
                    if (_rcvdObj == null ) { continue; }
                    var temp = (SimpleMessage)_rcvdObj.Obj;
                    //Console.WriteLine($"message from {Id}: {_rcvdObj.Type}, {temp.SMsg}");
                    if (temp.SMsg.Last() == lastChar)
                    { _count++; }
                    else
                    {
                        Console.WriteLine(_count);
                        _count = 0;
                        lastChar = temp.SMsg.Last();
                    }

                    SendData($"hello to {Id} from server...");

                }
                catch (Exception ex) 
                {
                    Console.WriteLine(ex.ToString());
                    Server.ClientDic[Id].Sock.Close();
                    Server.ClientDic.Remove(Id);
                    return;
                }
                //if (Server.QueuedClient == null)
                //{
                //    Server.QueuedClient = this;
                //}
                //else
                //{
                //    Match NewMatch = new Match(Server.QueuedClient, this);
                //    Server.MatchDic[NewMatch.Id] = NewMatch;
                //    // send data "match found"
                //    SendData("Match found");
                //    Server.QueuedClient.SendData("Match found");

                //    Server.QueuedClient = null;
                //}
            }
        }

        

        public async void SendData(string s)
        {
            // 1st 2 bytes (ushort) for length of the packet
            // the entire serialized packet after that
            var _pktArr = ObjToArr(new Packet((int)PacketType.SimpleMessage, new SimpleMessage(s)));
            await Stream.WriteAsync(BitConverter.GetBytes((ushort)_pktArr.Length));
            await Stream.WriteAsync(_pktArr);
        }


        // tujhe dekhta hu baadme
        public void Disconnect()
        {
            Sock.Close();
            Server.ClientDic.Remove(Id);
        }
    }
}
