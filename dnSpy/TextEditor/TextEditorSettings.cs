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
using System.ComponentModel.Composition;
using System.Windows.Media;
using dnSpy.Contracts.Settings;
using dnSpy.Contracts.TextEditor;
using dnSpy.Shared.Controls;
using dnSpy.Shared.MVVM;

namespace dnSpy.TextEditor {
	class TextEditorSettings : ViewModelBase, ITextEditorSettings {
		protected virtual void OnModified() {
		}

		public FontFamily FontFamily {
			get { return fontFamily; }
			set {
				if (fontFamily.Source != value.Source) {
					fontFamily = value;
					OnPropertyChanged("FontFamily");
					OnModified();
				}
			}
		}
		FontFamily fontFamily = new FontFamily(FontUtils.GetDefaultMonospacedFont());

		public double FontSize {
			get { return fontSize; }
			set {
				if (fontSize != value) {
					fontSize = FontUtils.FilterFontSize(value);
					OnPropertyChanged("FontSize");
					OnModified();
				}
			}
		}
		double fontSize = FontUtils.DEFAULT_FONT_SIZE;

		public bool ShowLineNumbers {
			get { return showLineNumbers; }
			set {
				if (showLineNumbers != value) {
					showLineNumbers = value;
					OnPropertyChanged("ShowLineNumbers");
					OnModified();
				}
			}
		}
		bool showLineNumbers = true;

		public bool AutoHighlightRefs {
			get { return autoHighlightRefs; }
			set {
				if (autoHighlightRefs != value) {
					autoHighlightRefs = value;
					OnPropertyChanged("AutoHighlightRefs");
					OnModified();
				}
			}
		}
		bool autoHighlightRefs = true;

		public bool HighlightCurrentLine {
			get { return highlightCurrentLine; }
			set {
				if (highlightCurrentLine != value) {
					highlightCurrentLine = value;
					OnPropertyChanged("HighlightCurrentLine");
					OnModified();
				}
			}
		}
		bool highlightCurrentLine = true;

		public bool WordWrap {
			get { return wordWrap; }
			set {
				if (wordWrap != value) {
					wordWrap = value;
					OnPropertyChanged("WordWrap");
					OnModified();
				}
			}
		}
		bool wordWrap = false;

		public TextEditorSettings Clone() {
			return CopyTo(new TextEditorSettings());
		}

		public TextEditorSettings CopyTo(TextEditorSettings other) {
			other.FontFamily = this.FontFamily;
			other.FontSize = this.FontSize;
			other.ShowLineNumbers = this.ShowLineNumbers;
			other.AutoHighlightRefs = this.AutoHighlightRefs;
			other.HighlightCurrentLine = this.HighlightCurrentLine;
			other.WordWrap = this.WordWrap;
			return other;
		}
	}

	[Export, Export(typeof(ITextEditorSettings)), PartCreationPolicy(CreationPolicy.Shared)]
	sealed class TextEditorSettingsImpl : TextEditorSettings {
		static readonly Guid SETTINGS_GUID = new Guid("9D40E1AD-5922-4BBA-B386-E6BABE5D185D");

		readonly ISettingsManager settingsManager;

		[ImportingConstructor]
		TextEditorSettingsImpl(ISettingsManager settingsManager) {
			this.settingsManager = settingsManager;

			this.disableSave = true;
			var sect = settingsManager.GetOrCreateSection(SETTINGS_GUID);
			this.FontFamily = new FontFamily(sect.Attribute<string>("FontFamily") ?? FontUtils.GetDefaultMonospacedFont());
			this.FontSize = sect.Attribute<double?>("FontSize") ?? this.FontSize;
			this.ShowLineNumbers = sect.Attribute<bool?>("ShowLineNumbers") ?? this.ShowLineNumbers;
			this.AutoHighlightRefs = sect.Attribute<bool?>("AutoHighlightRefs") ?? this.AutoHighlightRefs;
			this.HighlightCurrentLine = sect.Attribute<bool?>("HighlightCurrentLine") ?? this.HighlightCurrentLine;
			this.WordWrap = sect.Attribute<bool?>("WordWrap") ?? this.WordWrap;
			this.disableSave = false;
		}
		readonly bool disableSave;

		protected override void OnModified() {
			if (disableSave)
				return;
			var sect = settingsManager.RecreateSection(SETTINGS_GUID);
			sect.Attribute("FontFamily", FontFamily.Source);
			sect.Attribute("FontSize", FontSize);
			sect.Attribute("ShowLineNumbers", ShowLineNumbers);
			sect.Attribute("AutoHighlightRefs", AutoHighlightRefs);
			sect.Attribute("HighlightCurrentLine", HighlightCurrentLine);
			sect.Attribute("WordWrap", WordWrap);
		}
	}
}
