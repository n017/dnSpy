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

using dnlib.DotNet;
using dnSpy.AsmEditor.Properties;
using dnSpy.Contracts.Languages;

namespace dnSpy.AsmEditor.DnlibDialogs {
	sealed class SecurityAttributesVM : ListVM<SecurityAttributeVM, SecurityAttribute> {
		public SecurityAttributesVM(ModuleDef ownerModule, ILanguageManager languageManager, TypeDef ownerType, MethodDef ownerMethod)
			: base(dnSpy_AsmEditor_Resources.EditSecurityAttribute, dnSpy_AsmEditor_Resources.CreateSecurityAttribute, ownerModule, languageManager, ownerType, ownerMethod) {
		}

		protected override SecurityAttributeVM Create(SecurityAttribute model) {
			return new SecurityAttributeVM(model, ownerModule, languageManager, ownerType, ownerMethod);
		}

		protected override SecurityAttributeVM Clone(SecurityAttributeVM obj) {
			return new SecurityAttributeVM(obj.CreateSecurityAttribute(), ownerModule, languageManager, ownerType, ownerMethod);
		}

		protected override SecurityAttributeVM Create() {
			return new SecurityAttributeVM(new SecurityAttribute(), ownerModule, languageManager, ownerType, ownerMethod);
		}
	}
}
