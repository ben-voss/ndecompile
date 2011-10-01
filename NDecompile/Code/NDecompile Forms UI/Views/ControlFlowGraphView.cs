using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using LittleNet.NDecompile.Model;

namespace LittleNet.NDecompile.FormsUI.Views
{
	public partial class ControlFlowGraphView : UserControl
	{
		private class Box
		{
			private readonly String _text;
			private Rectangle _rect;

			public Box(String text)
			{
				_text = text;
			}

			public Rectangle Rect
			{
				get
				{
					return _rect;
				}
				set
				{
					_rect = value;
				}
			}

			public String Text
			{
				get
				{
					return _text;
				}
			}
		}

		private class Line
		{
			private readonly Box _start;
			private readonly Box _end;

			public Line(Box start, Box end)
			{
				_start = start;
				_end = end;
			}

			public Point Start
			{
				get
				{
					return new Point(_start.Rect.Left + _start.Rect.Width / 2, _start.Rect.Bottom);
				}
			}

			public Point End
			{
				get
				{
					return new Point(_end.Rect.Left + _end.Rect.Width / 2, _end.Rect.Top);
				}
			}

		}

		private class Grid
		{
			private int _columns;

			private readonly List<List<Box>> _boxes = new List<List<Box>>();

			public int Rows
			{
				get
				{
					return _boxes.Count;
				}
			}

			public int Cols
			{
				get
				{
					return _columns;
				}
			}

			public void InsertRow(int index)
			{
				List<Box> row = new List<Box>();
				for (int i = 0; i < _columns; i++)
					row.Add(null);

				_boxes.Insert(index, row);
			}

			public void InsertColumn(int index)
			{
				for (int i = 0; i < _boxes.Count; i++)
					_boxes[i].Insert(index, null);

				_columns++;
			}

			public Box this[int column, int row]
			{
				get
				{
					if (row >= _boxes.Count)
						return null;

					if (column >= _columns)
						return null;

					return _boxes[row][column];
				}
				set
				{
					while (row >= _boxes.Count)
						InsertRow(_boxes.Count);

					while (column >= _columns)
						InsertColumn(_columns);

					_boxes[row][column] = value;
				}
			}
		}


		private const int BoxWidth = 90;
		private const int BoxHorizontalGap = 25;
		private const int BoxHeight = 65;
		private const int BoxVerticalGap = 25;

		private IControlFlowGraph _graph;
		private readonly List<Box> _boxes = new List<Box>();
		private readonly List<Line> _lines = new List<Line>();

		private int _totalWidth;
		private int _totalHeight;

		private Pen _linePen;

		public ControlFlowGraphView()
		{
			InitializeComponent();

			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
			PlaceScrollBars();

			_linePen = new Pen(Color.Black, 1);
			_linePen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;

			Disposed += ControlFlowGraphView_Disposed;
		}

		protected override void OnResize(EventArgs e)
		{
			PlaceScrollBars();

			base.OnResize(e);
		}

		void ControlFlowGraphView_Disposed(object sender, EventArgs e)
		{
			if (_linePen != null)
			{
				_linePen.Dispose();
				_linePen = null;
			}
		}

		public IControlFlowGraph Graph
		{
			get
			{
				return _graph;
			}
			set
			{
				_graph = value;

				_boxes.Clear();
				_lines.Clear();
				_totalHeight = 0;
				_totalWidth = 0;

				if ((_graph != null) && (_graph.RootNode != null))
					BuildRenderModel();

				Refresh();
			}
		}

		private void BuildRenderModel()
		{
			Grid grid = new Grid();
			Dictionary<IGraphNode, Box> visited = new Dictionary<IGraphNode, Box>();

			// Layout the nodes in a grid
			LayoutSubNode(grid, _graph.RootNode, visited);

			// Set the box positions
			for (int col = 0; col < grid.Cols; col++)
				for (int row = 0; row < grid.Rows; row++)
					if (grid[col, row] != null)
						grid[col, row].Rect = new Rectangle(BoxHorizontalGap + col * (BoxWidth + BoxHorizontalGap), BoxVerticalGap + row * (BoxHeight + BoxVerticalGap), BoxWidth, BoxHeight);

			_totalHeight = grid.Rows * (BoxVerticalGap + BoxHeight);
			_totalWidth = grid.Cols * (BoxHorizontalGap + BoxWidth);

			PlaceScrollBars();
		}

