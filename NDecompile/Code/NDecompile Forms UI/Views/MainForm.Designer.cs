namespace LittleNet.NDecompile.FormsUI.Views
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer _components;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (_components != null))
			{
				_components.Dispose();
				DoDispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.SplitContainer splitContainer1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            LittleNet.NDecompile.FormsUI.Views.ContextHeaderControl label2;
            LittleNet.NDecompile.FormsUI.Views.ContextHeaderControl label3;
            LittleNet.NDecompile.FormsUI.Views.ContextHeaderControl label4;
            LittleNet.NDecompile.FormsUI.Views.ContextHeaderControl label5;
            LittleNet.NDecompile.FormsUI.Views.ContextHeaderControl label6;
            System.Windows.Forms.ToolStrip toolStrip1;
            System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
            System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
            System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
            System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
            System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
            System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
            System.Windows.Forms.ToolStripMenuItem aboutNDecompileToolStripMenuItem;
            this._assembliesTree = new System.Windows.Forms.TreeView();
            this._imageList = new System.Windows.Forms.ImageList(this.components);
            this._tabControl = new System.Windows.Forms.TabControl();
            this._msilDisassemblyTabPage = new System.Windows.Forms.TabPage();
            this._msilCodeBrowser = new LittleNet.NDecompile.FormsUI.Views.CodeBrowser();
            this.contextHeaderControl1 = new LittleNet.NDecompile.FormsUI.Views.ContextHeaderControl();
            this._controlFlowTabPage = new System.Windows.Forms.TabPage();
            this._controlFlowGraphView = new LittleNet.NDecompile.FormsUI.Views.ControlFlowGraphView();
            this._cSharpTabPage = new System.Windows.Forms.TabPage();
            this._cSharpBrowser = new LittleNet.NDecompile.FormsUI.Views.CodeBrowser();
            this._hexTabPage = new System.Windows.Forms.TabPage();
            this._hexCodeBrowser = new LittleNet.NDecompile.FormsUI.Views.CodeBrowser();
            this._imageTabPage = new System.Windows.Forms.TabPage();
            this._imagePictureBox = new System.Windows.Forms.PictureBox();
            this._stringTableTabPage = new System.Windows.Forms.TabPage();
            this._stringTableListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this._openButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this._backButton = new System.Windows.Forms.ToolStripButton();
            this._nextButton = new System.Windows.Forms.ToolStripButton();
            this._openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._backToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._forwardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this._menuStrip = new System.Windows.Forms.MenuStrip();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            label2 = new LittleNet.NDecompile.FormsUI.Views.ContextHeaderControl();
            label3 = new LittleNet.NDecompile.FormsUI.Views.ContextHeaderControl();
            label4 = new LittleNet.NDecompile.FormsUI.Views.ContextHeaderControl();
            label5 = new LittleNet.NDecompile.FormsUI.Views.ContextHeaderControl();
            label6 = new LittleNet.NDecompile.FormsUI.Views.ContextHeaderControl();
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            aboutNDecompileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            this._tabControl.SuspendLayout();
            this._msilDisassemblyTabPage.SuspendLayout();
            this._controlFlowTabPage.SuspendLayout();
            this._cSharpTabPage.SuspendLayout();
            this._hexTabPage.SuspendLayout();
            this._imageTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._imagePictureBox)).BeginInit();
            this._stringTableTabPage.SuspendLayout();
            toolStrip1.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this._menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(0, 25);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(this._assembliesTree);
            splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(2, 3, 0, 2);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(this._tabControl);
            splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(0, 3, 2, 1);
            splitContainer1.Size = new System.Drawing.Size(724, 370);
            splitContainer1.SplitterDistance = 239;
            splitContainer1.TabIndex = 0;
            // 
            // _assembliesTree
            // 
            this._assembliesTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this._assembliesTree.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._assembliesTree.HideSelection = false;
            this._assembliesTree.ImageIndex = 0;
            this._assembliesTree.ImageList = this._imageList;
            this._assembliesTree.Location = new System.Drawing.Point(2, 3);
            this._assembliesTree.Name = "_assembliesTree";
            this._assembliesTree.SelectedImageIndex = 0;
            this._assembliesTree.Size = new System.Drawing.Size(237, 365);
            this._assembliesTree.TabIndex = 0;
            this._assembliesTree.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this._assembliesTree_NodeMouseDoubleClick);
            this._assembliesTree.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this._assembliesTree_BeforeExpand);
            this._assembliesTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this._assembliesTree_AfterSelect);
            // 
            // _imageList
            // 
            this._imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_imageList.ImageStream")));
            this._imageList.TransparentColor = System.Drawing.Color.Magenta;
            this._imageList.Images.SetKeyName(0, "Assembly.bmp");
            this._imageList.Images.SetKeyName(1, "Namespace.bmp");
            this._imageList.Images.SetKeyName(2, "Class.bmp");
            this._imageList.Images.SetKeyName(3, "Enum.bmp");
            this._imageList.Images.SetKeyName(4, "Interface.bmp");
            this._imageList.Images.SetKeyName(5, "Method.bmp");
            this._imageList.Images.SetKeyName(6, "ProtectedMethod.bmp");
            this._imageList.Images.SetKeyName(7, "StaticMethod.bmp");
            this._imageList.Images.SetKeyName(8, "InternalMethod.bmp");
            this._imageList.Images.SetKeyName(9, "InternalProperty.bmp");
            this._imageList.Images.SetKeyName(10, "PrivateProperty.bmp");
            this._imageList.Images.SetKeyName(11, "ProtectedProperty.bmp");
            this._imageList.Images.SetKeyName(12, "PublicProperty.bmp");
            this._imageList.Images.SetKeyName(13, "PublicField.bmp");
            this._imageList.Images.SetKeyName(14, "InternalField.bmp");
            this._imageList.Images.SetKeyName(15, "ProtectedField.bmp");
            this._imageList.Images.SetKeyName(16, "PrivateField.bmp");
            this._imageList.Images.SetKeyName(17, "PublicConst.bmp");
            this._imageList.Images.SetKeyName(18, "InternalConst.bmp");
            this._imageList.Images.SetKeyName(19, "ProtectedConst.bmp");
            this._imageList.Images.SetKeyName(20, "PrivateConst.bmp");
            // 
            // _tabControl
            // 
            this._tabControl.Controls.Add(this._msilDisassemblyTabPage);
            this._tabControl.Controls.Add(this._controlFlowTabPage);
            this._tabControl.Controls.Add(this._cSharpTabPage);
            this._tabControl.Controls.Add(this._hexTabPage);
            this._tabControl.Controls.Add(this._imageTabPage);
            this._tabControl.Controls.Add(this._stringTableTabPage);
            this._tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tabControl.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._tabControl.HotTrack = true;
            this._tabControl.Location = new System.Drawing.Point(0, 3);
            this._tabControl.Name = "_tabControl";
            this._tabControl.SelectedIndex = 0;
            this._tabControl.Size = new System.Drawing.Size(479, 366);
            this._tabControl.TabIndex = 2;
            // 
            // _msilDisassemblyTabPage
            // 
            this._msilDisassemblyTabPage.Controls.Add(this._msilCodeBrowser);
            this._msilDisassemblyTabPage.Controls.Add(this.contextHeaderControl1);
            this._msilDisassemblyTabPage.Location = new System.Drawing.Point(4, 22);
            this._msilDisassemblyTabPage.Name = "_msilDisassemblyTabPage";
            this._msilDisassemblyTabPage.Padding = new System.Windows.Forms.Padding(3);
            this._msilDisassemblyTabPage.Size = new System.Drawing.Size(471, 340);
            this._msilDisassemblyTabPage.TabIndex = 0;
            this._msilDisassemblyTabPage.Text = "MSIL Disassembly";
            this._msilDisassemblyTabPage.UseVisualStyleBackColor = true;
            // 
            // _msilCodeBrowser
            // 
            this._msilCodeBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this._msilCodeBrowser.Location = new System.Drawing.Point(3, 18);
            this._msilCodeBrowser.Name = "_msilCodeBrowser";
            this._msilCodeBrowser.Size = new System.Drawing.Size(465, 319);
            this._msilCodeBrowser.TabIndex = 1;
            // 
            // contextHeaderControl1
            // 
            this.contextHeaderControl1.BottomColor = System.Drawing.SystemColors.ActiveCaption;
            this.contextHeaderControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.contextHeaderControl1.Location = new System.Drawing.Point(3, 3);
            this.contextHeaderControl1.Name = "contextHeaderControl1";
            this.contextHeaderControl1.Size = new System.Drawing.Size(465, 15);
            this.contextHeaderControl1.TabIndex = 3;
            this.contextHeaderControl1.Text = "MSIL Disassembly";
            this.contextHeaderControl1.TopColor = System.Drawing.SystemColors.GradientActiveCaption;
            // 
            // _controlFlowTabPage
            // 
            this._controlFlowTabPage.Controls.Add(this._controlFlowGraphView);
            this._controlFlowTabPage.Controls.Add(label2);
            this._controlFlowTabPage.Location = new System.Drawing.Point(4, 22);
            this._controlFlowTabPage.Name = "_controlFlowTabPage";
            this._controlFlowTabPage.Padding = new System.Windows.Forms.Padding(3);
            this._controlFlowTabPage.Size = new System.Drawing.Size(471, 340);
            this._controlFlowTabPage.TabIndex = 1;
            this._controlFlowTabPage.Text = "Control Flow Graph";
            this._controlFlowTabPage.UseVisualStyleBackColor = true;
            // 
            // _controlFlowGraphView
            // 
            this._controlFlowGraphView.BackColor = System.Drawing.SystemColors.Info;
            this._controlFlowGraphView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._controlFlowGraphView.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._controlFlowGraphView.Graph = null;
            this._controlFlowGraphView.Location = new System.Drawing.Point(3, 18);
            this._controlFlowGraphView.Name = "_controlFlowGraphView";
            this._controlFlowGraphView.Size = new System.Drawing.Size(465, 319);
            this._controlFlowGraphView.TabIndex = 2;
            // 
            // label2
            // 
            label2.BackColor = System.Drawing.SystemColors.Highlight;
            label2.BottomColor = System.Drawing.SystemColors.ActiveCaption;
            label2.Dock = System.Windows.Forms.DockStyle.Top;
            label2.ForeColor = System.Drawing.SystemColors.HighlightText;
            label2.Location = new System.Drawing.Point(3, 3);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(465, 15);
            label2.TabIndex = 1;
            label2.Text = "Control Flow Graph";
            label2.TopColor = System.Drawing.SystemColors.GradientActiveCaption;
            // 
            // _cSharpTabPage
            // 
            this._cSharpTabPage.Controls.Add(this._cSharpBrowser);
            this._cSharpTabPage.Controls.Add(label3);
            this._cSharpTabPage.Location = new System.Drawing.Point(4, 22);
            this._cSharpTabPage.Name = "_cSharpTabPage";
            this._cSharpTabPage.Padding = new System.Windows.Forms.Padding(3);
            this._cSharpTabPage.Size = new System.Drawing.Size(471, 340);
            this._cSharpTabPage.TabIndex = 2;
            this._cSharpTabPage.Text = "C# Disassembly";
            this._cSharpTabPage.UseVisualStyleBackColor = true;
            // 
            // _cSharpBrowser
            // 
            this._cSharpBrowser.AllowWebBrowserDrop = false;
            this._cSharpBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this._cSharpBrowser.IsWebBrowserContextMenuEnabled = false;
            this._cSharpBrowser.Location = new System.Drawing.Point(3, 18);
            this._cSharpBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this._cSharpBrowser.Name = "_cSharpBrowser";
            this._cSharpBrowser.ScriptErrorsSuppressed = true;
            this._cSharpBrowser.Size = new System.Drawing.Size(465, 319);
            this._cSharpBrowser.TabIndex = 3;
            this._cSharpBrowser.WebBrowserShortcutsEnabled = false;
            this._cSharpBrowser.BeforeNavigate += new System.EventHandler<LittleNet.NDecompile.FormsUI.Views.CodeBrowser.NavigateEventArgs>(this._cSharpBrowser_BeforeNavigate);
            // 
            // label3
            // 
            label3.BackColor = System.Drawing.SystemColors.Highlight;
            label3.BottomColor = System.Drawing.SystemColors.ActiveCaption;
            label3.Dock = System.Windows.Forms.DockStyle.Top;
            label3.ForeColor = System.Drawing.SystemColors.HighlightText;
            label3.Location = new System.Drawing.Point(3, 3);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(465, 15);
            label3.TabIndex = 2;
            label3.Text = "C# Dissasembly";
            label3.TopColor = System.Drawing.SystemColors.GradientActiveCaption;
            // 
            // _hexTabPage
            // 
            this._hexTabPage.Controls.Add(this._hexCodeBrowser);
            this._hexTabPage.Controls.Add(label4);
            this._hexTabPage.Location = new System.Drawing.Point(4, 22);
            this._hexTabPage.Name = "_hexTabPage";
            this._hexTabPage.Padding = new System.Windows.Forms.Padding(3);
            this._hexTabPage.Size = new System.Drawing.Size(471, 340);
            this._hexTabPage.TabIndex = 3;
            this._hexTabPage.Text = "Hex Disassembly";
            this._hexTabPage.UseVisualStyleBackColor = true;
            // 
            // _hexCodeBrowser
            // 
            this._hexCodeBrowser.AllowWebBrowserDrop = false;
            this._hexCodeBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this._hexCodeBrowser.IsWebBrowserContextMenuEnabled = false;
            this._hexCodeBrowser.Location = new System.Drawing.Point(3, 18);
            this._hexCodeBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this._hexCodeBrowser.Name = "_hexCodeBrowser";
            this._hexCodeBrowser.ScriptErrorsSuppressed = true;
            this._hexCodeBrowser.Size = new System.Drawing.Size(465, 319);
            this._hexCodeBrowser.TabIndex = 4;
            this._hexCodeBrowser.WebBrowserShortcutsEnabled = false;
            // 
            // label4
            // 
            label4.BackColor = System.Drawing.SystemColors.Highlight;
            label4.BottomColor = System.Drawing.SystemColors.ActiveCaption;
            label4.Dock = System.Windows.Forms.DockStyle.Top;
            label4.ForeColor = System.Drawing.SystemColors.HighlightText;
            label4.Location = new System.Drawing.Point(3, 3);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(465, 15);
            label4.TabIndex = 1;
            label4.Text = "Hex Dissasembly";
            label4.TopColor = System.Drawing.SystemColors.GradientActiveCaption;
            // 
            // _imageTabPage
            // 
            this._imageTabPage.Controls.Add(this._imagePictureBox);
            this._imageTabPage.Controls.Add(label5);
            this._imageTabPage.Location = new System.Drawing.Point(4, 22);
            this._imageTabPage.Name = "_imageTabPage";
            this._imageTabPage.Padding = new System.Windows.Forms.Padding(3);
            this._imageTabPage.Size = new System.Drawing.Size(471, 340);
            this._imageTabPage.TabIndex = 4;
            this._imageTabPage.Text = "Image";
            this._imageTabPage.UseVisualStyleBackColor = true;
            // 
            // _imagePictureBox
            // 
            this._imagePictureBox.Location = new System.Drawing.Point(6, 21);
            this._imagePictureBox.Name = "_imagePictureBox";
            this._imagePictureBox.Size = new System.Drawing.Size(100, 50);
            this._imagePictureBox.TabIndex = 2;
            this._imagePictureBox.TabStop = false;
            // 
            // label5
            // 
            label5.BackColor = System.Drawing.SystemColors.Highlight;
            label5.BottomColor = System.Drawing.SystemColors.ActiveCaption;
            label5.Dock = System.Windows.Forms.DockStyle.Top;
            label5.ForeColor = System.Drawing.SystemColors.HighlightText;
            label5.Location = new System.Drawing.Point(3, 3);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(465, 15);
            label5.TabIndex = 1;
            label5.Text = "Image";
            label5.TopColor = System.Drawing.SystemColors.GradientActiveCaption;
            // 
            // _stringTableTabPage
            // 
            this._stringTableTabPage.Controls.Add(this._stringTableListView);
            this._stringTableTabPage.Controls.Add(label6);
            this._stringTableTabPage.Location = new System.Drawing.Point(4, 22);
            this._stringTableTabPage.Name = "_stringTableTabPage";
            this._stringTableTabPage.Padding = new System.Windows.Forms.Padding(3);
            this._stringTableTabPage.Size = new System.Drawing.Size(471, 340);
            this._stringTableTabPage.TabIndex = 5;
            this._stringTableTabPage.Text = "String Table";
            this._stringTableTabPage.UseVisualStyleBackColor = true;
            // 
            // _stringTableListView
            // 
            this._stringTableListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._stringTableListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this._stringTableListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._stringTableListView.Location = new System.Drawing.Point(3, 18);
            this._stringTableListView.Name = "_stringTableListView";
            this._stringTableListView.Size = new System.Drawing.Size(465, 319);
            this._stringTableListView.TabIndex = 3;
            this._stringTableListView.UseCompatibleStateImageBehavior = false;
            this._stringTableListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Key";
            this.columnHeader1.Width = 178;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Value";
            this.columnHeader2.Width = 171;
            // 
            // label6
            // 
            label6.BackColor = System.Drawing.SystemColors.Highlight;
            label6.BottomColor = System.Drawing.SystemColors.ActiveCaption;
            label6.Dock = System.Windows.Forms.DockStyle.Top;
            label6.ForeColor = System.Drawing.SystemColors.HighlightText;
            label6.Location = new System.Drawing.Point(3, 3);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(465, 15);
            label6.TabIndex = 2;
            label6.Text = "String Table";
            label6.TopColor = System.Drawing.SystemColors.GradientActiveCaption;
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._openButton,
            this.toolStripSeparator1,
            this._backButton,
            this._nextButton});
            toolStrip1.Location = new System.Drawing.Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new System.Drawing.Size(724, 25);
            toolStrip1.TabIndex = 3;
            toolStrip1.Text = "_toolStrip";
            // 
            // _openButton
            // 
            this._openButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._openButton.Image = ((System.Drawing.Image)(resources.GetObject("_openButton.Image")));
            this._openButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._openButton.Name = "_openButton";
            this._openButton.Size = new System.Drawing.Size(23, 22);
            this._openButton.Text = "Open";
            this._openButton.Click += new System.EventHandler(this.OpenToolStripMenuItemClickHandler);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // _backButton
            // 
            this._backButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._backButton.Image = ((System.Drawing.Image)(resources.GetObject("_backButton.Image")));
            this._backButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._backButton.Name = "_backButton";
            this._backButton.Size = new System.Drawing.Size(23, 22);
            this._backButton.Text = "Back";
            this._backButton.Click += new System.EventHandler(this.BackToolStripMenuItemClickHandler);
            // 
            // _nextButton
            // 
            this._nextButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._nextButton.Image = ((System.Drawing.Image)(resources.GetObject("_nextButton.Image")));
            this._nextButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._nextButton.Name = "_nextButton";
            this._nextButton.Size = new System.Drawing.Size(23, 22);
            this._nextButton.Text = "Forward";
            this._nextButton.Click += new System.EventHandler(this.ForwardToolStripMenuItemClickHandler);
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._openToolStripMenuItem,
            toolStripMenuItem1,
            exitToolStripMenuItem});
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            fileToolStripMenuItem.Text = "&File";
            // 
            // _openToolStripMenuItem
            // 
            this._openToolStripMenuItem.Name = "_openToolStripMenuItem";
            this._openToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this._openToolStripMenuItem.Text = "&Open...";
            this._openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItemClickHandler);
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new System.Drawing.Size(120, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            exitToolStripMenuItem.Text = "&Exit";
            exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItemClickHandler);
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._backToolStripMenuItem,
            this._forwardToolStripMenuItem,
            toolStripMenuItem2});
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            viewToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            viewToolStripMenuItem.Text = "&View";
            // 
            // _backToolStripMenuItem
            // 
            this._backToolStripMenuItem.Name = "_backToolStripMenuItem";
            this._backToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this._backToolStripMenuItem.Text = "&Back";
            this._backToolStripMenuItem.Click += new System.EventHandler(this.BackToolStripMenuItemClickHandler);
            // 
            // _forwardToolStripMenuItem
            // 
            this._forwardToolStripMenuItem.Name = "_forwardToolStripMenuItem";
            this._forwardToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this._forwardToolStripMenuItem.Text = "&Forward";
            this._forwardToolStripMenuItem.Click += new System.EventHandler(this.ForwardToolStripMenuItemClickHandler);
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new System.Drawing.Size(122, 6);
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            aboutNDecompileToolStripMenuItem});
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutNDecompileToolStripMenuItem
            // 
            aboutNDecompileToolStripMenuItem.Name = "aboutNDecompileToolStripMenuItem";
            aboutNDecompileToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            aboutNDecompileToolStripMenuItem.Text = "&About NDecompile";
            aboutNDecompileToolStripMenuItem.Click += new System.EventHandler(this.AboutNDecompileToolStripMenuItemClickHandler);
            // 
            // _openFileDialog
            // 
            this._openFileDialog.Filter = "Assemblies|*.dll|Executables|*.exe";
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(splitContainer1);
            this.toolStripContainer1.ContentPanel.Controls.Add(toolStrip1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(724, 395);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(724, 419);
            this.toolStripContainer1.TabIndex = 3;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this._menuStrip);
            // 
            // _menuStrip
            // 
            this._menuStrip.Dock = System.Windows.Forms.DockStyle.None;
            this._menuStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
            this._menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            fileToolStripMenuItem,
            viewToolStripMenuItem,
            helpToolStripMenuItem});
            this._menuStrip.Location = new System.Drawing.Point(0, 0);
            this._menuStrip.Name = "_menuStrip";
            this._menuStrip.Size = new System.Drawing.Size(724, 24);
            this._menuStrip.TabIndex = 5;
            this._menuStrip.Text = "_menuStrip";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(724, 419);
            this.Controls.Add(this.toolStripContainer1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "NDecompile";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.ResumeLayout(false);
            this._tabControl.ResumeLayout(false);
            this._msilDisassemblyTabPage.ResumeLayout(false);
            this._controlFlowTabPage.ResumeLayout(false);
            this._cSharpTabPage.ResumeLayout(false);
            this._hexTabPage.ResumeLayout(false);
            this._imageTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._imagePictureBox)).EndInit();
            this._stringTableTabPage.ResumeLayout(false);
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ContentPanel.PerformLayout();
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this._menuStrip.ResumeLayout(false);
            this._menuStrip.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TreeView _assembliesTree;
		private CodeBrowser _msilCodeBrowser;
		private System.Windows.Forms.OpenFileDialog _openFileDialog;
		private System.Windows.Forms.TabControl _tabControl;
		private ControlFlowGraphView _controlFlowGraphView;
		private System.Windows.Forms.ImageList _imageList;
		private CodeBrowser _cSharpBrowser;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.TabPage _hexTabPage;
		private CodeBrowser _hexCodeBrowser;
		private System.Windows.Forms.TabPage _imageTabPage;
		private System.Windows.Forms.TabPage _msilDisassemblyTabPage;
		private System.Windows.Forms.TabPage _controlFlowTabPage;
		private System.Windows.Forms.TabPage _cSharpTabPage;
		private System.Windows.Forms.PictureBox _imagePictureBox;
		private System.Windows.Forms.ToolStripContainer toolStripContainer1;
		private System.Windows.Forms.MenuStrip _menuStrip;
		private System.Windows.Forms.ToolStripMenuItem _backToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem _forwardToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem _openToolStripMenuItem;
		private System.Windows.Forms.TabPage _stringTableTabPage;
		private System.Windows.Forms.ListView _stringTableListView;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private ContextHeaderControl contextHeaderControl1;
        private System.Windows.Forms.ToolStripButton _backButton;
        private System.Windows.Forms.ToolStripButton _nextButton;
        private System.Windows.Forms.ToolStripButton _openButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
	}
}