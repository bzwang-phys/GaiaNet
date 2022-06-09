using System;
using GaiaNet.GaiaNets;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Threading;
using GaiaNet.Command;
using System.Text;
using GaiaNet.Relay;
using GaiaNet.HolePunching;
using System.Collections.Generic;

namespace GaiaNet.BasicNet
{
    public class MultiThreadServer
    {
        // private static const Logger logServer = Logger.getLogger( MultiThreadServer.class.getName() );
        // public FileHandler fileHandler;  // For log file.
        public Net gaiaNet = null;
        private int _port;
        public String BasePath { get; set; }
        private TcpListener _server;
        private Boolean _isRunning;
        private Dictionary<string, Socket> reverseProxys = new Dictionary<string, Socket>();

        public MultiThreadServer(int port):this(port, null) {
            _port = port;
        }

        public MultiThreadServer(int port, Net net) {
            _port = port;
            try{
                _server = new TcpListener(IPAddress.Any, _port);
                BasePath = Directory.GetCurrentDirectory();
                _isRunning = true;
            }
            catch (IOException e){
                Console.WriteLine(e);
            }
            gaiaNet = net;
        }

        public void serverRun(){
            _server.Start();
            while (_isRunning){
                try {
                    Socket newClient = _server.AcceptSocket();
                    Console.WriteLine("Connection from: " + newClient.RemoteEndPoint);
                    
                    // NetType type = NetType.Relay;
                    NetType type = new NetHeader().ReadHeader(newClient);
                    Console.WriteLine("GaiaType from this connection: " + type.ToString());

                    if (type == NetType.Command){
                        new Thread(()=> new CommandHandler().handle(newClient)).Start();
                    } else if (type == NetType.File){  // file transfer
                        //fileheader fileheader = new fileheader();
                        //fileheader.receive(dis);
                        //logserver.info("file from: "+socket.getremotesocketaddress()+ " file name: "+fileheader.name+" type: "+fileheader.type);
                        //if (fileheader.type == 0)  createfile(socket,dis,fileheader);
                        //if (fileHeader.type == 1)  new Thread(()-> threadRcvFile(socket,dis,fileHeader)).start();
                    } else if (type == NetType.GaiaNet){
                        //new Thread(()-> this.gaiaNet.net.handle(socket, logServer)).start();
                    } else if (type == NetType.Relay){
                        new Thread( ()=>new TcpRelay(newClient).Relay() ).Start();
                    } else if (type == NetType.HolePunch) {
                        new Thread( ()=>new TcpPunchServer().handle(newClient) ).Start();
                    } else if (type == NetType.ReverseProxy) {
                        new Thread( ()=>new ReverseProxy(reverseProxys).handle(newClient) ).Start();
                    } else {
                        Console.WriteLine("MultiThreadServer: Unknown Socket.");
                    }
                } catch (IOException e) {
                    Console.WriteLine(e);
                }

            }
        }

        public void stop() {
            try {
                _server.Stop();
            } catch (IOException e) {
                Console.WriteLine(e);
            }
        }

        // Just print out the stream with hex string
        //public void printStream(Socket socket, DataInputStream dis){
        //    try {
        //        byte[] byt = new byte[10];
        //        dis.read(byt);
        //        System.out.print(ByteTools.HexString(byt));
        //    } catch (IOException e) {
        //        e.printStackTrace();
        //    }

        //}
    }
}