using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaiaNet.BasicNet
{
    class UdpServer
    {
        private int _port;

        public UdpServer(int port)
        {
            _port = port;
            try{
            }
            catch (IOException e){
                Console.WriteLine(e);
            }
        }

        private void writeFile(String s)
        {
            //logServer.info(s);
        }


        //    receive the data and write in log.
        public void run()
        {
            try{
                byte[] buf = new byte[1024];
                //DatagramSocket ds = new DatagramSocket(this.port);
                //DatagramPacket dpReceive = new DatagramPacket(buf, buf.length);
                //while (true)
                //{
                    //ds.receive(dpReceive);
                    //String clientInfo = dpReceive.getAddress().toString() + ":" + dpReceive.getPort();
                    //Date dNow = new Date();
                    //String timeNow = "[ " + ft.format(dNow) + " ] ";
                    //String dataReceive = new String(dpReceive.getData(), 0, dpReceive.getLength(), StandardCharsets.UTF_8);
                    //writeFile("From: " + clientInfo + "  Data: " + dataReceive + System.lineSeparator());
                //}
            }
            catch (Exception e){
                Console.WriteLine(e);
            }

        }
}
}
