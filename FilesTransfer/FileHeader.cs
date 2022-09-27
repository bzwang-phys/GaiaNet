using System;
using System.Net;
using System.Linq;
using System.Text;

/*FileHeader Byte Struct:
+------+-------+-------+-------+------------+-------------+
| size | start |  end  | index |  len(int)  |    name     |
+------+-------+-------+-------+------------+-------------+
|   8  |   8   |   8   |   8   |      4     |    len      |
+------+-------+-------+-------+------------+-------------+

size:    long
start:   long
end:     long
index:   long
LEN:     int
Name:    len
*/


namespace GaiaNet.FilesTransfer
{
    public enum TransferType {LOCAL, NET};
    public enum  {LOCAL, NET};
    public class FileHeader
    {
        public byte type; // 0x01 : request, 0x02:transfer.
        public string name;
        public long size;
        public long start;
        public long end;
        public long index;
        public string destPath;

        public FileHeader(byte type, string name, long size){
            this.name = name;
            this.size = size;
            this.start = 0L;
            this.end = 0L;
            this.index = 0L;
        }
        public FileHeader(string name, long start, long end, long index){
        this.name = name;
        this.size = 0L;
        this.start = start;
        this.end = end;
        this.index = index;
        }

        public FileHeader(){
            this.name = "";
            this.size = 0L;
            this.start = 0L;
            this.end = 0L;
            this.index = 0L;
        }
        
        public FileHeader FromBytes(byte[] byts)
        {
            if (byts == null){ return null;}
            FileHeader header = new FileHeader();
            // header.Type = (RelayType)byts[0];
            // header.Id = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(byts[1..5]));
            // header.Len = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(byts[5..9]));
            // header.Name = Encoding.UTF8.GetString(byts[9..^0]);
            return header;
        }

        public byte[] ToBytes()
        {try{
            byte[] byts = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(this.size));
            byte[] startByte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(this.start));
            byte[] endByte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(this.end));
            byte[] indexByte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(this.index));
            byte[] nameByte = Encoding.UTF8.GetBytes(this.name);
            int nameLen = nameByte.Length;
            byte[] lenByte = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(nameLen));

            byts = byts.Concat(startByte).Concat(endByte).Concat(indexByte).Concat(lenByte).Concat(nameByte).ToArray();
            return byts;
        } catch (System.Exception){ return null;}
        }
    }
}