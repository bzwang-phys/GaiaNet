using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Net.Sockets;
using GaiaNet.BasicNet;

namespace GaiaNet.FilesTransfer
{
    public class ProgressInfo{
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

    public class NetTransferInfo{
        public String filePath;
        public String fileName;
        public long size;
        public long start;
        public long end;
        public long index;  // for resuming from breakpoint.
        public long progress;  // for collecting the information of progress.
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
    
    public class SendFiles
    {
        private String ip;
        private int port;
        private ReaderWriterLockSlim  rwLocks = new ReaderWriterLockSlim();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(
                System.Reflection.MethodBase.GetCurrentMethod().ReflectedType);


        public SendFiles(String ip, int port) {
            this.ip = ip;
            this.port = port;
        }


        /*
        * Use threadPool to send the blocks of the file,
        * the block is described by fs.
        */
        public void SendFile(String filePath){
            try {
            if (!File.Exists(filePath)){
                System.Console.WriteLine("File Does not exist.");
                throw new FileNotFoundException();
            }
            String fileName = Path.GetFileName(filePath);
            FileSegment fsg = new FileSegment(filePath, TransferType.NET);
            NetTransferInfo[] ntis = new NetTransferInfo[fsg.blockNum];
            ProgressInfo pi = new ProgressInfo();
            System.Console.WriteLine(String.Format("blockNum: {0}, blockSize: {1}", fsg.blockNum, fsg.blockSize/1024/1024));
            
            ThreadPool.SetMaxThreads(40, 40);
            // this list is used to collect all netTransferInfo, and the progrss.
            for (int blockIndex = 0; blockIndex < fsg.blockNum; blockIndex++) {
                NetTransferInfo netTransferInfo = new NetTransferInfo(filePath,fileName,
                        fsg.size,
                        blockIndex * fsg.blockSize,
                        (blockIndex + 1) * fsg.blockSize,
                        0);
                ntis[blockIndex] = netTransferInfo;
                ThreadPool.QueueUserWorkItem(state => runSend(netTransferInfo));
            }

            // shows the progress = send/size.
            while (true){
                long start = 0L;
                foreach (var nti in ntis) {
                    start += nti.progress - nti.start;
                }
                pi.index = start;
                pi.sum = fsg.size;
                int progress = (int) Math.floor(100.0*start/fsg.size);
                System.Console.WriteLine("progress: " + progress + "%");
                Thread.sleep(2000);
                if (progress >= 100){ break; }
            }
            } catch (Exception) {
                System.Console.WriteLine("Something wrong in SendFile() function.");
            }

        }

        /*
        * Send a block of the file,
        * the block is described by fs.
        */
        private void runSend(NetTransferInfo nti) {
            long start = nti.start;
            if (start > nti.size-1){    // 0 <= start <= size-1
                // Solve the problem that blockNum/threadNum may be large than ...
                return;
            }
            if (nti.end > nti.size){ // 1 <= end <= size
                nti.end = nti.size;
            }
            try {
            TcpClient sendClient = new TcpClient(ip, port);
            FileHeader fileHeader = new FileHeader(nti.fileName,
                    nti.start, nti.end, nti.index);
            byte[] dataSend = (new byte[] {(byte)NetType.File}).Concat(fileHeader.ToBytes()).ToArray();
            sendClient.Client.Send(dataSend);
            using (FileStream fs = new FileStream(nti.filePath, FileMode.Open, FileAccess.Read)){
                fs.Seek(start, SeekOrigin.Begin);
                long blockSize = 500L * 1024;   //500Kb
                byte[] data = new byte[(int) blockSize];
                while (true) {
                    if (start + blockSize >= nti.end) {
                        blockSize = nti.end - start;
                    }
                    // read data
                    rwLocks.EnterReadLock();
                    fs.Seek(start, SeekOrigin.Begin);
                    int readlen = fs.Read(data, 0, (int) blockSize);
                    rwLocks.ExitReadLock();

                    // write data
                    sendClient.Client.Send(data[0..readlen]);

                    start += readlen;
                    nti.progress = start;
                    if (start >= nti.end) {sendClient.Close(); break;}
                }
            }
            }catch (Exception) {
                System.Console.WriteLine(String.Format("Send {0} failed from {1} to {2}", nti.filePath,nti.start,nti.end));
            }
        }
    }



}
