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
using System.Diagnostics;
using System.IO;
using System.Linq;
using dnSpy.Contracts.Scripting;
using dnSpy.Contracts.TextEditor;
using dnSpy.Scripting.Roslyn.Common;
using dnSpy.Scripting.Roslyn.Properties;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting.Hosting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.Hosting;

namespace dnSpy.Scripting.Roslyn.CSharp {
	sealed class CSharpControlVM : ScriptControlVM {
		protected override string Logo {
			get {
				// This is how MS gets the version, see roslyn/src/Interactive/EditorFeatures/CSharp/Interactive/CSharpReplServiceProvider.cs
				return string.Format("Microsoft (R) Roslyn C# Compiler version {0}",
					FileVersionInfo.GetVersionInfo(typeof(CSharpCommandLineArguments).Assembly.Location).FileVersion);
			}
		}

		protected override string Help {
			get { return dnSpy_Scripting_Roslyn_Resources.HelpString; }
		}

		protected override ObjectFormatter ObjectFormatter {
			get { return CSharpObjectFormatter.Instance; }
		}

		protected override DiagnosticFormatter DiagnosticFormatter {
			get { return CSharpDiagnosticFormatter.Instance; }
		}

		public CSharpControlVM(IReplEditor replEditor, IServiceLocator serviceLocator)
			: base(replEditor, serviceLocator) {
		}

		protected override Script<T> Create<T>(string code, ScriptOptions options, Type globalsType, InteractiveAssemblyLoader assemblyLoader) {
			return CSharpScript.Create<T>(code, options, globalsType, assemblyLoader);
		}

		protected override bool IsCompleteSubmission(string text) {
			return SyntaxFactory.IsCompleteSubmission(SyntaxFactory.ParseSyntaxTree(text, parseOptions));
		}
		// See roslyn/src/Interactive/EditorFeatures/CSharp/Interactive/CSharpInteractiveEvaluator.cs
		// Needs to be updated whenever the above file gets updated (eg when CSharp7 gets added)
		static readonly CSharpParseOptions parseOptions = new CSharpParseOptions(languageVersion: LanguageVersion.CSharp6, kind: SourceCodeKind.Script);

		protected override void InitializeUserScriptOptions(UserScriptOptions options) {
			var rspFile = GetResponseFile("CSharpInteractive.rsp");
			if (rspFile == null)
				return;
			this.replEditor.OutputPrintLine(string.Format(dnSpy_Scripting_Roslyn_Resources.LoadingContextFromFile, Path.GetFileName(rspFile)));

			foreach (var t in ResponseFileReader.Read(rspFile)) {
				switch (t.Item1.ToLowerInvariant()) {
				case "/r":
				case "/reference":
					options.References.AddRange(GetReferences(t.Item2));
					break;

				case "/u":
				case "/using":
				case "/usings":
				case "/import":
				case "/imports":
					options.Imports.AddRange(t.Item2.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
					break;

				case "/lib":
				case "/libpath":
				case "/libpaths":
					options.LibPaths.AddRange(GetReferencePaths(t.Item2));
					break;

				case "/loadpath":
				case "/loadpaths":
					options.LoadPaths.AddRange(GetReferencePaths(t.Item2));
					break;

				default:
					Debug.Fail(string.Format("Unknown option: '{0}'", t.Item1));
					break;
				}
			}
		}

		IEnumerable<string> GetReferencePaths(string s) {
			return s.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries).Select(a => RemoveQuotes(a));
		}

		IEnumerable<string> GetReferences(string s) {
			if (s.Length == 0)
				yield break;
			if (s[0] == '"') {
				yield return RemoveQuotes(s);
				yield break;
			}
			foreach (var x in s.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
				yield return x;
		}

		string RemoveQuotes(string s) {
			if (s.Length == 0 || s[0] != '"')
				return s;
			s = s.Substring(1);
			if (s.Length > 0 && s[s.Length - 1] == '"')
				s = s.Substring(0, s.Length - 1);
			return s;
		}
	}
}
