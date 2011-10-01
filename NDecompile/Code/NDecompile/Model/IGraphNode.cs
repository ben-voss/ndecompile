using System.Collections.Generic;

namespace LittleNet.NDecompile.Model
{
	/// <summary>
	/// Represents a node in a graph
	/// </summary>
	public interface IGraphNode
	{
		/// <summary>
		/// Gets this nodes number
		/// </summary>
		int Number
		{
			get;
		}

		/// <summary>
		/// Gets this nodes immediate dominator number
		/// </summary>
		int ImmediateDominatorNumber
		{
			get;
		}

		/// <summary>
		/// Gets this nodes number in the graphs depth first traversal order
		/// </summary>
		int DepthFirstSearchLastNumber
		{
			get;
		}

		/// <summary>
		/// Gets a list of nodes that this node has edges to
		/// </summary>
		IList<IGraphNode> OutEdges
		{
			get;
		}

		/// <summary>
		/// Gets a list of nodes that have an edge that points to this node
		/// </summary>
		IList<IGraphNode> InEdges
		{
			get;
		}

		/// <summary>
		/// Gets the interval that this node belongs to
		/// </summary>
		IGraphNode Interval
		{
			get;
			set;
		}
	}
}
