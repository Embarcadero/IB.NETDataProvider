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

//$Authors = Carlos Guzman Alvarez, Jiri Cincura (jiri@cincura.net)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace InterBaseSql.Data.Common;

internal sealed class Charset
{
	internal const string Octets = "OCTETS";
	internal const string None = "NONE";

	private readonly static Dictionary<int, Charset> charsetsById;
	private readonly static Dictionary<string, Charset> charsetsByName;

	static Charset()
	{
		var charsets = GetSupportedCharsets();
		charsetsById = charsets.ToDictionary(x => x.Identifier);
		charsetsByName = charsets.ToDictionary(x => x.Name, StringComparer.CurrentCultureIgnoreCase);
	}

	public static Charset DefaultCharset => charsetsByName[None];

	public static Charset GetCharset(int charsetId)
	{
		return TryGetById(charsetId, out var value) ? value : null;
	}
	public static bool TryGetById(int id, out Charset charset) => charsetsById.TryGetValue(id, out charset);

	public static Charset GetCharset(string charsetName)
	{
		return TryGetByName(charsetName, out var value) ? value : null;
	}
	public static bool TryGetByName(string name, out Charset charset) => charsetsByName.TryGetValue(name, out charset);

	private static List<Charset> GetSupportedCharsets()
	{

#if NET5_0_OR_GREATER
		// Register the extra encoding providers if possible.  EncodingProvider was introduced in net5.0
		try
		{
			Assembly assembly = Assembly.LoadFrom("System.Text.Encoding.CodePages.dll");
			EncodingProvider provider = CodePagesEncodingProvider.Instance;
			Encoding.RegisterProvider(provider);
		}
		catch
		{
			// Do nothing here, just means they won't get the additional code pages
			// and work like it did before.
		}
#endif
		var charsets = new List<Charset>();

		charsets.Add(new Charset(0, None, 1, None));
		charsets.Add(new Charset(1, Octets, 1, Octets));
		charsets.Add(new Charset(2, "ASCII", 1, "ascii"));
		charsets.Add(new Charset(3, "UNICODE_FSS", 3, "UTF-8"));

		TryAddCharset(charsets, () => new Charset(5, "SJIS_0208", 2, "shift_jis"));
		TryAddCharset(charsets, () => new Charset(6, "EUCJ_0208", 2, "euc-jp"));
		//TryAddCharset(charsets, () => new Charset(7, "ISO2022-JP", 2, "iso-2022-jp"));
		TryAddCharset(charsets, () => new Charset(10, "DOS437", 1, "IBM437"));
		TryAddCharset(charsets, () => new Charset(11, "DOS850", 1, "ibm850"));
		TryAddCharset(charsets, () => new Charset(12, "DOS865", 1, "IBM865"));
		TryAddCharset(charsets, () => new Charset(13, "DOS860", 1, "IBM860"));
		TryAddCharset(charsets, () => new Charset(14, "DOS863", 1, "IBM863"));
		TryAddCharset(charsets, () => new Charset(21, "ISO8859_1", 1, "iso-8859-1"));
		TryAddCharset(charsets, () => new Charset(22, "ISO8859_2", 1, "iso-8859-2"));
		TryAddCharset(charsets, () => new Charset(39, "ISO8859_15", 1, "iso-8859-15"));
		TryAddCharset(charsets, () => new Charset(44, "KSC_5601", 2, "ks_c_5601-1987"));
		TryAddCharset(charsets, () => new Charset(45, "DOS852", 1, "ibm852"));
		TryAddCharset(charsets, () => new Charset(46, "DOS857", 1, "ibm857"));
		TryAddCharset(charsets, () => new Charset(47, "DOS861", 1, "ibm861"));
		TryAddCharset(charsets, () => new Charset(51, "WIN1250", 1, "windows-1250"));
		TryAddCharset(charsets, () => new Charset(52, "WIN1251", 1, "windows-1251"));
		TryAddCharset(charsets, () => new Charset(53, "WIN1252", 1, "windows-1252"));
		TryAddCharset(charsets, () => new Charset(54, "WIN1253", 1, "windows-1253"));
		TryAddCharset(charsets, () => new Charset(55, "WIN1254", 1, "windows-1254"));
		TryAddCharset(charsets, () => new Charset(56, "BIG_5", 2, "big5"));
		TryAddCharset(charsets, () => new Charset(57, "GB_2312", 2, "gb2312"));
		TryAddCharset(charsets, () => new Charset(58, "KOI8R", 2, "koi8-r"));
		charsets.Add(new Charset(59, "UTF8", 4, "UTF-8"));

		return charsets;
	}

	private static void TryAddCharset(List<Charset> charsets, Func<Charset> charsetCreator)
	{
		try
		{
			charsets.Add(charsetCreator());
		}
		catch
		{ }
	}

	public int Identifier { get; }
	public string Name { get; }
	public string SystemName { get; private set; }
	public int BytesPerCharacter { get; }
	public Encoding Encoding { get; }
	public bool IsOctetsCharset { get; }
	public bool IsNoneCharset { get; }

	public Charset(int id, string name, int bytesPerCharacter, string systemName)
	{
		Identifier = id;
		Name = name;
		BytesPerCharacter = bytesPerCharacter;
		SystemName = systemName;
		IsNoneCharset = false;
		IsOctetsCharset = false;
		switch (SystemName)
		{
			case None:
				Encoding = Encoding2.Default;
				IsNoneCharset = true;
				break;
			case Octets:
				Encoding = new BinaryEncoding();
				IsOctetsCharset = true;
				break;
			default:
				Encoding = Encoding.GetEncoding(SystemName);
				break;
		}
	}

	public byte[] GetBytes(string s)
	{
		return Encoding.GetBytes(s);
	}

	public int GetBytes(string s, int charIndex, int charCount, byte[] bytes, int byteIndex)
	{
		return Encoding.GetBytes(s, charIndex, charCount, bytes, byteIndex);
	}

	public string GetString(byte[] buffer)
	{
		return Encoding.GetString(buffer);
	}

	public string GetString(byte[] buffer, int index, int count)
	{
		return Encoding.GetString(buffer, index, count);
	}
}