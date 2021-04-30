unit StrFncs;

interface

function Reverse(sz : PAnsiChar) : PAnsiChar; cdecl; export;
function Position(SubStr, MainStr: PAnsiChar; var i: Integer): Integer; cdecl; export;
function StringLength(sz: PAnsiChar): Integer; cdecl; export;
function ToLower(sz : PAnsiChar) : PAnsiChar; cdecl; export;
function Trim(what : PAnsiChar; sz : PAnsiChar) : PAnsiChar; cdecl; export;
function Left(sz: PAnsiChar; var Number: Integer): PAnsiChar; cdecl; export;
function Right(sz: PAnsiChar; var Number: Integer): PAnsiChar; cdecl; export;
function Replace(sz : PAnsiChar; find : PAnsiChar; repl : PAnsiChar) : PAnsiChar; cdecl; export;
function SubStr(s : PAnsiChar; var m, n : Integer) : PAnsiChar; cdecl; export;
function NewGuid : PAnsiChar; cdecl; export;
function UUID_TO_CHAR(guid : PAnsiChar) : PAnsiChar; cdecl; export;
function CHAR_TO_UUID(guid : PAnsiChar) : PAnsiChar; cdecl; export;


implementation

uses StdFuncs, System.AnsiStrings, udf_glob, System.SysUtils, System.Types;

const
  WordChars: TCharSet = ['0' .. '9', 'A' .. 'Z', 'a' .. 'z'];


function Reverse(sz : PAnsiChar) : PAnsiChar;
begin
  Result := MakeResultString(PAnsiChar(System.AnsiStrings.ReverseString(sz)), nil, 0);
end;

function StringLength(sz: PAnsiChar): Integer;
begin
  result := System.AnsiStrings.StrLen(sz);
end;

function Position(SubStr, MainStr: PAnsiChar; var i: Integer): Integer;
var
  Start: Integer;
  s : String;
begin
  Start := i;
  if Start > 0 then
    Dec(Start);
  s := String(MainStr);
  Result := s.IndexOf(String(SubStr), Start) + 1;
end;

function ToLower(sz : PAnsiChar) : PAnsiChar; cdecl; export;
var
  temp : AnsiString;
begin
  temp := AnsiString(System.SysUtils.LowerCase(String(sz), loUserLocale));
  result := MakeResultString(PAnsiChar(temp), nil, 0);
end;

function Trim(what : PAnsiChar; sz : PAnsiChar) : PAnsiChar;
var
  temp : AnsiString;
  sWhat : String;
begin
  sWhat := String(what).ToUpper.Trim;
  if sWhat.Equals('BOTH') then
    temp := System.AnsiStrings.Trim(sz)
  else
    if sWhat.Equals('LEADING') then
      temp := System.AnsiStrings.TrimLeft(sz)
    else
      temp := System.AnsiStrings.TrimRight(sz);
  Result := MakeResultString(PAnsiChar(temp), nil, 0);
end;

function Left(sz: PAnsiChar; var Number: Integer): PAnsiChar;
begin
  result := MakeResultString(PAnsiChar(Copy(AnsiString(sz), 1, Number)), nil, 0);
end;

function Right(sz: PAnsiChar; var Number: Integer): PAnsiChar;
begin
  result := MakeResultString(PAnsiChar(Copy(AnsiString(sz),
    Length(AnsiString(sz)) - Number + 1, Number)), nil, 0);
end;

function Replace(sz : PAnsiChar; find : PAnsiChar; repl : PAnsiChar) : PAnsiChar;
begin
   Result := MakeResultString(PAnsiChar(System.AnsiStrings.StringReplace(sz, find, repl, [rfReplaceAll])), nil, 0);
end;

{===============================================================
 fn_substr(s, m, n) - Returns the substr starting m and n length in s.
================================================================= }
function SubStr(s : PAnsiChar; var m, n : Integer) : PAnsiChar;
var
  temp : String;
begin
  if (m < 1) or (n < 1) then
  begin
    temp := 'Bad parameters in substring';
    result := MakeResultString(PAnsiChar(AnsiString(temp)), nil, 0);
    exit;
  end;
  temp := String(s);
  temp := temp.Substring(m - 1, n);
  result := MakeResultString(PAnsiChar(AnsiString(temp)), nil, 0);
end;

function NewGuid : PAnsiChar;
var
  ansiStr : AnsiString;
  ba : TBytes;
begin
  ba := TGuid.NewGuid.ToByteArray;
  SetString(ansiStr, PAnsiChar(@ba[0]), Length(ba));
  Result := MakeResultString(PAnsiChar(ansiStr));
end;

function UUID_TO_CHAR(guid : PAnsiChar) : PAnsiChar; cdecl;
var
  aGuid : TGuid;
begin
  aGuid := TGuid.Create(BytesOf(guid));
  Result := MakeResultString(PAnsiChar(AnsiString(aGuid.ToString.Substring(1, 36))), nil, 0);
end;

function CHAR_TO_UUID(guid : PAnsiChar) : PAnsiChar; cdecl;
var
  aGuid : TGuid;
  ba : TBytes;
  ansiStr : AnsiString;
begin
  try
    aGuid := StringToGUID('{' + String(guid) + '}');
    ba := aGuid.ToByteArray;
    SetString(ansiStr, PAnsiChar(@ba[0]), Length(ba));
    Result := MakeResultString(PAnsiChar(ansiStr));
  except
    Result := MakeResultString('', nil, 0);
  end;
end;

end.
