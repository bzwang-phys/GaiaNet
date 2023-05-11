using System;
using System.Net;
using System.Linq;
using System.Text;

/*FileHeader Byte Struct:
+------+------+-------+-------+-------+------------+-------------+
|  op  | size | start |  end  | index |  len(int)  |    name     |
+------+------+-------+-------+-------+------------+-------------+
|   1  |   8  |   8   |   8   |   8   |      4     |    len      |
+------+------+-------+-------+-------+------------+-------------+

op:      byte(1)
size:    long(8)
start:   long(8)
end:     long(8)
index:   long(8)
LEN:     int(4)
Name:    string(len)
*/


namespace GaiaNet.FilesTransfer
{
    public enum TransferType {LOCAL, NET};
    public enum FileOperator:byte {REQUEST, TRANSFER, CHECKMD5};
    public class FileHeader
    {
        public FileOperator op;
        public string name;
        public long size;
        public long start;
        public long end;
        public long index;
        public string destPath;
        public Int32 nameLen = 0;

        public FileHeader(FileOperator op, byte type, string name, long size){
            this.op = op;
            this.name = name;
            this.size = size;
            this.start = 0L;
            this.end = 0L;
            this.index = 0L;
        }
        public FileHeader(FileOperator op, string name, long start, long end, long index){
            this.op = op;
            this.name = name;
            this.size = 0L;
            this.start = start;
            this.end = end;
            this.index = index;
        }

        public FileHeader(){
            this.op = FileOperator.REQUEST;
            this.name = "";
            this.size = 0L;
            this.start = 0L;
            this.end = 0L;
            this.index = 0L;
        }
        
        public static FileHeader FromBytes(byte[] byts)
        {
            if (byts == null){ return null;}
            FileHeader header = new FileHeader();
            header.op = (FileOperator)byts[0];
            header.size = IPAddress.NetworkToHostOrder(BitConverter.ToInt64(byts[1..9]));
            header.start = IPAddress.NetworkToHostOrder(BitConverter.ToInt64(byts[9..17]));
            header.end = IPAddress.NetworkToHostOrder(BitConverter.ToInt64(byts[17..25]));
            header.index = IPAddress.NetworkToHostOrder(BitConverter.ToInt64(byts[25..33]));
            header.nameLen = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(byts[33..37]));
            return header;
        }

        public byte[] ToBytes()
        {try{
            byte[] byts = new byte[] {(byte)this.op};
            byte[] nameByte = Encoding.UTF8.GetBytes(this.name);
            int nameLen = nameByte.Length;

            byts = byts.Concat(Int2Byte(size)).Concat(Int2Byte(start)).Concat(Int2Byte(end))
                   .Concat(Int2Byte(index)).Concat(Int2Byte(nameLen)).Concat(nameByte).ToArray();
            return byts;
        } catch (System.Exception){ return null;}
        }

        private byte[] Int2Byte(int i){
            return BitConverter.GetBytes(IPAddress.HostToNetworkOrder(i));
        }
        private byte[] Int2Byte(long l){
            return BitConverter.GetBytes(IPAddress.HostToNetworkOrder(l));
        }
    }
}