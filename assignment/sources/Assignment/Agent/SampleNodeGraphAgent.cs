using GXPEngine;
using System.Linq;

/**
 * Very simple example of a nodegraphagent that walks directly to the node you clicked on,
 * ignoring walls, connections etc.
 */
class SampleNodeGraphAgent : NodeGraphAgent
{
	//Current target to move towards
	protected Node _target = null;

	public SampleNodeGraphAgent(NodeGraph pNodeGraph) : base(pNodeGraph)
	{
		SetOrigin(width / 2, height / 2);

		//position ourselves on a random node
		if (pNodeGraph.nodes.Count > 0)
		{
			int rnd = Utils.Random(0, pNodeGraph.nodes.Count);
			Node startingNode = pNodeGraph.nodes.Keys.First(n => n.id == rnd.ToString());
            jumpToNode(startingNode);
			_target = startingNode; //Sets the target node to the starting one so that it can be read and assigned to endNode in the children' constructors and then nullified again
		}

		//listen to nodeclicks
		pNodeGraph.OnNodeLeftClicked += onNodeClickHandler;
	}

	protected virtual void onNodeClickHandler(Node pNode)
	{
		_target = pNode;
	}

	protected override void Update()
	{
		//no target? Don't walk
		if (_target == null) return;

		//Move towards the target node, if we reached it, clear the target
		if (moveTowardsNode(_target))
		{
			_target = null;
		}
	}
}
