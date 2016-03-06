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

namespace dnSpy.Contracts.Files.Tabs {
	/// <summary>
	/// Passed to <see cref="IFileTabContent.OnShow(IShowContext)"/>
	/// </summary>
	public interface IShowContext {
		/// <summary>
		/// UI Context created by <see cref="IFileTabContent.CreateUIContext(IFileTabUIContextLocator)"/>
		/// </summary>
		IFileTabUIContext UIContext { get; }

		/// <summary>
		/// true if the view is refreshed
		/// </summary>
		bool IsRefresh { get; }

		/// <summary>
		/// If non-null, gets called after the content has been shown
		/// </summary>
		Action<ShowTabContentEventArgs> OnShown { get; set; }

		/// <summary>
		/// Can be initialized by the <see cref="IFileTabContent"/> instance
		/// </summary>
		object UserData { get; set; }
	}
}
