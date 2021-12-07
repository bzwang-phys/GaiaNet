package GaiaNet.NetTransfer;

import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;

public class FileHeader {
    public Integer type; // 0: create file or communication. 1:thread to transfer file.
    public String name;
    public Long size;
    public Long start;
    public Long end;
    public Long index;

    public FileHeader(Integer type, String name, Long size){
        this.type = type;
        this.name = name;
        this.size = size;
        this.start = 0L;
        this.end = 0L;
        this.index = 0L;
    }
    public FileHeader(Integer type, String name, Long start, Long end, Long index){
        this.type = type;
        this.name = name;
        this.size = 0L;
        this.start = start;
        this.end = end;
        this.index = index;
    }

    public FileHeader(){
        this.type = 0;
        this.name = "";
        this.size = 0L;
        this.start = 0L;
        this.end = 0L;
        this.index = 0L;
    }

    public void send(DataOutputStream dosSocket) throws IOException {
        dosSocket.writeInt(this.type);
        dosSocket.writeUTF(this.name);
        dosSocket.writeLong(this.size);
        dosSocket.writeLong(this.start);
        dosSocket.writeLong(this.end);
        dosSocket.writeLong(this.index);
    }

    public void receive(DataInputStream disSocket) throws IOException {
        this.type = disSocket.readInt();
        this.name = disSocket.readUTF();
        this.size = disSocket.readLong();
        this.start = disSocket.readLong();
        this.end = disSocket.readLong();
        this.index = disSocket.readLong();
    }
}
