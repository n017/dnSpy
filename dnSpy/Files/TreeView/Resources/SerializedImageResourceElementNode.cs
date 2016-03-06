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
using System.IO;
using System.Windows.Media;
using dnlib.DotNet;
using dnlib.DotNet.Resources;
using dnSpy.Contracts.Files.TreeView;
using dnSpy.Contracts.Files.TreeView.Resources;
using dnSpy.Contracts.Images;
using dnSpy.Contracts.Languages;
using dnSpy.Contracts.TreeView;
using dnSpy.Decompiler.Shared;
using dnSpy.Properties;
using dnSpy.Shared.Decompiler;
using dnSpy.Shared.Files.TreeView.Resources;

namespace dnSpy.Files.TreeView.Resources {
	[ExportResourceNodeCreator(Order = FileTVConstants.ORDER_RSRCCREATOR_SERIALIZED_IMAGE_RESOURCE_ELEMENT_NODE)]
	sealed class SerializedImageResourceElementNodeCreator : IResourceNodeCreator {
		public IResourceNode Create(ModuleDef module, Resource resource, ITreeNodeGroup treeNodeGroup) {
			return null;
		}

		public IResourceElementNode Create(ModuleDef module, ResourceElement resourceElement, ITreeNodeGroup treeNodeGroup) {
			var serializedData = resourceElement.ResourceData as BinaryResourceData;
			if (serializedData == null)
				return null;

			byte[] imageData;
			if (SerializedImageUtils.GetImageData(module, serializedData.TypeName, serializedData.Data, out imageData))
				return new SerializedImageResourceElementNode(treeNodeGroup, resourceElement, imageData);

			return null;
		}
	}

	sealed class SerializedImageResourceElementNode : ResourceElementNode, ISerializedImageResourceElementNode {
		public ImageSource ImageSource {
			get { return imageSource; }
		}
		ImageSource imageSource;
		byte[] imageData;

		public override Guid Guid {
			get { return new Guid(FileTVConstants.SERIALIZED_IMAGE_RESOURCE_ELEMENT_NODE); }
		}

		protected override ImageReference GetIcon() {
			return new ImageReference(GetType().Assembly, "ImageFile");
		}

		public SerializedImageResourceElementNode(ITreeNodeGroup treeNodeGroup, ResourceElement resourceElement, byte[] imageData)
			: base(treeNodeGroup, resourceElement) {
			InitializeImageData(imageData);
		}

		void InitializeImageData(byte[] imageData) {
			this.imageData = imageData;
			this.imageSource = ImageResourceUtils.CreateImageSource(imageData);
		}

		public override void WriteShort(ITextOutput output, ILanguage language, bool showOffset) {
			var smartOutput = output as ISmartTextOutput;
			if (smartOutput != null) {
				smartOutput.AddUIElement(() => {
					return new System.Windows.Controls.Image {
						Source = ImageSource,
					};
				});
			}

			base.WriteShort(output, language, showOffset);
		}

		protected override IEnumerable<ResourceData> GetDeserializedData() {
			var id = imageData;
			yield return new ResourceData(ResourceElement.Name, token => new MemoryStream(id));
		}

		public ResourceElement GetAsRawImage() {
			return new ResourceElement {
				Name = ResourceElement.Name,
				ResourceData = new BuiltInResourceData(ResourceTypeCode.ByteArray, imageData),
			};
		}

		public override string CheckCanUpdateData(ResourceElement newResElem) {
			var res = base.CheckCanUpdateData(newResElem);
			if (!string.IsNullOrEmpty(res))
				return res;

			var binData = (BinaryResourceData)newResElem.ResourceData;
			byte[] imageData;
			if (!SerializedImageUtils.GetImageData(this.GetModule(), binData.TypeName, binData.Data, out imageData))
				return dnSpy_Resources.NewDataIsNotAnImage;

			try {
				ImageResourceUtils.CreateImageSource(imageData);
			}
			catch {
				return dnSpy_Resources.NewDataIsNotAnImage;
			}

			return string.Empty;
		}

		public override void UpdateData(ResourceElement newResElem) {
			base.UpdateData(newResElem);

			var binData = (BinaryResourceData)newResElem.ResourceData;
			byte[] imageData;
			SerializedImageUtils.GetImageData(this.GetModule(), binData.TypeName, binData.Data, out imageData);
			InitializeImageData(imageData);
		}
	}
}
