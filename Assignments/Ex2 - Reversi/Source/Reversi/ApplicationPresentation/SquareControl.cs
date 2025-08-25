using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using GameAI.GamePlaying.Core;

namespace GameAI.GamePlaying
{
	/// <summary>
	/// Summary description for SquareControl.
	/// </summary>
	/// 

	public class SquareControl : System.Windows.Forms.UserControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		// Default color values.
		public static readonly Color ActiveSquareBackColorDefault = Color.FromArgb(0, 224, 0);
		public static readonly Color MoveIndicatorColorDefault    = Color.Red;
		public static readonly Color NormalBackColorDefault       = Color.Green;
		public static readonly Color ValidMoveBackColorDefault    = Color.FromArgb(0, 176, 0);

		// Colors used in rendering the control.
		public static Color ActiveSquareBackColor = ActiveSquareBackColorDefault;
		public static Color MoveIndicatorColor    = MoveIndicatorColorDefault;
		public static Color NormalBackColor       = NormalBackColorDefault;
		public static Color ValidMoveBackColor    = ValidMoveBackColorDefault;

		// This represents the contents of the square, see the values defined
		// in the Board class.
		public int Contents;
		public int PreviewContents;

		// These are used to set highlighting.
		public bool IsValid  = false;
		public bool IsActive = false;
		public bool IsNew    = false;

		// Used for animation.
		public static readonly int AnimationStart =  6;
		public static readonly int AnimationStop  = -SquareControl.AnimationStart;
		public int AnimationCounter = SquareControl.AnimationStop;

		// These reflect the position of the square on the board.
		public int Row
		{
			get { return this.row; }
		}
		public int Col
		{
			get { return this.col; }
		}

		// These reflect the public row and column properties.
		private int row;
		private int col;

		// Drawing tools.
		private static Pen pen = new Pen(Color.Black);
		private static SolidBrush solidBrush = new SolidBrush(Color.Black);
		private static GraphicsPath path = new GraphicsPath();
		private static PathGradientBrush gradientBrush;

		public SquareControl(int row, int col)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitForm call

			this.Contents = Board.Empty;
			this.row = row;
			this.col = col;

			// Prevent the control from receiving focus via the TAB key.
			this.TabStop = false;

			// Set the background color.
			this.BackColor = SquareControl.NormalBackColor;

			// Redraw the control on a resize.
			this.ResizeRedraw = true;

