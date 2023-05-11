using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Net.Sockets;
using GaiaNet.BasicNet;

namespace GaiaNet.GaiaNets
{
    public enum NetRequest : byte { JoinNet = 0x01, LeaveNet = 0x02, NearestNodes = 0x03 };
    public class DHTLikeNet
    {
        public static Node node = null;
        public static Dictionary<String, String> masterNodesIp = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);
        public static Dictionary<String, String> neighbours = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().ReflectedType);

        public DHTLikeNet(Node node1)
        {
            node = node1;
            masterNodesIp = Config.masterNodesIp;
            neighbours = Config.neighbours;
            log.Info("DHTLikeNet initialize: " + this.ToString());
            Console.WriteLine("DHTLikeNet initialize: " + this.ToString());
            // JoinNet();
        }

        public int JoinNet()
        {
            if (masterNodesIp.Count <= 0) return 1;
            foreach (String serverIP in masterNodesIp.Values)
            {
                log.Info("joinNet, send to " + serverIP);
                Socket sock = null;
                try
                {
                    sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    sock.Connect(serverIP, 9190);
                    sock.Send(new byte[] { (byte)NetType.GaiaNet, (byte)NetRequest.JoinNet });
                    sock.Send(node.ToBytes());
                    Node remoteNode = null; // (Node) sock.Receive();
                    log.Info("master response: " + remoteNode.ToString());

                    Boolean isMyneigh = IsMyNeighbour(remoteNode);
                    if (isMyneigh) JoinMyNeighbourList(remoteNode);
                }
                catch (IOException e)
                {
                    log.Error(e);
                    return 1;
                }
                finally
                {
                    if (sock != null) sock.Close();
                }
            }

            return 0; // normal
        }


        public void JoinNetResponse(Socket socket)
        {
            try
            {
                Node remoteNode = null; //(Node) ois.readObject();
                // oos.writeObject(this.node);
                log.Info("A node ask for joining net: " + remoteNode.ToString());

                Boolean isMyneigh = IsMyNeighbour(remoteNode);
                if (isMyneigh) JoinMyNeighbourList(remoteNode);
            }
            catch (IOException e)
            {
                log.Error(e);
            }
            finally
            {
                if (socket != null) socket.Close();
            }

        }

        public void handle(Socket socket)
        {
            try
            {
                // socket.Receive();
                // int flag = dis.readInt();
                // NetRequest type = NetRequest.values()[flag];

                // if (type == NetRequest.JoinNet) JoinNetResponse(socket, is, os);
                // else if (type == NetRequest.LeaveNet);
                // else if (type == NetRequest.NearestNodes);

                socket.Close();
            }
            catch (IOException e)
            {
                log.Error(e);
            }
        }

        private void JoinMyNeighbourList(Node node)
        {
        }

        private Boolean IsMyNeighbour(Node node)
        {
            return false;
        }

        private String GetMyNearestNodes()
        {
            return neighbours.ToString();
        }


        public int distance(Node node1, Node node2)
        {
            return 0;
        }

        public int leaveNet()
        {
            return 0;
        }

        public void nearest(Node node)
        {

        }

        public override String ToString()
        {
            String s = "\n";
            s += "\t- Node Name: " + node.nodeName + "\n";
            s += "\t- masterNodesIp: " + String.Join(", ", masterNodesIp.Select(x => x.Key + " = " + x.Value).ToArray()) + "\n";
            s += "\t- neighbours: " + String.Join(", ", neighbours.Select(x => x.Key + " = " + x.Value).ToArray()) + "\n";
            return s;
        }


        /// <summary> 
        /// Parse the ip string 192.168.1.1[:8081], if the port is not specified, the default port is "serverPort".
        /// </summary>
        /// <param name="ipstr">The string of ip[:port]</param>
        /// <returns>IPEndPoint (ip and port)</returns>
        public static IPEndPoint ParseIPStr(string ipstr)
        {
            string ip = "127.0.0.1";
            int port = Config.serverPort;
            if (ipstr.Contains(":"))
            {
                string[] ip_port = ipstr.Split(':');
                if (ip_port.Length == 2)
                {
                    ip = ip_port[0];
                    int.TryParse(ip_port[1], out port);
                }
            }
            else { ip = ipstr; }
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            return iPEndPoint;
        }

        /// <summary> Get the IP from the node name.
        /// </summary>
        /// <param name="nodename"> node name. </param>
        /// <returns>IPEndPoint (ip and port)</returns>
        public static IPEndPoint GetNodeIP(string nodename)
        {
            IPEndPoint iPEndPoint = null;
            if (nodename.Trim().Equals(node.nodeName.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                iPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Config.serverPort);
            }
            else if (DHTLikeNet.masterNodesIp.ContainsKey(nodename))
            {
                iPEndPoint = ParseIPStr(DHTLikeNet.masterNodesIp[nodename]);
            }
            else if (DHTLikeNet.neighbours.ContainsKey(nodename))
            {
                iPEndPoint = ParseIPStr(DHTLikeNet.neighbours[nodename]);
            }
            return iPEndPoint;
        }
    }
}