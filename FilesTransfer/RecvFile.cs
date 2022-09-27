using System;
using System.IO;
using System.Net;

namespace GaiaNet.FilesTransfer
{
    public class RecvFiles
    {
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
}