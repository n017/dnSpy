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

using System.ComponentModel.Composition;
using dnSpy.Contracts.Controls;
using dnSpy.Contracts.Menus;
using dnSpy.Contracts.TextEditor;
using dnSpy.Contracts.Themes;

namespace dnSpy.TextEditor {
	[Export, Export(typeof(IReplEditorCreator)), PartCreationPolicy(CreationPolicy.Shared)]
	sealed class ReplEditorCreator : IReplEditorCreator {
		readonly IThemeManager themeManager;
		readonly IWpfCommandManager wpfCommandManager;
		readonly IMenuManager menuManager;
		readonly ITextEditorSettings textEditorSettings;

		[ImportingConstructor]
		ReplEditorCreator(IThemeManager themeManager, IWpfCommandManager wpfCommandManager, IMenuManager menuManager, ITextEditorSettings textEditorSettings) {
			this.themeManager = themeManager;
			this.wpfCommandManager = wpfCommandManager;
			this.menuManager = menuManager;
			this.textEditorSettings = textEditorSettings;
		}

		public IReplEditorUI Create(ReplEditorOptions options) {
			return new ReplEditorUI(options, themeManager, wpfCommandManager, menuManager, textEditorSettings);
		}
	}
}
