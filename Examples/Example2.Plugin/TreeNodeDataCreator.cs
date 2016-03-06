﻿using System;
using System.Collections.Generic;
using System.Threading;
using dnSpy.Contracts.Files.Tabs.TextEditor;
using dnSpy.Contracts.Files.TreeView;
using dnSpy.Contracts.Highlighting;
using dnSpy.Contracts.Images;
using dnSpy.Contracts.Languages;
using dnSpy.Contracts.TreeView;
using dnSpy.Decompiler.Shared;
using dnSpy.Shared.Decompiler;
using dnSpy.Shared.Files.TreeView;

// This file contains classes that create new child nodes of IAssemblyFileNode and IModuleFileNode

namespace Example2.Plugin {
	// This class adds a new child node to all assembly nodes
	[ExportTreeNodeDataCreator(Guid = FileTVConstants.ASSEMBLY_NODE_GUID)]
	sealed class AssemblyTreeNodeDataCreator : ITreeNodeDataCreator {
		public IEnumerable<ITreeNodeData> Create(TreeNodeDataCreatorContext context) {
			yield return new AssemblyChildNode();
		}
	}

	// This class adds a new child node to all module nodes
	[ExportTreeNodeDataCreator(Guid = FileTVConstants.MODULE_NODE_GUID)]
	sealed class ModuleTreeNodeDataCreator : ITreeNodeDataCreator {
		public IEnumerable<ITreeNodeData> Create(TreeNodeDataCreatorContext context) {
			yield return new ModuleChildNode();
		}
	}

	sealed class AssemblyChildNode : FileTreeNodeData { // All file tree nodes should implement IFileTreeNodeData or derive from FileTreeNodeData
		//TODO: Use your own guid
		public static readonly Guid THE_GUID = new Guid("6CF91674-16CE-44EA-B9E8-80B68C327D30");

		public override Guid Guid {
			get { return THE_GUID; }
		}

		public override NodePathName NodePathName {
			// If there could be more instances of this class with the same parent, add a second
			// argument to the NodePathName constructor, eg. an index or some other unique string.
			// Eg. IMethodNode uses the name of the method.
			get { return new NodePathName(Guid); }
		}

		protected override ImageReference GetIcon(IDotNetImageManager dnImgMgr) {
			// The image must be in an Images folder (in the resources) and have a .png extension
			return new ImageReference(GetType().Assembly, "EntryPoint");
		}

		protected override void Write(ISyntaxHighlightOutput output, ILanguage language) {
			output.Write("Assembly Child", TextTokenKind.Text);
		}

		// If you don't want the node to be appended to the children, override this
		public override ITreeNodeGroup TreeNodeGroup {
			get { return TreeNodeGroupImpl.Instance; }
		}

		sealed class TreeNodeGroupImpl : ITreeNodeGroup {
			public static readonly TreeNodeGroupImpl Instance = new TreeNodeGroupImpl(1);

			public TreeNodeGroupImpl(double order) {
				this.order = order;
			}

			public double Order {
				get { return order; }
				set { order = value; }
			}
			double order;

			public int Compare(ITreeNodeData x, ITreeNodeData y) {
				if (x == y)
					return 0;
				var a = x as AssemblyChildNode;
				var b = y as AssemblyChildNode;
				if (a == null) return -1;
				if (b == null) return 1;
				// More checks can be added here...
				return 0;
			}
		}
	}

	// This class can decompile its own output and implements IDecompileSelf
	sealed class ModuleChildNode : FileTreeNodeData, IDecompileSelf { // All file tree nodes should implement IFileTreeNodeData or derive from FileTreeNodeData
		//TODO: Use your own guid
		public static readonly Guid THE_GUID = new Guid("C8892F6C-6A49-4537-AAA0-D0DEF1E87277");

		public override Guid Guid {
			get { return THE_GUID; }
		}

		public override NodePathName NodePathName {
			// If there could be more instances of this class with the same parent, add a second
			// argument to the NodePathName constructor, eg. an index or some other unique string.
			// Eg. IMethodNode uses the name of the method.
			get { return new NodePathName(Guid); }
		}

		// Initialize() is called after the constructor has been called, and after the TreeNode prop
		// has been initialized
		public override void Initialize() {
			// Set LazyLoading if creating the children could take a while
			TreeNode.LazyLoading = true;
		}

		// If TreeNode.LazyLoading is false, CreateChildren() is called after Initialize(), else it's
		// called when this node gets expanded.
		public override IEnumerable<ITreeNodeData> CreateChildren() {
			// Add some children in random order. They will be sorted because SomeMessageNode
			// overrides the TreeNodeGroup prop.
			yield return new SomeMessageNode("ZZZZZZZZZZZZZ");
			yield return new SomeMessageNode("AAAAaaaaaAAAAAAA");
			yield return new SomeMessageNode("SAY");
		}