			// Set double-buffering to prevent flicker when drawing the control.
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// SquareControl
			// 
			this.Name = "SquareControl";
			this.Size = new System.Drawing.Size(32, 32);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.SquareControl_Paint);

		}
		#endregion

		//
		// Returns a lighter or darker version of the given color.
		//
		private static Color AdjustBrightness(Color color, double m)
		{
			int r = (int) Math.Max(0, Math.Min(255, Math.Round((double) color.R * m)));
			int g = (int) Math.Max(0, Math.Min(255, Math.Round((double) color.G * m)));
			int b = (int) Math.Max(0, Math.Min(255, Math.Round((double) color.B * m)));

			return Color.FromArgb(r, g, b);
		}

		// ===================================================================
		// Paint event handler.
		// ===================================================================

		private void SquareControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			// Clear the square, filling with the appropriate background color.
			Color backColor = SquareControl.NormalBackColor;
			if (this.IsValid)
				backColor = SquareControl.ValidMoveBackColor;
			if (this.IsActive)
				backColor = SquareControl.ActiveSquareBackColor;

			e.Graphics.Clear(backColor);

			// Set drawing options.
			e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

			// Draw the border.
			Point topLeft     = new Point(0, 0);
			Point topRight    = new Point(this.Width - 1, 0);
			Point bottomLeft  = new Point(0, this.Height - 1);
			Point bottomRight = new Point(this.Width - 1, this.Height - 1);
			SquareControl.pen.Color = SquareControl.AdjustBrightness(backColor, 1.5);
			SquareControl.pen.Width = 1;
			e.Graphics.DrawLine(SquareControl.pen, bottomLeft, topLeft);
			e.Graphics.DrawLine(SquareControl.pen, topLeft, topRight);
			SquareControl.pen.Color = SquareControl.AdjustBrightness(backColor, 0.6);
			e.Graphics.DrawLine(SquareControl.pen, bottomLeft, bottomRight);
			e.Graphics.DrawLine(SquareControl.pen, bottomRight, topRight);

			// Draw the disc, if any.
			if (this.Contents != Board.Empty || this.PreviewContents != Board.Empty)
			{
				// Get size and position parameters based on the control size and animation state.
				int size      = (int) (this.Width * (this.AnimationCounter > SquareControl.AnimationStop ? 0.85 : 0.80));
				int offset    = (int) ((this.Width - size) / 2);
				int thickness = (int) (size * 0.08);
				int width     = size;
				int height    = Math.Max(thickness, (int) Math.Round(size * Math.Abs((double) this.AnimationCounter / SquareControl.AnimationStart)));
				int left      = offset;
				int top       = offset + (int) Math.Round((double) (size - height) / 2.0);

				// Draw the shadow.
				SquareControl.solidBrush.Color = Color.FromArgb((this.PreviewContents == Board.Empty ? 48 : 24), Color.Black);
				if (this.AnimationCounter <= SquareControl.AnimationStop)
					e.Graphics.FillEllipse(SquareControl.solidBrush, left + thickness, top + thickness, width, height);
				else
					e.Graphics.FillEllipse(SquareControl.solidBrush, left + thickness, top + thickness, width, size - top + thickness);

				// Draw the disc edge, if animating.
				if (this.AnimationCounter > SquareControl.AnimationStop)
				{
					double pct = 1.0 - Math.Abs((double) this.AnimationCounter / SquareControl.AnimationStart);
					thickness = (int) Math.Ceiling(1.5 * thickness * pct);

					SquareControl.path.Reset();
					if (this.AnimationCounter > 0)
					{
						SquareControl.path.AddArc(new Rectangle(left, top + thickness, width, height),   0, 180);
						SquareControl.path.AddArc(new Rectangle(left, top,             width, height), 180, 180);
					}
					else if (this.AnimationCounter == 0)
					{
						SquareControl.path.AddRectangle(new Rectangle(left, top - (int) (thickness / 2), width, thickness));
					}
					else
					{
						SquareControl.path.AddArc(new Rectangle(left, top,             width, height),   0, 180);
						SquareControl.path.AddArc(new Rectangle(left, top - thickness, width, height), 180, 180);
					}
					SquareControl.path.CloseFigure();

					SquareControl.solidBrush.Color = Color.Gray;
					e.Graphics.FillPath(SquareControl.solidBrush, SquareControl.path);
				}

				// Draw the disc face, if not on edge.
				if (this.AnimationCounter != 0)
				{
					if (this.PreviewContents == Board.Empty)
					{
						if (this.Contents == Board.Black)
						{
							SquareControl.solidBrush.Color = Color.Black;

							// If the disc is being flipped, switch the color.
							if (this.AnimationCounter > 0)
								SquareControl.solidBrush.Color = SquareControl.AdjustBrightness(Color.White, 0.80);
						}
						else
						{
							SquareControl.solidBrush.Color = SquareControl.AdjustBrightness(Color.White, 0.80);

							// If the disc is being flipped, switch the color.
							if (this.AnimationCounter > 0)
								SquareControl.solidBrush.Color = Color.Black;
						}
					}
					else
					{
						if (this.PreviewContents == Board.Black)
							SquareControl.solidBrush.Color = Color.FromArgb(96, Color.Black);
						else
							SquareControl.solidBrush.Color = Color.FromArgb(96, SquareControl.AdjustBrightness(Color.White, 0.80));
					}
					e.Graphics.FillEllipse(SquareControl.solidBrush, left, top, width, height);

					// Highlight the disc face.
					SquareControl.path.Reset();
					SquareControl.path.AddEllipse(left, top, width, height);
					SquareControl.gradientBrush = new PathGradientBrush(SquareControl.path);
					SquareControl.gradientBrush.CenterPoint = new Point((int) (width / 3), (int) (height / 3));
					if (this.PreviewContents == Board.Empty)
					{
						if (SquareControl.solidBrush.Color == Color.Black)
							SquareControl.gradientBrush.CenterColor = Color.FromArgb(128, Color.DarkGray);
						else
							SquareControl.gradientBrush.CenterColor = Color.White;
					}
					else
					{
						if (this.PreviewContents == Board.Black)
							SquareControl.gradientBrush.CenterColor = Color.FromArgb(48, Color.DarkGray);
						else
							SquareControl.gradientBrush.CenterColor = Color.FromArgb(96, Color.White);
					}
					SquareControl.gradientBrush.SurroundColors = new Color[] { SquareControl.solidBrush.Color };
					e.Graphics.FillEllipse(SquareControl.gradientBrush, left, top, width, height);
					SquareControl.gradientBrush.Dispose();

					// Draw a circle around the disc if it has been newly added.
					if (this.IsNew)
					{
						SquareControl.pen.Color = SquareControl.MoveIndicatorColor;
						SquareControl.pen.Width = 2;
						e.Graphics.DrawEllipse(SquareControl.pen, left, top, width, height);
					}
				}
			}
		}
	}
}
