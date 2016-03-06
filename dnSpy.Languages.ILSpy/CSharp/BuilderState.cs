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
using ICSharpCode.Decompiler.Ast;

namespace dnSpy.Languages.ILSpy.CSharp {
	/// <summary>
	/// Gets the <see cref="AstBuilderState"/> from the pool and returns it when <see cref="Dispose"/>
	/// gets called.
	/// </summary>
	struct BuilderState : IDisposable {
		public AstBuilder AstBuilder {
			get { return State.AstBuilder; }
		}

		public readonly AstBuilderState State;
		readonly DecompilationContext ctx;
		readonly BuilderCache cache;

		public BuilderState(DecompilationContext ctx, BuilderCache cache) {
			this.ctx = ctx;
			this.cache = cache;
			this.State = cache.AllocateAstBuilderState();
		}

		public void Dispose() {
			cache.Free(State);
		}
	}
}
