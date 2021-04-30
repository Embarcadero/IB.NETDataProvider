unit ComparerU;

interface

uses Classes, SysUtils;

type
  TComparer = class(TComponent)
  private
    FWarnings : TStringList;
    FErrors : TStringList;
    FComments : TStringList;
    function InternalFloatCompare(Expected,Received, epsilon: double; const theDiagnosis : string): boolean;
    function InternalVariantCompare(Expected, Received: Variant; TypeSensitive: Boolean): Boolean;
  public
    constructor Create(AOwner : TComponent); override;
    destructor Destroy; override;

    procedure LogComment(const aDiagnosis: String);
    procedure LogWarning(const aWarning: String);
    procedure LogFailure(const aFailure: String);
    function Int64Compare(Expected, Received: Int64; const theDiagnosis: string): boolean;
    function IntegerCompare(Expected, Received: longint; const theDiagnosis: string): boolean;
    function BooleanCompare(Expected,Received: LongBool; const theDiagnosis: string): boolean; overload;
    function BooleanCompare(Expected,Received: Boolean; const theDiagnosis: string): boolean; overload;
    function StringCompare(const Expected,Received : AnsiString; theDiagnosis : string): boolean; overload;
    function StringCompare(const Expected,Received : String; theDiagnosis : string): boolean; overload;
    function StringICompare(const Expected,Received : AnsiString; theDiagnosis: String): boolean;
    function WideStringCompare(const Expected,Received: WideString; theDiagnosis : string): boolean;
    function PWideCharCompare(Expected, Received: PWideChar; const theDiagnosis: string): boolean;
    function SubStringCompare(const little, big, theDiagnosis: String): Boolean;
    function CharCompare(Expected,Received: AnsiChar; const theDiagnosis: string): boolean;
    function WideCharCompare(Expected,Received: WideChar; const theDiagnosis: string): boolean;
    function PAnsiCharCompare(Expected, Received: PAnsiChar; sensitive: Boolean; const theDiagnosis: string): boolean;
    function AnsiStringCompare(Expected, Received: AnsiString; const theDiagnosis: string): boolean;
    function VariantCompare(Expected, Received: variant; TypeSensitive: Boolean; const theDiagnosis: string): boolean;
    function VarArrayCompare(Expected, Received: Variant; TypeSensitive: Boolean; const theDiagnosis: string): Boolean;
    function RealCompare(Expected, Received, Epsilon: Real; const theDiagnosis: string): Boolean;
    function SingleCompare(Expected, Received, Epsilon: Single; const theDiagnosis: string): Boolean;
    function DoubleCompare(Expected, Received, Epsilon: Double; const theDiagnosis: string): Boolean;
    function CompCompare(Expected, Received: Comp; Epsilon: Comp; const theDiagnosis: string): Boolean;
    function CurrencyCompare(Expected, Received: Currency; const theDiagnosis: string): Boolean;
    function ExtendedCompare(Expected, Received, Epsilon: Extended; const theDiagnosis: string): Boolean;
    function PointerCompare(Expected,Received: Pointer; const theDiagnosis: string): boolean;
    function FileCompare(FileName1, FileName2: String; const theDiagnosis: string): Boolean;
    function BufCompare(a,b: pointer; numbytes: word; const theDiagnosis : string): boolean;
    function ScanCompare(var Big, Small; BytesEach, Count: Word; const theDiagnosis: String): Boolean;
    function NotZero(var Expected; const theDiagnosis: string): Boolean;
    function AndCompare(Expected, Received: Longint; const theDiagnosis: String): Boolean;
    function BytesCompare(const Expected : TBytes; Received : TBytes; theDiagnosis : string): boolean; overload;

    procedure Clear;
    property Warnings : TStringList read FWarnings;
    property Errors : TStringList read FErrors;
    property Comments : TStringList read FComments;
  end;

implementation

{ TComparer }
uses Variants, Math, Windows, System.AnsiStrings;

