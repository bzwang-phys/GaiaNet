using System;
using System.IO;
using System.Net.Sockets;
using System.Linq;
using System.Net;
using System.Threading;
using GaiaNet.BasicNet;

namespace GaiaNet.FilesTransfer
{
    public class RecvFiles
    {
        private Socket socket;
        private string basePath = "path/to/base";
        private ReaderWriterLockSlim  rwLocks = new ReaderWriterLockSlim();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(
                System.Reflection.MethodBase.GetCurrentMethod().ReflectedType);

        public RecvFiles(Socket socket){
            this.socket = socket;
        }

        public void Recv(){
            // Receive all datas of FileHeader.
            byte[] byts = new byte[2048];
            int revnum = this.socket.Receive(byts, 0, 37, SocketFlags.None); 
            while (revnum < 37){
                // 37 comes from the fileheader 1+8+8+8+8+4=37 
                Console.WriteLine("recv sockets data length: " + revnum);
                int numTem = this.socket.Receive(byts, revnum, 37-revnum, SocketFlags.None);
                revnum = revnum + numTem;
            }

            // Parse the FileHeader.
            FileHeader fileHeader = FileHeader.FromBytes(byts[0..37]);
            while (revnum < (37 + fileHeader.nameLen)){
                Console.WriteLine("recv sockets data length: " + revnum);
                int numTem = this.socket.Receive(byts, revnum, 37 + fileHeader.nameLen-revnum, SocketFlags.None);
                revnum = revnum + numTem;
            }

        }
        //  used to receive files. first read the [name,star,end], then write in file.
        private void ThreadRcvFile(FileHeader fileHeader) {
            int len;
            try{
            string fileName = fileHeader.name;
            string filePath = Path.Combine(basePath, fileName);
            if (!File.Exists(filePath)){
                using (File.Create(filePath)) { };
            }
            long start = fileHeader.start;
            long end = fileHeader.end;
            long index = fileHeader.index;
            Console.WriteLine("thread: " + Thread.CurrentThread.Name + " is ready to receive: " + fileName);
            Console.WriteLine(", start: {0}, end: {1}", start, end);
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite)){
                BinaryWriter bw = new BinaryWriter(fileStream);
                byte[] byt = new byte[500 * 1024];
                while ((len = socket.Receive(byt, 0, 500 * 1024, SocketFlags.None)) != 0)
                {
                    rwLocks.EnterWriteLock();
                    fileStream.Seek(start + index, SeekOrigin.Begin);
                    bw.Write(byt, 0, len);
                    index += len;
                    Console.WriteLine("receiving: " + Thread.CurrentThread.Name + ", start: {0}, end: {1}, index: {2}", start, end, index);
                    rwLocks.ExitWriteLock();
                }
                bw.Close();
            }
            }
            catch (IOException e){}
        }


    // //  used to write the
        private void createFile(Socket socket, FileHeader fileHeader){
            string fileName;
            try{
            fileName = fileHeader.name;
            long size = fileHeader.size;
            string filePath = Path.Combine(basePath, fileName);
            if (File.Exists(filePath) && (new FileInfo(filePath).Length == size)){
                // read configuration
            }
            else{
                using (File.Create(filePath))
                {
                    if (size > 0)
                    {
                        // File.SetLength(filePath, size);
                    }
                }
            }
            // rwLocks.TryAdd(filePath.GetHashCode(), new ReaderWriterLockSlim());
            Console.WriteLine(Thread.CurrentThread.Name + ", New file: " + fileName + ", size: " + size);
            } catch (IOException e) { }
        }


        public void stop() {
            try{
            socket.Shutdown(SocketShutdown.Both);
            } catch (SocketException e) { }
        }

        // Just print out the stream with hex string
        // public void PrintStream(Socket socket){
        //     try{
        //         byte[] byt = new byte[10];
        //         dis.Read(byt);
        //         Console.Write(ByteTools.HexString(byt));
        //     }
        //     catch (IOException e){
        //         Console.WriteLine(e.StackTrace);
        //     }
        // }

    }


    
}
