﻿// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System.Text;

namespace dnSpy.Decompiler.Shared {
	public interface ITextOutput {
		TextPosition Location { get; }

		void Indent();
		void Unindent();
		void Write(string text, TextTokenKind tokenKind);//TODO: Try to use one of the methods below
		void Write(string text, int index, int count, TextTokenKind tokenKind);
		void Write(StringBuilder sb, int index, int count, TextTokenKind tokenKind);
		void WriteLine();
		void WriteDefinition(string text, object definition, TextTokenKind tokenKind, bool isLocal = true);
		void WriteReference(string text, object reference, TextTokenKind tokenKind, bool isLocal = false);

		void AddDebugSymbols(MemberMapping methodDebugSymbols);
	}

	public static class TextOutputExtensions {
		public static void WriteLine(this ITextOutput output, string text, TextTokenKind tokenKind) {
			output.Write(text, tokenKind);
			output.WriteLine();
		}

		public static void WriteSpace(this ITextOutput output) {
			output.Write(" ", TextTokenKind.Text);
		}

		public static void WriteLineLeftBrace(this ITextOutput output) {
			output.Write("{", TextTokenKind.Brace);
			output.WriteLine();
		}

		public static void WriteLineRightBrace(this ITextOutput output) {
			output.Write("}", TextTokenKind.Brace);
			output.WriteLine();
		}

		public static void WriteLeftBrace(this ITextOutput output) {
			output.Write("{", TextTokenKind.Brace);
		}

		public static void WriteRightBrace(this ITextOutput output) {
			output.Write("}", TextTokenKind.Brace);
		}

		public static void WriteXmlDoc(this ITextOutput output, string text) {
			foreach (var kv in SimpleXmlParser.Parse(text))
				output.Write(kv.Key, kv.Value);
		}
	}
}
