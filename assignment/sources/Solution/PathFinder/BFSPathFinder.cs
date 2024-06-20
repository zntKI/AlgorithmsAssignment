using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class BFSPathFinder : PathFinder
{
    public BFSPathFinder(NodeGraph pGraph) : base(pGraph)
    {
    }

    protected override void generate()
    {
        Queue<Node> todoQueue = new Queue<Node>();
        List<Node> doneList = new List<Node>();

        todoQueue.Enqueue(_startNode);

        //bool done = false;

        Node currentNode;
        while (todoQueue.Count > 0)
        {
            currentNode = todoQueue.Dequeue();
            doneList.Add(currentNode);

            if (currentNode == _endNode)
                GeneratePath(currentNode);
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

    void GeneratePath(Node lastNode)
    {
        Stack<Node> path = new Stack<Node>();
        path.Push(lastNode);

        Node tempNode = lastNode;
        Node currentParent = lastNode.parent;
        while (currentParent != null)
        {
            path.Push(currentParent);

            currentParent = currentParent.parent;

            tempNode.parent = null;
            tempNode = currentParent;
        }

        for (int i = path.Count - 1; i >= 0; i--)
        {
            _lastCalculatedPath.Enqueue(path.Pop());
        }
    }
}