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
using System.Collections.Generic;
using dnlib.DotNet;
using dnSpy.Contracts.Highlighting;
using dnSpy.Decompiler.Shared;
using ICSharpCode.AvalonEdit.Highlighting;

namespace dnSpy.Contracts.Languages {
	/// <summary>
	/// A language
	/// </summary>
	public interface ILanguage {
		/// <summary>
		/// Gets the settings
		/// </summary>
		IDecompilerSettings Settings { get; }

		/// <summary>
		/// Real name of the language, eg. "C#" if it's C#. See also <see cref="UniqueNameUI"/>.
		/// It's used when the real language name must be shown to the user.
		/// </summary>
		string GenericNameUI { get; }

		/// <summary>
		/// Language name shown to the user, and can contain extra info eg. "C# XYZ", see also
		/// <see cref="GenericNameUI"/>.
		/// </summary>
		string UniqueNameUI { get; }

		/// <summary>
		/// Order of language when shown to the user, eg. <see cref="LanguageConstants.CSHARP_ILSPY_ORDERUI"/>
		/// </summary>
		double OrderUI { get; }

		/// <summary>
		/// Language guid, eg. <see cref="LanguageConstants.LANGUAGE_CSHARP"/>, see also <see cref="UniqueGuid"/>
		/// </summary>
		Guid GenericGuid { get; }

		/// <summary>
		/// Unique language guid, see also <see cref="GenericGuid"/>
		/// </summary>
		Guid UniqueGuid { get; }

		/// <summary>
		/// File extension, eg. .cs, can't be null
		/// </summary>
		string FileExtension { get; }

		/// <summary>
		/// Project file extension, eg. .csproj or null if it's not supported
		/// </summary>
		string ProjectFileExtension { get; }

		/// <summary>
		/// Writes a type name
		/// </summary>
		/// <param name="output">Output</param>
		/// <param name="type">Type</param>
		void WriteName(ISyntaxHighlightOutput output, TypeDef type);

		/// <summary>
		/// Writes a property name
		/// </summary>
		/// <param name="output">Output</param>
		/// <param name="property">Type</param>
		/// <param name="isIndexer">true if it's an indexer</param>
		void WriteName(ISyntaxHighlightOutput output, PropertyDef property, bool? isIndexer);

		/// <summary>
		/// Writes a type name
		/// </summary>
		/// <param name="output">Output</param>
		/// <param name="type">Type</param>
		/// <param name="includeNamespace">true to include namespace</param>
		/// <param name="pd"><see cref="ParamDef"/> or null</param>
		void WriteType(ISyntaxHighlightOutput output, ITypeDefOrRef type, bool includeNamespace, ParamDef pd = null);

		/// <summary>
		/// Decompiles a method
		/// </summary>
		/// <param name="method">Method</param>
		/// <param name="output">Output</param>
		/// <param name="ctx">Context</param>
		void Decompile(MethodDef method, ITextOutput output, DecompilationContext ctx);

		/// <summary>
		/// Decompiles a property
		/// </summary>
		/// <param name="property">Property</param>
		/// <param name="output">Output</param>
		/// <param name="ctx">Context</param>
		void Decompile(PropertyDef property, ITextOutput output, DecompilationContext ctx);

		/// <summary>
		/// Decompiles a field
		/// </summary>
		/// <param name="field">Field</param>
		/// <param name="output">Output</param>
		/// <param name="ctx">Context</param>
		void Decompile(FieldDef field, ITextOutput output, DecompilationContext ctx);

		/// <summary>
		/// Decompiles an event
		/// </summary>
		/// <param name="ev">Event</param>
		/// <param name="output">Output</param>
		/// <param name="ctx">Context</param>
		void Decompile(EventDef ev, ITextOutput output, DecompilationContext ctx);

		/// <summary>
		/// Decompiles a type
		/// </summary>
		/// <param name="type">Type</param>
		/// <param name="output">Output</param>
		/// <param name="ctx">Context</param>
		void Decompile(TypeDef type, ITextOutput output, DecompilationContext ctx);

