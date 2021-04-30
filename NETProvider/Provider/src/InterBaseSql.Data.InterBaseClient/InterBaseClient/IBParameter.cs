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

//$Authors = Carlos Guzman Alvarez, Jiri Cincura (jiri@cincura.net)

using System;
using System.Data;
using System.Data.Common;
using System.ComponentModel;

using InterBaseSql.Data.Common;
using System.Text;

namespace InterBaseSql.Data.InterBaseClient
{
	[ParenthesizePropertyName(true)]
	public sealed class IBParameter : DbParameter, ICloneable
	{
		#region Fields

		private IBParameterCollection _parent;
		private IBDbType _ibDbType;
		private ParameterDirection _direction;
		private DataRowVersion _sourceVersion;
		private IBCharset _charset;
		private bool _isNullable;
		private bool _sourceColumnNullMapping;
		private byte _precision;
		private byte _scale;
		private int _size;
		private object _value;
		private string _parameterName;
		private string _sourceColumn;
		private string _internalParameterName;
		private bool _isUnicodeParameterName;

		#endregion

		#region DbParameter properties

		[DefaultValue("")]
		public override string ParameterName
		{
			get { return _parameterName; }
			set
			{
				_parameterName = value;
				_internalParameterName = NormalizeParameterName(_parameterName);
				_isUnicodeParameterName = IsNonAsciiParameterName(_parameterName);
				_parent?.ParameterNameChanged();
			}
		}

		[Category("Data")]
		[DefaultValue(0)]
		public override int Size
		{
			get
			{
				return (HasSize ? _size : RealValueSize ?? 0);
			}
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException();

				_size = value;

				// Hack for Clob parameters
				if (value == 2147483647 &&
					(IBDbType == IBDbType.VarChar || IBDbType == IBDbType.Char))
				{
					IBDbType = IBDbType.Text;
				}
			}
		}

		[Category("Data")]
		[DefaultValue(ParameterDirection.Input)]
		public override ParameterDirection Direction
		{
			get { return _direction; }
			set { _direction = value; }
		}

