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
using dnSpy.Contracts.Languages;
using dnSpy.Contracts.TreeView;
using dnSpy.Decompiler.Shared;
using ICSharpCode.AvalonEdit.Highlighting;

namespace dnSpy.Contracts.Files.Tabs.TextEditor {
	/// <summary>
	/// <see cref="IDecompileNode"/> context
	/// </summary>
	public interface IDecompileNodeContext {
		/// <summary>
		/// Output to use
		/// </summary>
		ITextOutput Output { get; }

		/// <summary>
		/// Language to use
		/// </summary>
		ILanguage Language { get; }

		/// <summary>
		/// Gets the decompilation context
		/// </summary>
		DecompilationContext DecompilationContext { get; }

		/// <summary>
		/// Executes <paramref name="func"/> in the UI thread and waits for it to complete, then
		/// returns the result to the caller. This can be used to load the node's
		/// <see cref="ITreeNode.Children"/> property since it can only be loaded in the UI thread.
		/// </summary>
		/// <typeparam name="T">Return type</typeparam>
		/// <param name="func">Delegate to execute</param>
		/// <returns></returns>
		T ExecuteInUIThread<T>(Func<T> func);

		/// <summary>
		/// Sets the <see cref="IHighlightingDefinition"/> to use or null to use the default one.
		/// See also <see cref="HighlightingExtension"/>
		/// </summary>
		IHighlightingDefinition HighlightingDefinition { get; set; }

		/// <summary>
		/// Sets the file extension (including the period) to use or null to use the default one.
		/// See also <see cref="HighlightingDefinition"/>
		/// </summary>
		string HighlightingExtension { get; set; }
	}
}
