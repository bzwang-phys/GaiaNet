package GaiaNet.Windows;

import GaiaNet.Common.Files;
import javafx.beans.value.ChangeListener;
import javafx.beans.value.ObservableValue;
import javafx.concurrent.Service;
import javafx.concurrent.Task;
import javafx.scene.control.ProgressBar;
import javafx.scene.control.TextField;

import java.io.File;

public class CopyController {

    public TextField textFieldSource;
    public TextField textFieldTarget;
    public ProgressBar progressCopy;
    String pathSource;
    String pathTarget;

    public void button1Click() throws InterruptedException {
        pathSource = textFieldSource.getText();
        pathTarget = textFieldTarget.getText();

        NewService newService = new NewService();
        newService.start();

        progressCopy.progressProperty().bind(newService.progressProperty());

        newService.titleProperty().addListener(new ChangeListener<String>() {
            @Override
            public void changed(ObservableValue<? extends String> observableValue, String s, String t1) {
                textFieldSource.setText(t1);
                textFieldTarget.setText(t1);
            }
        });

    }







    class NewService extends Service<Number>{

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
            /*int j;
            for (j = 0; j < 1000; j++) {
                if (isCancelled()){
                    updateMessage("Cancelled");
                    break;
                }
                System.out.println("println: "+j);
                Thread.sleep(10);
                updateProgress(j, 1000);
                updateMessage("at: " + j);
            }*/
            Files.ProgressInfo progressInfo = new Files.ProgressInfo(0, 1);
            new Thread(()->{
                try {
                    Files.copy(pathSource, pathTarget, progressInfo);
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