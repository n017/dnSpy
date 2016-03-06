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
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using dndbg.Engine;
using dnSpy.Contracts.App;
using dnSpy.Contracts.Files;
using dnSpy.Contracts.Files.Tabs;
using dnSpy.Contracts.Files.Tabs.TextEditor;
using dnSpy.Debugger.Properties;
using dnSpy.Decompiler.Shared;

namespace dnSpy.Debugger.Breakpoints {
	sealed class BreakpointListModifiedEventArgs : EventArgs {
		/// <summary>
		/// Added/removed breakpoint
		/// </summary>
		public Breakpoint Breakpoint { get; private set; }

		/// <summary>
		/// true if added, false if removed
		/// </summary>
		public bool Added { get; private set; }

		public BreakpointListModifiedEventArgs(Breakpoint bp, bool added) {
			this.Breakpoint = bp;
			this.Added = added;
		}
	}

	interface IBreakpointManager {
		Breakpoint[] Breakpoints { get; }
		ILCodeBreakpoint[] ILCodeBreakpoints { get; }
		event EventHandler<BreakpointListModifiedEventArgs> OnListModified;
		void Add(Breakpoint bp);
		void Remove(Breakpoint bp);
		void Clear();
		bool? GetAddRemoveBreakpointsInfo(out int count);
		bool GetEnableDisableBreakpointsInfo(out int count);
		void Toggle(ITextEditorUIContext uiContext, int line, int column = 0);
		Func<object, object> OnRemoveBreakpoints { get; set; }
	}

	[Export, Export(typeof(IBreakpointManager)), Export(typeof(ILoadBeforeDebug)), PartCreationPolicy(CreationPolicy.Shared)]
	sealed class BreakpointManager : IBreakpointManager, ILoadBeforeDebug {
		public event EventHandler<BreakpointListModifiedEventArgs> OnListModified;

		readonly HashSet<DebugEventBreakpoint> otherBreakpoints = new HashSet<DebugEventBreakpoint>();

		public Breakpoint[] Breakpoints {
			get {
				var bps = new List<Breakpoint>(textLineObjectManager.GetObjectsOfType<ILCodeBreakpoint>());
				bps.AddRange(otherBreakpoints);
				return bps.ToArray();
			}
		}

		public ILCodeBreakpoint[] ILCodeBreakpoints {
			get { return textLineObjectManager.GetObjectsOfType<ILCodeBreakpoint>(); }
		}

		public DebugEventBreakpoint[] DebugEventBreakpoints {
			get { return otherBreakpoints.ToArray(); }
		}

		readonly ITextLineObjectManager textLineObjectManager;
		readonly IFileTabManager fileTabManager;
		readonly ITheDebugger theDebugger;
		readonly IMessageBoxManager messageBoxManager;
		readonly ISerializedDnModuleCreator serializedDnModuleCreator;

		[ImportingConstructor]
		BreakpointManager(ITextLineObjectManager textLineObjectManager, IFileTabManager fileTabManager, ITheDebugger theDebugger, IMessageBoxManager messageBoxManager, ISerializedDnModuleCreator serializedDnModuleCreator) {
			this.textLineObjectManager = textLineObjectManager;
			this.fileTabManager = fileTabManager;
			this.theDebugger = theDebugger;
			this.messageBoxManager = messageBoxManager;
			this.serializedDnModuleCreator = serializedDnModuleCreator;
			textLineObjectManager.OnListModified += MarkedTextLinesManager_OnListModified;
			foreach (var bp in Breakpoints)
				InitializeDebuggerBreakpoint(bp);

			fileTabManager.FileCollectionChanged += FileTabManager_FileCollectionChanged;
			theDebugger.OnProcessStateChanged += TheDebugger_OnProcessStateChanged;
			if (theDebugger.IsDebugging)
				AddDebuggerBreakpoints();
		}

		void MarkedTextLinesManager_OnListModified(object sender, TextLineObjectListModifiedEventArgs e) {
			BreakPointAddedRemoved(e.TextLineObject as Breakpoint, e.Added);
		}

		void BreakPointAddedRemoved(Breakpoint bp, bool added) {
			if (bp == null)
				return;
			if (added) {
				InitializeDebuggerBreakpoint(bp);
				if (OnListModified != null)
					OnListModified(this, new BreakpointListModifiedEventArgs(bp, true));
			}
			else {
				UninitializeDebuggerBreakpoint(bp);
				if (OnListModified != null)
					OnListModified(this, new BreakpointListModifiedEventArgs(bp, false));
			}
		}

