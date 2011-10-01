using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace LittleNet.NDecompile.FormsUI.Views
{
	[DefaultBindingProperty("Text")]
	[DefaultProperty("Text")]
	[ToolboxItem("System.Windows.Forms.Design.AutoSizeToolboxItem,System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[Designer("System.Windows.Forms.Design.LabelDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public class ContextHeaderControl : Control
	{
		private Color _topColor = SystemColors.GradientActiveCaption;

		private Color _bottomColor = SystemColors.ActiveCaption;

		public ContextHeaderControl()
		{
			SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.Selectable | ControlStyles.FixedHeight, false);
			SetStyle(ControlStyles.ResizeRedraw, true);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			Rectangle drawArea = new Rectangle(0, 0, Width, Height);
			using (LinearGradientBrush linearBrush = new LinearGradientBrush(drawArea, _topColor, _bottomColor, LinearGradientMode.Vertical))
			{
				g.FillRectangle(linearBrush, drawArea);
			}

			g.DrawString(Text, Font, SystemBrushes.ActiveCaptionText, 0, 0);
		}

		public Color TopColor
		{
			get
			{
				return _topColor;
			}
			set
			{
				if (_topColor != value)
				{
					_topColor = value;
					Invalidate();
				}
			}
		}

		public Color BottomColor
		{
			get
			{
				return _bottomColor;
			}
			set
			{
				if (_bottomColor != value)
				{
					_bottomColor = value;
					Invalidate();
				}
			}
		}

		[Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), SettingsBindable(true)]
		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				if (base.Text != value)
				{
					base.Text = value;
					Invalidate();
				}
			}
		}
	}
}