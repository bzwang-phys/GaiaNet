package GaiaNet.NetTransfer;

import GaiaNet.Common.ByteTools;
import GaiaNet.GaiaNet.Net;

import java.io.*;
import java.net.ServerSocket;
import java.net.Socket;
import java.nio.file.Path;
import java.util.Hashtable;
import java.util.concurrent.locks.ReadWriteLock;
import java.util.concurrent.locks.ReentrantReadWriteLock;
import java.util.logging.FileHandler;
import java.util.logging.Logger;
import java.util.logging.SimpleFormatter;

public class MultiThreadServer {
    private static final Logger logServer = Logger.getLogger( MultiThreadServer.class.getName() );
    public FileHandler fileHandler;  // For log file.
    public Net gaiaNet = null;
    private int port;
    private String basePath;
    ServerSocket sSocket;
    Hashtable<Integer, ReadWriteLock>  rwLocks = new Hashtable<>();

    public String getBasePath() {
        return basePath;
    }
    public void setBasePath(String basePath) {
        this.basePath = basePath;
    }

    public MultiThreadServer(int port) {
        this.port = port;
        try {
            sSocket = new ServerSocket(port);
            basePath = System.getProperty("user.dir");
            fileHandler = new FileHandler("./log/MultiThreadServer.log");   // For log file.
            fileHandler.setFormatter(new SimpleFormatter());
            logServer.addHandler(fileHandler);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public MultiThreadServer(int port, Net net) {
        this(port);
        this.gaiaNet = net;
    }

//
    public void serverRun(){
        int flag;
        while (true){
            try {
                Socket socket = sSocket.accept();
                DataInputStream dis = new DataInputStream(socket.getInputStream());
                flag = dis.readInt();
                GaiaType type = GaiaType.values()[flag];

                if (type == GaiaType.Command){
                    CommandHandler cmdHandler = new CommandHandler();
                    new Thread(()-> cmdHandler.handle(socket, logServer)).start();
                } else if (type == GaiaType.File){  // file transfer
                    FileHeader fileHeader = new FileHeader();
                    fileHeader.receive(dis);
                    logServer.info("File from: "+socket.getRemoteSocketAddress()+ " File Name: "+fileHeader.name+" Type: "+fileHeader.type);
                    if (fileHeader.type == 0)  createFile(socket,dis,fileHeader);
                    if (fileHeader.type == 1)  new Thread(()-> threadRcvFile(socket,dis,fileHeader)).start();
                } else if (type == GaiaType.GaiaNet){
                    new Thread(()-> this.gaiaNet.net.handle(socket, logServer)).start();
                }
                else if (flag==100){
                    printStream(socket, dis);
                } else {
                    System.out.println("MultiThreadServer: Unknown Socket.");
                }
            } catch (IOException e) {
                e.printStackTrace();
            }

        }
    }

//  used to receive files. first read the [name,star,end], then write in file.
    private void threadRcvFile(Socket socket, DataInputStream dis, FileHeader fileHeader) {
        int len;
        try {
            String fileName = fileHeader.name;
            Path filePath = Path.of(basePath, fileName);
            File file = filePath.toFile();
            if (!file.exists()){
                throw new IOException("File is not created!!");
            }

            long start = fileHeader.start;
            long end = fileHeader.end;
            long index = fileHeader.index;
            System.out.printf("thread: "+Thread.currentThread().getName()+" is ready to receive: "+fileName);
            System.out.printf(", start: %d, end: %d \n", start,end);
            RandomAccessFile rw = new RandomAccessFile(file, "rw");
            byte[] byt = new byte[500 * 1024];
            while ((len = dis.read(byt,0, 500 * 1024))!=-1){
                rwLocks.get(file.toString().hashCode()).writeLock().lock();
                rw.seek(start+index);
                rw.write(byt,0, len);
                index += len;
                System.out.printf("receiving: "+Thread.currentThread().getName()+", start: %d, end: %d, index: %d\n", start,end,index);
                rwLocks.get(file.toString().hashCode()).writeLock().unlock();
            };
            rw.close();

        } catch (IOException e) {
            e.printStackTrace();
        }
    }

//  used to write the
    private void createFile(Socket socket, DataInputStream dis, FileHeader fileHeader){
        String fileName;
        try {
            fileName = fileHeader.name;
            long size = fileHeader.size;
            File f = Path.of(basePath, fileName).toFile();
            if ( f.exists() && (f.length()==size) ){
                //  read configuration
            }else {
                RandomAccessFile rw = new RandomAccessFile(f, "rw");
                rw.setLength(size);
            }
            rwLocks.put(f.toString().hashCode(), new ReentrantReadWriteLock());
            System.out.println(Thread.currentThread().getName()+", New file: " +fileName+", size: "+size);
        } catch (IOException e) {
            e.printStackTrace();
        }

    }

    public void stop() {
        try {
            sSocket.close();
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    // Just print out the stream with hex string
    public void printStream(Socket socket, DataInputStream dis){
        try {
            byte[] byt = new byte[10];
            dis.read(byt);
            System.out.print(ByteTools.HexString(byt));
        } catch (IOException e) {
            e.printStackTrace();
        }

    }

}
