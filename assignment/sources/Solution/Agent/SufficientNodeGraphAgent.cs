using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class SufficientNodeGraphAgent : SampleNodeGraphAgent
{
    Queue<Node> _targets;

    public SufficientNodeGraphAgent(NodeGraph pNodeGraph) : base(pNodeGraph)
    {
        _targets = new Queue<Node>();
    }

    protected override void onNodeClickHandler(Node pNode)
    {
        _targets.Enqueue(pNode);
        if (_target == null)
            _target = _targets.Dequeue();
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
