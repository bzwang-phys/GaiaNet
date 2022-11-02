using System;
using GaiaNet.GaiaNets;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Threading;
using GaiaNet.Command;
using System.Text;
using GaiaNet.Relay;
using GaiaNet.FilesTransfer;
using GaiaNet.HolePunching;
using System.Collections.Generic;

namespace GaiaNet.BasicNet
{
    public class MultiThreadServer
    {
        public Net gaiaNet = null;
        private int _port;
        public String BasePath { get; set; }
        private TcpListener _server;
        private Boolean _isRunning;
        private Dictionary<string, Socket> reverseProxys = new Dictionary<string, Socket>();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().ReflectedType);

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
            catch (IOException e){ log.Error(e); }
            gaiaNet = net;
        }

        public void serverRun(){
            _server.Start();
            log.Info(String.Format("MultiThreadServer has started at: {0}", _port));
            
            while (_isRunning){
                try {
                    Socket newClient = _server.AcceptSocket();
                    NetType type = new NetHeader().ReadHeader(newClient);
                    log.Info(String.Format("Connection: {0} with GaiaType {1}",newClient.RemoteEndPoint,type.ToString()));

                    switch (type)
                    {
                        case NetType.Command:
                            // new Thread(()=> new CommandHandler().handle(newClient)).Start();
                            break;
                        case NetType.File:
                            new Thread( ()=>new RecvFiles(newClient).Recv() ).Start();
                            break;
                        case NetType.GaiaNet:
                            //new Thread(()-> this.gaiaNet.net.handle(socket, logServer)).start();
                            break;
                        case NetType.Relay:
                            new Thread( ()=>new TcpRelay(newClient).Relay() ).Start();
                            break;
                        case NetType.HolePunch:
                            // new Thread( ()=>new TcpPunchServer().handle(newClient) ).Start();
                            break;
                        case NetType.ReverseProxy:
                            // new Thread( ()=>new ReverseProxy(reverseProxys).handle(newClient) ).Start();
                            break;
                        default:
                            log.Info("MultiThreadServer: Unknown Socket.");
                            break;
                    }

                } catch (IOException e) {
                    log.Error(e);
                }

            }
        }

        public void stop() {
            try {
                _server.Stop();
            } catch (IOException e) { log.Error(e); }
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