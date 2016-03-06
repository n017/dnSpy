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
	sealed class ImageOptionalHeader32Node : HexNode {
		public override Guid Guid {
			get { return new Guid(FileTVConstants.IMGOPTHEADER32_NODE_GUID); }
		}

		public override NodePathName NodePathName {
			get { return new NodePathName(Guid); }
		}

		public override object VMObject {
			get { return imageOptionalHeader32VM; }
		}

		protected override IEnumerable<HexVM> HexVMs {
			get { yield return imageOptionalHeader32VM; }
		}

		protected override string IconName {
			get { return "BinaryFile"; }
		}

		readonly ImageOptionalHeader32VM imageOptionalHeader32VM;

		public ImageOptionalHeader32Node(HexDocument doc, ImageOptionalHeader32 optHdr)
			: base((ulong)optHdr.StartOffset, (ulong)optHdr.EndOffset - 1) {
			this.imageOptionalHeader32VM = new ImageOptionalHeader32VM(this, doc, StartOffset, EndOffset);
		}

		protected override void Write(ISyntaxHighlightOutput output) {
			output.Write(dnSpy_AsmEditor_Resources.HexNode_OptHeader32, TextTokenKind.Keyword);
		}
	}
}
