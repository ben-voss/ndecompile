using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LittleNet.NDecompile.FormsUI.Interfaces;
using LittleNet.NDecompile.FormsUI.Models;
using LittleNet.NDecompile.Model;
using LittleNet.NDecompile.FormsUI.Properties;

namespace LittleNet.NDecompile.FormsUI.Views
{
	internal partial class MainForm : ViewFormBase
	{

		#region View Context Enum

		private enum ViewContext
		{
			None,
			Code,
			StringTable,
			Image,
			Hex
		}

		#endregion

		#region Tree Node Image Indexes

		private const int AssemblyImageIndex = 0;
		private const int ModuleImageIndex = 0;
		private const int NamespaceImageIndex = 1;
		private const int ClassImageIndex = 2;
		private const int EnumImageIndex = 3;
		private const int InterfaceImageIndex = 4;
		private const int MethodImageIndex = 5;
		private const int ProtectedMethodImageIndex = 6;
		private const int PrivateMethodImageIndex = 7;
		private const int InternalMethodImageIndex = 8;
		private const int InternalPropertyImageIndex = 11;
		private const int PrivatePropertyImageIndex = 10;
		private const int ProtectedPropertyImageIndex = 9;
		private const int PublicPropertyImageIndex = 12;
		private const int PublicFieldIndex = 13;
		private const int InternalFieldIndex = 14;
		private const int ProtectedFieldIndex = 15;
		private const int PrivateFieldIndex = 16;
		private const int PublicConstIndex = 17;
		private const int InternalConstIndex = 18;
		private const int ProtectedConstIndex = 19;
		private const int PrivateConstIndex = 20;

		#endregion

		#region Fields

        private readonly String EmptyHtml = "<html><body bgcolor=\"#" + (SystemColors.Info.ToArgb() & 0xffffff).ToString("x6") + "\"></body></html>";

		private ViewContext _currentViewContext = ViewContext.None;

		#endregion

		#region Constructor

		/// <summary>
		/// Initialises a new instance of the <see cref="MainForm"/>.
		/// </summary>
		public MainForm()
		{
			InitializeComponent();

			Left = Settings.Default.MainFormLeft;
			Top = Settings.Default.MainFormTop;
			Width = Settings.Default.MainFormWidth;
			Height = Settings.Default.MainFormHeight;
			_assembliesTree.Width = Settings.Default.SplitterPosition;
			_tabControl.SelectedIndex = Settings.Default.ActiveTab;
            _cSharpBrowser.DocumentText = EmptyHtml;
            _hexCodeBrowser.DocumentText = EmptyHtml;
            _msilCodeBrowser.DocumentText = EmptyHtml;

            SetViewContext(ViewContext.Code);

			AssemblyManager.AssemblyResolve += HandleAssemblyResolve;
		}

		/// <summary>
		/// Called from the designer generated code to perform the dispose
		/// </summary>
		private void DoDispose()
		{
			AssemblyManager.AssemblyResolve -= HandleAssemblyResolve;
		}

		#endregion

		private FileInfo HandleAssemblyResolve(object sender, ResolveEventArgs args)
		{
			using (FindAssemblyForm form = new FindAssemblyForm())
			{
				form.AssemblyName = args.Name;
				form.ShowDialog(this);
				return form.FileInfo;
			}
		}

		#region Model

		public IMainModel MainModel
		{
			get
			{
				return (IMainModel)Model;
			}
			set
			{
				Model = value;
			}
		}

		protected override void OnModelChange(object hint)
		{
			MainModelChangeHint changeHint = hint as MainModelChangeHint;
			if (changeHint == null)
			{
				SetAssemblyList();
				SetControlEnabledStates();
			}
			else
			{
				switch (changeHint.Hint)
				{
					case MainModelChangeHint.HintType.AssemblyList:
						SetAssemblyList();
						SetControlEnabledStates();
						break;

					case MainModelChangeHint.HintType.CurrentReference:
						SetCurrentReference();
						SetControlEnabledStates();
						break;
				}
			}

			base.OnModelChange(hint);
		}

