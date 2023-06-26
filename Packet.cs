using GameServer2.PacketTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GameServer2
{
    [DataContract(Namespace = "gamePacket")]
    [KnownType(typeof(SimpleMessage))]
    public class Packet
    {
        [DataMember] public int Type;
        [DataMember] public object Obj;

        public Packet(int _type, object _obj)
        {
            this.Type = _type;
            this.Obj = _obj;
        }
    }
}
