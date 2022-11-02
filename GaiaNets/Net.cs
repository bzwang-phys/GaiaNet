using System;
using System.Linq;


namespace GaiaNet.GaiaNets
{
    public class Net {
        public DHTLikeNet net;
        public Node node;

        public Net(){
            Config.ReadConfig();
            this.node = new Node("local");
            this.net = new DHTLikeNet(this.node);
        }

    }

}