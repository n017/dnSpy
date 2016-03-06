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
using dnSpy.Analyzer.Properties;
using dnSpy.Contracts.Files.TreeView;
using dnSpy.Contracts.Images;
using dnSpy.Contracts.TreeView;
using dnSpy.Shared.TreeView;

namespace dnSpy.Analyzer.TreeNodes {
	sealed class AsyncFetchChildrenHelper : AsyncNodeCreator {
		readonly SearchNode node;
		readonly Action completed;

		public AsyncFetchChildrenHelper(SearchNode node, Action completed)
			: base(node) {
			this.node = node;
			this.completed = completed;
			Start();
		}

		sealed class MessageNodeTreeNodeGroup : ITreeNodeGroup {
			public double Order {
				get { return order; }
				set { order = value; }
			}
			double order;

			public int Compare(ITreeNodeData x, ITreeNodeData y) {
				if (x == y)
					return 0;
				var a = x as IMessageNode;
				var b = y as IMessageNode;
				if (a == null) return -1;
				if (b == null) return 1;
				return 0;
			}
		}

		sealed class MessageNode : TreeNodeData {
			public override Guid Guid {
				get { return Guid.Empty; }
			}

			public override ImageReference Icon {
				get { return new ImageReference(GetType().Assembly, "Search"); }
			}

			public override object Text {
				get { return dnSpy_Analyzer_Resources.Searching; }
			}

			public override object ToolTip {
				get { return null; }
			}

			public override void OnRefreshUI() {
			}

			public override ITreeNodeGroup TreeNodeGroup {
				get { return treeNodeGroup; }
			}
			readonly ITreeNodeGroup treeNodeGroup = new MessageNodeTreeNodeGroup();
		}

		protected override void ThreadMethod() {
			AddMessageNode(() => new MessageNode());
			foreach (var child in node.FetchChildrenInternal(cancellationToken))
				AddNode(child);
		}

		protected override void OnCompleted() {
			completed();
		}
	}
}
