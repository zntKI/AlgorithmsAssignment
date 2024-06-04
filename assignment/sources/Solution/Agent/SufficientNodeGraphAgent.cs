using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class SufficientNodeGraphAgent : SampleNodeGraphAgent
{
    NodeGraph nodeGraph;
    Queue<Node> _targets;

    //Stores the last node in the queue
    Node endNode;

    public SufficientNodeGraphAgent(NodeGraph pNodeGraph) : base(pNodeGraph)
    {
        nodeGraph = pNodeGraph;
        _targets = new Queue<Node>();

        //Sets the starting node to the 'endNode' and then resets '_target'
        endNode = _target;
        _target = null;
    }

    protected override void onNodeClickHandler(Node pNode)
    {
        TryEnqueue(pNode);
        if (_target == null)
            _target = _targets.Count > 0 ? _targets.Dequeue() : null;
    }

    /// <summary>
    /// Checks if the node that is to be enqueued is connected to the last node added to the queue. If so, adds it, otherwise skips it.
    /// </summary>
    void TryEnqueue(Node pNode)
    {
        foreach (var connection in nodeGraph.nodes[endNode])
        {
            if (pNode == connection)
            {
                endNode = pNode;
                _targets.Enqueue(endNode);
                break;
            }
        }
    }

    protected override void Update()
    {
        //no target? Don't walk
        if (_target == null) return;

        //Move towards the target node, if we reached it, clear the target
        if (moveTowardsNode(_target))
        {
            _target = _targets.Count > 0 ? _targets.Dequeue() : null;
        }
    }
}
