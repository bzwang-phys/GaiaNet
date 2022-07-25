using System.Net.Sockets;
using System;

namespace GaiaNet.BasicNet
{
    public enum NetType:byte {Others=0x00,Command=0x01, File=0x02, GaiaNet=0x03, Relay=0x04, HolePunch=0x05, ReverseProxy=0x06 };
    public class NetHeader
    {
        public int headerLen = 1;
        public NetType netType {get;set;}

        public NetType ReadHeader(Socket socket){
            try{
                byte[] byts = new byte[headerLen];
                int num = socket.Receive(byts);
                if (num == headerLen){
                    netType = (NetType) byts[0];
                }else{
                    Console.WriteLine("Failed to read NetHeader.");
                }
                return netType;
            }
            catch (System.Exception e){
                Console.WriteLine("Failed in reading NetHeader." + e);
                return NetType.Others;
            }
            
        }
    }
}