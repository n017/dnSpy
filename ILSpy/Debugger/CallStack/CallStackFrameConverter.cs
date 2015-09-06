﻿/*
    Copyright (C) 2014-2015 de4dot@gmail.com

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
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using dnSpy.TextView;

namespace dnSpy.Debugger.CallStack {
	sealed class CallStackFrameConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var vm = value as CallStackFrameVM;
			if (vm == null) {
				var vm2 = value as ICallStackFrameVM;
				Debug.Assert(vm2 != null);
				if (vm2 != null) {
					return new TextBlock {
						Text = vm2.Name,
						TextTrimming = TextTrimming.CharacterEllipsis,
					};
				}
				return string.Empty;
			}

			try {
				var gen = new SimpleHighlighter();
				vm.Frame.Write(new OutputConverter(gen.TextOutput), vm.TypePrinterFlags);
				var tb = gen.Create();
				tb.TextTrimming = TextTrimming.CharacterEllipsis;
				return tb;
			}
			catch (Exception ex) {
				Debug.Fail(ex.ToString());
			}

			if (value == null)
				return string.Empty;
			return value.ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}