package GaiaNet.BasicNet;

import java.io.IOException;
import java.net.InetSocketAddress;
import java.nio.channels.SelectionKey;
import java.nio.channels.Selector;
import java.nio.channels.ServerSocketChannel;
import java.util.Set;

public class NioServer implements Runnable{

    private ServerSocketChannel serverSocketChannel;
    private Selector selector;


    public NioServer(int port){
        try {
            serverSocketChannel = ServerSocketChannel.open();
            selector = Selector.open();
            serverSocketChannel.configureBlocking(false);
            serverSocketChannel.socket().bind(new InetSocketAddress(port), 1024);
            serverSocketChannel.register(selector, SelectionKey.OP_ACCEPT);
        } catch (IOException e){
            e.printStackTrace();
            System.exit(1);
        }
    }

    @Override
    public void run() {
        while (true){
            try {
                int client = selector.select();
                System.out.println("new client: " + client);
                if (client==0) continue;
                Set<SelectionKey> selectionKeySet = selector.selectedKeys();
            } catch (IOException e) {
                e.printStackTrace();
            }
        }

    }
}
