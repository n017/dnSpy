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
using System.ComponentModel.Composition;

namespace dnSpy.Contracts.Files.TreeView {
	/// <summary>
	/// Creates <see cref="IDnSpyFileNode"/>s. Use <see cref="ExportDnSpyFileNodeCreatorAttribute"/>
	/// to export an instance.
	/// </summary>
	public interface IDnSpyFileNodeCreator {
		/// <summary>
		/// Creates a new <see cref="IDnSpyFileNode"/> instance or returns null
		/// </summary>
		/// <param name="fileTreeView">File treeview</param>
		/// <param name="owner">Owner node or null if owner is the root node</param>
		/// <param name="file">New file</param>
		/// <returns></returns>
		IDnSpyFileNode Create(IFileTreeView fileTreeView, IDnSpyFileNode owner, IDnSpyFile file);
	}

	/// <summary>Metadata</summary>
	public interface IDnSpyFileNodeCreatorMetadata {
		/// <summary>See <see cref="ExportDnSpyFileNodeCreatorAttribute.Order"/></summary>
		double Order { get; }
	}

	/// <summary>
	/// Exports a <see cref="IDnSpyFileNodeCreator"/> instance
	/// </summary>
	[MetadataAttribute, AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class ExportDnSpyFileNodeCreatorAttribute : ExportAttribute, IDnSpyFileNodeCreatorMetadata {
		/// <summary>Constructor</summary>
		public ExportDnSpyFileNodeCreatorAttribute()
			: base(typeof(IDnSpyFileNodeCreator)) {
		}

		/// <summary>
		/// Order of this instance
		/// </summary>
		public double Order { get; set; }
	}
}
