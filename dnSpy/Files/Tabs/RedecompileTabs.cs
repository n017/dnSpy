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
using dnSpy.Contracts.App;
using dnSpy.Contracts.Files.Tabs;
using dnSpy.Contracts.Languages;
using dnSpy.Contracts.Plugin;

namespace dnSpy.Files.Tabs {
	[ExportAutoLoaded(LoadType = AutoLoadedLoadType.BeforePlugins)]
	sealed class RedecompileTabs : IAutoLoaded {
		readonly IFileTabManager fileTabManager;
		readonly IAppWindow appWindow;

		[ImportingConstructor]
		RedecompileTabs(IFileTabManager fileTabManager, ILanguageManager languageManager, IAppWindow appWindow) {
			this.fileTabManager = fileTabManager;
			this.appWindow = appWindow;
			languageManager.LanguageChanged += LanguageManager_LanguageChanged;
		}

		void LanguageManager_LanguageChanged(object sender, EventArgs e) {
			if (!appWindow.AppLoaded)
				return;
			var tab = fileTabManager.ActiveTab;
			if (tab == null)
				return;
			var langContent = tab.Content as ILanguageTabContent;
			if (langContent == null)
				return;
			var languageManager = (ILanguageManager)sender;
			if (langContent.Language == languageManager.Language)
				return;
			langContent.Language = languageManager.Language;
			fileTabManager.Refresh(new IFileTab[] { tab });
		}
	}
}
