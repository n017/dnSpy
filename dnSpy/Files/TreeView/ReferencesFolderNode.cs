﻿/*
    Copyright (C) 2014-2016 de4dot@gmail.com

    This file is part of dnSpy

    dnSpy is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    dnSpy is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with dnSpy.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using dnlib.DotNet;
using dnSpy.Contracts.Files.TreeView;
using dnSpy.Contracts.Highlighting;
using dnSpy.Contracts.Images;
using dnSpy.Contracts.Languages;
using dnSpy.Contracts.TreeView;
using dnSpy.Decompiler.Shared;
using dnSpy.Properties;
using dnSpy.Shared.Files.TreeView;

namespace dnSpy.Files.TreeView {
	sealed class ReferencesFolderNode : FileTreeNodeData, IReferencesFolderNode {
		public override Guid Guid {
			get { return new Guid(FileTVConstants.REFERENCES_FOLDER_NODE_GUID); }
		}

		protected override ImageReference GetIcon(IDotNetImageManager dnImgMgr) {
			return new ImageReference(GetType().Assembly, "ReferenceFolder");
		}

		public override NodePathName NodePathName {
			get { return new NodePathName(Guid); }
		}

		public override void Initialize() {
			TreeNode.LazyLoading = true;
		}

		public override ITreeNodeGroup TreeNodeGroup {
			get { return treeNodeGroup; }
		}
		readonly ITreeNodeGroup treeNodeGroup;

		readonly IModuleFileNode moduleNode;

		public ReferencesFolderNode(ITreeNodeGroup treeNodeGroup, IModuleFileNode moduleNode) {
			Debug.Assert(moduleNode.DnSpyFile.ModuleDef != null);
			this.treeNodeGroup = treeNodeGroup;
			this.moduleNode = moduleNode;
		}

		protected override void Write(ISyntaxHighlightOutput output, ILanguage language) {
			output.Write(dnSpy_Resources.ReferencesFolder, TextTokenKind.Text);
		}

		public override IEnumerable<ITreeNodeData> CreateChildren() {
			foreach (var asmRef in moduleNode.DnSpyFile.ModuleDef.GetAssemblyRefs())
				yield return new AssemblyReferenceNode(Context.FileTreeView.FileTreeNodeGroups.GetGroup(FileTreeNodeGroupType.AssemblyRefTreeNodeGroupReferences), moduleNode.DnSpyFile.ModuleDef, asmRef);
			foreach (var modRef in moduleNode.DnSpyFile.ModuleDef.GetModuleRefs())
				yield return new ModuleReferenceNode(Context.FileTreeView.FileTreeNodeGroups.GetGroup(FileTreeNodeGroupType.ModuleRefTreeNodeGroupReferences), modRef);
		}

		public IAssemblyReferenceNode Create(AssemblyRef asmRef) {
			return Context.FileTreeView.Create(asmRef, moduleNode.DnSpyFile.ModuleDef);
		}

		public IModuleReferenceNode Create(ModuleRef modRef) {
			return Context.FileTreeView.Create(modRef);
		}

		public override FilterType GetFilterType(IFileTreeNodeFilter filter) {
			return filter.GetResult(this).FilterType;
		}
	}
}
