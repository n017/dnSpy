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
using dnlib.DotNet.MD;
using dnSpy.Contracts.Files.TreeView;
using dnSpy.Contracts.Highlighting;
using dnSpy.Decompiler.Shared;
using dnSpy.Shared.HexEditor;
using dnSpy.Shared.Highlighting;
using dnSpy.Shared.MVVM;

namespace dnSpy.AsmEditor.Hex.Nodes {
	sealed class MetaDataTableRecordNode : HexNode {
		public override Guid Guid {
			get { return new Guid(FileTVConstants.MDTBLREC_NODE_GUID); }
		}

		public override NodePathName NodePathName {
			get { return new NodePathName(Guid, index.ToString()); }
		}

		public override object VMObject {
			get { return Record; }
		}

		protected override IEnumerable<HexVM> HexVMs {
			get { yield return Record; }
		}

		protected override string IconName {
			get { return "MetaData"; }
		}

		MetaDataTableNode MDParent {
			get { return (MetaDataTableNode)TreeNode.Parent.Data; }
		}

		MetaDataTableRecordVM Record {
			get { return MDParent.MetaDataTableVM.Get(index); }
		}

		public override bool SingleClickExpandsChildren {
			get { return false; }
		}

		readonly int index;
		readonly Tuple<int[], Action<ISyntaxHighlightOutput>> infoTuple;

		public MetaDataTableRecordNode(TableInfo tableInfo, int index, ulong startOffset, ulong endOffset)
			: base(startOffset, endOffset) {
			this.index = index;
			this.infoTuple = GetInfoTuple(tableInfo);
		}

		protected override void Write(ISyntaxHighlightOutput output) {
			output.Write(string.Format("{0}", index + 1), TextTokenKind.Number);
			if (infoTuple != null) {
				output.WriteSpace();
				output.Write("-", TextTokenKind.Operator);
				output.WriteSpace();
				infoTuple.Item2(output);
			}
		}

		public override void OnDocumentModified(ulong modifiedStart, ulong modifiedEnd) {
			if (infoTuple != null) {
				var tableInfo = ((MetaDataTableNode)TreeNode.Parent.Data).TableInfo;
				foreach (var index in infoTuple.Item1) {
					var col = tableInfo.Columns[index];
					ulong start = StartOffset + (ulong)col.Offset;
					if (HexUtils.IsModified(start, start + (ulong)col.Size - 1, modifiedStart, modifiedEnd)) {
						TreeNode.RefreshUI();
						break;
					}
				}
			}
		}

		Tuple<int[], Action<ISyntaxHighlightOutput>> GetInfoTuple(TableInfo tableInfo) {
			switch (tableInfo.Table) {
			case Table.Module:					return new Tuple<int[], Action<ISyntaxHighlightOutput>>(new int[] { 1 }, WriteModuleInfo);
			case Table.TypeRef:					return new Tuple<int[], Action<ISyntaxHighlightOutput>>(new int[] { 1, 2 }, WriteTypeRefInfo);
			case Table.TypeDef:					return new Tuple<int[], Action<ISyntaxHighlightOutput>>(new int[] { 1, 2 }, WriteTypeDefInfo);
			case Table.FieldPtr:				return null;
			case Table.Field:					return new Tuple<int[], Action<ISyntaxHighlightOutput>>(new int[] { 1 }, WriteFieldInfo);
			case Table.MethodPtr:				return null;
			case Table.Method:					return new Tuple<int[], Action<ISyntaxHighlightOutput>>(new int[] { 3 }, WriteMethodInfo);
			case Table.ParamPtr:				return null;
			case Table.Param:					return new Tuple<int[], Action<ISyntaxHighlightOutput>>(new int[] { 2 }, WriteParamInfo);
			case Table.InterfaceImpl:			return null;
			case Table.MemberRef:				return new Tuple<int[], Action<ISyntaxHighlightOutput>>(new int[] { 1 }, WriteMemberRefInfo);
			case Table.Constant:				return null;
			case Table.CustomAttribute:			return null;
			case Table.FieldMarshal:			return null;
			case Table.DeclSecurity:			return null;
			case Table.ClassLayout:				return null;
			case Table.FieldLayout:				return null;
			case Table.StandAloneSig:			return null;
			case Table.EventMap:				return null;
			case Table.EventPtr:				return null;
			case Table.Event:					return new Tuple<int[], Action<ISyntaxHighlightOutput>>(new int[] { 1 }, WriteEventInfo);
			case Table.PropertyMap:				return null;
			case Table.PropertyPtr:				return null;
			case Table.Property:				return new Tuple<int[], Action<ISyntaxHighlightOutput>>(new int[] { 1 }, WritePropertyInfo);
			case Table.MethodSemantics:			return null;
			case Table.MethodImpl:				return null;
			case Table.ModuleRef:				return new Tuple<int[], Action<ISyntaxHighlightOutput>>(new int[] { 0 }, WriteModuleRefInfo);
			case Table.TypeSpec:				return null;
			case Table.ImplMap:					return new Tuple<int[], Action<ISyntaxHighlightOutput>>(new int[] { 2 }, WriteImplMapInfo);
			case Table.FieldRVA:				return null;
			case Table.ENCLog:					return null;
			case Table.ENCMap:					return null;
			case Table.Assembly:				return new Tuple<int[], Action<ISyntaxHighlightOutput>>(new int[] { 7 }, WriteAssemblyInfo);
			case Table.AssemblyProcessor:		return null;
			case Table.AssemblyOS:				return null;
			case Table.AssemblyRef:				return new Tuple<int[], Action<ISyntaxHighlightOutput>>(new int[] { 6 }, WriteAssemblyRefInfo);
			case Table.AssemblyRefProcessor:	return null;
			case Table.AssemblyRefOS:			return null;
			case Table.File:					return new Tuple<int[], Action<ISyntaxHighlightOutput>>(new int[] { 1 }, WriteFileInfo);
			case Table.ExportedType:			return new Tuple<int[], Action<ISyntaxHighlightOutput>>(new int[] { 2, 3 }, WriteExportedTypeInfo);
			case Table.ManifestResource:		return new Tuple<int[], Action<ISyntaxHighlightOutput>>(new int[] { 2 }, WriteManifestResourceInfo);
			case Table.NestedClass:				return null;
			case Table.GenericParam:			return new Tuple<int[], Action<ISyntaxHighlightOutput>>(new int[] { 3 }, WriteGenericParamInfo);
			case Table.MethodSpec:				return null;
			case Table.GenericParamConstraint:	return null;
			default:							throw new InvalidOperationException();
			}
		}

