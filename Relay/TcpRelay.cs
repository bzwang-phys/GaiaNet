using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Net.Http.Headers;
using System.Net;

namespace GaiaNet.Relay
{
    public class TcpRelay
    {
        private Socket inSocket;
        private Socket outSocket;
        private byte[] inByts = new byte[2048];
        private byte[] outByts = new byte[2048];
        private int inBytsNum, outBytsNum, closeSockNum=0;
        private Boolean upStreamOpen = true;
        private Boolean downStreamOpen = true;

        public TcpRelay(Socket inSocket){
            this.inSocket = inSocket;
        }

        private byte[] InByts(){
            return inByts.Take(inBytsNum).ToArray();
        }
        private byte[] OutByts(){
            return outByts.Take(outBytsNum).ToArray();
        }

        public void Relay(){
            try {
                Console.WriteLine("Receive Relay request from: " + inSocket.RemoteEndPoint);
                IPEndPoint iPEndPoint = GetIPFromHeader();
                if (iPEndPoint == null) { Console.WriteLine("Failed to get ip."); return; }
                outSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                AsyncRelay(iPEndPoint);

            } catch (Exception e) {
                Console.WriteLine(e);
            }
        }
        
        private void AsyncRelay(IPEndPoint iPEndPoint){
            outSocket.BeginConnect(iPEndPoint, asyncRes=>{
                    outSocket.EndConnect(asyncRes);
                    outSocket.Send(InByts());
                    UpStream();
                    DownStream();
                }, null);
        }

        private void UpStream(){
            try{
                inSocket.BeginReceive(inByts, 0, inByts.Length, SocketFlags.None, asyncRes=>{
                    inBytsNum = inSocket.EndReceive(asyncRes);
                    if (inBytsNum == 0 || !upStreamOpen || !downStreamOpen){ // Close the Relay.
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


        private void DownStream(){
            try{
                outSocket.BeginReceive(outByts, 0, outByts.Length, SocketFlags.None, asyncRes=>{
                    outBytsNum = outSocket.EndReceive(asyncRes);
                    if (outBytsNum == 0 || !upStreamOpen || !downStreamOpen){  // Close the Relay.
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
                inBytsNum = inSocket.Receive(inByts);
                string headers = Encoding.UTF8.GetString(inByts.Take(inBytsNum).ToArray());  //Maybe not the UTF8 code.

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