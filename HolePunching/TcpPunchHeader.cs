namespace GaiaNet.HolePunching
{
/*HEAD STRUCT:
+----+-------+-------------+
|TYP |  LEN  | ServerNames |
+----+-------+-------------+
| 1  |  int  |    names    |
+----+-------+-------------+

TYP: Request(0x01), Response(0x02)
LEN: bytes Transformed from int.
ServerNames: server names need to request.
*/
    public class TcpReqHeader
    {
        private byte type = 0x01;
        private int len = 0;
        private string serverNames = "";

        public byte[] build(){
            byte[] byts = new byte[100];
            return byts;
        }

        public void parse(byte[] byts){

        }
    }


/*HEAD STRUCT:
+----+-------+-------+------+
|TYP | STATE |  LEN  | DATA |
+----+-------+-------+------+
| 1  |   1   |   4   |  m   |
+----+-------+-------+------+

TYP: command(0x01), file(0x02), Tcp stream(0x03)
STATE:  0x01(request), 0x02(response)
LEN:
DATA:   command or filename.
*/
    public class TcpRespHeader
    {
        public byte[] build(){
            byte[] byts = new byte[100];
            return byts;
        }

        public void parse(byte[] byts){

        }

    }
}