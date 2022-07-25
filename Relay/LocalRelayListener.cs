using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using GaiaNet.GaiaNets;

namespace GaiaNet.Relay
{
    public class LocalRelayListener
    {
        private int _port;
        private TcpListener _server;
        private Boolean _isRunning;
        private string relayNode;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().ReflectedType);

        public LocalRelayListener(int port) {
            try{
                this._port = port;
                this._server = new TcpListener(IPAddress.Any, _port);
                this._isRunning = true;
                this.relayNode = Config.relayNode;
                if (_server==null || relayNode==null ){
                    log.Info("relayListener can not be initialized");
                    return;
                }
            } catch (IOException e){ Console.WriteLine(e); }
        }

        public void serverRun(){
            _server.Start();
            while (_isRunning){
                try {
                    Socket newClient = _server.AcceptSocket();
                    IPEndPoint remoteIp = (IPEndPoint)newClient.RemoteEndPoint;
                    // log.Info("connection from: " + remoteIp);
                    if (!IPAddress.IsLoopback(remoteIp.Address)) { 
                        log.Info("Unknow danger connection from: " + remoteIp);
                        continue;
                    }
                    byte[] headerbyts = new RelayHeader(RelayType.Node, this.relayNode).ToBytes();
                    new Thread( ()=>new TcpRelay(headerbyts, newClient).Relay() ).Start();
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
    }
}