		[Browsable(false)]
		[DesignOnly(true)]
		[DefaultValue(false)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public override bool IsNullable
		{
			get { return _isNullable; }
			set { _isNullable = value; }
		}

		[Category("Data")]
		[DefaultValue("")]
		public override string SourceColumn
		{
			get { return _sourceColumn; }
			set { _sourceColumn = value; }
		}

		[Category("Data")]
		[DefaultValue(DataRowVersion.Current)]
		public override DataRowVersion SourceVersion
		{
			get { return _sourceVersion; }
			set { _sourceVersion = value; }
		}

		[Browsable(false)]
		[Category("Data")]
		[RefreshProperties(RefreshProperties.All)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DbType DbType
		{
			get { return TypeHelper.GetDbTypeFromDbDataType((DbDataType)_ibDbType); }
			set { IBDbType = (IBDbType)TypeHelper.GetDbDataTypeFromDbType(value); }
		}

		[RefreshProperties(RefreshProperties.All)]
		[Category("Data")]
		[DefaultValue(IBDbType.VarChar)]
		public IBDbType IBDbType
		{
			get { return _ibDbType; }
			set
			{
				_ibDbType = value;
				IsTypeSet = true;
			}
		}

		[Category("Data")]
		[TypeConverter(typeof(StringConverter)), DefaultValue(null)]
		public override object Value
		{
			get { return _value; }
			set
			{
				if (value == null)
				{
					value = DBNull.Value;
				}

				if (IBDbType == IBDbType.Guid && value != null &&
					value != DBNull.Value && !(value is Guid) && !(value is byte[]))
				{
					throw new InvalidOperationException("Incorrect Guid value.");
				}

				_value = value;

				if (!IsTypeSet)
				{
					SetIBDbType(value);
				}
			}
		}

		[Category("Data")]
		[DefaultValue(IBCharset.Default)]
		public IBCharset Charset
		{
			get { return _charset; }
			set { _charset = value; }
		}

		public override bool SourceColumnNullMapping
		{
			get { return _sourceColumnNullMapping; }
			set { _sourceColumnNullMapping = value; }
		}

		#endregion

		#region Properties

		[Category("Data")]
		[DefaultValue((byte)0)]
		public override byte Precision
		{
			get { return _precision; }
			set { _precision = value; }
		}

		[Category("Data")]
		[DefaultValue((byte)0)]
		public override byte Scale
		{
			get { return _scale; }
			set { _scale = value; }
		}

		#endregion

		#region Internal Properties

		internal IBParameterCollection Parent
		{
			get { return _parent; }
			set
			{
				_parent?.ParameterNameChanged();
				_parent = value;
				_parent?.ParameterNameChanged();
			}
		}

		internal string InternalParameterName
		{
			get
			{
				return _internalParameterName;
			}
		}

		internal bool IsTypeSet { get; private set; }

		internal object InternalValue
		{
			get
			{
				switch (_value)
				{
					case string svalue:
						return svalue.Substring(0, Math.Min(Size, svalue.Length));
					case byte[] bvalue:
						var result = new byte[Math.Min(Size, bvalue.Length)];
						Array.Copy(bvalue, result, result.Length);
						return result;
					default:
						return _value;
				}
			}
		}

		internal bool HasSize
		{
			get { return _size != default; }
		}

		#endregion

		#region Constructors

		public IBParameter()
		{
			_ibDbType = IBDbType.VarChar;
			_direction = ParameterDirection.Input;
			_sourceVersion = DataRowVersion.Current;
			_sourceColumn = string.Empty;
			_parameterName = string.Empty;
			_charset = IBCharset.Default;
			_internalParameterName = string.Empty;
		}

		public IBParameter(string parameterName, object value)
			: this()
		{
			ParameterName = parameterName;
			Value = value;
		}

		public IBParameter(string parameterName, IBDbType ibType)
			: this()
		{
			ParameterName = parameterName;
			IBDbType = ibType;
		}

		public IBParameter(string parameterName, IBDbType ibType, int size)
			: this()
		{
			ParameterName = parameterName;
			IBDbType = ibType;
			Size = size;
		}

		public IBParameter(string parameterName, IBDbType ibType, int size, string sourceColumn)
			: this()
		{
			ParameterName = parameterName;
			IBDbType = ibType;
			Size = size;
			_sourceColumn = sourceColumn;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public IBParameter(
			string parameterName,
			IBDbType dbType,
			int size,
			ParameterDirection direction,
			bool isNullable,
			byte precision,
			byte scale,
			string sourceColumn,
			DataRowVersion sourceVersion,
			object value)
		{
			ParameterName = parameterName;
			IBDbType = dbType;
			Size = size;
			_direction = direction;
			_isNullable = isNullable;
			_precision = precision;
			_scale = scale;
			_sourceColumn = sourceColumn;
			_sourceVersion = sourceVersion;
			Value = value;
			_charset = IBCharset.Default;
		}

		#endregion

		#region ICloneable Methods
		object ICloneable.Clone()
		{
			return new IBParameter(
				_parameterName,
				_ibDbType,
				_size,
				_direction,
				_isNullable,
				_precision,
				_scale,
				_sourceColumn,
				_sourceVersion,
				_value)
			{
				Charset = _charset
			};
		}

		#endregion

		#region DbParameter methods

		public override string ToString()
		{
			return _parameterName;
		}

		public override void ResetDbType()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Private Methods

		private void SetIBDbType(object value)
		{
			if (value == null)
			{
				value = DBNull.Value;
			}

			var code = Type.GetTypeCode(value.GetType());

			switch (code)
			{
				case TypeCode.Char:
					_ibDbType = IBDbType.Char;
					break;

				case TypeCode.DBNull:
				case TypeCode.String:
					_ibDbType = IBDbType.VarChar;
					break;

				case TypeCode.Boolean:
					_ibDbType = IBDbType.Boolean;
					break;

				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
					_ibDbType = IBDbType.SmallInt;
					break;

				case TypeCode.Int32:
				case TypeCode.UInt32:
					_ibDbType = IBDbType.Integer;
					break;

				case TypeCode.Int64:
				case TypeCode.UInt64:
					_ibDbType = IBDbType.BigInt;
					break;

				case TypeCode.Single:
					_ibDbType = IBDbType.Float;
					break;

				case TypeCode.Double:
					_ibDbType = IBDbType.Double;
					break;

				case TypeCode.Decimal:
					_ibDbType = IBDbType.Decimal;
					break;

				case TypeCode.DateTime:
					_ibDbType = IBDbType.TimeStamp;
					break;

				case TypeCode.Empty:
				default:
					if (value is Guid)
					{
						_ibDbType = IBDbType.Guid;
					}
					else if (code == TypeCode.Object)
					{
						_ibDbType = IBDbType.Binary;
					}
					else
					{
						throw new ArgumentException("Parameter type is unknown.");
					}
					break;
			}
		}

		#endregion

		#region Private Properties

		private int? RealValueSize
		{
			get
			{
				var svalue = (_value as string);
				if (svalue != null)
				{
					return svalue.Length;
				}
				var bvalue = (_value as byte[]);
				if (bvalue != null)
				{
					return bvalue.Length;
				}
				return null;
			}
		}

		internal bool IsUnicodeParameterName
		{
			get
			{
				return _isUnicodeParameterName;
			}
		}

		#endregion

		#region Static Methods

		internal static string NormalizeParameterName(string parameterName)
		{
			return string.IsNullOrEmpty(parameterName) || parameterName[0] == '@'
				? parameterName
				: "@" + parameterName;
		}

		internal static bool IsNonAsciiParameterName(string parameterName)
		{
			return string.IsNullOrEmpty(parameterName) || Encoding.UTF8.GetByteCount(parameterName) != parameterName.Length;
		}

		#endregion
	}
}
