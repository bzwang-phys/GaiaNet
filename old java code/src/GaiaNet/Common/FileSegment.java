package GaiaNet.Common;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.nio.channels.FileChannel;

public class FileSegment {
    public enum Flag {LOCAL, NET;}
    private Flag flag;
    private final long LOCAL_BLOCK = 10L * 1024L * 1024L;      // 10M
//    private final long LOCAL_MAX_BLOCK =  150L * 1024L * 1024L;   // 150 M
//    private final long LOCAL_MAX_SIZE =  4L * 1024L * 1024L * 1024L;   // 4G
    private final long NET_BLOCK = 4L * 1024L * 1024L;      //4M
//    private final long NET_MAX_BLOCK =  10L * 1024L * 1024L;
//    private final long NET_MAX_SIZE =  1L * 1024L * 1024L * 1024L;   // 1G
    private String fileName;
    private String path;
    private long size;
    private long blockSize;
    private long blockNum;
    private long threadNum;

    public long getBlockNum() {
        return blockNum;
    }
    public void setBlockNum(long blockNum) {
        this.blockNum = blockNum;
        threadNum = blockNum;
        blockSize = (long) Math.ceil(1.0*size/blockNum);
    }
    public Flag getFlag() {
        return flag;
    }
    public String getFileName() {
        return fileName;
    }
    public String getPath() {
        return path;
    }
    public long getSize() {
        return size;
    }
    public long getBlockSize() {
        return blockSize;
    }
    public void setBlockSize(int blockSize) {
        this.blockSize = blockSize;
    }
    public long getThreadNum() {
        return threadNum;
    }
    public void setThreadNum(int threadNum) {
        this.threadNum = threadNum;
        blockNum = threadNum;
        blockSize = (long) Math.ceil(1.0*size/threadNum);
    }


    public FileSegment(String fn, Flag flag){
        this.fileName = fn;
        this.flag = flag;
        this.threadNum = 20;
        this.size = getFileSizeLocal(fn);

        if (flag == Flag.NET){
            segmentNet();
        }else if (flag == Flag.LOCAL){
            segmentLocal();
        }
    }

    public static long getFileSizeLocal(String path){
        File file = new File(path);
        return file.length();
    }

    public static long getFileSizeNet(String path){
        long sz = 0;
        try {
            FileInputStream fis = new FileInputStream(path);
            FileChannel fc = fis.getChannel();
            sz = fc.size();
        } catch (IOException e) {
            e.printStackTrace();
        }
        return sz;
    }

    public void segmentLocal(){
        if (size <= LOCAL_BLOCK){       //10M
            threadNum = 1;
            blockNum = 1;
            blockSize = LOCAL_BLOCK;
        }else if (size <= LOCAL_BLOCK * 20){    //200M
            blockSize = LOCAL_BLOCK;
            threadNum = (long) Math.ceil(1.0*size/LOCAL_BLOCK);
            blockNum = threadNum;
        }else if (size <= LOCAL_BLOCK * 110){    //1100M
            threadNum = 20;
            blockNum = threadNum;
            blockSize = (long) Math.ceil(1.0*size/blockNum);
        }else{    //1100M
            threadNum = 30;
            blockNum = threadNum;
            blockSize = (long) Math.ceil(1.0*size/blockNum);
        }

    }

    public void segmentNet(){
        //
        if (size <= NET_BLOCK){       //4M
            blockNum = 1;
            blockSize = NET_BLOCK;
        }else if (size <= NET_BLOCK*30){       //120M
            blockSize = NET_BLOCK*2;
            blockNum = (long) Math.ceil(1.0*size/blockSize);
        }else if (size <= NET_BLOCK*200){       //800M
            blockSize = NET_BLOCK*5;
            blockNum = (long) Math.ceil(1.0*size/blockSize);
        }else {       //>800M
            blockSize = NET_BLOCK*8;
            blockNum = (long) Math.ceil(1.0*size/blockSize);
        }
    }

}
