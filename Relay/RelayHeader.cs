using System.Net.Sockets;
using System;
using System.Net;
using System.Linq;
using System.Text;

/*HEAD STRUCT:
+----+------------+-------------+
|TYP |  LEN(int)  |    Name     |
+----+------------+-------------+
| 1  |      4     |    len      |
+----+------------+-------------+

TYP: IP=0x01, Node=0x02, Browser=0x03
LEN: bytes Transformed from int.
Names: Ip or Node Name.
*/

namespace GaiaNet.Relay
{
    public enum RelayType : byte { IP = 0x01, Node = 0x02};

    public class RelayHeader
    {
        private RelayType type;
        private int len;
        private string name;

        public string Name { get => name; set => name = value; }
        internal RelayType Type { get => type; set => type = value; }
        public int Len { get => len; set => len = value; }

        public RelayHeader(){ }
        public RelayHeader(RelayType type, string name){
            this.type = type;
            this.name = name;
        }

        public RelayHeader FromSocket(Socket sock)
        {
            RelayHeader header = new RelayHeader();
            try{
                byte[] byts = new byte[1024];
                int revnum = sock.Receive(byts, 0, 1, SocketFlags.None);
                if (revnum != 1) throw new Exception("Socket receive number is wrong");
                header.Type = (RelayType)byts[0];
                revnum = sock.Receive(byts, 0, 4, SocketFlags.None);
                if (revnum != 4) throw new Exception("Socket receive number is wrong");
                int lenNetworkOrder = BitConverter.ToInt32(byts[0..4]);
                header.Len = IPAddress.NetworkToHostOrder(lenNetworkOrder);
                revnum = sock.Receive(byts, 0, header.Len, SocketFlags.None);
                if (revnum != header.Len) throw new Exception("Socket receive number is wrong");
                header.Name = Encoding.UTF8.GetString(byts[0..header.Len]);
            } catch (System.Exception) {
                Console.WriteLine("Failed to parse the relay header.");
                sock.Close();
                return null;
            }
            return header;
        }

        public RelayHeader FromBytes(byte[] byts)
        {
            if (byts == null){ return null;}
            RelayHeader header = new RelayHeader();
            header.Type = (RelayType)byts[0];
            int lenNetworkOrder = BitConverter.ToInt32(byts[1..5]);
            header.Len = IPAddress.NetworkToHostOrder(lenNetworkOrder);
            header.Name = Encoding.UTF8.GetString(byts[5..^0]);
            return header;
        }

        public byte[] ToBytes()
        {try{
            byte[] byts = new byte[] {(byte)this.type};
            byte[] bytsName = Encoding.UTF8.GetBytes(this.name);
            this.Len = bytsName.Length;
            int lenNetworkOrder = IPAddress.HostToNetworkOrder(this.Len);
            byts = byts.Concat(BitConverter.GetBytes(lenNetworkOrder)).Concat(bytsName).ToArray();
            return byts;
        } catch (System.Exception){ return null;}
        }

    }
}