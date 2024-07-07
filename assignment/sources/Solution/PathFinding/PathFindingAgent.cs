using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class PathFindingAgent : SampleNodeGraphAgent
{
    PathFinder _pathFinder;

    Queue<Node> pathToFollow;
    Node stayNode;

    public PathFindingAgent(NodeGraph pNodeGraph, PathFinder pathFinder) : base(pNodeGraph)
    {
        _pathFinder = pathFinder;

        pathToFollow = new Queue<Node>();

        stayNode = _target; // Since in the base constructor 'target' is temporary set to the starting node, just to retrieve in child's constructors
        _target = null; // Then set it to null again
    }

    protected override void onNodeClickHandler(Node pNode)
    {
        if (_target == null)
        {
            pathToFollow = new Queue<Node>(_pathFinder.Generate(stayNode, pNode));
            _target = pathToFollow.Dequeue();
        }
    }

    protected override void Update()
    {
        //no target? Don't walk
        if (_target == null) return;

        //Move towards the target node, if we reached it, clear the target
        if (moveTowardsNode(_target))
        {
            if (pathToFollow.Count > 0)
            {
                _target = pathToFollow.Dequeue();
                if (pathToFollow.Count == 0)
                    stayNode = _target;
            }
            else
            {
                _target = null;
                _pathFinder.Clear();
            }
        }
    }
}