		private Box LayoutSubNode(Grid grid, IGraphNode node, Dictionary<IGraphNode, Box> visited)
		{
			Box box;

			if (visited.TryGetValue(node, out box))
				return box;

			ICallGraphNode callGraphNode = (ICallGraphNode) node;
			box = new Box(String.Format("{0}\r\nDFS# x{1:x4}\r\nDom# x{2:x4}\r\nInt# {3:x4}\r\nIP x{4:x4} - x{5:x4}", callGraphNode.NodeType, node.DepthFirstSearchLastNumber, node.ImmediateDominatorNumber, 0, callGraphNode.StartIP, callGraphNode.EndIP));

			visited.Add(node, box);

			if (node.OutEdges.Count > 0)
			{
				_lines.Add(new Line(box, LayoutSubNode(grid, node.OutEdges[0], visited)));
			}

			for (int i = 1; i < node.OutEdges.Count; i++)
			{
				if (grid[0, 0] != null)
				{
					grid.InsertColumn(0);
					grid.InsertRow(0);
				}
				_lines.Add(new Line(box, LayoutSubNode(grid, node.OutEdges[i], visited)));
			}

			if (grid[0, 0] != null)
				grid.InsertRow(0);

			grid[0, 0] = box;
			_boxes.Add(box);

			return box;
		}

		/*

		#region Layout

		private void BuildRenderModel()
		{
			if (_graph.RootNode == null)
				return;

			// Assign the graph nodes to rows of blocks a the same level
			Dictionary<ICallGraphNode, Box> seen = new Dictionary<ICallGraphNode, Box>();
			List<List<Box>> rows = new List<List<Box>>();
			RecursivelyAssignRows(rows, new Dictionary<Box, int>(), _graph.RootNode, 0, seen);

			// Count the number of boxes in each row finding the largest number
			int maxBoxCount = 0;
			foreach (List<Box> row in rows)
				maxBoxCount = Math.Max(maxBoxCount, row.Count);

			// Layout the boxes
			int top = BoxVerticalGap;
			foreach (List<Box> row in rows)
			{
				int left = ((maxBoxCount - row.Count) / 2 * (BoxWidth + BoxHorizontalGap)) + BoxHorizontalGap;

				foreach (Box box in row)
				{
					box.Rect = new Rectangle(left, top, BoxWidth, BoxHeight);

					left += BoxWidth + BoxHorizontalGap;
				}

				_totalWidth = Math.Max(_totalWidth, left);

				top += BoxHeight + BoxVerticalGap;
			}

			_totalHeight = top;

			PlaceScrollBars();
		}

		private Box RecursivelyAssignRows(List<List<Box>> rows, Dictionary<Box, int> boxByRowNumber, ICallGraphNode node, int levelNumber, Dictionary<ICallGraphNode, Box> seen)
		{
			Box box;
			if (seen.TryGetValue(node, out box))
			{
				int currentRowNumber = boxByRowNumber[box];

				if (currentRowNumber <= levelNumber)
				{
					if (rows.Count <= levelNumber)
						rows.Add(new List<Box>());

					rows[currentRowNumber].Remove(box);
					rows[levelNumber].Add(box);
				}

				return box;
			}

			box = new Box(String.Format("x{0:x4}\r\nx{1:x4}", node.StartIP, node.EndIP));
			seen.Add(node, box);
			boxByRowNumber.Add(box, levelNumber);

			if (rows.Count <= levelNumber)
				rows.Add(new List<Box>());

			_boxes.Add(box);
			rows[levelNumber].Add(box);

			foreach (ICallGraphNode childNode in node.Links)
				_lines.Add(new Line(box, RecursivelyAssignRows(rows, boxByRowNumber, childNode, levelNumber + 1, seen)));

			return box;
		}

		#endregion
        
		*/

