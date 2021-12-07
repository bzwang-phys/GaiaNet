package GaiaNet.Common;

import java.io.*;
import java.util.ArrayList;
import java.util.concurrent.locks.ReadWriteLock;
import java.util.concurrent.locks.ReentrantReadWriteLock;


public class Files {
    public static int progress = 0;
    private static ReadWriteLock rwLock = new ReentrantReadWriteLock();
    public static ArrayList<String> listAllFiles(String fs, Integer... lev){
        Integer level = 1024;
        if (lev.length == 1 ){
            level = lev[0];
        }
        ArrayList<String> res = new ArrayList<>();
        res.add(fs);
        if (level <= 0) {
            return res;
        }
        File[] files = new File(fs).listFiles();
        if (files != null){
            for (File f1 : files) {
                if (f1.isDirectory()){
                    res.addAll(listAllFiles(f1.toString(), level-1));
                }else {
                    res.add(f1.toString());
                }
            }
        }
        return res;
    }

    public static ArrayList<String> search(String fs){
        ArrayList<String> res = new ArrayList<>();

        return res;
    }

    static class CopyInfo{
        String source;
        String target;
        long start;
        long startPoint;
        long end;
        double process;
        public CopyInfo(String source, String target, long start, long end) {
            this.source = source;
            this.target = target;
            this.start = start;
            this.startPoint = start;
            this.end = end;
        }
    }
    public static class ProgressInfo{
        public long index;
        public long sum;
        public ProgressInfo(long index, long sum) {
            this.index = index;
            this.sum = sum;
        }
        public ProgressInfo() {
            this.index = 0;
            this.sum = 0;
        }
    }

    public static void copy(String sourceFile, String targetFile, ProgressInfo progressInfo) throws Exception {
        long start = 0;
        long end = 0;
        FileSegment fsg = new FileSegment(sourceFile, FileSegment.Flag.LOCAL);
        System.out.printf("size=%6.2fM, threadNum=%d, blockNum=%d, blockSize=%6.2fM\n", 1.0*fsg.getSize()/1024/1024,
                fsg.getThreadNum(),fsg.getBlockNum(),1.0*fsg.getBlockSize()/1024/1024);
        new RandomAccessFile(targetFile, "rw").setLength(fsg.getSize());
        ArrayList<CopyInfo> copyInfos = new ArrayList<>();
        for (int iThread = 0; iThread < fsg.getThreadNum(); iThread++) {
            start = iThread * fsg.getBlockSize();
            end = (iThread+1) * fsg.getBlockSize();
            if (end > fsg.getSize()){ // 1 <= end <= size
                end = fsg.getSize();
            }
            if (start > fsg.getSize()-1){    // 0 <= start <= size-1
                // Solve the problem that blockNum/threadNum may be large than ...
                break;
            }
            CopyInfo copyInfo = new CopyInfo(sourceFile, targetFile, start, end);
            copyInfos.add(copyInfo);
            Thread thread = new Thread(() -> copyRun(copyInfo));
            thread.start();
        }

        while (true){
            start = 0;
            for (CopyInfo c : copyInfos) {
                start += c.start - c.startPoint;
            }
            progressInfo.index = start;
            progressInfo.sum = fsg.getSize();
            progress = (int) Math.floor(100.0*start/fsg.getSize());
            System.out.println("progress: " + progress + "%");
            Thread.sleep(1000);
            if (progress == 100){
                break;
            }
        }

    }

    // read and write with [start, end), because len=end-start
    private static void copyRun(CopyInfo copyInfo)  {
        try {
            RandomAccessFile fis = new RandomAccessFile(copyInfo.source, "r");
            RandomAccessFile fos = new RandomAccessFile(copyInfo.target, "rw");
            fis.seek(copyInfo.start);
            long blockSize = 2L*1024*1024;
            byte[] data = new byte[(int)blockSize];
            while(true){

                if (copyInfo.start+blockSize >= copyInfo.end){
                    blockSize = copyInfo.end - copyInfo.start;
                }
                // read data
                rwLock.readLock().lock();
                try {
                    fis.seek(copyInfo.start);
                    fis.read(data,0,(int)blockSize);
                }catch (Exception e){
                    e.printStackTrace();
                }finally {
                    rwLock.readLock().unlock();
                }

                // write data
                rwLock.writeLock().lock();
                try {
                    fos.seek(copyInfo.start);
                    fos.write(data,0,(int)blockSize);
                }catch (Exception e){
                    e.printStackTrace();
                }finally {
                    rwLock.writeLock().unlock();
                }
                copyInfo.start += blockSize;
                if (copyInfo.start >= copyInfo.end ){
                    fos.close();
                    fis.close();
                    break;
                }
                // end
            }
        } catch (IOException e) {
            e.printStackTrace();
        }
    }


}
