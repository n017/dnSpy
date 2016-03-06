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
using dnSpy.Contracts.Files;
using dnSpy.Contracts.Files.TreeView;
using dnSpy.Shared.Files.TreeView;

namespace dnSpy.Files.TreeView {
	abstract class DnSpyFileNode : FileTreeNodeData, IDnSpyFileNode {
		public IDnSpyFile DnSpyFile {
			get { return dnSpyFile; }
		}
		readonly IDnSpyFile dnSpyFile;

		protected DnSpyFileNode(IDnSpyFile dnSpyFile) {
			if (dnSpyFile == null)
				throw new ArgumentNullException();
			this.dnSpyFile = dnSpyFile;
		}

		public sealed override NodePathName NodePathName {
			get { return new NodePathName(Guid, (DnSpyFile.Filename ?? string.Empty).ToUpperInvariant()); }
		}

		public override FilterType GetFilterType(IFileTreeNodeFilter filter) {
			return filter.GetResult(DnSpyFile).FilterType;
		}
	}
}
