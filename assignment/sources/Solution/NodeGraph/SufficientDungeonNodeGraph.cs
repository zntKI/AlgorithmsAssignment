using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class SufficientDungeonNodeGraph : SampleDungeonNodeGraph
{
    public SufficientDungeonNodeGraph(Dungeon pDungeon) : base(pDungeon)
    {
    }

    protected override void generate()
    {
        //Loop through all the doors and create a node at their position and at the room's center that the door links
        foreach (var door in _dungeon.doors)
        {
            //Create and add the door Node
            Node doorNode = new Node(getDoorCenter(door));
            nodes.Add(doorNode, new List<Node>());

            //Room1
            Node nodeToConnect = GenerateRoomNode(door.roomA);
            AddConnection(nodeToConnect, doorNode);
            //Room2
            nodeToConnect = GenerateRoomNode(door.roomB);
            AddConnection(nodeToConnect, doorNode);
        }
    }

    /// <summary>
    /// Generates the node for the given room if one does not exist already
    /// </summary>
    /// <returns>Node to connect to the door Node</returns>
    Node GenerateRoomNode(Room room)
    {
        Point room1Pos = getRoomCenter(room);
        //Check if there is already a node generated at the given position
        Node nodeToCheck = nodes.Keys.FirstOrDefault(n => n.location == room1Pos);
        if (nodeToCheck == null)
        { 
            Node roomNode = new Node(room1Pos);
            nodes.Add(roomNode, new List<Node>());
            nodeToCheck = roomNode;
        }

        return nodeToCheck;
    }
}