const
  MinShortInt = -128;
  MaxShortInt = 127;
  MinLongInt = -2147483647;
  MaxByte = 255;
  MinByte = 0;
  MaxWord = 65535;
  MinWord = 0;

  MinInt = MinLongInt;
  MinInt16 = -32768;
  MaxInt16 = 32767;
  IntSize = 4;
  MaxCardinal = $FFFFFFFF; //4294967295
  MaxLongWord = MaxCardinal;
  MinLongWord =  0;
  MaxInt64 = $7FFFFFFFFFFFFFFF; //9223372036854775807
  MinInt64 = $8000000000000000; //-9223372036854775808
  MinCardinal = 0;

  MinSingle = 1.5e-45;
  MaxSingle = 3.4e+38;
  MinDouble = 5.0e-324;
  MaxDouble = 1.7e+308;
  MinExtended = -3.6e+4951;
  MaxExtended = 1.1e+493;
  MinComp = -9.223372036854775807e+18;
  MaxComp = 9.223372036854775808e+18;

  MinReal = MinSingle;
  MaxReal = MaxSingle;
  MinReal48 =  2.9E-39;
  MaxReal48 = 1.7E+38;


function TComparer.AndCompare(Expected, Received: Integer;
  const theDiagnosis: String): Boolean;
const
  Bits : array [Boolean] of AnsiChar = ('0','1');
var
  S1, S2: String[33];
  y: byte;
begin
  Result := (Expected and Received) <> Received;
  s1 := '';
  s2 := '';
  for y := 1 to 32 do
  begin
    S1[y] := Bits[(1 and Expected) = 1];
    S2[y] := Bits[(1 and Received) = 1];
  end;
  S1[0] := AnsiChar(32);
  S2[0] := AnsiChar(32);
  if (not Result) then
    LogFailure(theDiagnosis);
end;

function TComparer.AnsiStringCompare(Expected, Received: AnsiString;
              const theDiagnosis: String): boolean;

  function ChopAnsiString(Source : AnsiString; errorpt : integer) : AnsiString;
  var
    Start, Stop, i : integer;
  begin
    i := 1;
    if errorpt > 12 then
      Start := errorpt - 12
    else
      Start := 0;
    if (Length(Source) > errorpt + 12) then
     Stop := errorpt + 12
    else
      Stop := Length(Source);
    SetLength(Result, Stop - Start);
    if Stop - Start > 0 then
    begin
      while i <= (Stop - Start) do
      begin
        Result[i] := Source[Start + i];
        Inc(i);
      end;
      Insert('Offset:'+IntToStr(errorpt)+' ', Result, 1);
    end;
  end;

var
  i, L1, L2 : integer;
