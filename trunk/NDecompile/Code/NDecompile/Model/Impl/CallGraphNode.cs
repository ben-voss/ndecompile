using System;
using System.Collections.Generic;

namespace LittleNet.NDecompile.Model.Impl
{
	internal class CallGraphNode : ICallGraphNode
	{
		// Start IP
		private readonly ushort _start;

		// Instructions
		private readonly IList<IInstruction> _instructions = new List<IInstruction>();

		/// <summary>
		/// Forward edges
		/// </summary>
		private readonly IList<IGraphNode> _outEdges = new List<IGraphNode>();

		/// <summary>
		/// Previous forward edges
		/// </summary>
		private readonly IList<IGraphNode> _inEdges = new List<IGraphNode>();

		private readonly IList<ICallGraphNode> _backEdges = new List<ICallGraphNode>();

		private int _immediateDominatorNumber = Int32.MaxValue;

		private int _depthFirstSearchLastNumber;

		private bool _traversed;

		private NodeType _nodeType;

		private IGraphNode _interval;

		private ICallGraphNode _ifFollow;

        private ICallGraphNode _caseHead;

        private ICallGraphNode _caseTail;

        private ICallGraphNode _latchNode;

        private ICallGraphNode _loopHead;

        private ICallGraphNode _loopFollow;

        private LoopType _loopType = LoopType.None;
		
		private ICallGraphNode _handlerNode;
		
		private ICallGraphNode _followNode;

		public CallGraphNode(ushort startIp, IList<IInstruction> instructions)
		{
			_instructions = instructions;
			_start = startIp;
		}

		public CallGraphNode(IList<IInstruction> instructions)
		{
			_instructions = instructions;
			_start = instructions[0].IP;
		}
		
		public CallGraphNode(ushort ip) {
			_instructions =  new List<IInstruction>();
			_start = ip;
		}

		public int Number
		{
			get
			{
				return _start;
			}
		}

		public IGraphNode Interval
		{
			get
			{
				return _interval;
			}
			set
			{
				_interval = value;
			}
		}

		public NodeType NodeType
		{
			get
			{
				return _nodeType;
			}
			set
			{
				_nodeType = value;
			}
		}

		/// <summary>
		/// Gets or sets a value that indicates if this node has been traversed
		/// </summary>
		public bool Traversed
		{
			get
			{
				return _traversed;
			}
			set
			{
				_traversed = value;
			}
		}

		/// <summary>
		/// Gets or sets this nodes position within the reversed depth first search order
		/// </summary>
		public int DepthFirstSearchLastNumber
		{
			get
			{
				return _depthFirstSearchLastNumber;
			}
			set
			{
				_depthFirstSearchLastNumber = value;
			}
		}

		/// <summary>
		/// Gets a reference to the list of nodes that the last instruction in this node
		/// can transfer control to.
		/// </summary>
		public IList<IGraphNode> OutEdges
		{
			get
			{
				return _outEdges;
			}
		}

		/// <summary>
		/// Gets a reference to the list of nodes that can transfer control to this node
		/// </summary>
		public IList<IGraphNode> InEdges
		{
			get
			{
				return _inEdges;
			}
		}

		/// <summary>
		/// Gets or sets this nodes immediate dominator node
		/// </summary>
		public int ImmediateDominatorNumber
		{
			get
			{
				return _immediateDominatorNumber;
			}
			set
			{
				_immediateDominatorNumber = value;
			}
		}

		/// <summary>
		/// The list of instructions in this block
		/// </summary>
		public IList<IInstruction> Instructions
		{
			get
			{
				return _instructions;
			}
		}

		/// <summary>
		/// The IP of the first instruction in this block
		/// </summary>
		public ushort StartIP
		{
			get
			{
				return _start;
			}
		}

		/// <summary>
		/// The IP of the last instruction in this block
		/// </summary>
		public ushort EndIP
		{
			get
			{
				if (_instructions.Count == 0)
					return _start;
				
				return _instructions[_instructions.Count - 1].IP;
			}
		}

		/// <summary>
		/// Gets or sets the most nested case node to which this node belongs
		/// </summary>
		public ICallGraphNode CaseHead
		{
			get
			{
				return _caseHead;
			}
			set
			{
				_caseHead = value;
			}
		}

		/// <summary>
		/// Gets or sets the tail node for the case
		/// </summary>
		public ICallGraphNode CaseTail
		{
			get
			{
				return _caseTail;
			}
			set
			{
				_caseTail = value;
			}
		}

		public IList<ICallGraphNode> BackEdges
		{
			get
			{
				return _backEdges;
			}
		}

		public ICallGraphNode IfFollow
		{
			get
			{
				return _ifFollow;
			}
			set
			{
				_ifFollow = value;
			}
		}

        public ICallGraphNode LatchNode
        {
            get
            {
                return _latchNode;
            }
            set
            {
                _latchNode = value;
            }
        }

        public ICallGraphNode LoopHead
        {
            get
            {
                return _loopHead;
            }
            set
            {
                _loopHead = value;
            }
        }

        public ICallGraphNode LoopFollow
        {
            get
            {
                return _loopFollow;
            }
            set
            {
                _loopFollow = value;
            }
        }

        public LoopType LoopType
        {
            get
            {
                return _loopType;
            }
            set
            {
                _loopType = value;
            }
        }
		
		public ICallGraphNode HandlerNode {
			get {
				return _handlerNode;
			}
			set {
				_handlerNode = value;
			}
		}
		
		public ICallGraphNode FollowNode {
			get {
				return _followNode;
			}
			set {
				_followNode = value;
			}
		}

	}
}