		#endregion

		#region Menu Event Handlers

		private void ExitToolStripMenuItemClickHandler(object sender, EventArgs e)
		{
			Close();
		}

		private void OpenToolStripMenuItemClickHandler(object sender, EventArgs e)
		{
            if (MainModel == null)
                return;

            if (_openFileDialog.ShowDialog(this) != DialogResult.OK)
                return;

            MainModel.LoadAssembly(_openFileDialog.FileName);
        }

		private void AboutNDecompileToolStripMenuItemClickHandler(object sender, EventArgs e)
		{
			using (AboutForm aboutForm = new AboutForm())
				aboutForm.ShowDialog(this);
		}

		private void BackToolStripMenuItemClickHandler(object sender, EventArgs e)
		{
			if (MainModel != null)
				MainModel.Back();
		}

		private void ForwardToolStripMenuItemClickHandler(object sender, EventArgs e)
		{
			if (MainModel != null)
				MainModel.Forward();
		}

		#endregion

		#region Form Event Handlers

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			Settings.Default.MainFormHeight = Height;
			Settings.Default.MainFormLeft = Left;
			Settings.Default.MainFormTop = Top;
			Settings.Default.MainFormWidth = Width;
			Settings.Default.SplitterPosition = _assembliesTree.Width;
			Settings.Default.ActiveTab = _tabControl.SelectedIndex;

			Settings.Default.Save();
		}

		#endregion

		#region Tree Event Handlers