begin
  {don't rely on RTL}
  i := 0;
  L1 := 0;
  L2 := 0;
  while PAnsiChar(Expected)[L1] <> #0 do
    Inc(L1);
  while PAnsiChar(Received)[L2] <> #0 do
    Inc(L2);
  while (PAnsiChar(Expected)[i] = PAnsiChar(Received)[i]) and (i <= L1) do
    Inc(i);
  Result := (L1 = L2) and (i = L1+1);
  if (not Result)  then
    LogFailure(theDiagnosis);
end;

function TComparer.BooleanCompare(Expected, Received: LongBool;
  const theDiagnosis: string): boolean;

begin
  Result := (not Expected) = (not Received);  { use Not to force 1 or 0 reduction }
  if (not Result) then
    LogFailure(theDiagnosis);
end;

function TComparer.BooleanCompare(Expected, Received: Boolean;
  const theDiagnosis: string): boolean;
begin
  Result := Expected = Received;  
  if (not Result) then
    LogFailure(theDiagnosis);
end;

function TComparer.BufCompare(a, b: pointer; numbytes: word;
  const theDiagnosis: string): boolean;
var
  X : Word;
begin
  X := 0;
  while (X < numBytes) and (PByte(A)[X] = PByte(B)[X]) do
    Inc(X);
  Result := (X = numBytes);
  if (not Result)  then
    LogFailure(theDiagnosis);
end;

function TComparer.BytesCompare(const Expected: TBytes; Received: TBytes;
  theDiagnosis: string): boolean;
var
  I: Integer;
begin
  Result := true;
  for I := 0 to Length(Expected) - 1 do
    if Expected[i] <> Received[i] then
    begin
      LogFailure(theDiagnosis);
      Result := false;
    end;
end;

function TComparer.CharCompare(Expected, Received: AnsiChar;
  const theDiagnosis: string): boolean;
begin
  Result := (Expected = Received);
  if (not Result) then
    LogFailure(theDiagnosis);
end;

procedure TComparer.Clear;
begin
  FWarnings.Clear;
  FErrors.Clear;
  FComments.Clear;
end;

function TComparer.CompCompare(Expected, Received, Epsilon: Comp;
  const theDiagnosis: string): Boolean;
begin
  Result := InternalFloatCompare(Expected, Received, Epsilon, theDiagnosis);
end;

constructor TComparer.Create(AOwner : TComponent);
begin
  inherited;
  FWarnings := TStringList.Create;
  FErrors := TStringList.Create;
  FComments := TStringList.Create;
end;

function TComparer.CurrencyCompare(Expected, Received: Currency;
  const theDiagnosis: string): Boolean;
begin
  Result := (Expected - Received) = 0;
  if (not Result) and (not Result) then
    LogFailure(theDiagnosis);
end;

destructor TComparer.Destroy;
begin
  FWarnings.Free;
  FErrors.Free;
  FComments.Free;
  inherited;
end;

function TComparer.DoubleCompare(Expected, Received, Epsilon: Double;
  const theDiagnosis: string): Boolean;
begin
  Result := InternalFloatCompare(Expected, Received, epsilon, theDiagnosis);
end;

function TComparer.ExtendedCompare(Expected, Received, Epsilon: Extended;
  const theDiagnosis: string): Boolean;
begin
  Result := InternalFloatCompare(Expected, Received, epsilon, theDiagnosis);
end;

function TComparer.FileCompare(FileName1, FileName2: String;
  const theDiagnosis: string): Boolean;
var
  F1, F2, SizeF1, SizeF2, Larger: integer;
  B1, B2: PChar;
begin
  Result := False;
  F1 := FileOpen(FileName1, fmOpenRead);
  F2 := FileOpen(FileName2, fmOpenRead);
  if (F1 < 0) or (F2 < 0) then
  begin
    LogFailure('invalid filename - test skipped');
    Exit;
  end;
  SizeF1 := FileSeek(F1, 0, 2);
  SizeF2 := FileSeek(F2, 0, 2);
  FileSeek(F1, 0, 0); //reposition file pointers to beginning of files
  FileSeek(F2, 0, 0);
  B1 := StrAlloc(SizeF1+1);
  B2 := StrAlloc(SizeF2+1);
  Larger := SizeF1;
  if SizeF2 > SizeF1 then Larger := SizeF2;
  SizeF1 := FileRead(F1, B1[0], SizeF1);
  SizeF2 := FileRead(F2, B2[0], SizeF2);
  if (SizeF1 < 0) or (SizeF2 < 0) then
  begin
    LogFailure('unsuccessful file read - test skipped');
    Exit;
  end;
  try
    Result := BufCompare(B1, B2, Larger, theDiagnosis);
  finally
    StrDispose(B1);
    StrDispose(B2);
    FileClose(F1);
    FileClose(F2);
  end;
end;

function TComparer.Int64Compare(Expected, Received: Int64;
  const theDiagnosis: string): boolean;
begin
  Result := (Expected = Received);
  if (not Result) then
    LogFailure(theDiagnosis);
end;

function TComparer.IntegerCompare(Expected, Received: Integer;
  const theDiagnosis: string): boolean;
begin
  Result := (Expected = Received);
  if (not Result) then
    Errors.Add(theDiagnosis);
end;

function TComparer.InternalFloatCompare(Expected, Received, epsilon: double;
  const theDiagnosis: string): boolean;
begin
  Result := (abs(Expected - Received) <= abs(epsilon));
  if not Result then
    LogFailure(theDiagnosis);
end;

function TComparer.InternalVariantCompare(Expected, Received: Variant;
  TypeSensitive: Boolean): Boolean;
const
  FloatTypes = [varSingle, varDouble, varCurrency];
var
  Epsilon: Extended;
begin
  //To do: float values are sensitive to formatting, so special treatment is necessary when
  //one variant is a float and the other is a string.
  Epsilon := 0.0;
  Result := True;
  if TypeSensitive then
  begin
    Result := (VarType(Expected) = VarType(Received));
  end;
  if (VarType(Expected) in FloatTypes) and (VarType(Received) in FloatTypes) then
  begin
    case MinIntValue([VarType(Expected), VarType(Received)]) of
      varSingle: Epsilon := Received / 1e7; //7 SD for single
      varDouble: Epsilon := Received / 1e15; //15 SD for double
      varCurrency: Epsilon := Received / 1e19; //19 SD for Currency
    end;
    Result := Result and (abs(Expected - Received) <= abs(epsilon));
  end
  else
    Result := Result and (Expected = Received);
end;

procedure TComparer.LogComment(const aDiagnosis: String);
begin
  FComments.Add(aDiagnosis)
end;

procedure TComparer.LogFailure(const aFailure: String);
begin
  FErrors.Add(aFailure);
end;

procedure TComparer.LogWarning(const aWarning: String);
begin
  FWarnings.Add(aWarning);
end;

function TComparer.NotZero(var Expected; const theDiagnosis: string): Boolean;
begin
  Result := Longint(Expected) <> 0;
  if (not Result) then
    LogFailure(theDiagnosis);
end;

function TComparer.PAnsiCharCompare(Expected, Received: PAnsiChar; sensitive: Boolean;
  const theDiagnosis: string): boolean;
var
  L1, L2, i: integer;

  function PCharToString(Source : PAnsiChar; errorpt : Cardinal) : AnsiString;
  var
    Start, Stop, i : integer;
  begin
    i := 1;
    if errorpt > 12 then
      Start := errorpt - 12
    else
      Start := 0;
    if (System.AnsiStrings.StrLen(Source) > errorpt + 12) then
      Stop := errorpt + 12
    else
      Stop := System.AnsiStrings.StrLen(Source);
    SetLength(Result, Stop - Start);
    if Stop - Start > 0 then
    begin
      while i <= (Stop - Start) do
      begin
        Result[i] := Source[Start + i - 1];
        Inc(i);
      end;
      Insert('Offset:'+IntToStr(errorpt)+' ', Result, 1);
    end;
  end;

begin
  Result := False;
  i := 0;
  if (Expected<>nil) and (Received<>nil) then
  begin
    L1 := 0; L2 := 0;
    while Expected[L1]<>#0 do
      Inc(L1);
    while Received[L2]<>#0 do
      Inc(L2);
    { start by assuming strings are equal - including null terminator }
    if sensitive then
      while (Expected[i] = Received[i]) and (i <= L1) do
        Inc(i)
    else
      while (UpCase(Expected[i]) = UpCase(Received[i])) and (i <= L1) do
        Inc(i);
    Result := (L1 = L2) and (i = L1+1);
  end;
  if (not Result) then
    LogFailure(theDiagnosis);
end;

function TComparer.PointerCompare(Expected, Received: Pointer;
  const theDiagnosis: string): boolean;
begin
  Result := (Expected = Received);
  if (not Result) then
    LogFailure(theDiagnosis);
end;

function TComparer.PWideCharCompare(Expected, Received: PWideChar;
  const theDiagnosis: string): boolean;
var
  L1, L2, i: integer;

  function PWideCharToWideString(Source : PWideChar; errorpt : integer) : WideString;
  var
    Start, Stop, i : integer;
  begin
    i := 1;
    if errorpt > 12 then
      Start := errorpt - 12
    else
      Start := 0;
    if (Length(Source) > errorpt + 12) then
      Stop := errorpt + 12
    else
      Stop := Length(Source);
    SetLength(Result, Stop - Start);
    if Stop - Start > 0 then
    begin
      while i <= (Stop - Start) do
      begin
        Result[i] := Source[Start + i - 1];
        Inc(i);
      end;
      Insert('Offset:'+IntToStr(errorpt)+' ', Result, 1);
    end;
  end;

begin
  Result := False;
  i := 0;  
  if (Expected<>nil) and (Received<>nil) then
  begin
    L1 := 0;
    L2 := 0;
    while Expected[L1]<>#0 do
      Inc(L1);
    while Received[L2]<>#0 do
      Inc(L2);
    { start by assuming strings are equal - including null terminator }
    while (Expected[i] = Received[i]) and (i <= L1) do
      Inc(i);
    Result := (L1 = L2) and (i = L1+1);
  end;
  if (not Result)  then
    LogFailure(theDiagnosis);
end;

function TComparer.RealCompare(Expected, Received, Epsilon: Real;
  const theDiagnosis: string): Boolean;
begin
  Result := InternalFloatCompare(Expected, Received, epsilon, theDiagnosis);
end;

type
  EScanMismatch = class(Exception);

function TComparer.ScanCompare(var Big, Small; BytesEach, Count: Word;
  const theDiagnosis: String): Boolean;
type BA = array [0..65520] of Byte;
     CA = array [0..65520] of AnsiChar;
var
  S1,S2: string;
  X, Y : Word;
begin
  Result := True;
  try
    for X := 0 to Count-1 do
      for Y := 0 to BytesEach-1 do
        if BA(Big)[X*BytesEach + Y] <> BA(Small)[Y] then
          raise EScanMismatch.Create('scan mismatch');
  except
  end;
  if (not Result) then
    LogFailure(theDiagnosis + ' ' + S1 + ' ' + S2);
end;

function TComparer.SingleCompare(Expected, Received, Epsilon: Single;
  const theDiagnosis: string): Boolean;
begin
  Result := InternalFloatCompare(Expected, Received, epsilon, theDiagnosis);
end;

function TComparer.StringCompare(const Expected, Received : AnsiString;
  theDiagnosis: string): boolean;
begin
  Result := Expected = Received;
  if (not Result) then
    LogFailure(theDiagnosis);
end;

function TComparer.StringCompare(const Expected, Received: String;
  theDiagnosis: string): boolean;
begin
  Result := Expected = Received;
  if (not Result) then
    LogFailure(theDiagnosis);
end;

function TComparer.StringICompare(const Expected, Received : AnsiString;
  theDiagnosis: String): boolean;
begin
  Result := CompareText(String(Expected), String(Received)) = 0;
  if (not Result) then
    LogFailure(theDiagnosis);
end;

function TComparer.SubStringCompare(const little, big,
  theDiagnosis: String): Boolean;
begin
  Result := (Pos(Little, Big) <> 0);
  if (not Result) then
    LogFailure(theDiagnosis);
end;

function TComparer.VarArrayCompare(Expected, Received: Variant;
  TypeSensitive: Boolean; const theDiagnosis: string): Boolean;
var
  I: Integer;
begin
  if (VarArrayDimCount(Expected) > 1) or (VarArrayDimCount(Received) > 1) then
    LogFailure('Variant arrays must be 1-dimensional');
    //for now...
  Result := VarArrayHighBound(Expected, 1) = VarArrayHighBound(Received, 1);
  if not Result then
    LogFailure(theDiagnosis)
  else
  begin
    for I := 0 to VarArrayHighBound(Expected, 1)-1 do
    begin
      Result := InternalVariantCompare(Expected[I], Received[I], TypeSensitive);
    end;
    if (not Result) then
      LogFailure(theDiagnosis);
  end;
end;

function TComparer.VariantCompare(Expected, Received: variant;
  TypeSensitive: Boolean; const theDiagnosis: string): boolean;
begin
  Result := InternalVariantCompare(Expected, Received, TypeSensitive);
  if (not Result) then
    LogFailure(theDiagnosis);
end;

function TComparer.WideCharCompare(Expected, Received: WideChar;
  const theDiagnosis: string): boolean;
begin
  Result := Expected = Received;
  if (not Result) then
    LogFailure(theDiagnosis);
end;

function TComparer.WideStringCompare(const Expected, Received: WideString;
  theDiagnosis: string): boolean;
begin
  Result := Expected = Received;
  if (not Result) then
    LogFailure(theDiagnosis);
end;

end.
