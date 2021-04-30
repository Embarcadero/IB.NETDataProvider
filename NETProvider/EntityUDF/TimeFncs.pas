unit TimeFncs;

interface

uses
  IBX.IBHeader, IBX.IBExternals;

function UTCCurrentTime : PISC_QUAD; cdecl; export;
function DateAdd(TheUnit : PAnsiChar; var Value : Int64; ib_date : PISC_QUAD) : PISC_QUAD; cdecl; export;
function DateDiff(TheUnit : PansiChar; moment1, moment2 : PISC_QUAD) : int64; cdecl; export;

function AsDateTime(ib_date : PISC_QUAD) : TDateTime;
function AsTMStruct(aDate : TDateTime) : tm;

procedure isc_decode_date(ib_date: PISC_QUAD; tm_date: PCTimeStructure); stdcall; external IBASE_DLL;
procedure isc_encode_date(tm_date: PCTimeStructure; ib_date: PISC_QUAD); stdcall; external IBASE_DLL;

implementation

uses
  System.DateUtils, udf_glob, System.AnsiStrings, System.SysUtils;


function AsDateTime(ib_date : PISC_QUAD) : TDateTime;
var
  tm_date : tm;
begin
  InitializeTCTimeStructure(tm_date);
  isc_decode_date(ib_date, @tm_date);
  Result := EncodeDateTime(tm_date.tm_year + cYearOffset,
                       tm_date.tm_mon + 1,
                       tm_date.tm_mday, tm_date.tm_hour, tm_date.tm_min, tm_date.tm_sec, 0);
end;

function AsTMStruct(aDate : TDateTime) : tm;
var
  Yr, Mn, Dy, Hr, Mt, S, Ms: Word;
begin
  DecodeDate(aDate, Yr, Mn, Dy);
  DecodeTime(aDate, Hr, Mt, S, Ms);
  InitializeTCTimeStructure(Result);
  Result.tm_sec := S;
  Result.tm_min := Mt;
  Result.tm_hour := Hr;
  Result.tm_mday := Dy;
  Result.tm_mon := Mn - 1;
  Result.tm_year := Yr - cYearOffset;
end;

function UTCCurrentTime : PISC_QUAD;
var
  res_date : TDateTime;
  tm_date : tm;
begin
  res_date := TTimeZone.Local.ToUniversalTime(now);
  tm_date := AsTMStruct(res_date);
  result := MakeResultQuad(nil, nil);
  isc_encode_date(@tm_date, result);
end;


function DateAdd(TheUnit : PAnsiChar; var Value : Int64; ib_date : PISC_QUAD) : PISC_QUAD;
var
  dType : ansiString;
  res_date : tm;
  aDate : TDateTime;
begin
  InitializeTCTimeStructure(res_date);
  aDate := AsDateTime(ib_date);
  dType := Trim(System.AnsiStrings.UpperCase(AnsiString(TheUnit)));
  if dType = 'YEAR' then
    aDate := IncYear(aDate, Value)
  else if dType = 'MONTH' then
    aDate := IncMonth(aDate, Value)
  else if dType = 'WEEK' then
    aDate := IncWeek(aDate, Value)
  else if dType = 'DAY' then
    aDate := IncDay(aDate, Value)
  else if dType = 'HOUR' then
    aDate := IncHour(aDate, Value)
  else if dType = 'MINUTE' then
    aDate := IncMinute(aDate, Value)
  else if dType = 'SECOND' then
    aDate := IncSecond(aDate, Value);
  res_date := AsTMStruct(aDate);
  result := MakeResultQuad(nil, nil);
  isc_encode_date(@res_date, result);
end;

function DateDiff(TheUnit : PansiChar; moment1, moment2 : PISC_QUAD) : int64;
var
  dType : ansiString;
  res_date : tm;
  m1, m2 : TDateTime;
begin
  try
    InitializeTCTimeStructure(res_date);
    m1 := AsDateTime(moment1);
    m2 := AsDateTime(moment2);
    dType := Trim(System.AnsiStrings.UpperCase(AnsiString(TheUnit)));
    if dType = 'YEAR' then
      Result := YearsBetween(m1, m2)
    else if dType = 'MONTH' then
      Result := MonthsBetween(m1, m2)
    else if dType = 'WEEK' then
      Result := WeeksBetween(m1, m2)
    else if dType = 'DAY' then
      Result := DaysBetween(m1, m2)
    else if dType = 'HOUR' then
      Result := HoursBetween(m1, m2)
    else if dType = 'MINUTE' then
      Result := MinutesBetween(m1, m2)
    else if dType = 'SECOND' then
      Result := SecondsBetween(m1, m2)
    else
      Result := -1;
    if (m1 > m2) and (Result <> -1) then
      Result := Result * -1;
  except
    on e : Exception do
      Result := -666;
  end;
end;

end.