		public Func<object, object> OnRemoveBreakpoints { get; set; }
		void FileTabManager_FileCollectionChanged(object sender, NotifyFileCollectionChangedEventArgs e) {
			switch (e.Type) {
			case NotifyFileCollectionType.Clear:
			case NotifyFileCollectionType.Remove:
				var existing = new HashSet<SerializedDnModule>(fileTabManager.FileTreeView.GetAllModuleNodes().Select(a => a.DnSpyFile.ToSerializedDnModule()));
				var removed = new HashSet<SerializedDnModule>(e.Files.Select(a => a.ToSerializedDnModule()));
				existing.Remove(new SerializedDnModule());
				removed.Remove(new SerializedDnModule());
				object orbArg = null;
				if (OnRemoveBreakpoints != null)
					orbArg = OnRemoveBreakpoints(orbArg);
				foreach (var ilbp in ILCodeBreakpoints) {
					// Don't auto-remove BPs in dynamic modules since they have no disk file. The
					// user must delete these him/herself.
					if (ilbp.SerializedDnToken.Module.IsDynamic)
						continue;

					// If the file is still in the TV, don't delete anything. This can happen if
					// we've loaded an in-memory module and the node just got removed.
					if (existing.Contains(ilbp.SerializedDnToken.Module))
						continue;

					if (removed.Contains(ilbp.SerializedDnToken.Module))
						Remove(ilbp);
				}
				if (OnRemoveBreakpoints != null)
					OnRemoveBreakpoints(orbArg);
				break;

			case NotifyFileCollectionType.Add:
				break;
			}
		}

		void TheDebugger_OnProcessStateChanged(object sender, DebuggerEventArgs e) {
			switch (theDebugger.ProcessState) {
			case DebuggerProcessState.Starting:
				AddDebuggerBreakpoints();
				break;

			case DebuggerProcessState.Continuing:
			case DebuggerProcessState.Running:
			case DebuggerProcessState.Stopped:
				break;

			case DebuggerProcessState.Terminated:
				RemoveDebuggerBreakpoints();
				break;
			}
		}

		void AddDebuggerBreakpoints() {
			foreach (var bp in Breakpoints)
				InitializeDebuggerBreakpoint(bp);
		}

		void RemoveDebuggerBreakpoints() {
			foreach (var bp in Breakpoints)
				UninitializeDebuggerBreakpoint(bp);
		}

		void InitializeDebuggerBreakpoint(Breakpoint bp) {
			var debugger = theDebugger.Debugger;
			if (debugger == null || theDebugger.ProcessState == DebuggerProcessState.Terminated)
				return;

			IBreakpointCondition cond;
			switch (bp.Type) {
			case BreakpointType.ILCode:
				var ilbp = (ILCodeBreakpoint)bp;
				cond = AlwaysBreakpointCondition.Instance;//TODO: Let user pick what cond to use
				Debug.Assert(ilbp.DnBreakpoint == null);
				ilbp.DnBreakpoint = debugger.CreateBreakpoint(ilbp.SerializedDnToken.Module, ilbp.SerializedDnToken.Token, ilbp.ILOffset, cond);
				break;

			case BreakpointType.DebugEvent:
				//TODO:
				break;

			default:
				throw new InvalidOperationException();
			}
		}

		void UninitializeDebuggerBreakpoint(Breakpoint bp) {
			var dnbp = bp.DnBreakpoint;
			bp.DnBreakpoint = null;
			if (dnbp != null) {
				var dbg = theDebugger.Debugger;
				if (dbg != null)
					dbg.RemoveBreakpoint(dnbp);
			}
		}

		public void Add(Breakpoint bp) {
			var ilbp = bp as ILCodeBreakpoint;
			if (ilbp != null) {
				textLineObjectManager.Add(ilbp);
				return;
			}

			var debp = bp as DebugEventBreakpoint;
			if (debp != null) {
				otherBreakpoints.Add(debp);
				BreakPointAddedRemoved(debp, true);
				return;
			}
		}

		public void Remove(Breakpoint bp) {
			var ilbp = bp as ILCodeBreakpoint;
			if (ilbp != null) {
				textLineObjectManager.Remove(ilbp);
				return;
			}

			var debp = bp as DebugEventBreakpoint;
			if (debp != null) {
				otherBreakpoints.Remove(debp);
				BreakPointAddedRemoved(debp, false);
				return;
			}
		}

		public bool CanClear {
			get { return Breakpoints.Length != 0; }
		}

		public bool ClearAskUser() {
			var res = messageBoxManager.ShowIgnorableMessage(new Guid("37250D26-E844-49F4-904B-29600B90476C"), dnSpy_Debugger_Resources.AskDeleteAllBreakpoints, MsgBoxButton.Yes | MsgBoxButton.No);
			if (res != null && res != MsgBoxButton.Yes)
				return false;
			Clear();
			return true;
		}

		public void Clear() {
			foreach (var bp in Breakpoints)
				Remove(bp);
		}

