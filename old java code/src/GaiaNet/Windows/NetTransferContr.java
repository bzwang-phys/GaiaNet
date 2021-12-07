package GaiaNet.Windows;

import GaiaNet.Common.FileSegment;
import javafx.scene.control.Button;
import javafx.scene.control.Label;
import javafx.scene.control.TextField;

import java.io.File;

public class NetTransferContr {
    public TextField ip;
    public TextField port;
    public TextField files;
    public TextField blockNum;
    public Label size;
    public Label blockSize;
    public Button setting;
    public Button transfer;

    public void transferClick(){
        String ip1 = ip.getText();
        Integer port1 = Integer.valueOf(port.getText());
        String filePath = files.getText();


    }

    public void settingClick() {
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
    }
}
