namespace GaiaNet.Relay
{
    enum RelayType:byte {IP=0x01, Node=0x02, Browser=0x03};

    public class RelayHeader
    {
        private RelayType type;
        private int len;
        private string name;

        public string Name { get => name; set => name = value; }
        public int Len { get => len; set => len = value; }
        internal RelayType Type { get => type; set => type = value; }
    }
}