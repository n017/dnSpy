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
using System.IO;
using System.Linq;
using System.Windows.Threading;
using dnSpy.Contracts.App;
using dnSpy.Contracts.Files.Tabs;
using dnSpy.Contracts.Files.Tabs.TextEditor;
using dnSpy.Contracts.Files.TreeView;
using dnSpy.Contracts.Languages;
using dnSpy.Decompiler.Shared;
using dnSpy.Files.Tabs.TextEditor;
using dnSpy.Languages;
using dnSpy.Properties;
using Microsoft.Win32;

namespace dnSpy.Files.Tabs {
	[ExportTabSaverCreator(Order = TabConstants.ORDER_DEFAULTTABSAVERCREATOR)]
	sealed class NodeTabSaverCreator : ITabSaverCreator {
		readonly IFileTreeNodeDecompiler fileTreeNodeDecompiler;
		readonly IMessageBoxManager messageBoxManager;

		[ImportingConstructor]
		NodeTabSaverCreator(IFileTreeNodeDecompiler fileTreeNodeDecompiler, IMessageBoxManager messageBoxManager) {
			this.fileTreeNodeDecompiler = fileTreeNodeDecompiler;
			this.messageBoxManager = messageBoxManager;
		}

		public ITabSaver Create(IFileTab tab) {
			return NodeTabSaver.TryCreate(fileTreeNodeDecompiler, tab, messageBoxManager);
		}
	}

	sealed class NodeTabSaver : ITabSaver {
		readonly IMessageBoxManager messageBoxManager;
		readonly IFileTab tab;
		readonly IFileTreeNodeDecompiler fileTreeNodeDecompiler;
		readonly ILanguage language;
		readonly IFileTreeNodeData[] nodes;
		readonly ITextEditorUIContext uiContext;

		public static NodeTabSaver TryCreate(IFileTreeNodeDecompiler fileTreeNodeDecompiler, IFileTab tab, IMessageBoxManager messageBoxManager) {
			if (tab.IsAsyncExecInProgress)
				return null;
			var uiContext = tab.UIContext as ITextEditorUIContext;
			if (uiContext == null)
				return null;
			var langContent = tab.Content as ILanguageTabContent;
			var lang = langContent == null ? null : langContent.Language;
			if (lang == null)
				return null;
			var nodes = tab.Content.Nodes.ToArray();
			if (nodes.Length == 0)
				return null;
			return new NodeTabSaver(messageBoxManager, tab, fileTreeNodeDecompiler, lang, uiContext, nodes);
		}

		NodeTabSaver(IMessageBoxManager messageBoxManager, IFileTab tab, IFileTreeNodeDecompiler fileTreeNodeDecompiler, ILanguage language, ITextEditorUIContext uiContext, IFileTreeNodeData[] nodes) {
			this.messageBoxManager = messageBoxManager;
			this.tab = tab;
			this.fileTreeNodeDecompiler = fileTreeNodeDecompiler;
			this.language = language;
			this.uiContext = uiContext;
			this.nodes = nodes;
		}

		public bool CanSave {
			get { return !tab.IsAsyncExecInProgress; }
		}

		public string MenuHeader {
			get { return dnSpy_Resources.Button_SaveCode; }
		}

		sealed class DecompileContext : IDisposable {
			public DecompileNodeContext DecompileNodeContext;
			public TextWriter Writer;

			public void Dispose() {
				if (Writer != null)
					Writer.Dispose();
			}
		}

		DecompileContext CreateDecompileContext(string filename) {
			var decompileContext = new DecompileContext();
			try {
				var decompilationContext = new DecompilationContext();
				decompileContext.Writer = new StreamWriter(filename);
				var output = new PlainTextOutput(decompileContext.Writer);
				var dispatcher = Dispatcher.CurrentDispatcher;
				decompileContext.DecompileNodeContext = new DecompileNodeContext(decompilationContext, language, output, dispatcher);
				return decompileContext;
			}
			catch {
				decompileContext.Dispose();
				throw;
			}
		}

		DecompileContext CreateDecompileContext() {
			var saveDlg = new SaveFileDialog {
				FileName = FilenameUtils.CleanName(nodes[0].ToString(language)) + language.FileExtension,
				DefaultExt = language.FileExtension,
				Filter = string.Format("{0}|*{1}|{2}|*.*", language.GenericNameUI, language.FileExtension, dnSpy_Resources.AllFiles),
			};
			if (saveDlg.ShowDialog() != true)
				return null;
			return CreateDecompileContext(saveDlg.FileName);
		}

		public void Save() {
			if (!CanSave)
				return;

			var ctx = CreateDecompileContext();
			if (ctx == null)
				return;

			tab.AsyncExec(cs => {
				ctx.DecompileNodeContext.DecompilationContext.CancellationToken = cs.Token;
				uiContext.ShowCancelButton(() => cs.Cancel(), dnSpy_Resources.SavingCode);
			}, () => {
				fileTreeNodeDecompiler.Decompile(ctx.DecompileNodeContext, nodes);
			}, result => {
				ctx.Dispose();
				uiContext.HideCancelButton();
				if (result.Exception != null)
					messageBoxManager.Show(result.Exception);
			});
		}
	}
}
