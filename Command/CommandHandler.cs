using System;
using GaiaNet.GaiaNets;
using System.Net.Sockets;
using System.Net;
using GaiaNet.BasicNet;
using GaiaNet.Common;
using System.Text;
using System.Linq;

namespace GaiaNet.Command
{
    class CommandParse
    {
        public Boolean udp {get; set;}
        public IPEndPoint to {get; set;}
        public String cmdStr {get; set;}
        public CommandParse()
        {
            this.to = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Config.serverPort);
            this.udp = false;
        }
    }


    class CommandHandler
    {
        private TcpClient sock = null;
        private int _port;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().ReflectedType);

        public CommandHandler(int port){
            this._port = port;
        }
        public CommandHandler(){
            this._port = Config.serverPort;
        }


        public CommandParse parseCMD(String cmd){
            CommandParse cmdParse = new CommandParse();
            string[] cmdList = cmd.Split(null);

            if ("to" == cmdList[0]){
                cmdParse.to = NetTools.ParseIPPort(cmdList[1]);
                if (cmdParse.to == null) {
                    System.Console.WriteLine("IP and port can not be parsed correctly");
                    return null;
                }
                cmdParse.cmdStr = string.Join(" ", cmdList[2..^0]);
            } else if ("udp" == cmdList[0]){
                cmdParse.udp = true;
                cmdParse.to = NetTools.ParseIPPort(cmdList[2]);
                if ("to" != cmdList[1] || cmdParse.to == null) {
                    Console.WriteLine("Command should be like: udp to 192.168.X.X cmd");
                    return null;
                }
                cmdParse.cmdStr = string.Join(" ", cmdList[3..^0]);
            } else {
                cmdParse.cmdStr = cmd.Trim();
            }
            return cmdParse;
        }


        public int sendCMD(CommandParse cmdParse)
        {try{
            this.sock = new TcpClient();
            byte[] btysSend = new byte[] {(byte)NetType.Command};  //tell other side it's the Command package.
            this.sock.Connect(cmdParse.to);
            this.sock.Client.Send(btysSend);
            this.sock.Client.Send( (new NetStringHeader(cmdParse.cmdStr)).bytes );

            String recvString = new NetStringHeader().RecvString(this.sock.Client);
            Console.WriteLine(recvString);
            return 0; // normal
        }
        catch (Exception e){
            Console.WriteLine("Send Command failed!" + e);
            return 1;
        }
        }


        public void handle(Socket socket){
            try {
                System.Console.WriteLine("Receive command from: " + socket.RemoteEndPoint);
                log.Info("Receive command from: " + socket.RemoteEndPoint);
                String cmdStr = new NetStringHeader().RecvString(socket);
                Console.WriteLine(cmdStr);

                String cmdResult = " " + SystemTools.Execute(cmdStr);
                socket.Send(new NetStringHeader(cmdResult).bytes);
                // System.Console.WriteLine("Have Send");
            } catch (Exception e) { Console.WriteLine(e); }

    }


        public int udpSendCmd(){
//         DatagramSocket ds = new DatagramSocket();
//         InetAddress serverIp = InetAddress.getByName(cmd.to);
// //        this.dos.writeInt(GaiaType.Command.ordinal());  // 0 represent Command;
//         DatagramPacket dpSent = new DatagramPacket(cmd.cmdStr.getBytes(StandardCharsets.UTF_8),cmd.cmdStr.length(),serverIp,9190);
//         ds.send(dpSent);

        return 0; // normal
    }


        public void getCmdFromKey()
        {
            try{
                String cmdstr;
                while (true){
                    Console.Write("GaiaNet :> ");
                    cmdstr = Console.ReadLine();
                    cmdstr = cmdstr.Trim();
                    if (cmdstr.Trim() != string.Empty){
                        CommandParse cmdParse = parseCMD(cmdstr.Trim());
                        sendCMD(cmdParse);
                    }
                }
            }
            catch (Exception e){
                Console.WriteLine(e);
            }
        }

    }
}