		public bool CanToggleBreakpoint {
			get {
				var uiContext = fileTabManager.ActiveTab.TryGetTextEditorUIContext();
				return uiContext.GetCodeMappings().Count != 0;
			}
		}

		public bool ToggleBreakpoint() {
			if (!CanToggleBreakpoint)
				return false;

			var uiContext = fileTabManager.ActiveTab.TryGetTextEditorUIContext();
			if (uiContext == null)
				return false;
			var location = uiContext.Location;
			Toggle(uiContext, location.Line, location.Column);
			return true;
		}

		public bool? GetAddRemoveBreakpointsInfo(out int count) {
			count = 0;
			var uiContext = fileTabManager.ActiveTab.TryGetTextEditorUIContext();
			if (uiContext == null)
				return null;
			var location = uiContext.Location;
			var ilbps = GetILCodeBreakpoints(uiContext, location.Line, location.Column);
			count = ilbps.Count;
			if (ilbps.Count == 0)
				return null;
			return IsEnabled(ilbps);
		}

		public bool CanDisableBreakpoint {
			get {
				var uiContext = fileTabManager.ActiveTab.TryGetTextEditorUIContext();
				if (uiContext == null)
					return false;
				var location = uiContext.Location;
				return GetILCodeBreakpoints(uiContext, location.Line, location.Column).Count != 0;
			}
		}

		public bool DisableBreakpoint() {
			if (!CanDisableBreakpoint)
				return false;

			var uiContext = fileTabManager.ActiveTab.TryGetTextEditorUIContext();
			if (uiContext == null)
				return false;
			var location = uiContext.Location;
			var ilbps = GetILCodeBreakpoints(uiContext, location.Line, location.Column);
			bool isEnabled = IsEnabled(ilbps);
			foreach (var ilbp in ilbps)
				ilbp.IsEnabled = !isEnabled;
			return ilbps.Count > 0;
		}

		public bool GetEnableDisableBreakpointsInfo(out int count) {
			count = 0;
			var uiContext = fileTabManager.ActiveTab.TryGetTextEditorUIContext();
			if (uiContext == null)
				return false;
			var location = uiContext.Location;
			var ilbps = GetILCodeBreakpoints(uiContext, location.Line, location.Column);
			count = ilbps.Count;
			return IsEnabled(ilbps);
		}

		public bool CanDisableAllBreakpoints {
			get { return Breakpoints.Any(b => b.IsEnabled); }
		}

		public void DisableAllBreakpoints() {
			foreach (var bp in Breakpoints)
				bp.IsEnabled = false;
		}

		public bool CanEnableAllBreakpoints {
			get { return Breakpoints.Any(b => !b.IsEnabled); }
		}

		public void EnableAllBreakpoints() {
			foreach (var bp in Breakpoints)
				bp.IsEnabled = true;
		}

		static bool IsEnabled(IEnumerable<ILCodeBreakpoint> bps) {
			foreach (var bp in bps) {
				if (bp.IsEnabled)
					return true;
			}
			return false;
		}

		List<ILCodeBreakpoint> GetILCodeBreakpoints(ITextEditorUIContext uiContext, int line, int column) {
			return GetILCodeBreakpoints(uiContext, uiContext.GetCodeMappings().Find(line, column));
		}

		List<ILCodeBreakpoint> GetILCodeBreakpoints(ITextEditorUIContext uiContext, IList<SourceCodeMapping> mappings) {
			var list = new List<ILCodeBreakpoint>();
			if (mappings.Count == 0)
				return list;
			var mapping = mappings[0];
			foreach (var ilbp in ILCodeBreakpoints) {
				TextPosition location, endLocation;
				if (!ilbp.GetLocation(uiContext, out location, out endLocation))
					continue;
				if (location != mapping.StartPosition || endLocation != mapping.EndPosition)
					continue;

				list.Add(ilbp);
			}

			return list;
		}

		public void Toggle(ITextEditorUIContext uiContext, int line, int column = 0) {
			var bps = uiContext.GetCodeMappings().Find(line, column);
			var ilbps = GetILCodeBreakpoints(uiContext, bps);
			if (ilbps.Count > 0) {
				if (IsEnabled(ilbps)) {
					foreach (var ilbp in ilbps)
						Remove(ilbp);
				}
				else {
					foreach (var bpm in ilbps)
						bpm.IsEnabled = true;
				}
			}
			else if (bps.Count > 0) {
				foreach (var bp in bps) {
					var md = bp.Mapping.Method;
					var serMod = serializedDnModuleCreator.Create(md.Module);
					var key = new SerializedDnToken(serMod, md.MDToken);
					Add(new ILCodeBreakpoint(key, bp.ILRange.From));
				}
				uiContext.ScrollAndMoveCaretTo(bps[0].StartPosition.Line, bps[0].StartPosition.Column);
			}
		}
	}
}