		protected override ImageReference GetIcon(IDotNetImageManager dnImgMgr) {
			// The image must be in an Images folder (in the resources) and have a .png extension
			return new ImageReference(GetType().Assembly, "Strings");
		}

		protected override void Write(ISyntaxHighlightOutput output, ILanguage language) {
			output.Write("Module Child", TextTokenKind.Text);
		}

		// Gets called by dnSpy if there's only one node to decompile. This method gets called in a
		// worker thread. The output is also cached unless you call AvalonEditTextOutput.DontCacheOutput().
		public bool Decompile(IDecompileNodeContext context) {
			// Pretend we actually do something...
			Thread.Sleep(2000);

			// If you don't want the output to be cached, call DontCacheOutput()
			bool cacheOutput = true;
			if (!cacheOutput) {
				var avOutput = context.Output as AvalonEditTextOutput;
				if (avOutput != null)
					avOutput.DontCacheOutput();
			}

			// Create the output and a few references that other code in this plugin will use, eg.
			// to show a tooltip when hovering over the reference.
			context.HighlightingExtension = ".cs";
			context.Language.WriteCommentLine(context.Output, "Initialize it to the secret key");
			context.Output.WriteReference("int", new StringInfoReference("This is a reference added by the code"), TextTokenKind.Keyword);
			context.Output.WriteSpace();
			context.Output.WriteReference("secret", new StringInfoReference("The real secret is actually 42 not 1234"), TextTokenKind.Local);
			context.Output.WriteSpace();
			context.Output.Write("=", TextTokenKind.Operator);
			context.Output.WriteSpace();
			context.Output.Write("1234", TextTokenKind.Number);
			context.Output.Write(";", TextTokenKind.Operator);
			context.Output.WriteLine();

			// We decompiled ourselves so return true
			return true;
		}

		// If you don't want the node to be appended to the children, override this
		public override ITreeNodeGroup TreeNodeGroup {
			get { return TreeNodeGroupImpl.Instance; }
		}

		sealed class TreeNodeGroupImpl : ITreeNodeGroup {
			// Make sure the order is unique. 0 is already used by the PE node, so use 1
			public static readonly TreeNodeGroupImpl Instance = new TreeNodeGroupImpl(1);

			public TreeNodeGroupImpl(double order) {
				this.order = order;
			}

			public double Order {
				get { return order; }
				set { order = value; }
			}
			double order;

			public int Compare(ITreeNodeData x, ITreeNodeData y) {
				if (x == y)
					return 0;
				var a = x as ModuleChildNode;
				var b = y as ModuleChildNode;
				if (a == null) return -1;
				if (b == null) return 1;
				// More checks can be added here...
				return 0;
			}
		}
	}

	sealed class SomeMessageNode : FileTreeNodeData {
		//TODO: Use your own guid
		public static readonly Guid THE_GUID = new Guid("1751CD40-68CE-4F8A-84AF-99371B6FD843");

		public string Message {
			get { return msg; }
		}
		readonly string msg;

		public SomeMessageNode(string msg) {
			this.msg = msg;
		}

		public override Guid Guid {
			get { return THE_GUID; }
		}

		public override NodePathName NodePathName {
			get { return new NodePathName(THE_GUID, msg); }
		}

		protected override ImageReference GetIcon(IDotNetImageManager dnImgMgr) {
			return new ImageReference(GetType().Assembly, "Strings");
		}

		protected override void Write(ISyntaxHighlightOutput output, ILanguage language) {
			output.Write(msg, TextTokenKind.Comment);
		}

		public override ITreeNodeGroup TreeNodeGroup {
			get { return TreeNodeGroupImpl.Instance; }
		}

		sealed class TreeNodeGroupImpl : ITreeNodeGroup {
			public static readonly TreeNodeGroupImpl Instance = new TreeNodeGroupImpl(0);

			public TreeNodeGroupImpl(double order) {
				this.order = order;
			}

			public double Order {
				get { return order; }
				set { order = value; }
			}
			double order;

			public int Compare(ITreeNodeData x, ITreeNodeData y) {
				if (x == y)
					return 0;
				var a = x as SomeMessageNode;
				var b = y as SomeMessageNode;
				if (a == null) return -1;
				if (b == null) return 1;
				return StringComparer.OrdinalIgnoreCase.Compare(a.msg, b.msg);
			}
		}
	}

	// SomeMessageNode doesn't implement IDecompileSelf, so we add a "decompiler" that can "decompile"
	// those nodes.
	[ExportDecompileNode]
	sealed class SomeMessageNodeDecompiler : IDecompileNode {
		public bool Decompile(IDecompileNodeContext context, IFileTreeNodeData node) {
			var msgNode = node as SomeMessageNode;
			if (msgNode == null)
				return false;

			context.Language.WriteCommentLine(context.Output, "The secret message has been decrypted.");
			context.Language.WriteCommentLine(context.Output, string.Format("The message is: {0}", msgNode.Message));
			return true;
		}
	}
}