		string ReadStringsHeap(int index) {
			var mdt = (MetaDataTableNode)TreeNode.Parent.Data;
			var tableInfo = mdt.TableInfo;
			var s = NumberVMUtils.ToString(mdt.MetaDataTableVM.ReadStringsHeap(ReadFieldValue(mdt.Document, tableInfo.Columns[index])), false);
			Debug.Assert(s.Length >= 2);
			if (s.Length < 2)
				return s;
			return s.Substring(1, s.Length - 2);
		}

		uint ReadFieldValue(HexDocument doc, ColumnInfo col) {
			ulong start = StartOffset + (ulong)col.Offset;
			if (col.Size == 2)
				return doc.ReadUInt16(start);
			else if (col.Size == 4)
				return doc.ReadUInt32(start);
			throw new InvalidOperationException();
		}

		void WriteModuleInfo(ISyntaxHighlightOutput output) {
			output.Write(ReadStringsHeap(1), TextTokenKind.Module);
		}

		void WriteNamespaceName(ISyntaxHighlightOutput output, int nsIndex, int nameIndex) {
			var ns = ReadStringsHeap(nsIndex);
			var name = ReadStringsHeap(nameIndex);

			output.Write(name, TextTokenKind.Type);

			if (!string.IsNullOrEmpty(ns)) {
				output.WriteSpace();
				output.Write("-", TextTokenKind.Operator);
				output.WriteSpace();

				var parts = ns.Split('.');
				for (int i = 0; i < parts.Length; i++) {
					output.Write(parts[i], TextTokenKind.NamespacePart);
					if (i + 1 < parts.Length)
						output.Write(".", TextTokenKind.Operator);
				}
			}
		}

		void WriteTypeRefInfo(ISyntaxHighlightOutput output) {
			WriteNamespaceName(output, 2, 1);
		}

		void WriteTypeDefInfo(ISyntaxHighlightOutput output) {
			WriteNamespaceName(output, 2, 1);
		}

		void WriteFieldInfo(ISyntaxHighlightOutput output) {
			output.Write(ReadStringsHeap(1), TextTokenKind.InstanceField);
		}

		void WriteMethodInfo(ISyntaxHighlightOutput output) {
			output.Write(ReadStringsHeap(3), TextTokenKind.InstanceMethod);
		}

		void WriteParamInfo(ISyntaxHighlightOutput output) {
			output.Write(ReadStringsHeap(2), TextTokenKind.Parameter);
		}

		void WriteMemberRefInfo(ISyntaxHighlightOutput output) {
			output.Write(ReadStringsHeap(1), TextTokenKind.InstanceMethod);
		}

		void WriteEventInfo(ISyntaxHighlightOutput output) {
			output.Write(ReadStringsHeap(1), TextTokenKind.InstanceEvent);
		}

		void WritePropertyInfo(ISyntaxHighlightOutput output) {
			output.Write(ReadStringsHeap(1), TextTokenKind.InstanceProperty);
		}

		void WriteModuleRefInfo(ISyntaxHighlightOutput output) {
			output.Write(ReadStringsHeap(0), TextTokenKind.Module);
		}

		void WriteImplMapInfo(ISyntaxHighlightOutput output) {
			output.Write(ReadStringsHeap(2), TextTokenKind.InstanceMethod);
		}

		void WriteAssemblyInfo(ISyntaxHighlightOutput output) {
			output.Write(ReadStringsHeap(7), TextTokenKind.Assembly);
		}

		void WriteAssemblyRefInfo(ISyntaxHighlightOutput output) {
			output.Write(ReadStringsHeap(6), TextTokenKind.Assembly);
		}

		void WriteFileInfo(ISyntaxHighlightOutput output) {
			output.WriteFilename(ReadStringsHeap(1));
		}

		void WriteExportedTypeInfo(ISyntaxHighlightOutput output) {
			WriteNamespaceName(output, 3, 2);
		}

		void WriteManifestResourceInfo(ISyntaxHighlightOutput output) {
			output.WriteFilename(ReadStringsHeap(2));
		}

		void WriteGenericParamInfo(ISyntaxHighlightOutput output) {
			output.Write(ReadStringsHeap(3), TextTokenKind.TypeGenericParameter);
		}
	}
}
