using GXPEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class GoodNodeGraphAgent : SampleNodeGraphAgent
{
    NodeGraph nodeGraph;

    Node finalNode;
    Node lastVisited;

    public GoodNodeGraphAgent(NodeGraph pNodeGraph) : base(pNodeGraph)
    {
        nodeGraph = pNodeGraph;
    }

    protected override void onNodeClickHandler(Node pNode)
    {
        if (finalNode == null)
        {
            finalNode = pNode;
            PickRandomNode();
        }
    }

    protected override void Update()
    {
        //no target? Don't walk
        if (finalNode == null) return;

        //Move towards the target node, if we reached it, pick another one (sort of) randomly
        if (moveTowardsNode(_target))
        {
            PickRandomNode();
        }
    }

    void PickRandomNode()
    {
        if (_target == finalNode)
        {
            finalNode = null;
            return;
        }

        //If the final node is reachable from the current node, go straight to it
        if (nodeGraph.nodes[_target].Contains(finalNode))
        {
            _target = finalNode;
            return;
        }

        Node tempTarget = _target;

        int connectionsCount = nodeGraph.nodes[_target].Count;
        int rnd = Utils.Random(0, connectionsCount);
        _target = nodeGraph.nodes[tempTarget][rnd];

        //Prevents the player from going back to the node it previously came from, with the exception of nodes that have only one connection
        while (_target == lastVisited && connectionsCount > 1)
        {
            rnd = Utils.Random(0, connectionsCount);
            _target = nodeGraph.nodes[tempTarget][rnd];
        }

        lastVisited = tempTarget;
    }
}
