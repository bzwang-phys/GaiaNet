using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Net;

namespace GaiaNet.Relay
{
    public class TcpRelay
    {
        private Socket inSocket;
        private Socket outSocket;
        private byte[] upByts = new byte[2048];
        private byte[] downByts = new byte[2048];
        private int upBytsNum, downBytsNum, closeSockNum=0;
        private Boolean upStreamOpen = true;
        private Boolean downStreamOpen = true;

        public TcpRelay(Socket inSocket){
            this.inSocket = inSocket;
        }

        private byte[] InByts(){
            return upByts.Take(upBytsNum).ToArray();
        }
        private byte[] OutByts(){
            return downByts.Take(downBytsNum).ToArray();
        }

        public void Relay(){
            try {
                Console.WriteLine("Receive Relay request from: " + inSocket.RemoteEndPoint);
                IPEndPoint iPEndPoint = GetIPFromHeader();
                if (iPEndPoint == null) { Console.WriteLine("Failed to get ip.");inSocket.Close();return; }
                outSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                AsyncRelay(iPEndPoint);

            } catch (Exception e) {
                Console.WriteLine(e);
            }
        }
        
        // Based on event, Every TcpRelay has inSocket and outSocket to upstream / downstream
        private void AsyncRelay(IPEndPoint iPEndPoint){
            outSocket.BeginConnect(iPEndPoint, asyncRes=>{
                    outSocket.EndConnect(asyncRes);
                    if (iPEndPoint.Port == 443){  //https
                        inSocket.Send(Encoding.ASCII.GetBytes("HTTP/1.1 200 Connection Established\r\n\r\n"));
                    } else{ outSocket.Send(InByts()); } // http and others.

                    UpStream();
                    DownStream();
                }, null);
        }

        // inSocket --> UpStream --> outSocket
        private void UpStream(){
            try{
                inSocket.BeginReceive(upByts, 0, upByts.Length, SocketFlags.None, asyncRes=>{
                    upBytsNum = inSocket.EndReceive(asyncRes);
                    Console.WriteLine("UpStream, Receive data: " + downBytsNum);
                    if (upBytsNum == 0 || !upStreamOpen || !downStreamOpen){ // Close the Relay.
                        StopRelay();
                        CloseSocket();
                        return;
                    }
                    outSocket.BeginSend(InByts(), 0, InByts().Length, SocketFlags.None, asyncRes1=>{
                        outSocket.EndSend(asyncRes1);
                        UpStream();
                    }, null);
                }, null);
            }catch (System.Exception e){
                Console.WriteLine(e);
            }
            
        }


        // outSocket --> DownStream --> inSocket
        private void DownStream(){
            try{
                outSocket.BeginReceive(downByts, 0, downByts.Length, SocketFlags.None, asyncRes=>{
                    downBytsNum = outSocket.EndReceive(asyncRes);
                    Console.WriteLine("DownStream, Receive data: " + downBytsNum);
                    if (downBytsNum == 0 || !upStreamOpen || !downStreamOpen){  // Close the Relay.
                        StopRelay();
                        CloseSocket();
                        return;
                    }
                    inSocket.BeginSend(OutByts(), 0, OutByts().Length, SocketFlags.None, asyncRes1=>{
                        inSocket.EndSend(asyncRes1);
                        DownStream();
                    }, null);
                }, null);
            } catch (System.Exception e){
                Console.WriteLine(e);
            }
        }

        private void StopRelay(){
            upStreamOpen = false;
            downStreamOpen = false;
        }
        private void CloseSocket(){
            closeSockNum += 1;
            if (closeSockNum > 1){ //Only work when this is called by both DownStream() and UpStream().
                inSocket.Close();
                outSocket.Close();
            }
        }


        private IPEndPoint GetIPFromHeader(){
            try {
                upBytsNum = inSocket.Receive(upByts);
                string headers = Encoding.UTF8.GetString(upByts.Take(upBytsNum).ToArray());  //Maybe not the UTF8 code.

                // find the host name from the headers.
                char[] delimiterChars = {' ', '\n'};
                string[] headersSplit = headers.Split(delimiterChars);
                string host = "";
                int port = 0;
                for (int i = 0; i < headersSplit.Length; i++){
                    if (headersSplit[i].Contains("Host:")){
                        string hostport = headersSplit[i+1];
                        host = hostport.Split(":")[0].Trim();
                        port = 80;
                        if (hostport.Split(":").Length > 1){
                            port = int.Parse( hostport.Split(":")[1].Trim() );
                        }
                        break;
                    }
                };

                // get ip from the hostname with the DNS query.
                IPAddress[] iPAddresses = Dns.GetHostAddresses(host);
                if (iPAddresses.Length == 0){
                    Console.WriteLine("Can not resolve the ip from: " + host);
                    return null;
                }
                IPEndPoint iPEndPoint = new IPEndPoint(iPAddresses[0], port);
                return iPEndPoint;
            } catch (Exception e) {
                Console.WriteLine(e);
                return null;
            }
        }






    }
}