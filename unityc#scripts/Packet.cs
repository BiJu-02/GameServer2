using Assets.Scripts.PacketTypes;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;


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
