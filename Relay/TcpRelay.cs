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
        private Socket _inSocket;
        private TcpClient _outSocket;
        private byte[] _byts = new byte[2048];

        public TcpRelay(Socket inSocket){
            _inSocket = inSocket;
        }

        public void Relay(){
            try {
                Console.WriteLine("Receive command from: " + _inSocket.RemoteEndPoint);
                IPEndPoint iPEndPoint = GetIPFromHeader();
                if (iPEndPoint == null) { Console.WriteLine("Failed to get ip."); return; }
                _outSocket = new TcpClient(iPEndPoint);
                Socket.Select();
                
            
            } catch (Exception e) {
                Console.WriteLine(e);
            }
        }

        private IPEndPoint GetIPFromHeader(){
            try {
                int num = _inSocket.Receive(_byts);
                string headers = Encoding.UTF8.GetString(_byts.Take(num).ToArray());  //Maybe not the UTF8 code.

                // find the host name from the headers.
                char[] delimiterChars = {' ', '\n'};
                string[] headersSplit = headers.Split(delimiterChars);
                string host = "";
                int port = 0;
                for (int i = 0; i < headersSplit.Length; i++){
                    if (headersSplit[i].Contains("Host:")){
                        host = headersSplit[i+1];
                        host = host.Split(":")[0].Trim();
                        port = int.Parse( host.Split(":")[1].Trim() );
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