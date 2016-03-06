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

namespace dnSpy.Contracts.TreeView {
	/// <summary>
	/// Creates <see cref="ITreeNodeData"/>. Use <see cref="ExportTreeNodeDataCreatorAttribute"/> to
	/// export an instance.
	/// </summary>
	public interface ITreeNodeDataCreator {
		/// <summary>
		/// Creates new <see cref="ITreeNodeData"/>
		/// </summary>
		/// <param name="context">Context</param>
		/// <returns></returns>
		IEnumerable<ITreeNodeData> Create(TreeNodeDataCreatorContext context);
	}

	/// <summary>Metadata</summary>
	public interface ITreeNodeDataCreatorMetadata {
		/// <summary>See <see cref="ExportTreeNodeDataCreatorAttribute.Order"/></summary>
		double Order { get; }
		/// <summary>See <see cref="ExportTreeNodeDataCreatorAttribute.Guid"/></summary>
		string Guid { get; }
	}

	/// <summary>
	/// Exports a <see cref="ITreeNodeDataCreator"/> instance
	/// </summary>
	[MetadataAttribute, AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class ExportTreeNodeDataCreatorAttribute : ExportAttribute, ITreeNodeDataCreatorMetadata {
		/// <summary>Constructor</summary>
		public ExportTreeNodeDataCreatorAttribute()
			: base(typeof(ITreeNodeDataCreator)) {
			Order = double.MaxValue;
		}

		/// <summary>
		/// Order of this instance
		/// </summary>
		public double Order { get; set; }

		/// <summary>
		/// Guid of owner <see cref="ITreeNodeData"/> that will receive the new
		/// <see cref="ITreeNodeData"/> nodes
		/// </summary>
		public string Guid { get; set; }
	}
}
