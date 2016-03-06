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
using System.ComponentModel.Composition;
using System.Diagnostics;
using dnlib.DotNet;
using dnSpy.Contracts.Files;
using dnSpy.Contracts.Files.TreeView;
using dnSpy.Contracts.TreeView;

namespace dnSpy.AsmEditor.Hex.Nodes {
	abstract class PETreeNodeDataCreatorBase : ITreeNodeDataCreator {
		readonly Lazy<IHexDocumentManager> hexDocumentManager;

		protected PETreeNodeDataCreatorBase(Lazy<IHexDocumentManager> hexDocumentManager) {
			this.hexDocumentManager = hexDocumentManager;
		}

		public IEnumerable<ITreeNodeData> Create(TreeNodeDataCreatorContext context) {
			var fileNode = context.Owner.Data as IDnSpyFileNode;
			Debug.Assert(fileNode != null);
			if (fileNode == null)
				yield break;

			bool hasPENode = HasPENode(fileNode);
			var peImage = fileNode.DnSpyFile.PEImage;
			Debug.Assert(!hasPENode || peImage != null);
			if (hasPENode && peImage != null)
				yield return new PENode(hexDocumentManager.Value, peImage, fileNode.DnSpyFile.ModuleDef as ModuleDefMD);
		}

		public static bool HasPENode(IDnSpyFileNode node) {
			if (node == null)
				return false;

			var peImage = node.DnSpyFile.PEImage;

			// Only show the PE node if it was loaded from a file. The hex document is always loaded
			// from a file, so if the PEImage wasn't loaded from the same file, conversion to/from
			// RVA/FileOffset won't work and the wrong data will be displayed, eg. in the .NET
			// storage stream nodes.
			bool loadedFromFile = node.DnSpyFile.Key is FilenameKey;
			return loadedFromFile && peImage != null;
		}
	}

	[ExportTreeNodeDataCreator(Guid = FileTVConstants.MODULE_NODE_GUID)]
	sealed class ModulePETreeNodeDataCreator : PETreeNodeDataCreatorBase {
		[ImportingConstructor]
		ModulePETreeNodeDataCreator(Lazy<IHexDocumentManager> hexDocumentManager)
			: base(hexDocumentManager) {
		}
	}

	[ExportTreeNodeDataCreator(Guid = FileTVConstants.PEFILE_NODE_GUID)]
	sealed class PEFilePETreeNodeDataCreator : PETreeNodeDataCreatorBase {
		[ImportingConstructor]
		PEFilePETreeNodeDataCreator(Lazy<IHexDocumentManager> hexDocumentManager)
			: base(hexDocumentManager) {
		}
	}
}
