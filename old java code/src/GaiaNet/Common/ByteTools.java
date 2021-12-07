package GaiaNet.Common;

public class ByteTools {

    public static String UnicodeByteToString(byte[] bBuf) {
        StringBuffer res = new StringBuffer();
        Character ch = 0;
        for (int i = 0; i < bBuf.length; i += 2) {
            if (bBuf[i + 1] == 0) {
                ch = (char) bBuf[i];
            } else {
                ch = (char) ((bBuf[i] << 8) & 0xFF00 | bBuf[i + 1]);
            }

            if (ch == 0) { // 字符串结束符 \0
                break;
            }
            res.append(ch);
        }
        return res.toString();
    }

    public static String HexString(byte[] b) {
        String res = "";
        for (int i = 0; i < b.length; i++) {
            String hex = Integer.toHexString(b[i] & 0xFF);
            if (hex.length() == 1) {
                hex = "0" + hex;
            }
            res += " 0x"+hex;
        }
        return res;
    }

}