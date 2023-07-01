using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using UnityEngine;
using Assets.Scripts.PacketTypes;
using System.Xml;
using System.Threading.Tasks;

public class Client : MonoBehaviour
{
    public static Client Instance;

    public string IpAddress = "127.0.0.1";
    public int port = 4000;
    public int Id = 0;

    public static int BufSize = ushort.MaxValue;
    public TcpClient Sock;
    public NetworkStream Stream;

    public bool IsConnected = false;

    enum PacketType
    {
        SimpleMessage = 0,
        CharState = 1
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Debug.Log("Client Instance already exists");
            Destroy(Instance);
        }
    }
    public byte[] ObjToArr(Packet _pkt)
    {
        using (MemoryStream _ms = new())
        {
            var _serializer = new DataContractSerializer(typeof(Packet));
            _serializer.WriteObject(_ms, _pkt);
            return _ms.ToArray();
        }
    }

    public static Packet ArrToObj(byte[] _data, int _len)
    {
        using (MemoryStream _ms = new(_data, 0, _len))
        {
            var _serializer = new DataContractSerializer(typeof(Packet));
            try
            {
                var _pkt = (Packet)_serializer.ReadObject(_ms);
                return _pkt;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
            return null;
        }
    }

    public async void ConnectToServer()
    {
        Sock = new TcpClient();
        try
        {
            Sock.Connect(IpAddress, port);
            Stream = Sock.GetStream();
            IsConnected = true;
            Debug.Log("so far so good");
            await SendData("ehe");
            while (true)
            {
                await RecvData();
            }
        }
        catch (Exception ex)
        {
            //if (ex is SocketException || ex is InvalidOperationException || ex is ObjectDisposedException)
            Debug.Log($"Could not connect: {ex}");
        }
    }


    public void Disconnect()
    {
        Sock.Close();
        IsConnected = false;
        UIManager.Instance.Disconnected();
    }
        

    private async Task RecvData()
    {
        if (!IsConnected) { return; }
        try
        {
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
            var temp = (SimpleMessage)_rcvdObj.Obj;
            Debug.Log($"message from server: {_rcvdObj.Type}, {temp.SMsg}");

            await SendData($"new hello at {DateTime.Now}");
            //await SendData($"follow up");

        }
        catch (Exception e)
        {
            Debug.Log("oh brother");
            Debug.Log(e.ToString());
            Disconnect();
        }
    }

    private async Task SendData(string msg)
    {
        try
        {
            var _pktArr = ObjToArr(new Packet((int)PacketType.SimpleMessage, new SimpleMessage(msg)));
            ushort _len = (ushort) _pktArr.Length;
            Debug.Log(_len);
            await Stream.WriteAsync(BitConverter.GetBytes(_len));
            await Stream.WriteAsync(_pktArr);
            //await Stream.WriteAsync(ObjToArr(new Packet(1, new SimpleMessage(msg))));
        }
        catch (SocketException e)
        {
            Debug.Log("brother idhar");
            Debug.Log(e.ToString());
            Disconnect();
        }
    }
    
}
