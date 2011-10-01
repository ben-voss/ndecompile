using System.Collections.Generic;
using LittleNet.NDecompile.Model.Impl;

namespace LittleNet.NDecompile.Model
{
	/// <summary>
	/// Represents a node in a call graph
	/// </summary>
	public interface ICallGraphNode : IGraphNode
	{
		IList<IInstruction> Instructions
		{
			get;
		}

		ushort StartIP
		{
			get;
		}

		ushort EndIP
		{
			get;
		}

		NodeType NodeType
		{
			get;
		}

		/// <summary>
		/// List of out edges that travers to a back node - up the tree to a lower numbered node
		/// </summary>
		IList<ICallGraphNode> BackEdges
		{
			get;
		}

		ICallGraphNode IfFollow
		{
			get;
			set;
		}

        ICallGraphNode CaseHead
        {
            get;
            set;
        }

        ICallGraphNode CaseTail
        {
            get;
            set;
        }

        ICallGraphNode LatchNode
        {
            get;
            set;
        }

        ICallGraphNode LoopHead
        {
            get;
            set;
        }

        ICallGraphNode LoopFollow
        {
            get;
            set;
        }

        LoopType LoopType
        {
            get;
            set;
        }
		
		ICallGraphNode HandlerNode {
			get;
			set;
		}
		
		ICallGraphNode FollowNode {
			get;
			set;
		}
	}
}
