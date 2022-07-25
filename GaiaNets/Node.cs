using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace GaiaNet.GaiaNets
{
    public class Node {
        public int uid;
        public int gid;
        public List<String> macArray = new List<string>();
        public String outerIp;
        public List<IPAddress> ipv4;
        public List<IPAddress> ipv6;
        public String nodeName;
        public String hostName;
        public String osName;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().ReflectedType);

        // Three methods to build node.
        // 1.local  2.from txt info. 3. null
        public Node(String s){
            if ("local".Equals(s)) {
                this.GetIPAndHost();
                this.GetMacArray();
                this.osName = Environment.OSVersion.ToString();
                this.uid = GetUid();
                this.gid = GetGid();
                this.nodeName = Config.nodeName;
                log.Info("Node initialize: " + this.ToString());
            } else {
                BuildFromTxt(s);
            }
        }

        public Node(){}

        private void GetMacArray(){
            try {
                this.macArray = NetworkInterface
                    .GetAllNetworkInterfaces()
                    .Where( nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback )
                    .Where( nic => nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet || 
                        nic.NetworkInterfaceType == NetworkInterfaceType.GigabitEthernet  || 
                        nic.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                    .Select( nic => nic.GetPhysicalAddress().ToString() )
                    .ToList();

                if (this.macArray == null) { //Can not read MAC address, I don't know why?
                    // tmpMacList.add("AB-CD-EF-GH-IJ");
                    // this.macArray = tmpMacList;
                    log.Error("The MAC address can not be null");
                }

            } catch (Exception e) {
                log.Error(e);
            }
        }

        private void GetIPAndHost(){
            try {
                this.hostName = Dns.GetHostName();
                IPAddress[] ipaddress = Dns.GetHostAddresses(this.hostName);
                this.ipv4 = ipaddress.Where(ip => ip.AddressFamily==System.Net.Sockets.AddressFamily.InterNetwork).ToList();
                this.ipv6 = ipaddress.Where(ip => ip.AddressFamily== System.Net.Sockets.AddressFamily.InterNetworkV6 &&
                                !ip.IsIPv6LinkLocal).ToList();
            } catch (Exception e) {
                log.Error(e);
            }
        }

        private int GetUid(){
            int i, res=0;
            try {
                // int bitSize = 16;
                // String mac = this.macArray.get(0);
                // byte[] macHash = Digest.PearsonHash(mac.getBytes(StandardCharsets.UTF_8), bitSize);
                // for (i=0; i<macHash.length; i++){
                //     res += Byte.toUnsignedInt(macHash[i]) * Math.pow(2, 8*(macHash.length-1-i));
                // }
            } catch (Exception e) {
                log.Error(e);
            }
            return res;
        }


        private int GetGid(){
            return 0;
        }

        public String GetInfo(){
            String s = "\n";
            s += "\t- NodeName: " + this.nodeName + "\n";
            s += "\t- Uid: " + this.uid + "\n";
            s += "\t- Gid: " + this.gid + "\n";
            s += "\t- Mac: " + String.Join(", ", this.macArray) + "\n";
            s += "\t- HostName: " + this.hostName + "\n";
            s += "\t- IP4Addr: " + String.Join(", ", this.ipv4.Select(ip=>ip.ToString())) + "\n";
            s += "\t- IP6Addr: " + String.Join(", ", this.ipv6.Select(ip=>ip.ToString())) + "\n";
            s += "\t- OuterIP: " + this.outerIp + "\n";
            s += "\t- osName: " + this.osName + "\n";
            return s;
        }

        public override String ToString(){
            return GetInfo();
        }

        public String ToTxtForm(){
            String res = String.Format("%d  %s  %s%n", this.uid, this.nodeName, this.outerIp);
            return res;
        }

        public void BuildFromTxt(String txt){

        }

        public byte[] ToBytes(){
            return new byte[10];
        }



    }
}