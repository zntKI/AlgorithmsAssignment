﻿using GXPEngine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class DijkstraPathFinder : SearchPathFinder
{
    List<Node> disabledNodes;

    public DijkstraPathFinder(NodeGraph pGraph) : base(pGraph)
    {
        _nodeGraph.OnNodeRightClicked += AddNodeToDisabledNodes;

        disabledNodes = new List<Node>();
    }

    void AddNodeToDisabledNodes(Node node)
    {
        if (!disabledNodes.Contains(node))
        {
            disabledNodes.Add(node);
            drawNode(node, Brushes.Black);
        }
        else
        {
            disabledNodes.Remove(node);
            drawNode(node, _nodeGraph.defaultNodeColor);
        }

        Console.WriteLine(DungeonFullyConnected() ? "Yes" : "No");
    }

    bool DungeonFullyConnected()
    {
        Queue<Node> todoQueue = new Queue<Node>();
        List<Node> doneList = new List<Node>();

        todoQueue.Enqueue(_nodeGraph.nodes.First().Key);

        Node currentNode;
        while (todoQueue.Count > 0)
        {
            currentNode = todoQueue.Dequeue();
            doneList.Add(currentNode);

            foreach (var connectedNode in _nodeGraph.nodes[currentNode])
            {
                if (!disabledNodes.Contains(connectedNode) &&
                    !todoQueue.Contains(connectedNode) &&
                    !doneList.Contains(connectedNode))
                {
                    connectedNode.parent = currentNode;
                    todoQueue.Enqueue(connectedNode);
                }
            }
        }

        return doneList.Count == _nodeGraph.nodes.Count - disabledNodes.Count;
    }

    public void ClearDisabledNodes()
        => disabledNodes.Clear();

    protected override void generate()
    {
        SortedDictionary<int, List<Node>> todoDict = new SortedDictionary<int, List<Node>>();
        List<Node> doneList = new List<Node>();

        todoDict.Add(0, new List<Node>() { _startNode });

        Node currentNode;
        while (todoDict.Count > 0)
        {
            var kvp = todoDict.First();
            int rndIndex = Utils.Random(0, kvp.Value.Count);

            currentNode = kvp.Value[rndIndex];

            todoDict[kvp.Key].RemoveAt(rndIndex);
            doneList.Add(currentNode);

            if (currentNode == _endNode)
            {
                GeneratePath(currentNode);
                return;
            }
            else
            {
                foreach (var connectedNode in _nodeGraph.nodes[currentNode])
                {
                    if (!disabledNodes.Contains(connectedNode) &&
                        !doneList.Contains(connectedNode) &&
                        !todoDict.Any(k => k.Value.Contains(connectedNode)))
                    {
                        connectedNode.parent = currentNode;

                        Vec2 location1 = new Vec2(connectedNode.location.X, connectedNode.location.Y);
                        Vec2 location2 = new Vec2(currentNode.location.X, currentNode.location.Y);
                        float distance = location1.DistanceTo(location2);
                        int accumulatedDistance = kvp.Key + (int)distance;
                        if (!todoDict.ContainsKey(accumulatedDistance))
                        {
                            todoDict.Add(accumulatedDistance, new List<Node>());
                        }
                        todoDict[accumulatedDistance].Add(connectedNode);
                    }
                }

                if (todoDict[kvp.Key].Count == 0)
                {
                    todoDict.Remove(kvp.Key);
                }
            }
        }

        // Console.WriteLine("No possible path");
    }
}