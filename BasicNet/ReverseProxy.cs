using System;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Collections.Generic;

namespace GaiaNet.BasicNet
{
    public class ReverseProxy
    {
        private Socket reverseSock = null;
        private Dictionary<string, Socket> reverseProxys = null;
        private String nodename;


        public ReverseProxy(Dictionary<string, Socket> reverseProxys){
            this.reverseProxys = reverseProxys;
        }

        public void buildReverseProxy(IPAddress ip, int port){
            IPEndPoint iPEndPoint = new(ip, port);
            this.reverseSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            reverseProxys.Add(nodename, reverseSock);
            reverseSock.BeginConnect(iPEndPoint, anynRes=>{
                reverseSock.EndConnect(anynRes);
                KeepConnected();

            }, null);

        }

        private bool KeepConnected(){
            byte[] heartbeat = new byte[] {0x01};  // For heartbeat data.
            bool isConnected = false;
            reverseSock.Send(heartbeat);
            int receiveNum = reverseSock.Receive(heartbeat);
            if (receiveNum == 1){
                isConnected = true;
                if (heartbeat[0] == 0x02){  // Other data.
                    
                }
            } else {
                isConnected = false;
            }
            return isConnected;
        }

        public void acceptReverseProxy(){

        }

        public void handle(Socket socket){
            Console.WriteLine("A new Reverse Proxy request from: ");
              
        }
    }
}