using System.Runtime.Serialization;

namespace MM2p2msg;

[DataContract]
public class Contacts
{
    [DataMember]
    public string name { get; set; }
    [DataMember]
    public string ip { get; set; }
    [DataMember]
    public int port { get; set; }
}

//ja: lkashdfkljasdf : 13:45 
//ty: fdaslfjkaslkdfj: ..
//ja: aflkadsjfklas : ..