		#region Rendering

		private void PlaceScrollBars()
		{
			for (int i = 0; i < 2; i++)
			{
				_hScrollBar.Top = Height - _hScrollBar.Height;

				if (_vScrollBar.Visible)
					_hScrollBar.Width = Width - _vScrollBar.Width;
				else
					_hScrollBar.Width = Width;

				_hScrollBar.Minimum = 0;

				if (_totalWidth > _hScrollBar.Width)
				{
					_hScrollBar.Maximum = _totalWidth - _hScrollBar.Width;
					_hScrollBar.LargeChange = Width / 10;
					_hScrollBar.SmallChange = Width / 20;
					_hScrollBar.Maximum += _hScrollBar.LargeChange;
					_hScrollBar.Visible = (_hScrollBar.LargeChange < _hScrollBar.Maximum);
				}
				else
				{
					_hScrollBar.Visible = false;
					_hScrollBar.Value = 0;
				}

				_vScrollBar.Left = Width - _vScrollBar.Width;

				if (_hScrollBar.Visible)
					_vScrollBar.Height = Height - _hScrollBar.Height;
				else
					_vScrollBar.Height = Height;

				_vScrollBar.Minimum = 0;

				if (_totalHeight > _vScrollBar.Height)
				{
					_vScrollBar.Maximum = _totalHeight - _vScrollBar.Height;
					_vScrollBar.LargeChange = Height / 10;
					_vScrollBar.SmallChange = Height / 20;
					_vScrollBar.Maximum += _vScrollBar.LargeChange;
					_vScrollBar.Visible = (_vScrollBar.LargeChange < _vScrollBar.Maximum);
				}
				else
				{
					_vScrollBar.Visible = false;
					_vScrollBar.Value = 0;
				}
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			// Setup the rendering state
			Graphics g = e.Graphics;
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

			// Translate the drawing region and the clipping region by the scroll bar amounts
			g.TranslateTransform(-_hScrollBar.Value, -_vScrollBar.Value);

			RectangleF bounds = g.ClipBounds;

			// Clear the background
			g.FillRectangle(SystemBrushes.Info, bounds);

			// Draw any box that might intersect the clipping region
			foreach (Box box in _boxes)
			{
				if (!bounds.IntersectsWith(box.Rect))
					continue;

				g.DrawRectangle(Pens.Black, box.Rect);
				g.DrawString(box.Text, this.Font, Brushes.Black, box.Rect, StringFormat.GenericDefault);
			}

			// Draw any line that might intersect the clipping region
			foreach (Line line in _lines)
			{
				if ((line.Start.X < bounds.Left) && (line.End.X < bounds.Left))
					continue;

				if ((line.Start.X > bounds.Right) && (line.End.X > bounds.Right))
					continue;

				if ((line.Start.Y < bounds.Top) && (line.End.Y < bounds.Top))
					continue;

				if ((line.Start.Y > bounds.Bottom) && (line.End.Y > bounds.Bottom))
					continue;

				g.DrawBezier(_linePen, line.Start, new Point(line.Start.X, line.Start.Y + (BoxVerticalGap / 2)), new Point(line.End.X, line.Start.Y), line.End);
			}

			// Draw the box between the scroll bars
			if (_hScrollBar.Visible && _vScrollBar.Visible)
			{
				// Translate the drawing region and the clipping region by the scroll bar amounts
				g.TranslateTransform(_hScrollBar.Value, _vScrollBar.Value);

				g.FillRectangle(SystemBrushes.Control, _hScrollBar.Width - 1, _vScrollBar.Height - 1, _vScrollBar.Width + 1, _hScrollBar.Height + 1);
			}
		}

		private void _vScrollBar_ValueChanged(object sender, EventArgs e)
		{
			Refresh();
		}

		private void _hScrollBar_ValueChanged(object sender, EventArgs e)
		{
			Refresh();
		}

		#endregion
	}
}