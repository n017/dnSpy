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

using System.Collections.Generic;
using System.ComponentModel.Composition;
using dnSpy.Contracts.Settings.Dialog;
using dnSpy.Properties;
using dnSpy.Shared.Settings.Dialog;

namespace dnSpy.Files {
	[ExportSimpleAppOptionCreator(Guid = AppSettingsConstants.GUID_DYNTAB_MISC)]
	sealed class FileManagerOptionCreator : ISimpleAppOptionCreator {
		readonly FileManagerSettingsImpl fileManagerSettings;

		[ImportingConstructor]
		FileManagerOptionCreator(FileManagerSettingsImpl fileManagerSettings) {
			this.fileManagerSettings = fileManagerSettings;
		}

		public IEnumerable<ISimpleAppOption> Create() {
			yield return new SimpleAppOptionCheckBox(fileManagerSettings.UseMemoryMappedIO, (saveSettings, appRefreshSettings, newValue) => {
				if (!saveSettings)
					return;
				if (fileManagerSettings.UseMemoryMappedIO != newValue.Value) {
					fileManagerSettings.UseMemoryMappedIO = newValue.Value;
					if (!fileManagerSettings.UseMemoryMappedIO)
						appRefreshSettings.Add(AppSettingsConstants.DISABLE_MMAP);
				}
			}) {
				Order = AppSettingsConstants.ORDER_MISC_USEMMAPDIO,
				Text = dnSpy_Resources.Options_Misc_UseMmapdIO,
				ToolTip = dnSpy_Resources.Options_Misc_UseMmapdIO_ToolTip,
			};
		}
	}
}
