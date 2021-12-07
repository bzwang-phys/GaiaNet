using System;
using System.IO;

namespace GaiaNet.Common
{
    public class FileSegment
    {
        public enum Flag {LOCAL, NET}
        public Flag flag {get;set;}
        private const long LOCAL_BLOCK = 10L * 1024L * 1024L;      // 10M
        //    private final long LOCAL_MAX_BLOCK =  150L * 1024L * 1024L;   // 150 M
        //    private final long LOCAL_MAX_SIZE =  4L * 1024L * 1024L * 1024L;   // 4G
        private const long NET_BLOCK = 4L * 1024L * 1024L;      //4M
        //    private final long NET_MAX_BLOCK =  10L * 1024L * 1024L;
        //    private final long NET_MAX_SIZE =  1L * 1024L * 1024L * 1024L;   // 1G
        public string fileName {get;set;}
        public string path {get;set;}
        public long size {get;set;}
        public long blockSize {get;set;}
        public long blockNum {get;set;}
        public long threadNum {get;set;}
        
        public FileSegment(String fn, Flag flag){
            this.fileName = fn;
            this.flag = flag;
            this.size = getFileSize(fn);
            this.threadNum = 20;

            if (flag == Flag.NET){
                segmentNet();
            }else if (flag == Flag.LOCAL){
                segmentLocal();
            }
        }

        public static long getFileSize(String path){
            FileInfo fi = new FileInfo(path);
            if (!fi.Exists){ return 0; }
            long size = fi.Length;
            return size;
        }

        public void segmentLocal(){
            if (size <= LOCAL_BLOCK){       //10M
                threadNum = 1;
                blockNum = 1;
                blockSize = LOCAL_BLOCK;
            }else if (size <= LOCAL_BLOCK * 20){    //200M
                blockSize = LOCAL_BLOCK;
                threadNum = (long) Math.Ceiling(1.0*size/LOCAL_BLOCK);
                blockNum = threadNum;
            }else if (size <= LOCAL_BLOCK * 110){    //1100M
                threadNum = 20;
                blockNum = threadNum;
                blockSize = (long) Math.Ceiling(1.0*size/blockNum);
            }else{    //1100M
                threadNum = 30;
                blockNum = threadNum;
                blockSize = (long) Math.Ceiling(1.0*size/blockNum);
            }

        }

        public void segmentNet(){
            //
            if (size <= NET_BLOCK){       //4M
                blockNum = 1;
                blockSize = NET_BLOCK;
            }else if (size <= NET_BLOCK*30){       //120M
                blockSize = NET_BLOCK*2;
                blockNum = (long) Math.Ceiling(1.0*size/blockSize);
            }else if (size <= NET_BLOCK*200){       //800M
                blockSize = NET_BLOCK*5;
                blockNum = (long) Math.Ceiling(1.0*size/blockSize);
            }else {       //>800M
                blockSize = NET_BLOCK*8;
                blockNum = (long) Math.Ceiling(1.0*size/blockSize);
            }
        }

    }
}