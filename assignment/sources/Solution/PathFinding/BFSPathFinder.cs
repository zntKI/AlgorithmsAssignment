using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class BFSPathFinder : SearchPathFinder
{
    public BFSPathFinder(NodeGraph pGraph) : base(pGraph)
    {
    }

    protected override void generate()
    {
        Queue<Node> todoQueue = new Queue<Node>();
        HashSet<Node> doneList = new HashSet<Node>();

        todoQueue.Enqueue(_startNode);

        Node currentNode;
        while (todoQueue.Count > 0)
        {
            currentNode = todoQueue.Dequeue();
            doneList.Add(currentNode);

            if (currentNode == _endNode)
            {
                DrawNodeCoverage(doneList, todoQueue);

                GeneratePath(currentNode);
                return;
            }
            else
            {
                foreach (var connectedNode in _nodeGraph.nodes[currentNode])
                {
                    if (!todoQueue.Contains(connectedNode) && !doneList.Contains(connectedNode))
                    {
                        connectedNode.parent = currentNode;
                        todoQueue.Enqueue(connectedNode);
                    }
                }
            }
        }
    }
}