		private void _assembliesTree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            using (new WaitCursor())
            {
                if ((e.Node.Nodes.Count == 1) && (e.Node.Nodes[0].Text == "Loading..."))
                {
                    e.Node.Nodes.Clear();

                    ITypeDeclaration typeDeclaration = e.Node.Tag as ITypeDeclaration;
                    if (typeDeclaration != null)
                        BuildTypeTreeNode(typeDeclaration, e.Node);
                }
            }
        }

        private static void MakeTypeDeclarationName(ITypeDeclaration typeDeclaration, StringBuilder builder)
        {
            if ((typeDeclaration.IsGeneric) && (typeDeclaration.Name.LastIndexOf('`') > 0))
            {
                builder.Append(typeDeclaration.Name.Substring(0, typeDeclaration.Name.LastIndexOf('`')));

                builder.Append('<');
                if (typeDeclaration.GenericArguments.Count > 0)
                {
                    MakeTypeDeclarationName(typeDeclaration.GenericArguments[0].Resolve(), builder);
                    for (int i = 1; i < typeDeclaration.GenericArguments.Count; i++)
                    {
                        builder.Append(", ");
                        MakeTypeDeclarationName(typeDeclaration.GenericArguments[1].Resolve(), builder);
                    }
                }
                builder.Append('>');
            }
            else
            {
                builder.Append(typeDeclaration.Name);
            }
        }

        private void BuildTypeTreeNode(ITypeDeclaration typeDeclaration, TreeNode parentNode)
        {
            SortedList<String, TreeNode> list = new SortedList<string, TreeNode>();

            // Types
            foreach (ITypeDeclaration nestedTypeDeclaration in typeDeclaration.Types)
            {
                StringBuilder builder = new StringBuilder();
                MakeTypeDeclarationName(nestedTypeDeclaration, builder);

                TreeNode typeNode = new TreeNode(builder.ToString());
                typeNode.Tag = nestedTypeDeclaration;

                if (nestedTypeDeclaration.IsEnum)
                {
                    typeNode.ImageIndex = EnumImageIndex;
                    typeNode.SelectedImageIndex = EnumImageIndex;
                }
                else if (nestedTypeDeclaration.IsInterface)
                {
                    typeNode.ImageIndex = InterfaceImageIndex;
                    typeNode.SelectedImageIndex = InterfaceImageIndex;
                }
                else
                {
                    typeNode.ImageIndex = ClassImageIndex;
                    typeNode.SelectedImageIndex = ClassImageIndex;
                }

                BuildTypeTreeNode(nestedTypeDeclaration, typeNode);

                list.Add(builder.ToString(), typeNode);
            }

            parentNode.Nodes.AddRange(list.Values.ToArray());

            list.Clear();

            // Methods
            foreach (IMethodDeclaration methodDeclaration in typeDeclaration.Constructors)
            {
                TreeNode methodNode = GenerateMethodNode(methodDeclaration);

                if (!list.ContainsKey(methodNode.Text))
                    list.Add(methodNode.Text, methodNode);
                else
                    list.Add(methodNode.Text + Guid.NewGuid(), methodNode);
            }

            parentNode.Nodes.AddRange(list.Values.ToArray());

            list.Clear();

            // Methods
            foreach (IMethodDeclaration methodDeclaration in typeDeclaration.Methods)
            {
                TreeNode methodNode = GenerateMethodNode(methodDeclaration);

                if (!list.ContainsKey(methodNode.Text))
                    list.Add(methodNode.Text, methodNode);
                else
                    list.Add(methodNode.Text + Guid.NewGuid(), methodNode);
            }

            parentNode.Nodes.AddRange(list.Values.ToArray());

            list.Clear();

            // Properties
            foreach (IPropertyDeclaration propertyDeclaration in typeDeclaration.Properties)
            {
                TreeNode propertyNode = new TreeNode(propertyDeclaration.Name);
                propertyNode.Tag = propertyDeclaration;

                MethodVisibility propertyVisibility = DetermineVisibility(propertyDeclaration);

                if (propertyVisibility == MethodVisibility.Assembly)
                {
                    propertyNode.ImageIndex = ProtectedPropertyImageIndex;
                    propertyNode.SelectedImageIndex = ProtectedPropertyImageIndex;
                }
                else if ((propertyVisibility == MethodVisibility.Family) || (propertyVisibility == MethodVisibility.FamilyOrAssembly))
                {
                    propertyNode.ImageIndex = InternalPropertyImageIndex;
                    propertyNode.SelectedImageIndex = InternalPropertyImageIndex;
                }
                else if (propertyVisibility == MethodVisibility.Private)
                {
                    propertyNode.ImageIndex = PrivatePropertyImageIndex;
                    propertyNode.SelectedImageIndex = PrivatePropertyImageIndex;
                }
                else
                {
                    propertyNode.ImageIndex = PublicPropertyImageIndex;
                    propertyNode.SelectedImageIndex = PublicPropertyImageIndex;
                }

                list.Add(propertyDeclaration.Name, propertyNode);

                if (propertyDeclaration.GetMethod != null)
                    propertyNode.Nodes.Add(GenerateMethodNode(propertyDeclaration.GetMethod.Resolve()));

                if (propertyDeclaration.SetMethod != null)
                    propertyNode.Nodes.Add(GenerateMethodNode(propertyDeclaration.SetMethod.Resolve()));
            }

            parentNode.Nodes.AddRange(list.Values.ToArray());

            list.Clear();

            // Non-Constant fields
            foreach (IFieldDeclaration fieldDeclaration in typeDeclaration.Fields)
            {
                if (!fieldDeclaration.IsConst)
                {
                    TreeNode fieldNode = new TreeNode(fieldDeclaration.Name);
                    fieldNode.Tag = fieldDeclaration;

                    if (fieldDeclaration.Visibility == FieldVisibility.Public)
                    {
                        fieldNode.ImageIndex = PublicFieldIndex;
                        fieldNode.SelectedImageIndex = PublicFieldIndex;
                    }
                    else if (fieldDeclaration.Visibility == FieldVisibility.Private)
                    {
                        fieldNode.ImageIndex = PrivateFieldIndex;
                        fieldNode.SelectedImageIndex = PrivateFieldIndex;
                    }
                    else if (fieldDeclaration.Visibility == FieldVisibility.Assembly)
                    {
                        fieldNode.ImageIndex = InternalFieldIndex;
                        fieldNode.SelectedImageIndex = InternalFieldIndex;
                    }
                    else
                    {
                        fieldNode.ImageIndex = ProtectedFieldIndex;
                        fieldNode.SelectedImageIndex = ProtectedFieldIndex;
                    }

                    list.Add(fieldDeclaration.Name, fieldNode);
                }
            }

            parentNode.Nodes.AddRange(list.Values.ToArray());

            list.Clear();

            // Events
            foreach (IEventDeclaration eventDeclaration in typeDeclaration.Events)
            {
                TreeNode eventNode = new TreeNode(eventDeclaration.Name);
                eventNode.Tag = eventDeclaration;

                eventNode.Nodes.Add(GenerateMethodNode(eventDeclaration.AddMethod.Resolve()));
                eventNode.Nodes.Add(GenerateMethodNode(eventDeclaration.RemoveMethod.Resolve()));

                list.Add(eventDeclaration.Name, eventNode);
            }

            parentNode.Nodes.AddRange(list.Values.ToArray());

            list.Clear();

            // Constant fields
            foreach (IFieldDeclaration fieldDeclaration in typeDeclaration.Fields)
            {
                if (fieldDeclaration.IsConst)
                {
                    TreeNode fieldNode = new TreeNode(fieldDeclaration.Name);
                    fieldNode.Tag = fieldDeclaration;

                    if (fieldDeclaration.Visibility == FieldVisibility.Public)
                    {
                        fieldNode.ImageIndex = PublicConstIndex;
                        fieldNode.SelectedImageIndex = PublicConstIndex;
                    }
                    else if (fieldDeclaration.Visibility == FieldVisibility.Private)
                    {
                        fieldNode.ImageIndex = PrivateConstIndex;
                        fieldNode.SelectedImageIndex = PrivateConstIndex;
                    }
                    else if (fieldDeclaration.Visibility == FieldVisibility.Assembly)
                    {
                        fieldNode.ImageIndex = InternalConstIndex;
                        fieldNode.SelectedImageIndex = InternalConstIndex;
                    }
                    else
                    {
                        fieldNode.ImageIndex = ProtectedConstIndex;
                        fieldNode.SelectedImageIndex = ProtectedConstIndex;
                    }

                    list.Add(fieldDeclaration.Name, fieldNode);
                }
            }

            parentNode.Nodes.AddRange(list.Values.ToArray());
        }

		private void _assembliesTree_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (MainModel == null)
				return;

            using (new WaitCursor())
            {
                IMemberReference memberReference = e.Node.Tag as IMemberReference;
                if (memberReference != null)
                {
                    MainModel.CurrentReference = memberReference;
                    return;
                }
            }
		}

		#endregion

		private void SetControlEnabledStates()
		{
			if (MainModel == null)
			{
				_backToolStripMenuItem.Enabled = false;
				_forwardToolStripMenuItem.Enabled = false;
				_openToolStripMenuItem.Enabled = false;

                _backButton.Enabled = false;
                _nextButton.Enabled = false;
			}
			else
			{
				_backToolStripMenuItem.Enabled = MainModel.CanGoBack;
				_forwardToolStripMenuItem.Enabled = MainModel.CanGoForward;
				_openToolStripMenuItem.Enabled = true;

                _backButton.Enabled = MainModel.CanGoBack;
                _nextButton.Enabled = MainModel.CanGoForward;
			}
		}

		private void SetAssemblyList()
		{
			IAssembly[] assemblies = MainModel.Assemblies;

			_assembliesTree.BeginUpdate();
			try
			{
				foreach (IAssembly assembly in assemblies)
				{
					bool found = false;
					foreach (TreeNode node in _assembliesTree.Nodes)
						if (node.Tag == assembly)
						{
							found = true;
							break;
						}

					if (!found)
						AddAssemblyNode(_assembliesTree.Nodes, assembly);
				}
			}
			finally
			{
				_assembliesTree.EndUpdate();
			}
		}

		private static void AddAssemblyNode(TreeNodeCollection nodes, IAssembly assembly)
		{
			TreeNode treeNode = new TreeNode(assembly.Name);
			treeNode.Tag = assembly;
			treeNode.ImageIndex = AssemblyImageIndex;
			treeNode.SelectedImageIndex = AssemblyImageIndex;

			if (assembly.ReferencedAssemblies.Count > 0)
			{
				TreeNode referencesNode = new TreeNode("References");
				referencesNode.ImageIndex = ModuleImageIndex;
				referencesNode.SelectedImageIndex = ModuleImageIndex;

				foreach (IAssemblyReference assemblyReference in assembly.ReferencedAssemblies)
				{
					TreeNode assemblyReferenceNode = new TreeNode(assemblyReference.Name);
					assemblyReferenceNode.Tag = assemblyReference;
					assemblyReferenceNode.ImageIndex = ModuleImageIndex;
					assemblyReferenceNode.SelectedImageIndex = ModuleImageIndex;
					referencesNode.Nodes.Add(assemblyReferenceNode);
				}

				treeNode.Nodes.Add(referencesNode);
			}

			if (assembly.Modules.Count > 0)
			{
				foreach (IModule module in assembly.Modules)
				{
					TreeNode moduleNode = new TreeNode(module.Name);
					moduleNode.Tag = module;
					moduleNode.ImageIndex = ModuleImageIndex;
					moduleNode.SelectedImageIndex = ModuleImageIndex;
					treeNode.Nodes.Add(moduleNode);

					List<ITypeDeclaration> types = module.Types;
					SortedList<String, SortedList<String, TreeNode>> list = new SortedList<String, SortedList<String, TreeNode>>();

					foreach (ITypeDeclaration type in types)
					{
						String name = type.Namespace ?? "-";

						SortedList<String, TreeNode> typeNodes;
						if (!list.TryGetValue(name, out typeNodes))
						{
							typeNodes = new SortedList<String, TreeNode>();
							list.Add(name, typeNodes);
						}

                        StringBuilder builder = new StringBuilder();
                        MakeTypeDeclarationName(type, builder);

						TreeNode typeNode = new TreeNode(builder.ToString());
						typeNode.Tag = type;

						if (type.IsEnum)
						{
							typeNode.ImageIndex = EnumImageIndex;
							typeNode.SelectedImageIndex = EnumImageIndex;
						}
						else if (type.IsInterface)
						{
							typeNode.ImageIndex = InterfaceImageIndex;
							typeNode.SelectedImageIndex = InterfaceImageIndex;
						}
						else
						{
							typeNode.ImageIndex = ClassImageIndex;
							typeNode.SelectedImageIndex = ClassImageIndex;
						}

						typeNode.Nodes.Add("Loading...");
						typeNodes.Add(builder.ToString(), typeNode);
					}

					// Add the storted list of namespace nodes to its parent module node
					foreach (KeyValuePair<String, SortedList<String, TreeNode>> pair in list)
					{
						TreeNode node = new TreeNode(pair.Key);
						node.ImageIndex = NamespaceImageIndex;
						node.SelectedImageIndex = NamespaceImageIndex;
						node.Nodes.AddRange(pair.Value.Values.ToArray());
						moduleNode.Nodes.Add(node);
					}
				}
			}

			if (assembly.Resources.Count > 0)
			{
				TreeNode resourcesNode = new TreeNode("Resources");
				foreach (IResource resource in assembly.Resources)
				{
					TreeNode resourceNode = new TreeNode(resource.Name);
					resourceNode.Tag = resource;
					resourcesNode.Nodes.Add(resourceNode);
				}

				treeNode.Nodes.Add(resourcesNode);
			}

			nodes.Add(treeNode);
		}

		private static TreeNode GenerateMethodNode(IMethodDeclaration methodDeclaration)
		{
			TreeNode methodNode = new TreeNode(BuildMethodName(methodDeclaration));
			methodNode.Tag = methodDeclaration;

			if (methodDeclaration.Visibility == MethodVisibility.Assembly)
			{
				methodNode.ImageIndex = InternalMethodImageIndex;
				methodNode.SelectedImageIndex = InternalMethodImageIndex;
			}
			else if ((methodDeclaration.Visibility == MethodVisibility.Family) || (methodDeclaration.Visibility == MethodVisibility.FamilyOrAssembly))
			{
				methodNode.ImageIndex = ProtectedMethodImageIndex;
				methodNode.SelectedImageIndex = ProtectedMethodImageIndex;
			}
			else if (methodDeclaration.Visibility == MethodVisibility.Private)
			{
				methodNode.ImageIndex = PrivateMethodImageIndex;
				methodNode.SelectedImageIndex = PrivateMethodImageIndex;
			}
			else
			{
				methodNode.ImageIndex = MethodImageIndex;
				methodNode.SelectedImageIndex = MethodImageIndex;
			}

			return methodNode;
		}

		private static MethodVisibility DetermineVisibility(IPropertyDeclaration propertyDeclaration)
		{
			if (propertyDeclaration.SetMethod == null)
				return propertyDeclaration.GetMethod.Resolve().Visibility;

			if (propertyDeclaration.GetMethod == null)
				return propertyDeclaration.SetMethod.Resolve().Visibility;

			MethodVisibility getMethodVisibility = propertyDeclaration.GetMethod.Resolve().Visibility;
			MethodVisibility setMethodVisibility = propertyDeclaration.SetMethod.Resolve().Visibility;

			if (getMethodVisibility < setMethodVisibility)
				return setMethodVisibility;

			return getMethodVisibility;
		}

		private static String BuildMethodName(IMethodReference methodReference)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(methodReference.Name);

			builder.Append('(');
			List<IParameterDeclaration> parameters = methodReference.Parameters;

            if (parameters.Count > 0)
                builder.Append(CSharpBuiltInTypeNameTable.Lookup(parameters[0].ParameterType));

			for (int i = 1; i < parameters.Count; i++)
				builder.Append(", ").Append(CSharpBuiltInTypeNameTable.Lookup(parameters[i].ParameterType));

			builder.Append(')');

			return builder.ToString();
		}

		private void SetCurrentReference()
		{
			if (MainModel == null)
				return;

			IMemberReference memberReference = MainModel.CurrentReference;

			// Select the correct node in the tree
			TreeNode node = FindNode(memberReference);
			if (node == null)
				return;

			node.EnsureVisible();
			_assembliesTree.SelectedNode = node;

			// Set the display context content
			if (memberReference is IResource)
				SetResource((IResource)memberReference);
			else
				SetCodeReference(memberReference);
		}

		private void SetResource(IResource resource)
		{
			if (resource.Data is Dictionary<String, String>)
			{
				_stringTableListView.Items.Clear();
				Dictionary<String, String> stringTable = (Dictionary<String, String>)resource.Data;
				foreach (KeyValuePair<String, String> pair in stringTable)
				{
					ListViewItem item = new ListViewItem(pair.Key);
					item.SubItems.Add(pair.Value);
					_stringTableListView.Items.Add(item);
				}

				SetViewContext(ViewContext.StringTable);
			}
			else if (resource.Data is Bitmap)
			{
				Bitmap bitmap = (Bitmap)resource.Data;
				_imagePictureBox.Width = bitmap.Width;
				_imagePictureBox.Height = bitmap.Height;
				_imagePictureBox.Image = bitmap;

				SetViewContext(ViewContext.Image);
			}
			else if (resource.Data is byte[])
			{
				byte[] data = (byte[])resource.Data;

				StringBuilder builder = new StringBuilder();
				builder.Append(
                    "<html><body style=\"overflow: auto; background: #" + (SystemColors.Info.ToArgb() & 0xffffff).ToString("x6") + "; font-family: Tahoma; font-size :8.25pt\"><pre>");
				int i = 0;
				while (i < data.Length)
				{
					builder.AppendFormat("{0:x08}", i);
					builder.Append(":");

					int count = data.Length - i;
					if (count > 16)
						count = 16;

					for (int j = i; j < i + count; j++)
						builder.AppendFormat(" {0:x02}", data[j]);

					builder.Append(' ');
					for (int j = i; j < i + count; j++)
					{
						char c = (char)data[j];
						if (Char.IsControl(c))
							builder.Append(".");
						else
							builder.Append(c);
					}

					builder.AppendLine();
					i += 16;
				}

				builder.Append("</pre></body></html>");
				_hexCodeBrowser.DocumentText = builder.ToString();
				SetViewContext(ViewContext.Hex);
			}
		}

		private void SetCodeReference(IMemberReference memberReference)
		{
			ILHtmlFormattedCodeWriter msilWriter = new ILHtmlFormattedCodeWriter();
			CSharpHtmlFormattedCodeWriter cSharpWriter = new CSharpHtmlFormattedCodeWriter();

			IAssemblyReference assemblyReference = memberReference as IAssemblyReference;
			if (assemblyReference != null)
			{
				new ILWriter().WriteAssembly(assemblyReference, msilWriter);
				new CSharpWriter().WriteAssembly(assemblyReference, cSharpWriter);
			}

			IModule module = memberReference as IModule;
			if (module != null)
			{
				new ILWriter().WriteModule(module, msilWriter);
				new CSharpWriter().WriteModule(module, cSharpWriter);
			}

			ITypeDeclaration typeDeclaration = memberReference as ITypeDeclaration;
			if (typeDeclaration != null)
			{
				new ILWriter().WriteTypeDeclaration(typeDeclaration, msilWriter);
				new CSharpWriter().WriteTypeDeclaration(typeDeclaration, cSharpWriter);
			}

			IMethodDeclaration methodDeclaration = memberReference as IMethodDeclaration;
			if (methodDeclaration != null)
			{
				new ILWriter().WriteMethodDeclaration(methodDeclaration, msilWriter);
				_controlFlowGraphView.Graph = methodDeclaration.Body.ControlFlowGraph;
				new CSharpWriter().WriteMethodDeclaration(methodDeclaration, cSharpWriter);
			}

			IEventDeclaration eventDeclaration = memberReference as IEventDeclaration;
			if (eventDeclaration != null)
			{
				new ILWriter().WriteEventDeclaration(eventDeclaration, msilWriter);
				new CSharpWriter().WriteEventDeclaration(eventDeclaration, cSharpWriter);
			}

			IPropertyDeclaration propertyDeclaration = memberReference as IPropertyDeclaration;
			if (propertyDeclaration != null)
			{
				new ILWriter().WritePropertyDeclaration(propertyDeclaration, msilWriter);
				new CSharpWriter().WritePropertyDeclaration(propertyDeclaration, cSharpWriter);
			}

			IFieldDeclaration fieldDeclaration = memberReference as IFieldDeclaration;
			if (fieldDeclaration != null)
			{
				new ILWriter().WriteFieldDeclaration(fieldDeclaration, msilWriter);
				new CSharpWriter().WriteFieldDeclaration(fieldDeclaration, cSharpWriter);
			}

			_msilCodeBrowser.DocumentText = msilWriter.Html;
			_cSharpBrowser.DocumentText = cSharpWriter.Html;

			SetViewContext(ViewContext.Code);
		}

		private TreeNode FindNode(IMemberReference memberReference)
		{
			if (memberReference is IAssemblyReference)
			{
				foreach (TreeNode node in _assembliesTree.Nodes)
					if (node.Tag == memberReference)
						return node;

				return null;
			}

			if (memberReference is IResourceReference)
			{
				IResourceReference resourceReference = (IResourceReference)memberReference;
				TreeNode assemblyNode = FindNode(resourceReference.Resolve().Assembly);
				TreeNode resourcesNode = null;
				foreach (TreeNode childNode in assemblyNode.Nodes)
					if (childNode.Text == "Resources")
						resourcesNode = childNode;

				return FindNode(memberReference, resourcesNode);
			}

			if (memberReference is IModuleReference)
			{
				IModuleReference moduleReference = (IModuleReference)memberReference;
				return FindNode(memberReference, FindNode(moduleReference.Resolve().Assembly));
			}

			if (memberReference is ITypeReference)
			{
				ITypeReference typeReference = (ITypeReference)memberReference;


                if (typeReference.Resolve().DeclaringType != null)
                {
                    TreeNode declaringTypeNode = FindNode(typeReference.Resolve().DeclaringType);
                    return FindNode(memberReference, declaringTypeNode);
                }
                else
                {
                    TreeNode moduleNode = FindNode(typeReference.Resolve().Module);

                    foreach (TreeNode namespaceNode in moduleNode.Nodes)
                    {
                        if (((namespaceNode.Text == "-") && (typeReference.Namespace == null))
                            || (namespaceNode.Text == typeReference.Namespace))
                            return FindNode(memberReference, namespaceNode);
                    }
                }

				return null;
			}

			if (memberReference is IMethodReference)
			{
				IMethodReference methodReference = (IMethodReference)memberReference;
				TreeNode typeNode = FindNode(methodReference.Resolve().DeclaringType);

                if (typeNode == null)
                    return null;

                foreach (TreeNode node in typeNode.Nodes)
                {
                    if (node.Tag == memberReference)
                        return node;

                    if ((node.Tag is IPropertyReference) || (node.Tag is IEventReference))
                    {
                        TreeNode nestedEventNode = FindNode(memberReference, node);
                        if (nestedEventNode != null)
                            return nestedEventNode;
                    }
                }

                return null;
			}

			if (memberReference is IFieldReference)
			{
				IFieldReference fieldReference = (IFieldReference)memberReference;
				return FindNode(memberReference, FindNode(fieldReference.Resolve().DeclaringType));
			}

			if (memberReference is IPropertyReference)
			{
				IPropertyReference propertyReference = (IPropertyReference)memberReference;
				return FindNode(memberReference, FindNode(propertyReference.Resolve().DeclaringType));
			}

			if (memberReference is IEventReference)
			{
				IEventReference eventReference = (IEventReference)memberReference;
				return FindNode(memberReference, FindNode(eventReference.Resolve().DeclaringType));
			}

			return null;
		}

		private static TreeNode FindNode(IMemberReference memberReference, TreeNode parentNode)
		{
			if (parentNode == null)
				return null;

			foreach (TreeNode node in parentNode.Nodes)
				if (node.Tag == memberReference)
					return node;

			return null;
		}

		private void _cSharpBrowser_BeforeNavigate(object sender, CodeBrowser.NavigateEventArgs e)
		{
			if (!e.Uri.StartsWith("resource://"))
				return;

			e.Cancel = true;

			String resourceName = e.Uri.Substring(11);
			if (resourceName.EndsWith("/"))
				resourceName = resourceName.Substring(0, resourceName.Length - 1);

			IMemberReference memberReference = AssemblyManager.ParseMemberReference(resourceName);
			if (memberReference != null)
				MainModel.CurrentReference = memberReference;
		}

		private void SetViewContext(ViewContext viewContext)
		{
			if (viewContext == _currentViewContext)
				return;

			_tabControl.SuspendLayout();
			_tabControl.TabPages.Clear();

			switch (viewContext)
			{
				case ViewContext.Code:
					{
						_tabControl.TabPages.Add(_msilDisassemblyTabPage);
						_tabControl.TabPages.Add(_controlFlowTabPage);
						_tabControl.TabPages.Add(_cSharpTabPage);
						break;
					}

				case ViewContext.Hex:
					{
						_tabControl.TabPages.Add(_hexTabPage);
						break;
					}

				case ViewContext.Image:
					{
						_tabControl.TabPages.Add(_imageTabPage);
						break;
					}

				case ViewContext.StringTable:
					{
						_tabControl.TabPages.Add(_stringTableTabPage);
						break;
					}
			}

			_tabControl.ResumeLayout();

			_currentViewContext = viewContext;
		}

		private void _assembliesTree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if (MainModel == null)
				return;

			IAssemblyReference assemblyReference = e.Node.Tag as IAssemblyReference;
			if (assemblyReference != null)
			{
				MainModel.CurrentReference = assemblyReference.Resolve();
			}
		}
	}
}