unit MathFncs;

interface

function fn_Abs(var Value: Double): Double; cdecl; export;
function fn_Ceiling(var Value : Double) : Int64; cdecl; export;
function fn_Floor(var Value: Double): Int64; cdecl; export;
function fn_Round(var Value : Double; var Scale : Integer): Double; cdecl; export;
function fn_Power(var Value, scale : Double) : Double; cdecl; export;
function fn_Truncate(var Value: Double; var Scale : Integer): Double; cdecl; export;

function fn_BitAnd(var op1, op2 : Int64) : Int64; cdecl; export;
function fn_BitOr(var op1, op2 : Int64) : Int64; cdecl; export;
function fn_BitXor(var op1, op2 : Int64) : Int64; cdecl; export;
function fn_BitNot(var op1 : Int64) : Int64; cdecl; export;

function fn_Mod(var op1, op2 : Int64) : Int64; cdecl; export;
function fn_BinSHL(var op1, op2 : Int64) : Int64; cdecl; export;
function fn_BinSHR(var op1, op2 : Int64) : Int64; cdecl; export;


implementation

uses System.Math;

function fn_Abs(var Value: Double): Double;
begin
  result := Abs(Value);
end;

function fn_Ceiling(var Value: Double): Int64;
begin
  Result := Ceil(Value);
end;

function fn_Floor(var Value: Double): Int64;
begin
  Result := Floor(Value);
end;

function fn_Round(var Value : Double; var Scale : Integer): Double;
begin
  Result := RoundTo(Value, -1 * Scale);
end;

function fn_Power(var Value, scale : Double) : Double;
begin
  Result := Power(Value, Scale);
end;

function fn_Truncate(var Value: Double; var Scale : Integer): Double;
begin
  Result := Double(Trunc(Value * Power(10, Scale))) * Power(10, -1 * Scale);
end;

function fn_BitAnd(var op1, op2 : Int64) : Int64;
begin
  Result := op1 and op2;
end;

function fn_BitOr(var op1, op2 : Int64) : Int64;
begin
  Result := op1 or op2;
end;

function fn_BitXor(var op1, op2 : Int64) : Int64;
begin
  Result := op1 xor op2;
end;

function fn_BitNot(var op1 : Int64) : Int64;
begin
  Result := not op1;
end;

function fn_Mod(var op1, op2 : Int64) : Int64;
begin
  Result := op1 mod op2;
end;

function fn_BinSHL(var op1, op2 : Int64) : Int64; cdecl;
begin
  Result := op1 SHL op2;
end;

function fn_BinSHR(var op1, op2 : Int64) : Int64; cdecl;
begin
  Result := op1 SHR op2;
end;


end.
