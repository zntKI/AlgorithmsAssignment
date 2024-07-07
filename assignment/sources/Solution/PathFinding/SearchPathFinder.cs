using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

abstract class SearchPathFinder : PathFinder
{
    protected SearchPathFinder(NodeGraph pGraph) : base(pGraph)
    {
    }

    protected virtual void GeneratePath(Node endNode)
    {
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

    protected virtual void DrawNodeCoverage(IEnumerable<Node> doneList, IEnumerable<Node> leftTodoList)
    {
        drawNodes(doneList, Brushes.DarkBlue);
        drawNodes(leftTodoList, Brushes.DarkOrange);
    }
}