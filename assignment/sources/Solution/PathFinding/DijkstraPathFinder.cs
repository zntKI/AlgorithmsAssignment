using GXPEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class DijkstraPathFinder : SearchPathFinder
{
    public DijkstraPathFinder(NodeGraph pGraph) : base(pGraph)
    {
    }

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
                    if (!doneList.Contains(connectedNode) &&
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
