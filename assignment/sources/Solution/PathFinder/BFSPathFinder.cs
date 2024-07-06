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

        Node currentNode;
        while (todoQueue.Count > 0)
        {
            currentNode = todoQueue.Dequeue();
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
                    if (!todoQueue.Contains(connectedNode) && !doneList.Contains(connectedNode))
                    {
                        connectedNode.parent = currentNode;
                        todoQueue.Enqueue(connectedNode);
                    }
                }
            }
        }

        // Console.WriteLine("No possible path");
    }

    void GeneratePath(Node endNode)
    {
        Console.WriteLine("In");

        Stack<Node> path = new Stack<Node>();
        path.Push(endNode);

        Node tempNode = endNode;
        Node currentParent = endNode.parent;
        while (currentParent != null)
        {
            path.Push(currentParent);

            // Remove parent connections to prevent errors next time a path is being generated
            tempNode.parent = null;
            tempNode = currentParent;

            currentParent = currentParent.parent;
        }

        for (int i = path.Count - 1; i >= 0; i--)
        {
            _lastCalculatedPath.Enqueue(path.Pop());
        }
    }
}