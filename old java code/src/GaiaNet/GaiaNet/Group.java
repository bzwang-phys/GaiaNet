package GaiaNet.GaiaNet;

import java.util.LinkedList;
import java.util.TreeMap;

public class Group {
    private String groupName;
    private Node owner;
    private LinkedList<Node> nodes = new LinkedList<>();
    private static TreeMap<String, Group> groups = new TreeMap<>();

    public String getGroupName() {
        return groupName;
    }
    public Node[] getNodes() {
        Node[] nds = new Node[this.nodes.size()];
        nodes.toArray(nds);
        return nds;
    }

    public Group(String groupName) {
        this.groupName = groupName;
        groups.put(groupName, this);
        // Message.toAllGroup();
    }

    public static Group getGroupByName(String name){
        if (groups.containsKey(name)){
            return groups.get(name);
        }else {
            return null;
        }
    }

    public static Group[] getGroupsName(){
        Group[] groupsA = new Group[groups.size()];
        return groupsA;
    }

    public void addNode(Node node){
        if (nodes.contains(node))
            return;
        nodes.add(node);
        //        Message.toGroup();
    }

    public void removeNode(Node node){
        if (nodes.contains(node))
            nodes.remove(node);
        // Message.toGroup();
    }

    public void setPasswd() {
    }
}
