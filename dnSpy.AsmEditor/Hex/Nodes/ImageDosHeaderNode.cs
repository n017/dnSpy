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
using dnlib.PE;
using dnSpy.AsmEditor.Properties;
using dnSpy.Contracts.Files.TreeView;
using dnSpy.Contracts.Highlighting;
using dnSpy.Decompiler.Shared;
using dnSpy.Shared.HexEditor;

namespace dnSpy.AsmEditor.Hex.Nodes {
	sealed class ImageDosHeaderNode : HexNode {
		public override Guid Guid {
			get { return new Guid(FileTVConstants.IMGDOSHEADER_NODE_GUID); }
		}

		public override NodePathName NodePathName {
			get { return new NodePathName(Guid); }
		}

		public override object VMObject {
			get { return imageDosHeaderVM; }
		}

		protected override IEnumerable<HexVM> HexVMs {
			get { yield return imageDosHeaderVM; }
		}

		protected override string IconName {
			get { return "BinaryFile"; }
		}

		readonly ImageDosHeaderVM imageDosHeaderVM;

		public ImageDosHeaderNode(HexDocument doc, ImageDosHeader dosHeader)
			: base((ulong)dosHeader.StartOffset, (ulong)dosHeader.EndOffset - 1) {
			this.imageDosHeaderVM = new ImageDosHeaderVM(this, doc, StartOffset);
		}

		protected override void Write(ISyntaxHighlightOutput output) {
			output.Write(dnSpy_AsmEditor_Resources.HexNode_DOSHeader, TextTokenKind.Keyword);
		}
	}
}
