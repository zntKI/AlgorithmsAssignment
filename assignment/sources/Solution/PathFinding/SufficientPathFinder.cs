using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class SufficientPathFinder : PathFinder
{
    Stack<Node> _lastPathFound;

    public SufficientPathFinder(NodeGraph pGraph) : base(pGraph)
    {
        _lastPathFound = new Stack<Node>();
    }

    protected override void generate()
    {
        foreach (var node in _nodeGraph.nodes[_startNode])
        {
            //Prepare the temp collection
            _lastPathFound.Clear();
            _lastPathFound.Push(_startNode);
            _lastPathFound.Push(node);

            // If it is directly reachable from the start node
            if (_endNode == node)
            {
                SetCalculatedPath();
                return;
            }

            // Start recursion
            GeneratePath(_startNode, node);
        }
    }

    void GeneratePath(Node lastNode, Node currentNode)
    {
        foreach (var node in _nodeGraph.nodes[currentNode])
        {
            if (node == lastNode // If it tries to go back
                || _lastPathFound.Contains(node) // If the recursion reached a node that has already been processed
                || node == _startNode) // If the recursion made a 'circle' and reached the starting node
                continue;
            else if (node == _endNode)
            {
                _lastPathFound.Push(_endNode);
                CheckForShortestPath();
                _lastPathFound.Pop();
                continue;
            }

            _lastPathFound.Push(node);

            GeneratePath(currentNode, node);
            _lastPathFound.Pop();
        }
    }

    void CheckForShortestPath()
    {
        if (_lastCalculatedPath.Count == 0
            || _lastPathFound.Count < _lastCalculatedPath.Count)
        {
            _lastCalculatedPath.Clear();
            SetCalculatedPath();
        }
    }

    /// <summary>
    /// Transfer the nodes from the temp collection to the final one
    /// </summary>
    void SetCalculatedPath()
    {
        foreach (var node in _lastPathFound.Reverse())
        {
            _lastCalculatedPath.Enqueue(node);
        }
    }
}
