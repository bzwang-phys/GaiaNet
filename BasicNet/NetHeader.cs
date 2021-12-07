using System.Net.Sockets;
using System;

namespace GaiaNet.BasicNet
{
    public class NetHeader
    {
        public int headerLen = 1;
        public NetType netType {get;set;}

        public void ReadHeader(Socket socket){
            byte[] byts = new byte[headerLen];
            int num = socket.Receive(byts);
            if (num == headerLen){
                netType = (NetType) byts[0];
            }else{
                Console.WriteLine("Failed to read NetHeader.");
            }
            
        }
    }
}