		/// <summary>
		/// Decompiles a namespace
		/// </summary>
		/// <param name="namespace">Namespace</param>
		/// <param name="types">Types in namespace</param>
		/// <param name="output">Output</param>
		/// <param name="ctx">Context</param>
		void DecompileNamespace(string @namespace, IEnumerable<TypeDef> types, ITextOutput output, DecompilationContext ctx);

		/// <summary>
		/// Decompiles an assembly
		/// </summary>
		/// <param name="asm">Assembly</param>
		/// <param name="output">Output</param>
		/// <param name="ctx">Context</param>
		void Decompile(AssemblyDef asm, ITextOutput output, DecompilationContext ctx);

		/// <summary>
		/// Decompiles a module
		/// </summary>
		/// <param name="mod">Module</param>
		/// <param name="output">Output</param>
		/// <param name="ctx">Context</param>
		void Decompile(ModuleDef mod, ITextOutput output, DecompilationContext ctx);

		/// <summary>
		/// Writes a tooltip
		/// </summary>
		/// <param name="output">Output</param>
		/// <param name="member">Member</param>
		/// <param name="typeAttributes">Type containing attributes, used to detect the dynamic types and out/ref params</param>
		void WriteToolTip(ISyntaxHighlightOutput output, IMemberRef member, IHasCustomAttribute typeAttributes);

		/// <summary>
		/// Writes a tooltip
		/// </summary>
		/// <param name="output">Output</param>
		/// <param name="variable">Local or argument</param>
		/// <param name="name">Name or null</param>
		void WriteToolTip(ISyntaxHighlightOutput output, IVariable variable, string name);

		/// <summary>
		/// Writes <paramref name="member"/> to <paramref name="output"/>
		/// </summary>
		/// <param name="output">Output</param>
		/// <param name="member">Member</param>
		/// <param name="flags">Flags</param>
		void Write(ISyntaxHighlightOutput output, IMemberRef member, SimplePrinterFlags flags);

		/// <summary>
		/// Writes a comment prefix
		/// </summary>
		/// <param name="output">Output</param>
		/// <param name="addSpace">true to add a space before the comment prefix</param>
		void WriteCommentBegin(ITextOutput output, bool addSpace);

		/// <summary>
		/// Writes a comment suffix
		/// </summary>
		/// <param name="output">Output</param>
		/// <param name="addSpace">true to add a space before the comment suffix (if it's written)</param>
		void WriteCommentEnd(ITextOutput output, bool addSpace);

		/// <summary>
		/// Returns true if the member is visible. Can be used to hide compiler generated types, methods etc
		/// </summary>
		/// <param name="member">Member</param>
		/// <returns></returns>
		bool ShowMember(IMemberRef member);

		/// <summary>
		/// Returns true if <paramref name="decompilationType"/> is supported and
		/// <see cref="Decompile(DecompilationType, object)"/> can be called.
		/// </summary>
		/// <param name="decompilationType">Decompilation type</param>
		/// <returns></returns>
		bool CanDecompile(DecompilationType decompilationType);

		/// <summary>
		/// Decompiles some data. Should only be called if <see cref="CanDecompile(DecompilationType)"/>
		/// returns true
		/// </summary>
		/// <param name="decompilationType">Decompilation type</param>
		/// <param name="data">Data, see <see cref="DecompilationType"/></param>
		void Decompile(DecompilationType decompilationType, object data);
	}

	/// <summary>
	/// Extension methods
	/// </summary>
	public static class LanguageExtensionMethods {
		/// <summary>
		/// Writes a comment and a new line
		/// </summary>
		/// <param name="self">This</param>
		/// <param name="output">Output</param>
		/// <param name="comment">Comment</param>
		public static void WriteCommentLine(this ILanguage self, ITextOutput output, string comment) {
			self.WriteCommentBegin(output, true);
			output.Write(comment, TextTokenKind.Comment);
			self.WriteCommentEnd(output, true);
			output.WriteLine();
		}

		/// <summary>
		/// Gets the <see cref="IHighlightingDefinition"/> instance to use for this language
		/// </summary>
		/// <param name="self">This</param>
		/// <returns></returns>
		public static IHighlightingDefinition GetHighlightingDefinition(this ILanguage self) {
			return HighlightingManager.Instance.GetDefinitionByExtension(self.FileExtension);
		}
	}
}
