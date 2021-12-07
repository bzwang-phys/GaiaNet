namespace GaiaNet.FilesTransfer
{
    public class FileHeader
    {
        public int type; // 0: create file or communication. 1:thread to transfer file.
        public string name;
        public long size;
        public long start;
        public long end;
        public long index;

        public FileHeader(int type, string name, long size){
            this.type = type;
            this.name = name;
            this.size = size;
            this.start = 0L;
            this.end = 0L;
            this.index = 0L;
        }
        public FileHeader(int type, string name, long start, long end, long index){
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
        
        // public void send(DataOutputStream dosSocket) throws IOException {
        //     dosSocket.writeInt(this.type);
        //     dosSocket.writeUTF(this.name);
        //     dosSocket.writeLong(this.size);
        //     dosSocket.writeLong(this.start);
        //     dosSocket.writeLong(this.end);
        //     dosSocket.writeLong(this.index);
        // }

        // public void receive(DataInputStream disSocket){
        //     this.type = disSocket.readInt();
        //     this.name = disSocket.readUTF();
        //     this.size = disSocket.readLong();
        //     this.start = disSocket.readLong();
        //     this.end = disSocket.readLong();
        //     this.index = disSocket.readLong();
        // }
    }
}