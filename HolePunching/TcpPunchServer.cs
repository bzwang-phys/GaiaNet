using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Collections;

namespace GaiaNet.HolePunching
{
    public class TcpPunchServer
    {
        private Dictionary<string, HolePunchSocket> socketsDic = new Dictionary<string, HolePunchSocket>();

        public void handle(Socket socket){
            while (true) {
                Console.WriteLine("A new socket request Tcp Hole Punching");
                HolePunchSocket holePunchSocket = ParseTcpPunchHeader(socket);
                socketsDic.Add(holePunchSocket.name, holePunchSocket);
                CheckPeers();
            } 
        }

        private void CheckPeers(){
            List<string> delKeys = new List<string>();
            foreach (var item in socketsDic){
                HolePunchSocket hps = item.Value;
                if (socketsDic.ContainsKey(hps.peer)){
                    HolePunchSocket hpsNeed;
                    socketsDic.TryGetValue(hps.peer, out hpsNeed);
                    IPEndPoint epNeed = (IPEndPoint)hpsNeed.socket.RemoteEndPoint;
                    byte[] sendep = Encoding.ASCII.GetBytes(epNeed.ToString() +"-"+ epNeed.ToString());
                    hps.socket.Send(sendep, sendep.Length, 0);
                    delKeys.Add(item.Key);
                }
            }

            foreach (string item in delKeys){
                HolePunchSocket hps;
                socketsDic.TryGetValue(item, out hps);
                hps.socket.Close();
                socketsDic.Remove(item);
            }

        }

        private HolePunchSocket ParseTcpPunchHeader(Socket socket){
            byte[] recByte = new byte[4096];
            int bytes = socket.Receive(recByte);
            IPEndPoint ep1 = (IPEndPoint)socket.RemoteEndPoint;
            int port = ep1.Port;
            string ip = ep1.Address.ToString();
            string name = "";
            string peer = "";

            return new HolePunchSocket(ip, port, name, peer, socket);
        }
      

    private class HolePunchSocket{
        public string ip;
        public int port;
        public string name;
        public string peer;
        public Socket socket;
        public HolePunchSocket(string ip, int port, string name, string peer, Socket socket){
            this.ip = ip;
            this.peer = peer;
            this.socket = socket;
            this.name = name;
        }
    }


    }

}