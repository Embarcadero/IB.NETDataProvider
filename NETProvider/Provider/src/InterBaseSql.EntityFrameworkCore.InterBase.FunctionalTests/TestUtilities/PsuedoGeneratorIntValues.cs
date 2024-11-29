using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests.TestUtilities
{
	public class PsuedoGeneratorIntValues : ValueGenerator<int>
	{
		static Dictionary<string, int> values = new Dictionary<string, int>();

		public override bool GeneratesTemporaryValues => false;
		public override int Next(EntityEntry entry)
		{
			string eType = entry.Metadata.GetTableName();
			var baseType = entry.Metadata.BaseType;
			if (baseType != null)
				eType = baseType.GetTableName();
			//else
			//	eType = entry.Entity.GetType().GetTableName();
			if (!values.ContainsKey(eType))
				values.Add(eType, 0);
			values[eType] = values[eType] + 1;
			return values[eType];
		}

	}

	public class PsuedoGeneratorStringValues : ValueGenerator<string>
	{
		static Dictionary<string, int> values = new Dictionary<string, int>();

		public override bool GeneratesTemporaryValues => false;
		public override string Next(EntityEntry entry)
		{
			var eType = entry.Entity.GetType().Name;
			if (!values.ContainsKey(eType))
				values.Add(eType, 0);
			values[eType] = values[eType] + 1;
			return values[eType].ToString();
		}

	}
	public class PsuedoGeneratorGuidValues : ValueGenerator<Guid>
	{
		public override bool GeneratesTemporaryValues => false;
		public override Guid Next(EntityEntry entry)
		{
			return Guid.NewGuid();
		}

	}
}
