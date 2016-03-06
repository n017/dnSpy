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
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace dnSpy.Plugin {
	sealed class PluginConfigReader {
		const string XML_ROOT_NAME = "plugin";
		const string OS_VERSION_SECT = "os-version";
		const string FRAMEWORK_VERSION_SECT = "framework-version";
		const string APP_VERSION_SECT = "version";

		/// <summary>
		/// Reads the plugin config file
		/// </summary>
		/// <param name="filename">Filename</param>
		/// <returns></returns>
		public static PluginConfig Read(string filename) {
			var config = new PluginConfig();
			if (!File.Exists(filename))
				return config;
			var doc = XDocument.Load(filename, LoadOptions.None);
			var root = doc.Root;
			if (root.Name == XML_ROOT_NAME)
				Read(root, config);
			return config;
		}

		static void Read(XElement root, PluginConfig config) {
			config.OSVersion = ReadVersion(root, OS_VERSION_SECT);
			config.FrameworkVersion = ReadVersion(root, FRAMEWORK_VERSION_SECT);
			config.AppVersion = ReadVersion(root, APP_VERSION_SECT);
		}

		static Version ReadVersion(XElement elem, string name) {
			var verElem = elem.Element(name);
			if (verElem == null)
				return null;
			var fn = verElem.FirstNode;
			if (fn == null || fn.NodeType != XmlNodeType.Text)
				return null;

			var s = ((XText)fn).Value;
			Version version;
			if (Version.TryParse(s, out version))
				return version;

			return null;
		}
	}
}
