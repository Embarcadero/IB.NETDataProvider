/*
 *    The contents of this file are subject to the Initial
 *    Developer's Public License Version 1.0 (the "License");
 *    you may not use this file except in compliance with the
 *    License. You may obtain a copy of the License at
 *    https://github.com/FirebirdSQL/NETProvider/raw/master/license.txt.
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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using InterBaseSql.EntityFrameworkCore.InterBase.Metadata;
using InterBaseSql.EntityFrameworkCore.InterBase.Metadata.Internal;
using InterBaseSql.EntityFrameworkCore.InterBase.Storage.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using NUnit.Framework;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace InterBaseSql.EntityFrameworkCore.InterBase.Tests.Migrations;

#pragma warning disable EF1001
public class MigrationsTests : EntityFrameworkCoreTestsBase
{
	[Test]
	public void CreateTable()
	{
		var operation = new CreateTableOperation
		{
			Name = "People",
			Columns =
			{
					new AddColumnOperation
					{
						Name = "Id",
						Table = "People",
						ClrType = typeof(int),
						IsNullable = false,
						[IBAnnotationNames.ValueGenerationStrategy] = IBValueGenerationStrategy.None,
					},
					new AddColumnOperation
					{
						Name = "Id_Sequence",
						Table = "People",
						ClrType = typeof(int),
						IsNullable = false,
						[IBAnnotationNames.ValueGenerationStrategy] = IBValueGenerationStrategy.SequenceTrigger,
					},
					new AddColumnOperation
					{
						Name = "EmployerId",
						Table = "People",
						ClrType = typeof(int),
						IsNullable = true,
					},
					new AddColumnOperation
					{
						Name = "SSN",
						Table = "People",
						ClrType = typeof(string),
						ColumnType = "char(11)",
						IsNullable = true,
					},
					new AddColumnOperation
					{
						Name = "DEF_O",
						Table = "People",
						ClrType = typeof(string),
						MaxLength = 20,
						DefaultValue = "test",
						IsNullable = true,
					},
					new AddColumnOperation
					{
						Name = "DEF_S",
						Table = "People",
						ClrType = typeof(string),
						MaxLength = 20,
						DefaultValueSql = "'x'",
						IsNullable = true,
					},
			},
			PrimaryKey = new AddPrimaryKeyOperation
			{
				Columns = new[] { "Id" },
			},
			UniqueConstraints =
			{
					new AddUniqueConstraintOperation
					{
						Columns = new[] { "SSN" },
					},
			},
			ForeignKeys =
			{
					new AddForeignKeyOperation
					{
						Columns = new[] { "EmployerId" },
						PrincipalTable = "Companies",
						PrincipalColumns = new[] { "Id" },
					},
			},
		};
		var expectedCreateTable = @"CREATE TABLE ""People"" (
    ""Id"" INTEGER NOT NULL,
    ""Id_Sequence"" INTEGER NOT NULL,
    ""EmployerId"" INTEGER,
    ""SSN"" char(11),
    ""DEF_O"" VARCHAR(20) DEFAULT 'test',
    ""DEF_S"" VARCHAR(20) DEFAULT 'x',
    PRIMARY KEY (""Id""),
    UNIQUE (""SSN""),
    FOREIGN KEY (""EmployerId"") REFERENCES ""Companies"" (""Id"") ON UPDATE NO ACTION ON DELETE NO ACTION
);";
		var batch = Generate(new[] { operation });
		Assert.AreEqual(5, batch.Count());
		Assert.AreEqual(NewLineEnd(expectedCreateTable), batch[0].CommandText);
		StringAssert.Contains("execute statement 'create generator GEN_IDENTITY';", batch[1].CommandText);
		StringAssert.StartsWith("CREATE TRIGGER ", batch[4].CommandText);
	}

	[Test]
	public void CreateTableWithIdentity()
	{
		var operation = new CreateTableOperation
		{
			Name = "People",
			Columns =
			{
					new AddColumnOperation
					{
						Name = "Id_Identity",
						Table = "People",
						ClrType = typeof(int),
						IsNullable = false,
						[IBAnnotationNames.ValueGenerationStrategy] = IBValueGenerationStrategy.IdentityColumn,
					}
			}
		};
		Assert.Throws<NotSupportedInInterBase>(() => Generate(new[] { operation }));
	}

	[Test]
	public async Task CreateTableScript()
	{
		var operation = new CreateTableOperation
		{
			Name = "People",
			Columns =
				{
						new AddColumnOperation
						{
							Name = "Id",
							Table = "People",
							ClrType = typeof(int),
							IsNullable = false,
							[IBAnnotationNames.ValueGenerationStrategy] = IBValueGenerationStrategy.None,
						},
						new AddColumnOperation
						{
							Name = "Id_Sequence",
							Table = "People",
							ClrType = typeof(int),
							IsNullable = false,
							[IBAnnotationNames.ValueGenerationStrategy] = IBValueGenerationStrategy.SequenceTrigger,
						},
						new AddColumnOperation
						{
							Name = "EmployerId",
							Table = "People",
							ClrType = typeof(int),
							IsNullable = true,
						},
						new AddColumnOperation
						{
							Name = "SSN",
							Table = "People",
							ClrType = typeof(string),
							ColumnType = "char(11)",
							IsNullable = true,
						},
						new AddColumnOperation
						{
							Name = "DEF_O",
							Table = "People",
							ClrType = typeof(string),
							MaxLength = 20,
							DefaultValue = "test",
							IsNullable = true,
						},
						new AddColumnOperation
						{
							Name = "DEF_S",
							Table = "People",
							ClrType = typeof(string),
							MaxLength = 20,
							DefaultValueSql = "'x'",
							IsNullable = true,
						},
						new AddColumnOperation
						{
							Name = "COLLA",
							Table = "People",
							ClrType = typeof(string),
							MaxLength = 20,
							IsNullable = true,
							Collation = "UNICODE_CI_AI"
						},
				},
			PrimaryKey = new AddPrimaryKeyOperation
			{
				Columns = new[] { "Id" },
			},
			UniqueConstraints =
				{
						new AddUniqueConstraintOperation
						{
							Columns = new[] { "SSN" },
						},
				},
			ForeignKeys =
				{
						new AddForeignKeyOperation
						{
							Columns = new[] { "EmployerId" },
							PrincipalTable = "Companies",
							PrincipalColumns = new[] { "Id" },
						},
				},
		};
		var expectedCreateTable = @"CREATE TABLE ""People"" (
    ""Id"" INTEGER NOT NULL,
    ""Id_Sequence"" INTEGER NOT NULL,
    ""EmployerId"" INTEGER,
    ""SSN"" char(11),
    ""DEF_O"" VARCHAR(20) DEFAULT 'test',
    ""DEF_S"" VARCHAR(20) DEFAULT 'x',
    ""COLLA"" VARCHAR(20) COLLATE UNICODE_CI_AI,
    PRIMARY KEY (""Id""),
    UNIQUE (""SSN""),
    FOREIGN KEY (""EmployerId"") REFERENCES ""Companies"" (""Id"") ON UPDATE NO ACTION ON DELETE NO ACTION
);";
		var batch = Generate(new[] { operation }, MigrationsSqlGenerationOptions.Script);
		Assert.AreEqual(7, batch.Count());
		Assert.AreEqual(NewLineEnd(expectedCreateTable), batch[0].CommandText);
		Assert.AreEqual(NewLineEnd("SET TERM ~;"), batch[1].CommandText);
		StringAssert.Contains("rdb$generator_name = ", batch[2].CommandText);
		StringAssert.StartsWith("CREATE TRIGGER ", batch[5].CommandText);
		Assert.AreEqual(NewLineEnd("SET TERM ;~"), batch[6].CommandText);
	}

	[Test]
	public void DropTable()
	{
		var operation = new DropTableOperation()
		{
			Name = "People",
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(1, batch.Count());
		Assert.AreEqual(NewLineEnd(@"DROP TABLE ""People"";"), batch[0].CommandText);
	}

	[Test]
	public void AddColumn()
	{
		var operation = new AddColumnOperation()
		{
			Table = "People",
			Name = "NewColumn",
			ClrType = typeof(decimal),
			Schema = "schema",
			IsNullable = false,
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(1, batch.Count());
		Assert.AreEqual(NewLineEnd(@"ALTER TABLE ""schema"".""People"" ADD ""NewColumn"" DECIMAL(18,2) NOT NULL;"), batch[0].CommandText);
	}

	[Test]
	public void AddColumnWithCollation()
	{
		var operation = new AddColumnOperation()
		{
			Table = "People",
			Name = "NewColumn",
			ClrType = typeof(string),
			MaxLength = 10,
			IsNullable = false,
			Collation = "UNICODE_CI_AI",
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(1, batch.Count());
		Assert.AreEqual(NewLineEnd(@"ALTER TABLE ""People"" ADD ""NewColumn"" VARCHAR(10) COLLATE UNICODE_CI_AI NOT NULL;"), batch[0].CommandText);
	}

	[Test]
	public void DropColumn()
	{
		var operation = new DropColumnOperation()
		{
			Table = "People",
			Name = "DropMe",
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(1, batch.Count());
		Assert.AreEqual(NewLineEnd(@"ALTER TABLE ""People"" DROP ""DropMe"";"), batch[0].CommandText);
	}

	[Test]
	public void AlterColumnLength()
	{
		var operation = new AlterColumnOperation()
		{
			Table = "People",
			Name = "Col",
			ClrType = typeof(string),
			IsNullable = true,
			MaxLength = 200,
			OldColumn = new AddColumnOperation()
			{
				ClrType = typeof(string),
				IsNullable = true,
				MaxLength = 100,
			},
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(2, batch.Count());
		Assert.AreEqual(NewLineEnd(@"ALTER TABLE ""People"" ALTER COLUMN ""Col"" TYPE VARCHAR(200);"), batch[0].CommandText);
	}

	[Test]
	public void AlterColumnNullableToNotNull()
	{
		var operation = new AlterColumnOperation()
		{
			Table = "People",
			Name = "Col",
			ClrType = typeof(string),
			IsNullable = false,
			MaxLength = 100,
			OldColumn = new AddColumnOperation()
			{
				ClrType = typeof(string),
				IsNullable = true,
				MaxLength = 100,
			},
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(4, batch.Count());
		Assert.AreEqual(NewLineEnd(@"UPDATE RDB$RELATION_FIELDS SET RDB$NULL_FLAG = 0  WHERE RDB$FIELD_NAME = ""Col"" AND RDB$RELATION_NAME = ""People"";"), batch[0].CommandText);
		Assert.AreEqual(NewLineEnd(@"ALTER TABLE ""People"" ALTER COLUMN ""Col"" TYPE VARCHAR(100);"), batch[1].CommandText);
		Assert.AreEqual(NewLineEnd(@"UPDATE RDB$RELATION_FIELDS SET RDB$NULL_FLAG = 1  WHERE RDB$FIELD_NAME = ""Col"" AND RDB$RELATION_NAME = ""People"";"), batch[2].CommandText);
		Assert.AreEqual(NewLineEnd(@"UPDATE RDB$RELATION_FIELDS F1 SET   F1.RDB$DEFAULT_VALUE = NULL,   F1.RDB$DEFAULT_SOURCE = NULL WHERE (F1.RDB$RELATION_NAME = ""People"") AND (F1.RDB$FIELD_NAME = ""Col"");"), batch[3].CommandText);
	}

	[Test]
	public void AlterColumnNotNullToNullable()
	{
		var operation = new AlterColumnOperation()
		{
			Table = "People",
			Name = "Col",
			ClrType = typeof(string),
			IsNullable = true,
			MaxLength = 100,
			OldColumn = new AddColumnOperation()
			{
				ClrType = typeof(string),
				IsNullable = false,
				MaxLength = 100,
			},
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(2, batch.Count());
		Assert.AreEqual(NewLineEnd(@"ALTER TABLE ""People"" ALTER COLUMN ""Col"" TYPE VARCHAR(100);"), batch[0].CommandText);
		Assert.AreEqual(NewLineEnd(@"UPDATE RDB$RELATION_FIELDS F1 SET   F1.RDB$DEFAULT_VALUE = NULL,   F1.RDB$DEFAULT_SOURCE = NULL WHERE (F1.RDB$RELATION_NAME = ""People"") AND (F1.RDB$FIELD_NAME = ""Col"");"), batch[1].CommandText);
	}

	[Test]
	public void AlterColumnType()
	{
		var operation = new AlterColumnOperation()
		{
			Table = "People",
			Name = "Col",
			ClrType = typeof(long),
			IsNullable = false,
			OldColumn = new AddColumnOperation()
			{
				ClrType = typeof(int),
				IsNullable = false,
			},
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(4, batch.Count());
		Assert.AreEqual(NewLineEnd(@"UPDATE RDB$RELATION_FIELDS SET RDB$NULL_FLAG = 0  WHERE RDB$FIELD_NAME = ""Col"" AND RDB$RELATION_NAME = ""People"";"), batch[0].CommandText);
		Assert.AreEqual(NewLineEnd(@"ALTER TABLE ""People"" ALTER COLUMN ""Col"" TYPE NUMERIC(18, 0);"), batch[1].CommandText);
		Assert.AreEqual(NewLineEnd(@"UPDATE RDB$RELATION_FIELDS SET RDB$NULL_FLAG = 1  WHERE RDB$FIELD_NAME = ""Col"" AND RDB$RELATION_NAME = ""People"";"), batch[2].CommandText);
	}

	[Test]
	public void AlterColumnDefault()
	{
		var operation = new AlterColumnOperation()
		{
			Table = "People",
			Name = "Col",
			ClrType = typeof(int),
			DefaultValue = 20,
			OldColumn = new AddColumnOperation()
			{
				ClrType = typeof(int),
				DefaultValue = 10,
			},
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(6, batch.Count());
		Assert.AreEqual(NewLineEnd(@"ALTER TABLE ""People"" ALTER COLUMN ""Col"" TYPE INTEGER;"), batch[1].CommandText);
		Assert.AreEqual(NewLineEnd(@"ALTER TABLE ""People"" ADD COLUMN EFC$$$TEMP_COLUMN DEFAULT 20;"), batch[3].CommandText);
		StringAssert.StartsWith("UPDATE RDB$RELATION_FIELDS F1 SET", batch[4].CommandText);
		Assert.AreEqual(NewLineEnd(@"ALTER TABLE ""People"" DROP COLUMN EFC$$$TEMP_COLUMN;"), batch[5].CommandText);
	}

[Test]
	public void AlterColumnAddSequenceTrigger()
	{
		var operation = new AlterColumnOperation()
		{
			Table = "People",
			Name = "Col",
			ClrType = typeof(int),
			[IBAnnotationNames.ValueGenerationStrategy] = IBValueGenerationStrategy.SequenceTrigger,
			OldColumn = new AddColumnOperation()
			{
				ClrType = typeof(int),
			},
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(8, batch.Count());
		Assert.AreEqual(NewLineEnd(@"UPDATE RDB$RELATION_FIELDS SET RDB$NULL_FLAG = 0  WHERE RDB$FIELD_NAME = ""Col"" AND RDB$RELATION_NAME = ""People"";"), batch[0].CommandText);
		Assert.AreEqual(NewLineEnd(@"ALTER TABLE ""People"" ALTER COLUMN ""Col"" TYPE INTEGER;"), batch[1].CommandText);
		Assert.AreEqual(NewLineEnd(@"UPDATE RDB$RELATION_FIELDS SET RDB$NULL_FLAG = 1  WHERE RDB$FIELD_NAME = ""Col"" AND RDB$RELATION_NAME = ""People"";"), batch[2].CommandText);
		Assert.AreEqual(NewLineEnd(@"UPDATE RDB$RELATION_FIELDS F1 SET   F1.RDB$DEFAULT_VALUE = NULL,   F1.RDB$DEFAULT_SOURCE = NULL WHERE (F1.RDB$RELATION_NAME = ""People"") AND (F1.RDB$FIELD_NAME = ""Col"");"), batch[3].CommandText);
		StringAssert.Contains("create procedure IBEFC$GEN", batch[4].CommandText);
		Assert.AreEqual(NewLineEnd(@"execute procedure IBEFC$GEN;"), batch[5].CommandText);
		Assert.AreEqual(NewLineEnd(@"drop procedure IBEFC$GEN;"), batch[6].CommandText);
		StringAssert.StartsWith("CREATE TRIGGER ", batch[7].CommandText);
	}

	[Test]
	public void AlterColumnRemoveSequenceTrigger()
	{
		var operation = new AlterColumnOperation()
		{
			Table = "People",
			Name = "Col",
			ClrType = typeof(int),
			OldColumn = new AddColumnOperation()
			{
				ClrType = typeof(int),
				[IBAnnotationNames.ValueGenerationStrategy] = IBValueGenerationStrategy.SequenceTrigger,
			},
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(7, batch.Count());
		StringAssert.StartsWith(@"create procedure IBEFC$GENDROP", batch[0].CommandText);
		Assert.AreEqual(NewLineEnd(@"execute procedure IBEFC$GENDROP;"), batch[1].CommandText);
		Assert.AreEqual(NewLineEnd(@"drop procedure IBEFC$GENDROP;"), batch[2].CommandText);
		Assert.AreEqual(NewLineEnd(@"ALTER TABLE ""People"" ALTER COLUMN ""Col"" TYPE INTEGER;"), batch[4].CommandText);
	}

	[Test]
	public void RenameColumn()
	{
		var operation = new RenameColumnOperation()
		{
			Table = "People",
			Name = "OldCol",
			NewName = "NewCol",
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(1, batch.Count());
		Assert.AreEqual(NewLineEnd(@"ALTER TABLE ""People"" ALTER COLUMN ""OldCol"" TO ""NewCol"";"), batch[0].CommandText);
	}

	[Test]
	public void CreateIndexOneColumn()
	{
		var operation = new CreateIndexOperation()
		{
			Table = "People",
			Name = "MyIndex",
			Columns = new[] { "Foo" },
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(1, batch.Count());
		Assert.AreEqual(NewLineEnd(@"CREATE INDEX ""MyIndex"" ON ""People"" (""Foo"");"), batch[0].CommandText);
	}

	[Test]
	public void CreateIndexThreeColumn()
	{
		var operation = new CreateIndexOperation()
		{
			Table = "People",
			Name = "MyIndex",
			Columns = new[] { "Foo", "Bar", "Baz" },
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(1, batch.Count());
		Assert.AreEqual(NewLineEnd(@"CREATE INDEX ""MyIndex"" ON ""People"" (""Foo"", ""Bar"", ""Baz"");"), batch[0].CommandText);
	}

	[Test]
	public void CreateIndexUnique()
	{
		var operation = new CreateIndexOperation()
		{
			Table = "People",
			Name = "MyIndex",
			Columns = new[] { "Foo" },
			IsUnique = true,
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(1, batch.Count());
		Assert.AreEqual(NewLineEnd(@"CREATE UNIQUE INDEX ""MyIndex"" ON ""People"" (""Foo"");"), batch[0].CommandText);
	}

	[Test]
	public void CreateIndexFilter()
	{
		var operation = new CreateIndexOperation()
		{
			Table = "People",
			Name = "MyIndex",
			Filter = "xxx",
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(1, batch.Count());
		Assert.AreEqual(NewLineEnd(@"CREATE INDEX ""MyIndex"" ON ""People"" COMPUTED BY (xxx);"), batch[0].CommandText);
	}

	[Test]
	public void CreateSequence()
	{
		var operation = new CreateSequenceOperation()
		{
			Name = "MySequence",
			StartValue = 34,
			IncrementBy = 56,
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(2, batch.Count());
		Assert.AreEqual(NewLineEnd(@"create generator ""MySequence"";"), batch[0].CommandText);
		Assert.AreEqual(NewLineEnd(@"SET GENERATOR ""MySequence"" TO 34;"), batch[1].CommandText);
	}

	[Test]
	public void AlterSequence()
	{
		var operation = new AlterSequenceOperation()
		{
			Name = "MySequence",
			IncrementBy = 12,
		};
		Assert.Throws<NotSupportedInInterBase>(() => Generate(new[] { operation }));
	}

	[Test]
	public void RestartSequence()
	{
		var operation = new RestartSequenceOperation()
		{
			Name = "MySequence",
			StartValue = 23,
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(1, batch.Count());
		Assert.AreEqual(NewLineEnd(@"set generator ""MySequence"" TO 23;"), batch[0].CommandText);
	}

	[Test]
	public void DropSequence()
	{
		var operation = new DropSequenceOperation()
		{
			Name = "MySequence",
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(1, batch.Count());
		Assert.AreEqual(NewLineEnd(@"DROP SEQUENCE ""MySequence"";"), batch[0].CommandText);
	}

	[Test]
	public void AddPrimaryKey()
	{
		var operation = new AddPrimaryKeyOperation()
		{
			Table = "People",
			Name = "PK_People",
			Columns = new[] { "Foo" },
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(1, batch.Count());
		Assert.AreEqual(NewLineEnd(@"ALTER TABLE ""People"" ADD CONSTRAINT ""PK_People"" PRIMARY KEY (""Foo"");"), batch[0].CommandText);
	}

	[Test]
	public void AddPrimaryKeyNoName()
	{
		var operation = new AddPrimaryKeyOperation()
		{
			Table = "People",
			Columns = new[] { "Foo" },
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(1, batch.Count());
		Assert.AreEqual(NewLineEnd(@"ALTER TABLE ""People"" ADD PRIMARY KEY (""Foo"");"), batch[0].CommandText);
	}

	[Test]
	public void DropPrimaryKey()
	{
		var operation = new DropPrimaryKeyOperation()
		{
			Table = "People",
			Name = "PK_People",
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(1, batch.Count());
		Assert.AreEqual(NewLineEnd(@"ALTER TABLE ""People"" DROP CONSTRAINT ""PK_People"";"), batch[0].CommandText);
	}

	[Test]
	public void AddForeignKey()
	{
		var operation = new AddForeignKeyOperation()
		{
			Table = "People",
			Name = "FK_People_Principal",
			Columns = new[] { "Foo" },
			PrincipalTable = "Principal",
			PrincipalColumns = new[] { "Bar" },
			OnDelete = ReferentialAction.Restrict,
			OnUpdate = ReferentialAction.Restrict,
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(1, batch.Count());
		Assert.AreEqual(NewLineEnd(@"ALTER TABLE ""People"" ADD CONSTRAINT ""FK_People_Principal"" FOREIGN KEY (""Foo"") REFERENCES ""Principal"" (""Bar"");"), batch[0].CommandText);
	}

	[Test]
	public void AddForeignKeyNoName()
	{
		var operation = new AddForeignKeyOperation()
		{
			Table = "People",
			Columns = new[] { "Foo" },
			PrincipalTable = "Principal",
			PrincipalColumns = new[] { "Bar" },
			OnDelete = ReferentialAction.Restrict,
			OnUpdate = ReferentialAction.Restrict,
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(1, batch.Count());
		Assert.AreEqual(NewLineEnd(@"ALTER TABLE ""People"" ADD FOREIGN KEY (""Foo"") REFERENCES ""Principal"" (""Bar"");"), batch[0].CommandText);
	}

	[Test]
	public void AddForeignKeyDeleteCascade()
	{
		var operation = new AddForeignKeyOperation()
		{
			Table = "People",
			Name = "FK_People_Principal",
			Columns = new[] { "Foo" },
			PrincipalTable = "Principal",
			PrincipalColumns = new[] { "Bar" },
			OnDelete = ReferentialAction.Cascade,
			OnUpdate = ReferentialAction.Restrict,
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(1, batch.Count());
		Assert.AreEqual(NewLineEnd(@"ALTER TABLE ""People"" ADD CONSTRAINT ""FK_People_Principal"" FOREIGN KEY (""Foo"") REFERENCES ""Principal"" (""Bar"") ON DELETE CASCADE;"), batch[0].CommandText);
	}

	[Test]
	public void AddForeignKeyDeleteNoAction()
	{
		var operation = new AddForeignKeyOperation()
		{
			Table = "People",
			Name = "FK_People_Principal",
			Columns = new[] { "Foo" },
			PrincipalTable = "Principal",
			PrincipalColumns = new[] { "Bar" },
			OnDelete = ReferentialAction.NoAction,
			OnUpdate = ReferentialAction.Restrict,
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(1, batch.Count());
		Assert.AreEqual(NewLineEnd(@"ALTER TABLE ""People"" ADD CONSTRAINT ""FK_People_Principal"" FOREIGN KEY (""Foo"") REFERENCES ""Principal"" (""Bar"") ON DELETE NO ACTION;"), batch[0].CommandText);
	}

	[Test]
	public void AddForeignKeyDeleteRestrict()
	{
		var operation = new AddForeignKeyOperation()
		{
			Table = "People",
			Name = "FK_People_Principal",
			Columns = new[] { "Foo" },
			PrincipalTable = "Principal",
			PrincipalColumns = new[] { "Bar" },
			OnDelete = ReferentialAction.Restrict,
			OnUpdate = ReferentialAction.Restrict,
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(1, batch.Count());
		Assert.AreEqual(NewLineEnd(@"ALTER TABLE ""People"" ADD CONSTRAINT ""FK_People_Principal"" FOREIGN KEY (""Foo"") REFERENCES ""Principal"" (""Bar"");"), batch[0].CommandText);
	}

	[Test]
	public void AddForeignKeyDeleteSetDefault()
	{
		var operation = new AddForeignKeyOperation()
		{
			Table = "People",
			Name = "FK_People_Principal",
			Columns = new[] { "Foo" },
			PrincipalTable = "Principal",
			PrincipalColumns = new[] { "Bar" },
			OnDelete = ReferentialAction.SetDefault,
			OnUpdate = ReferentialAction.Restrict,
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(1, batch.Count());
		Assert.AreEqual(NewLineEnd(@"ALTER TABLE ""People"" ADD CONSTRAINT ""FK_People_Principal"" FOREIGN KEY (""Foo"") REFERENCES ""Principal"" (""Bar"") ON DELETE SET DEFAULT;"), batch[0].CommandText);
	}

	[Test]
	public void AddForeignKeyDeleteSetNull()
	{
		var operation = new AddForeignKeyOperation()
		{
			Table = "People",
			Name = "FK_People_Principal",
			Columns = new[] { "Foo" },
			PrincipalTable = "Principal",
			PrincipalColumns = new[] { "Bar" },
			OnDelete = ReferentialAction.SetNull,
			OnUpdate = ReferentialAction.Restrict,
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(1, batch.Count());
		Assert.AreEqual(NewLineEnd(@"ALTER TABLE ""People"" ADD CONSTRAINT ""FK_People_Principal"" FOREIGN KEY (""Foo"") REFERENCES ""Principal"" (""Bar"") ON DELETE SET NULL;"), batch[0].CommandText);
	}

	[Test]
	public void AddForeignKeyUpdateCascade()
	{
		var operation = new AddForeignKeyOperation()
		{
			Table = "People",
			Name = "FK_People_Principal",
			Columns = new[] { "Foo" },
			PrincipalTable = "Principal",
			PrincipalColumns = new[] { "Bar" },
			OnDelete = ReferentialAction.Restrict,
			OnUpdate = ReferentialAction.Cascade,
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(1, batch.Count());
		Assert.AreEqual(NewLineEnd(@"ALTER TABLE ""People"" ADD CONSTRAINT ""FK_People_Principal"" FOREIGN KEY (""Foo"") REFERENCES ""Principal"" (""Bar"") ON UPDATE CASCADE;"), batch[0].CommandText);
	}

	[Test]
	public void AddForeignKeyUpdateNoAction()
	{
		var operation = new AddForeignKeyOperation()
		{
			Table = "People",
			Name = "FK_People_Principal",
			Columns = new[] { "Foo" },
			PrincipalTable = "Principal",
			PrincipalColumns = new[] { "Bar" },
			OnDelete = ReferentialAction.Restrict,
			OnUpdate = ReferentialAction.NoAction,
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(1, batch.Count());
		Assert.AreEqual(NewLineEnd(@"ALTER TABLE ""People"" ADD CONSTRAINT ""FK_People_Principal"" FOREIGN KEY (""Foo"") REFERENCES ""Principal"" (""Bar"") ON UPDATE NO ACTION;"), batch[0].CommandText);
	}

	[Test]
	public void AddForeignKeyUpdateRestrict()
	{
		var operation = new AddForeignKeyOperation()
		{
			Table = "People",
			Name = "FK_People_Principal",
			Columns = new[] { "Foo" },
			PrincipalTable = "Principal",
			PrincipalColumns = new[] { "Bar" },
			OnDelete = ReferentialAction.Restrict,
			OnUpdate = ReferentialAction.Restrict,
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(1, batch.Count());
		Assert.AreEqual(NewLineEnd(@"ALTER TABLE ""People"" ADD CONSTRAINT ""FK_People_Principal"" FOREIGN KEY (""Foo"") REFERENCES ""Principal"" (""Bar"");"), batch[0].CommandText);
	}

	[Test]
	public void AddForeignKeyUpdateSetDefault()
	{
		var operation = new AddForeignKeyOperation()
		{
			Table = "People",
			Name = "FK_People_Principal",
			Columns = new[] { "Foo" },
			PrincipalTable = "Principal",
			PrincipalColumns = new[] { "Bar" },
			OnDelete = ReferentialAction.Restrict,
			OnUpdate = ReferentialAction.SetDefault,
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(1, batch.Count());
		Assert.AreEqual(NewLineEnd(@"ALTER TABLE ""People"" ADD CONSTRAINT ""FK_People_Principal"" FOREIGN KEY (""Foo"") REFERENCES ""Principal"" (""Bar"") ON UPDATE SET DEFAULT;"), batch[0].CommandText);
	}

	[Test]
	public void AddForeignKeyUpdateSetNull()
	{
		var operation = new AddForeignKeyOperation()
		{
			Table = "People",
			Name = "FK_People_Principal",
			Columns = new[] { "Foo" },
			PrincipalTable = "Principal",
			PrincipalColumns = new[] { "Bar" },
			OnDelete = ReferentialAction.Restrict,
			OnUpdate = ReferentialAction.SetNull,
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(1, batch.Count());
		Assert.AreEqual(NewLineEnd(@"ALTER TABLE ""People"" ADD CONSTRAINT ""FK_People_Principal"" FOREIGN KEY (""Foo"") REFERENCES ""Principal"" (""Bar"") ON UPDATE SET NULL;"), batch[0].CommandText);
	}

	[Test]
	public void DropForeignKey()
	{
		var operation = new DropForeignKeyOperation()
		{
			Table = "People",
			Name = "FK_People_Principal",
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(1, batch.Count());
		Assert.AreEqual(NewLineEnd(@"ALTER TABLE ""People"" DROP CONSTRAINT ""FK_People_Principal"";"), batch[0].CommandText);
	}

	[Test]
	public void AddUniqueConstraintOneColumn()
	{
		var operation = new AddUniqueConstraintOperation()
		{
			Table = "People",
			Name = "UNQ_People_Foo",
			Columns = new[] { "Foo" },
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(1, batch.Count());
		Assert.AreEqual(NewLineEnd(@"ALTER TABLE ""People"" ADD CONSTRAINT ""UNQ_People_Foo"" UNIQUE (""Foo"");"), batch[0].CommandText);
	}

	[Test]
	public void AddUniqueConstraintTwoColumns()
	{
		var operation = new AddUniqueConstraintOperation()
		{
			Table = "People",
			Name = "UNQ_People_Foo_Bar",
			Columns = new[] { "Foo", "Bar" },
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(1, batch.Count());
		Assert.AreEqual(NewLineEnd(@"ALTER TABLE ""People"" ADD CONSTRAINT ""UNQ_People_Foo_Bar"" UNIQUE (""Foo"", ""Bar"");"), batch[0].CommandText);
	}

	[Test]
	public void AddUniqueConstraintNoName()
	{
		var operation = new AddUniqueConstraintOperation()
		{
			Table = "People",
			Columns = new[] { "Foo" },
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(1, batch.Count());
		Assert.AreEqual(NewLineEnd(@"ALTER TABLE ""People"" ADD UNIQUE (""Foo"");"), batch[0].CommandText);
	}

	[Test]
	public void DropUniqueConstraint()
	{
		var operation = new DropUniqueConstraintOperation()
		{
			Table = "People",
			Name = "UNQ_People_Foo",
		};
		var batch = Generate(new[] { operation });
		Assert.AreEqual(1, batch.Count());
		Assert.AreEqual(NewLineEnd(@"ALTER TABLE ""People"" DROP CONSTRAINT ""UNQ_People_Foo"";"), batch[0].CommandText);
	}

	IReadOnlyList<MigrationCommand> Generate(IReadOnlyList<MigrationOperation> operations, MigrationsSqlGenerationOptions options = MigrationsSqlGenerationOptions.Default)
	{
		using (var db = GetDbContext<IBTestDbContext>())
		{
			var generator = db.GetService<IMigrationsSqlGenerator>();
			return generator.Generate(operations, db.Model, options);
		}
	}

	async Task<IReadOnlyList<MigrationCommand>> GenerateAsync(IReadOnlyList<MigrationOperation> operations, MigrationsSqlGenerationOptions options = MigrationsSqlGenerationOptions.Default)
	{
		await using (var db = await GetDbContextAsync<IBTestDbContext>())
		{
			var generator = db.GetService<IMigrationsSqlGenerator>();
			return generator.Generate(operations, db.Model, options);
		}
	}

	static string NewLineEnd(string s) => s + Environment.NewLine;
}
#pragma warning restore EF1001

