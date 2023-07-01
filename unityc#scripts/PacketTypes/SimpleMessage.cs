using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace Assets.Scripts.PacketTypes
{
    [DataContract(Namespace = "gamePacket")]
    public class SimpleMessage
    {
        [DataMember] public string SMsg;

        public SimpleMessage(string s)
        {
            this.SMsg = s;
        }
    }
}
