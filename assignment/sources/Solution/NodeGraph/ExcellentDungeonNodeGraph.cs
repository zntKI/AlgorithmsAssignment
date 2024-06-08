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
        for (int i = 0; i < _tiledView.columns * _tiledView.rows; ++i)
        {
            //TODO: Think of better way of choosing the first tile (ignore rooms that do not have a connection with other rooms)
            TileType tileType = _tiledView.GetTileType(i % _tiledView.columns, i / _tiledView.columns);
            if (tileType.walkable) // Get the first accessible ground tile and starts the recursion from there
            {
                GenerateNode(i, null);
                break;
            }
        }
    }

    void GenerateNode(int index, Node previousNode)
    {
        int x = index % _tiledView.columns;
        int y = index / _tiledView.columns;

        TileType tileType = _tiledView.GetTileType(x, y);
        Point pos = getPointCenter(new Point(x, y));
        if (!tileType.walkable)
            return;
        else if (nodes.Keys.FirstOrDefault(n => n.location == pos) != null
            /*Check if Node already exists on the given pos*/)
        {
            Node nodeToConnectWith = nodes.Keys.FirstOrDefault(n => n.location == pos);
            nodes[previousNode].Add(nodeToConnectWith);
            return;
        }

        //Create the Node
        Node node = new Node(pos);
        nodes.Add(node, new List<Node>());

        //Recursively traverse neighbouring nodes
        GenerateNode(index + 1, node); //To the right
        GenerateNode(index + _tiledView.columns, node); //To the bottom
        GenerateNode(index - 1, node); //To the left
        GenerateNode(index - _tiledView.columns, node); //To the top

        //Make connection as well
        if (previousNode != null)
            nodes[previousNode].Add(node);
    }
}