using System;
using System.Net;
using System.Linq;
using System.Text;
using GaiaNet.GaiaNets;
using System.Net.Sockets;

namespace GaiaNet.Common
{
    public class NetTools
    {
        public static int progress = 0;

        public static IPEndPoint ParseIPPort(String ipstr){
            IPAddress ip = null;
            int port = Config.serverPort;
            IPEndPoint endpoint = null;

            if (ipstr.Contains(":")) { //if contains port.
                string[] ipPort = ipstr.Split(":");
                Boolean isIPOK = IPAddress.TryParse(ipPort[0], out ip);
                Boolean isPortOK = int.TryParse(ipPort[1], out port);
                if (isIPOK && isPortOK) endpoint = new IPEndPoint(ip, port); else return null;
            } else { //if doesn't contains port.
                Boolean isIPOK = IPAddress.TryParse(ipstr, out ip);
                if (isIPOK) endpoint = new IPEndPoint(ip, port); else return null;
            }

            return endpoint;

        }


    }


    /// <summary>
    /// A Class to conversion between string and NetStringHeader
    /// </summary>
    public class NetStringHeader
    {
        /*NetStringHeader Byte Struct:
        +------+-------------+
        | size |   string    |
        +------+-------------+
        |   4  |    len      |
        +------+-------------+

        size:      int(8)
        string:    string(len)
        */

        public int len { get; set; }
        public string str { get; set; }
        public byte[] bytes { get; set; }

        public NetStringHeader(){
            this.str = "";
            this.len = 0;
        }

        public NetStringHeader(String str){
            byte[] byteStr = Encoding.UTF8.GetBytes(str);
            byte[] byteLen = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(byteStr.Length));
            this.len = byteStr.Length;
            this.bytes = byteLen.Concat(byteStr).ToArray();
        }

        public NetStringHeader(byte[] bytes){
            this.len = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes[0..4]));
            this.str = Encoding.UTF8.GetString(bytes[4..^0]);
        }

        public string RecvString(Socket socket){
            byte[] bytesRecv = new byte[1024];
            int recvNum = 0;
            while (recvNum < 4) recvNum += socket.Receive(bytesRecv, recvNum, 1024, SocketFlags.None);
            this.len = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytesRecv[0..4]));
            if (recvNum > 4) this.bytes = bytesRecv[4..recvNum];
            int resSum = recvNum - 4;
            while ( resSum < this.len) {
                System.Console.WriteLine(resSum + " " + this.len );
                recvNum = socket.Receive(bytesRecv, 0, 1024, SocketFlags.None);
                this.bytes = this.bytes.Concat(bytesRecv[0..recvNum]).ToArray();
                resSum += recvNum;
            }

            this.str = Encoding.UTF8.GetString(this.bytes);
            return this.str;
        }

    }
}