using GXPEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class ExcellentDungeonNodeGraph : SampleDungeonNodeGraph
{
    TiledView _tiledView;

    public ExcellentDungeonNodeGraph(Dungeon pDungeon, TiledView pTiledView) : base(pDungeon)
    {
        Debug.Assert(pTiledView != null, "Please provide a valid TiledView" + pTiledView);
        _tiledView = pTiledView;
    }

    protected override void generate()
    {

        //If sufficient dungeon
        //
        int i = _tiledView.columns + 1;
        GenerateNode(i, null);

        // If excellent dungeon
        //
        //for (int i = 0; i < _tiledView.columns * _tiledView.rows; ++i)
        //{
        //    TileType tileType = _tiledView.GetTileType(i % _tiledView.columns, i / _tiledView.columns);
        //    if (tileType.walkable) // Get the first accessible ground tile and starts the recursion from there
        //    {
        //        GenerateNode(i, null);
        //        break;
        //    }
        //}

        // If good dungeon
        //
        //Get a random room that has at least ome connection to another room
        //int rnd = Utils.Random(0, _dungeon.rooms.Count);
        //Room roomToStartFrom = _dungeon.rooms[rnd];
        //bool doesRoomHaveConnections = CheckRoomForConnections(roomToStartFrom);
        //while (!doesRoomHaveConnections)
        //{
        //    rnd = Utils.Random(0, _dungeon.rooms.Count);
        //    roomToStartFrom = _dungeon.rooms[rnd];
        //    doesRoomHaveConnections = CheckRoomForConnections(roomToStartFrom);
        //}

        ////Gets the top left ground tile
        //Point pos = new Point(roomToStartFrom.area.X + 1, roomToStartFrom.area.Y + 1);
        ////Converts the pos to a tiledView index
        //int index = pos.Y * _tiledView.columns + pos.X;

        ////Start recursion
        //GenerateNode(index, null);
    }

    /// <summary>
    /// Checks if a given room has at least one connection to another room
    /// </summary>
    //bool CheckRoomForConnections(Room roomToChek)
    //{
    //    foreach (var door in _dungeon.doors)
    //    {
    //        if (door.roomA == roomToChek || door.roomB == roomToChek)
    //            return true;
    //    }

    //    return false;
    //}

    /// <summary>
    /// Recursive function which generates and connects nodes
    /// </summary>
    void GenerateNode(int index, Node previousNode)
    {
        int x = index % _tiledView.columns;
        int y = index / _tiledView.columns;

        TileType tileType = _tiledView.GetTileType(x, y);
        Point pos = getPointCenter(new Point(x, y));
        Node nodeToConnectWith;
        if (!tileType.walkable)
            return;
        else if (TryGetNodeAtPosition(pos, out nodeToConnectWith))
        {
            nodes[previousNode].Add(nodeToConnectWith);
            return;
        }

        //Create the Node
        Node node = new Node(pos);
        nodes.Add(node, new List<Node>());

        //Recursively traverse neighbouring nodes
        GenerateNode(index + 1, node); //To the right
        GenerateNode(index + _tiledView.columns + 1, node); //To the bottom-right
        GenerateNode(index + _tiledView.columns, node); //To the bottom
        GenerateNode(index + _tiledView.columns - 1, node); //To the bottom-left
        GenerateNode(index - 1, node); //To the left
        GenerateNode(index - _tiledView.columns - 1, node); //To the top-left
        GenerateNode(index - _tiledView.columns, node); //To the top
        GenerateNode(index - _tiledView.columns + 1, node); //To the top-right

        //Make connection as well
        if (previousNode != null)
            nodes[previousNode].Add(node);
    }

    /// <summary>
    /// Checks if a node already exists on the given pos
    /// </summary>
    /// <param name="node">The node if there was such</param>
    bool TryGetNodeAtPosition(Point positionToCheck, out Node node)
    {
        node = nodes.Keys.FirstOrDefault(n => n.location == positionToCheck);
        return node != null ? true : false;
    }
}