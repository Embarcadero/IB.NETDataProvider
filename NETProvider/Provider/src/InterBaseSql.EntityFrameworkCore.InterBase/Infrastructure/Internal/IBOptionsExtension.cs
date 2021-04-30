/*
 *    The contents of this file are subject to the Initial
 *    Developer's Public License Version 1.0 (the "License");
 *    you may not use this file except in compliance with the
 *    License. You may obtain a copy of the License at
 *    https://github.com/FirebirdSQL/NETProvider/blob/master/license.txt.
 *
 *    Software distributed under the License is distributed on
 *    an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either
 *    express or implied. See the License for the specific
 *    language governing rights and limitations under the License.
 *
 *    The Initial Developer(s) of the Original Code are listed below.
 *    Portions created by Embarcadero are Copyright (C) Embarcadero.
 *
 *    All Rights Reserved.
 */

//$Authors = Jiri Cincura (jiri@cincura.net)

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace InterBaseSql.EntityFrameworkCore.InterBase.Infrastructure.Internal
{
	public class IBOptionsExtension : RelationalOptionsExtension
	{
		DbContextOptionsExtensionInfo _info;
		bool? _explicitParameterTypes;
		bool? _explicitStringLiteralTypes;

		public IBOptionsExtension()
		{ }

		public IBOptionsExtension(IBOptionsExtension copyFrom)
			: base(copyFrom)
		{
			_explicitParameterTypes = copyFrom._explicitParameterTypes;
			_explicitStringLiteralTypes = copyFrom._explicitStringLiteralTypes;
		}

		protected override RelationalOptionsExtension Clone()
			=> new IBOptionsExtension(this);

		public override void ApplyServices(IServiceCollection services)
			=> services.AddEntityFrameworkInterBase();

		public override DbContextOptionsExtensionInfo Info => _info ??= new ExtensionInfo(this);
		public virtual bool? ExplicitParameterTypes => _explicitParameterTypes;
		public virtual bool? ExplicitStringLiteralTypes => _explicitStringLiteralTypes;

		public virtual IBOptionsExtension WithExplicitParameterTypes(bool explicitParameterTypes)
		{
			var clone = (IBOptionsExtension)Clone();
			clone._explicitParameterTypes = explicitParameterTypes;
			return clone;
		}

		public virtual IBOptionsExtension WithExplicitStringLiteralTypes(bool explicitStringLiteralTypes)
		{
			var clone = (IBOptionsExtension)Clone();
			clone._explicitStringLiteralTypes = explicitStringLiteralTypes;
			return clone;
		}

		sealed class ExtensionInfo : RelationalExtensionInfo
		{
			long? _serviceProviderHash;

			public ExtensionInfo(IDbContextOptionsExtension extension)
				: base(extension)
			{ }

			new IBOptionsExtension Extension => (IBOptionsExtension)base.Extension;

			public override long GetServiceProviderHashCode()
			{
				return _serviceProviderHash ??= HashCode.Combine(base.GetServiceProviderHashCode(), Extension._explicitParameterTypes, Extension._explicitStringLiteralTypes);
			}

			public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
			{ }
		}
	}
}
