using System.Net.Sockets;
using System;
using System.Net;
using System.Linq;
using System.Text;

/*HEAD STRUCT:
+----+------------+------------+-------------+
|TYP |RelayID(int)|  LEN(int)  |    Name     |
+----+------------+------------+-------------+
| 1  |      4     |      4     |    len      |
+----+------------+------------+-------------+

TYP:         IP=0x01, Node=0x02, Browser=0x03
RelayID(int):A random integer, (maybe repeated)
LEN:         bytes Transformed from int.
Names:       Ip or Node Name.
*/

namespace GaiaNet.Relay
{
    public enum RelayType : byte { IP = 0x01, Node = 0x02};

    public class RelayHeader
    {
        private RelayType type;
        private int id;
        private int len;
        private string name;

        public string Name { get => name; set => name = value; }
        internal RelayType Type { get => type; set => type = value; }
        public int Len { get => len; set => len = value; }
        public int Id { get => id; set => id = value; }

    public RelayHeader(){ }
        public RelayHeader(RelayType type, string name){
            this.type = type;
            this.name = name;
            this.id = new System.Random().Next(10000);
        }

        public RelayHeader FromSocket(Socket sock)
        {
            RelayHeader header = new RelayHeader();
            try{
                byte[] byts = new byte[1024];
                int revnum = sock.Receive(byts, 0, 1, SocketFlags.None);
                if (revnum != 1) throw new Exception("Socket receive number is wrong");
                header.Type = (RelayType)byts[0];

                revnum = sock.Receive(byts, 0, 8, SocketFlags.None);
                if (revnum != 8) throw new Exception("Socket receive number is wrong");
                header.Id = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(byts[0..4]));
                header.Len = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(byts[4..8]));

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
            header.Id = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(byts[1..5]));
            header.Len = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(byts[5..9]));
            header.Name = Encoding.UTF8.GetString(byts[9..^0]);
            return header;
        }

        public byte[] ToBytes()
        {try{
            byte[] byts = new byte[] {(byte)this.type};
            byte[] idByte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(this.id));
            byte[] bytsName = Encoding.UTF8.GetBytes(this.name);
            this.Len = bytsName.Length;
            byte[] lenByte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(this.Len));
            byts = byts.Concat(idByte).Concat(lenByte).Concat(bytsName).ToArray();
            return byts;
        } catch (System.Exception){ return null;}
        }

    }
}