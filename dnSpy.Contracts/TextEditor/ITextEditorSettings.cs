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

using System.ComponentModel;
using System.Windows.Media;

namespace dnSpy.Contracts.TextEditor {
	/// <summary>
	/// Settings used by all text editors
	/// </summary>
	public interface ITextEditorSettings : INotifyPropertyChanged {
		/// <summary>
		/// Font family
		/// </summary>
		FontFamily FontFamily { get; }

		/// <summary>
		/// Font size
		/// </summary>
		double FontSize { get; }

		/// <summary>
		/// true if line numbers should be shown
		/// </summary>
		bool ShowLineNumbers { get; }

		/// <summary>
		/// true if references are highlighted
		/// </summary>
		bool AutoHighlightRefs { get; }

		/// <summary>
		/// true if current line should be highlighted
		/// </summary>
		bool HighlightCurrentLine { get; }

		/// <summary>
		/// true if word wrapping is enabled
		/// </summary>
		bool WordWrap { get; }
	}
}
