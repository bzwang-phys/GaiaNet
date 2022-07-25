using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Net;
using GaiaNet.GaiaNets;
using GaiaNet.BasicNet;

namespace GaiaNet.Relay
{
  public class TcpRelay
  {
    private Socket inSocket;
    private TcpClient outSocket;
    private byte[] upByts = new byte[1024*3000];
    private byte[] downByts = new byte[1024*3000];
    private int upBytsNum, downBytsNum, closeSockNum=0;
    private Boolean upStreamOpen = true;
    private Boolean downStreamOpen = true;

    private Boolean isStartPoint = false;
    private Boolean isEndPoint = false;
    private RelayHeader relayHeader = null;

    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(
      System.Reflection.MethodBase.GetCurrentMethod().ReflectedType);

    public TcpRelay(Socket inSocket){
      this.inSocket = inSocket;
    }
    public TcpRelay(byte[] header, Socket inSocket){
      if (header==null || inSocket==null) return;
      this.isStartPoint = true;
      this.relayHeader = new RelayHeader().FromBytes(header);
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
        log.Info("Receive Relay request from: " + inSocket.RemoteEndPoint);
        if (!isStartPoint){ this.relayHeader = new RelayHeader().FromSocket(inSocket);}
        
        IPEndPoint relayDest = GetRealyDest(this.relayHeader);
        if (relayDest == null) { log.Info("Failed to get ip.");inSocket.Close();return; }
        outSocket = new TcpClient(); // Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        AsyncRelay(relayDest);

      } catch (Exception e) { log.Info(e); }
    }
    
    // Based on event, Every TcpRelay has inSocket and outSocket to upstream / downstream
    private void AsyncRelay(IPEndPoint iPEndPoint){
      outSocket.BeginConnect(iPEndPoint.Address, iPEndPoint.Port, asyncRes=>{
        try{
          outSocket.EndConnect(asyncRes);
          if (!isEndPoint){    // continue to relay
            byte[] headers = (new byte[] {(byte)NetType.Relay}).Concat(this.relayHeader.ToBytes()).ToArray();
            outSocket.Client.Send(headers);
          } else {
            if (iPEndPoint.Port == 443){  //https
              inSocket.Send(Encoding.ASCII.GetBytes("HTTP/1.1 200 Connection Established\r\n\r\n"));
            } else{ outSocket.Client.Send(InByts()); } // http and others.
          }
          UpStream();
          DownStream();
        } catch (System.Exception) { StopRelay(); CloseSocket(); }
      }, null);
    }

    // inSocket --> UpStream --> outSocket
    private void UpStream(){
      inSocket.BeginReceive(upByts, 0, upByts.Length, SocketFlags.None, asyncRes=>{try{
        // every asyn need a try/catch
        upBytsNum = inSocket.EndReceive(asyncRes);
        if (upBytsNum == 0 || !upStreamOpen || !downStreamOpen){ // Close the Relay.
          StopRelay();
          CloseSocket();
          return;
        }
        // System.Console.WriteLine(Encoding.UTF8.GetString(InByts()));
        outSocket.Client.BeginSend(InByts(), 0, InByts().Length, SocketFlags.None, asyncRes1=>{try{
          outSocket.Client.EndSend(asyncRes1);
          UpStream();
          }catch (System.Exception){log.Error("outSocket send failed"); StopRelay(); CloseSocket(); }
        }, null);
        }catch (System.Exception){log.Error("inSocket receive and outSocket send failed ");
            StopRelay(); CloseSocket(); }
      }, null);

    }


    // outSocket --> DownStream --> inSocket
    private void DownStream(){
      outSocket.Client.BeginReceive(downByts, 0, downByts.Length, SocketFlags.None, asyncRes=>{try{
        downBytsNum = outSocket.Client.EndReceive(asyncRes);
        if (downBytsNum == 0 || !upStreamOpen || !downStreamOpen){  // Close the Relay.
          StopRelay();
          CloseSocket();
          return;
        }
        // System.Console.WriteLine("DownStream");
        // System.Console.WriteLine(Encoding.UTF8.GetString(OutByts()));
        inSocket.BeginSend(OutByts(), 0, OutByts().Length, SocketFlags.None, asyncRes1=>{try{
          inSocket.EndSend(asyncRes1);
          DownStream();
          }catch (System.Exception){log.Error("inSocket send failed");StopRelay(); CloseSocket(); }
        }, null);
        }catch (System.Exception){ log.Error("outSocket receive and inSocket send failed");
            StopRelay(); CloseSocket(); }
      }, null);
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
        System.Console.WriteLine(upBytsNum);
        string headers = Encoding.UTF8.GetString(upByts.Take(upBytsNum).ToArray());  //Maybe not the UTF8 code.

        System.Console.WriteLine("headers: " + headers);
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
        log.Info("Host: " + host);

        // get ip from the hostname with the DNS query.
        IPAddress[] iPAddresses = Dns.GetHostAddresses(host);
        if (iPAddresses.Length == 0){
            log.Info("Can not resolve the ip from: " + host);
            return null;
        }
        IPEndPoint iPEndPoint = new IPEndPoint(iPAddresses[0], port);
        return iPEndPoint;
      } catch (Exception e) {log.Info(e); return null;}
    }

    private IPEndPoint GetRealyDest(RelayHeader relayHeader){
        IPEndPoint relayDest = null;
        if (relayHeader.Type == RelayType.IP){
            relayDest = new IPEndPoint(IPAddress.Parse(relayHeader.Name), Config.serverPort);
        } else if (relayHeader.Type == RelayType.Node) {
            relayDest = DHTLikeNet.GetNodeIP(relayHeader.Name);
        } else{
            log.Error(String.Format("The relayHeader.Type={0} is not defined.", relayHeader.Type));
            return null;
        }

        if (relayDest!=null && IPAddress.IsLoopback(relayDest.Address) && relayDest.Port==Config.serverPort) { 
            this.isEndPoint = true;
            relayDest = GetIPFromHeader(); 
        }
        return relayDest;
    }
  }
}