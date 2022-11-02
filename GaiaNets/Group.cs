using System;
using System.Collections.Generic;
using System.Linq;

namespace GaiaNet.GaiaNets
{
    public class Group {
        private String groupName;
        private Node owner;
        // private LinkedList<Node> nodes = new LinkedList<Node>;
        // private static TreeMap<String, Group> groups = new TreeMap<>();

        public string GroupName { get => groupName; set => groupName = value; }
        public Node Owner { get => owner; set => owner = value; }

        // public Node[] GetNodes() {
        //     Node[] nds = new Node[this.nodes.size()];
        //     nodes.toArray(nds);
        //     return nds;
        // }

        public Group(String groupName) {
            this.GroupName = groupName;
            // groups.put(groupName, this);
            // Message.toAllGroup();
        }

        // public static Group getGroupByName(String name){
        //     if (groups.containsKey(name)){
        //         return groups.get(name);
        //     }else {
        //         return null;
        //     }
        // }

        // public static Group[] getGroupsName(){
        //     Group[] groupsA = new Group[groups.size()];
        //     return groupsA;
        // }

        // public void addNode(Node node){
        //     if (nodes.contains(node))
        //         return;
        //     nodes.add(node);
        //     //        Message.toGroup();
        // }

        // public void removeNode(Node node){
        //     if (nodes.contains(node))
        //         nodes.remove(node);
        //     // Message.toGroup();
        // }

        public void setPasswd() {
        }
    }
}