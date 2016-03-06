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
using System.Runtime.InteropServices;
using System.Windows;
using dnlib.DotNet;
using dnSpy.Contracts.Files.Tabs.TextEditor;
using dnSpy.Contracts.Menus;
using dnSpy.Contracts.TreeView;
using dnSpy.Decompiler.Shared;
using dnSpy.Shared.Menus;

namespace dnSpy.Files.Tabs {
	static class CopyTokenCommand {
		static void ExecuteInternal(uint? token) {
			if (token != null) {
				try {
					Clipboard.SetText(string.Format("0x{0:X8}", token.Value));
				}
				catch (ExternalException) { }
			}
		}

		[ExportMenuItem(Header = "res:CopyMDTokenCommand", Group = MenuConstants.GROUP_CTX_CODE_TOKENS, Order = 50)]
		sealed class CodeCommand : MenuItemBase {
			public override bool IsVisible(IMenuItemContext context) {
				return GetReference(context) != null;
			}

			public override void Execute(IMenuItemContext context) {
				ExecuteInternal(GetReference(context));
			}

			static uint? GetReference(IMenuItemContext context) {
				return GetReference(context, MenuConstants.GUIDOBJ_TEXTEDITORCONTROL_GUID);
			}

			internal static uint? GetReference(IMenuItemContext context, string guid) {
				if (context.CreatorObject.Guid != new Guid(guid))
					return null;
				var @ref = context.Find<CodeReference>();
				if (@ref != null && @ref.Reference is IMDTokenProvider)
					return ((IMDTokenProvider)@ref.Reference).MDToken.Raw;
				if (@ref != null && @ref.Reference is TokenReference)
					return ((TokenReference)@ref.Reference).Token;
				return null;
			}
		}

		[ExportMenuItem(Header = "res:CopyMDTokenCommand", Group = MenuConstants.GROUP_CTX_SEARCH_TOKENS, Order = 0)]
		sealed class SearchCommand : MenuItemBase {
			public override bool IsVisible(IMenuItemContext context) {
				return GetReference(context) != null;
			}

			public override void Execute(IMenuItemContext context) {
				ExecuteInternal(GetReference(context));
			}

			static uint? GetReference(IMenuItemContext context) {
				return CodeCommand.GetReference(context, MenuConstants.GUIDOBJ_SEARCH_GUID);
			}
		}

		[ExportMenuItem(Header = "res:CopyMDTokenCommand", Group = MenuConstants.GROUP_CTX_FILES_TOKENS, Order = 40)]
		sealed class FilesCommand : MenuItemBase {
			public override bool IsVisible(IMenuItemContext context) {
				return GetReference(context) != null;
			}

			public override void Execute(IMenuItemContext context) {
				ExecuteInternal(GetReference(context));
			}

			static uint? GetReference(IMenuItemContext context) {
				return GetReference(context, MenuConstants.GUIDOBJ_FILES_TREEVIEW_GUID);
			}

			internal static uint? GetReference(IMenuItemContext context, string guid) {
				if (context.CreatorObject.Guid != new Guid(guid))
					return null;
				var nodes = context.Find<ITreeNodeData[]>();
				if (nodes == null || nodes.Length == 0)
					return null;
				var node = nodes[0] as IMDTokenNode;
				return node == null ? (uint?)null : node.Reference == null ? (uint?)null : node.Reference.MDToken.Raw;
			}
		}

		[ExportMenuItem(Header = "res:CopyMDTokenCommand", Group = MenuConstants.GROUP_CTX_ANALYZER_TOKENS, Order = 0)]
		sealed class AnalyzerCommand : MenuItemBase {
			public override bool IsVisible(IMenuItemContext context) {
				return GetReference(context) != null;
			}

			public override void Execute(IMenuItemContext context) {
				ExecuteInternal(GetReference(context));
			}

			static uint? GetReference(IMenuItemContext context) {
				return FilesCommand.GetReference(context, MenuConstants.GUIDOBJ_ANALYZER_TREEVIEW_GUID);
			}
		}
	}
}
