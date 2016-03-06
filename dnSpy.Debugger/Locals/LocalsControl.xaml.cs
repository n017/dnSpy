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
using System.Windows.Controls;
using dnSpy.Contracts.Plugin;
using dnSpy.Contracts.Themes;
using ICSharpCode.TreeView;

namespace dnSpy.Debugger.Locals {
	sealed partial class LocalsControl : UserControl {
		public ListView ListView {
			get { return treeView; }
		}

		public LocalsControl() {
			InitializeComponent();
			InitializeTreeView(treeView);
		}

		[ExportAutoLoaded]
		sealed class ThemeManagerLoader : IAutoLoaded {
			[ImportingConstructor]
			ThemeManagerLoader(IThemeManager themeManager) {
				LocalsControl.themeManager = themeManager;
			}
		}
		static IThemeManager themeManager;

		static void InitializeTreeView(SharpTreeView treeView) {
			treeView.GetPreviewInsideTextBackground = () => themeManager.Theme.GetColor(ColorType.SystemColorsHighlight).Background;
			treeView.GetPreviewInsideForeground = () => themeManager.Theme.GetColor(ColorType.SystemColorsHighlightText).Foreground;
		}
	}
}
