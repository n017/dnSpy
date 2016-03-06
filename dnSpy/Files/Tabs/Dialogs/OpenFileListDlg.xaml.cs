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
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using dnSpy.Shared.Controls;
using dnSpy.Shared.MVVM;

namespace dnSpy.Files.Tabs.Dialogs {
	sealed partial class OpenFileListDlg : WindowBase {
		public OpenFileListDlg() {
			InitializeComponent();
			this.listView.SelectionChanged += ListView_SelectionChanged;
			this.listView.KeyDown += ListView_KeyDown;
			InputBindings.Add(new KeyBinding(new RelayCommand(a => searchBox.Focus()), Key.E, ModifierKeys.Control));
			InputBindings.Add(new KeyBinding(new RelayCommand(a => searchBox.Focus()), Key.F, ModifierKeys.Control));
		}

		void ListView_KeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.Delete && Keyboard.Modifiers == ModifierKeys.None) {
				var vm = DataContext as OpenFileListVM;
				if (vm != null && vm.CanRemove)
					vm.Remove();
				e.Handled = true;
				return;
			}
		}

		void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			var vm = DataContext as OpenFileListVM;
			if (vm != null)
				vm.SelectedItems = listView.SelectedItems.OfType<FileListVM>().ToArray();
		}

		public IEnumerable<FileListVM> SelectedItems {
			get {
				foreach (FileListVM vm in listView.SelectedItems)
					yield return vm;
			}
		}

		protected override void OnClosed(EventArgs e) {
			var id = DataContext as IDisposable;
			if (id != null)
				id.Dispose();
		}

		void listView_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
			if (!UIUtils.IsLeftDoubleClick<ListViewItem>(listView, e))
				return;
			this.ClickOK();
		}
	}
}
