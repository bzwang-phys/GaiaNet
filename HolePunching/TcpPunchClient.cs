using System.Net;
using System.Net.Sockets;
using System;
using System.Text;
using GaiaNet.GaiaNets;

namespace GaiaNet.HolePunching
{
    public class TcpPunchClient
    {
        private Socket clientSocket;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().ReflectedType);


        public void run(){
            string holePunchServer = "123.56.221.246";
            int port = Config.serverPort;
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            // set ReusedAddress.

            clientSocket.Connect(holePunchServer, port);
            log.Info("Tcp Hole Punching Connect: " + holePunchServer + "  " + port);

            byte[] recBytes = new byte[2048];
            int bytes = clientSocket.Receive(recBytes, recBytes.Length, 0);
            string result = Encoding.ASCII.GetString(recBytes, 0, bytes);
            log.Info("Recv:" +result);
            clientSocket.Close();

            string[] ips = result.Split(':'); 
            int myPort = Convert.ToInt32(ips[1]);
            string otherIp = ips[2];
            int otherPort = Convert.ToInt32(ips[3]);


            Socket mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            mySocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            //绑定到之前连通过的端口号
            IPEndPoint ipe = new IPEndPoint(IPAddress.Any, Convert.ToInt32(myPort));
            mySocket.Bind(ipe);
            //尝试5次连接
            for (int j = 0; j < 5; j++)
            {
                try
                {
                    mySocket.Connect(otherIp, otherPort);
                    log.Info(String.Format("Connect: 成功{0},{1}", otherIp,otherPort));
                    break;
                }
                catch (Exception)
                {
                    log.Info("Connect: 失败");
                    // otherPort++;//如果是对称NAT，则有可能客户端的端口号已经改变，正常有规律的应该是顺序加1，可以尝试+1再试（我使用手机热点连接的时候端口号就变成+1的了）除非是碰到随机端口，那就不行了。
                }

            }
            while (true)
            {
                mySocket.Send(Encoding.ASCII.GetBytes("hello,the other client!"));

                byte[] recv = new byte[4096];
                int len = mySocket.Receive(recv, recv.Length, 0);
                result = Encoding.ASCII.GetString(recv, 0, len);
                log.Info("recv :" + result);

                System.Threading.Thread.Sleep(1000); 
            }
        } 
        
    
    }
}