using GXPEngine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class AStarPathFinder : DijkstraPathFinder
{
    public AStarPathFinder(NodeGraph pGraph) : base(pGraph)
    {
    }

    protected override void generate()
    {
        SortedDictionary<int, SortedDictionary<(int, int), List<Node>>> todoDict = new SortedDictionary<int, SortedDictionary<(int, int), List<Node>>>();
        List<Node> doneList = new List<Node>();

        int distanceStartToEndNode = DistanceBetweenNodes(_startNode.location, _endNode.location);
        todoDict.Add(distanceStartToEndNode + 0,
            new SortedDictionary<(int, int), List<Node>>()
            {
                { (distanceStartToEndNode, 0), new List<Node>() { _startNode } }
            });

        Node currentNode;
        while (todoDict.Count > 0)
        {
            var kvp = todoDict.First();

            if (kvp.Value.Count == 0)
                throw new InvalidOperationException("Dictionary element with no nodes should have been deleted!");

            var innerKvp = kvp.Value.First();
            currentNode = innerKvp.Value[0];

            todoDict[kvp.Key][innerKvp.Key].Remove(currentNode);
            doneList.Add(currentNode);

            if (currentNode == _endNode)
            {
                DrawNodeCoverage(doneList, todoDict.Values.SelectMany(list => list.Values.SelectMany(il => il)).ToList());

                GeneratePath(currentNode);

                disabledNodes.Clear();

                return;
            }
            else
            {
                foreach (var connectedNode in _nodeGraph.nodes[currentNode])
                {
                    if (!disabledNodes.Contains(connectedNode) &&
                        !doneList.Contains(connectedNode) &&
                        !todoDict.Any(k => k.Value.Any(ik => ik.Value.Contains(connectedNode))))
                    {
                        connectedNode.parent = currentNode;

                        int distanceToEnd = DistanceBetweenNodes(connectedNode.location, _endNode.location);

                        int distanceFromPrevious = DistanceBetweenNodes(connectedNode.location, currentNode.location);
                        int accumulatedDistance = innerKvp.Key.Item2 + distanceFromPrevious;

                        int distanceSum = distanceToEnd + accumulatedDistance;
                        if (!todoDict.ContainsKey(distanceSum))
                        {
                            todoDict.Add(distanceSum, new SortedDictionary<(int, int), List<Node>>());
                        }
                        if (!todoDict[distanceSum].ContainsKey((distanceToEnd, accumulatedDistance)))
                        {
                            todoDict[distanceSum].Add((distanceToEnd, accumulatedDistance), new List<Node>());
                        }
                        todoDict[distanceSum][(distanceToEnd, accumulatedDistance)].Add(connectedNode);
                    }
                }

                if (todoDict[kvp.Key][innerKvp.Key].Count == 0)
                {
                    todoDict[kvp.Key].Remove(innerKvp.Key);
                    if (todoDict[kvp.Key].Count == 0)
                    {
                        todoDict.Remove(kvp.Key);
                    }
                }

            }
        }

        // Console.WriteLine("No possible path");
    }
}