package GaiaNet.Windows;

import GaiaNet.Common.FileSegment;
import GaiaNet.Common.Files;
import GaiaNet.NetTransfer.Client;
import GaiaNet.NetTransfer.Server;
import javafx.concurrent.Service;
import javafx.concurrent.Task;
import javafx.scene.control.Button;
import javafx.scene.control.Label;
import javafx.scene.control.ProgressBar;
import javafx.scene.control.TextField;

import java.io.File;

public class NetTransferSingleContr {
    public TextField ip;
    public TextField port;
    public TextField portServer;
    public TextField files;
    public TextField blockNum;
    public Label size;
    public Label blockSize;
    public Button setting;
    public Button transfer;
    public Button listen;
    public ProgressBar progressBar;

    private Server server;
    private Client client;
    private FileSegment fs;
    private String filePath;

    public void transferClick(){
        String ip1 = ip.getText();
        Integer port1 = Integer.valueOf(port.getText());
        filePath = files.getText();

        try {
            client = new Client(ip1, port1);
            fs = settingClick();

            NewService newService = new NewService();
            newService.start();

            progressBar.progressProperty().bind(newService.progressProperty());

        } catch (Exception e){
            e.printStackTrace();
        }
    }

    public void listenClick(){
        Integer portServer1 = Integer.valueOf(portServer.getText());
        server = new Server(portServer1);
        server.serverRun();
    }

    public void stopListenClick(){
        server.stop();
    }


    public FileSegment settingClick() {
        File file = new File(files.getText());
        long sizeL = file.length();
        size.setText(sizeL/1024/1024+" M");
        FileSegment fs = new FileSegment(file.toString(), FileSegment.Flag.NET);
        String blockNum1 = blockNum.getText();
        if (blockNum1!=null && blockNum1.length()!=0 && Long.parseLong(blockNum1)!=0){
            long blockNumLong = Long.parseLong(blockNum1);
            fs.setBlockNum(blockNumLong);
        }else {
            blockNum.setText(String.valueOf(fs.getBlockNum()));
        }
        blockSize.setText(fs.getBlockSize()/1024/1024+" M");

        return fs;
    }



    class NewService extends Service<Number> {
        @Override
        protected Task<Number> createTask() {
            NewTask newTask = new NewTask();
            return newTask;
        }
    }

    class NewTask extends Task<Number> {
        @Override
        protected Number call() throws Exception {
            this.updateTitle("Copying......");
            Files.ProgressInfo progressInfo = new Files.ProgressInfo(0, 1);
            new Thread(()->{
                try {
                    client.sendFile(filePath, fs, progressInfo);
                } catch (Exception e) {
                    e.printStackTrace();
                }
            }).start();
            while (progressInfo.index < progressInfo.sum){
                updateProgress(progressInfo.index, progressInfo.sum);
                Thread.sleep(1000);
            }
            return 0;
        }
    }


}
