namespace GaiaNet.Relay
{
    public class RelayHeader
    {
        private enum RelayType:byte {IP=0x01, Node=0x02, Browser=0x03};
        private RelayType type;
        private int len;
        
        
    }
}