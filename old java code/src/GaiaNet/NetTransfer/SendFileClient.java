package GaiaNet.NetTransfer;

import GaiaNet.Common.FileSegment;
import GaiaNet.Common.Files;

import java.io.*;
import java.net.Socket;
import java.util.ArrayList;
import java.util.Hashtable;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.locks.ReadWriteLock;
import java.util.concurrent.locks.ReentrantReadWriteLock;


public class SendFileClient {
    private String ip;
    private int port;
    private Socket clientSock;
    private ExecutorService threadPool = Executors.newFixedThreadPool(40);
    private Hashtable<Integer, ReadWriteLock>  rwLocks = new Hashtable<>();

    public String getIp() {
        return ip;
    }
    public void setIp(String ip) {
        this.ip = ip;
    }
    public int getPort() {
        return port;
    }
    public void setPort(int port) {
        this.port = port;
    }

    public SendFileClient(String ip, int port) {
        this.ip = ip;
        this.port = port;
        try {
            clientSock = new Socket(ip,port);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    static class NetTransferInfo{
        String filePath;
        String fileName;
        long size;
        long start;
        long end;
        long index;  // for resuming from breakpoint.
        long progress;  // for collecting the information of progress.
        public NetTransferInfo(String filePath,String fileName,long size,long start,long end,long index) {
            this.filePath = filePath;
            this.fileName = fileName;
            this.size = size;
            this.start = start;
            this.end = end;
            this.index = index;
            this.progress = start;
        }
    }


    /*
     * Use threadPool to send the blocks of the file,
     * the block is described by fs.
     */
    public void sendFile(String filePath){
        try {
            File file = new File(filePath);
            if (!file.exists()){
                throw new FileNotFoundException();
            }
            String fileName = file.getName();
            FileSegment fsg = new FileSegment(filePath, FileSegment.Flag.NET);
            Files.ProgressInfo pi = new Files.ProgressInfo();
            rwLocks.put(filePath.hashCode(), new ReentrantReadWriteLock());

            DataOutputStream dosSocket = new DataOutputStream(clientSock.getOutputStream());
            FileHeader fileHeader = new FileHeader(0, fileName, fsg.getSize());
            dosSocket.writeInt(1);  // Here 1 represent the File transfer.
            fileHeader.send(dosSocket);
            System.out.printf("blockNum: %d, blockSize: %d M \n",fsg.getBlockNum(),fsg.getBlockSize()/1024/1024);
            ArrayList<NetTransferInfo> ntis = new ArrayList<>((int) fsg.getBlockNum());
            // this list is used to collect all netTransferInfo, and the progrss.
            for (int blockIndex = 0; blockIndex < fsg.getBlockNum(); blockIndex++) {
                NetTransferInfo netTransferInfo = new NetTransferInfo(filePath,fileName,
                        fsg.getSize(),
                        blockIndex * fsg.getBlockSize(),
                        (blockIndex + 1) * fsg.getBlockSize(),
                        0);
                ntis.add(netTransferInfo);
                threadPool.execute(() -> runSend(netTransferInfo));
            }

            // shows the progress = send/size.
            while (true){
                Long start = 0L;
                for (NetTransferInfo nti : ntis) {
                    start += nti.progress - nti.start;
                }
                pi.index = start;
                pi.sum = fsg.getSize();
                Integer progress = (int) Math.floor(100.0*start/fsg.getSize());
                System.out.println("progress: " + progress + "%");
                Thread.sleep(2000);
                if (progress >= 100){
                    break;
                }
            }
        } catch (Exception e) {
            e.printStackTrace();
        }

    }

    /*
     * Send a block of the file,
     * the block is described by fs.
     */
    private void runSend(NetTransferInfo netTransferInfo) {
        long start = netTransferInfo.start;
        if (start > netTransferInfo.size-1){    // 0 <= start <= size-1
            // Solve the problem that blockNum/threadNum may be large than ...
            return;
        }
        if (netTransferInfo.end > netTransferInfo.size){ // 1 <= end <= size
            netTransferInfo.end = netTransferInfo.size;
        }
        try {
            Socket socket = new Socket(ip, port);
            DataOutputStream dos = new DataOutputStream(socket.getOutputStream());
            RandomAccessFile fis = new RandomAccessFile(netTransferInfo.filePath, "r");
            FileHeader fileHeader = new FileHeader(1, netTransferInfo.fileName,
                    netTransferInfo.start, netTransferInfo.end, netTransferInfo.index);
            dos.writeInt(1);  // Here 1 represent the File transfer.
            fileHeader.send(dos);

            fis.seek(start);
            long blockSize = 500L * 1024;   //500Kb
            byte[] data = new byte[(int) blockSize];
            while (true) {
                if (start + blockSize >= netTransferInfo.end) {
                    blockSize = netTransferInfo.end - start;
                }
                // read data
                rwLocks.get(netTransferInfo.filePath.hashCode()).readLock().lock();
                try {
                    fis.seek(start);
                    fis.read(data, 0, (int) blockSize);
                } catch (Exception e) {
                    e.printStackTrace();
                } finally {
                    rwLocks.get(netTransferInfo.filePath.hashCode()).readLock().unlock();
                }

                // write data
                dos.write(data, 0, (int) blockSize);

                start += blockSize;
                netTransferInfo.progress = start;
                if (start >= netTransferInfo.end) {
                    dos.close();
                    fis.close();
                    socket.close();
                    break;
                }
            }
        }catch (IOException e) {
            e.printStackTrace();
        }
    }



}
