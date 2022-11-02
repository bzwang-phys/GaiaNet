using System;
using GaiaNet.GaiaNets;
using System.Net.Sockets;
using System.IO;
using CommandLine;
using System.Collections.Generic;
using GaiaNet.BasicNet;
using System.Text;
using System.Linq;

namespace GaiaNet.Command
{
    class CommandHandler
    {
        private TcpClient sock;
        private int _port;

        public CommandHandler(int port){
            this._port = port;
        }
        public CommandHandler(){
            this._port = Config.serverPort;
        }


        public int sendCmd(String cmd)
        {
            try{
                string[] cmdList = cmd.Split(null);
                CommandTo cmdto = null;
                Parser.Default.ParseArguments<CommandTo>(cmdList).WithParsed<CommandTo>(o=>{ cmdto = o; });

                if (cmdto.udp) {udpSendCmd(cmdto); return 0;}
                this.sock = new TcpClient(cmdto.to, _port);
                byte[] btysSend = new byte[] {(byte)NetType.Command};
                this.sock.Client.Send(btysSend);
                this.sock.Client.Send(Encoding.UTF8.GetBytes(cmdto.cmdStr));

                // byte[] btysReceive = new byte[1024];
                // int resNum = this.sock.Client.Receive(btysReceive);
                // Console.WriteLine(resNum);
                return 0; // normal
            }
            catch (Exception e){
                Console.WriteLine("Send Command failed!" + e);
                return 1;
            }
        }


        public void handle(Socket socket){
            try {
                Console.WriteLine("Receive command from: " + socket.RemoteEndPoint);
                byte[] byts = new byte[2048];  // length of command should < 2048.
                int num = socket.Receive(byts);
                Console.WriteLine(Encoding.UTF8.GetString(byts.Take(num).ToArray()));

                // string cmd = sr.ReadLine();
                // Console.WriteLine(tcpClient);
                // Console.WriteLine(cmd);
                // Command command = cmdParse(cmd);
                // String result = CommandExec.exec(command);
                // dos.writeUTF(result);
            } catch (Exception e) {
                Console.WriteLine(e);
            }

    }


        public int udpSendCmd(CommandTo cmd){
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
                    if (cmdstr.Trim() != string.Empty)
                        sendCmd(cmdstr.Trim());
                }
            }
            catch (Exception e){
                Console.WriteLine(e);
            }
        }

    }
}
