unit dmTimeFuncsU;

interface

uses
  System.SysUtils, System.Classes, dmDataConvU, IBX.IBScript, IBX.IBTable, IBX.IBStoredProc,
  IBX.IBCustomDataSet, IBX.IBUpdateSQL, IBX.IBSQL, Data.DB, IBX.IBQuery,IBX. IBDatabase;

type
  TdmTimefuncs = class(TdmDataConv)
  private
    { Private declarations }
  public
    { Public declarations }
    procedure UTCCurrentTime;
    procedure AddYear;
    procedure AddMonth;
    procedure AddWeek;
    procedure AddDay;
    procedure AddHour;
    procedure AddMinute;
    procedure AddSecond;

    procedure DiffYear;
    procedure DiffMonth;
    procedure DiffWeek;
    procedure DiffDay;
    procedure DiffHour;
    procedure DiffMinute;
    procedure DiffSecond;
  end;

implementation

uses System.DateUtils;

{%CLASSGROUP 'Vcl.Controls.TControl'}

{$R *.dfm}

{ TdmTimefuncs }

const
  sDateDiffSQL = 'SELECT EF_DateDiff(''%s'', ''%s'', ''%s'') from RDB$DATABASE';

procedure TdmTimefuncs.UTCCurrentTime;
var
  d : TDateTime;
begin
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := 'SELECT EF_UTCCurrentTime() from RDB$DATABASE';
  d := TTimeZone.Local.ToUniversalTime(now);
  IBSQL1.ExecQuery;
  Comparer.DoubleCompare(d, IBSQL1.Fields[0].AsDateTime, 0.001, 'Expected ' + DateTimeToStr(d)
    + ' Recieved ' + IBSQL1.Fields[0].AsString);
  IBTransaction1.Rollback;
end;


procedure TdmTimefuncs.AddMonth;
var
  d : TDateTime;
begin
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := 'SELECT EF_DateAdd(''Month'', 1, cast(''1998-01-01'' as timestamp)) from RDB$DATABASE';
  d := StrToDateTime('1/1/1998');
  IBSQL1.ExecQuery;
  Comparer.DoubleCompare(IncMonth(d, 1), IBSQL1.Fields[0].AsDateTime, 0, 'Expected ' +
    DateTimeToStr(IncMonth(d)) + ' Recieved ' + IBSQL1.Fields[0].AsString);
  IBSQL1.Close;
  IBSQL1.SQL.Text := 'SELECT EF_DateAdd(''Month'', -1, cast(''1998-01-01'' as timestamp)) from RDB$DATABASE';
  IBSQL1.ExecQuery;
  Comparer.DoubleCompare(IncMonth(d, -1), IBSQL1.Fields[0].AsDateTime, 0, 'Expected ' +
    DateTimeToStr(IncMonth(d)) + ' Recieved ' + IBSQL1.Fields[0].AsString);
  IBTransaction1.Rollback;
end;

procedure TdmTimefuncs.AddYear;
var
  d : TDateTime;
begin
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := 'SELECT EF_DateAdd(''Year'', 1, cast(''1998-01-01'' as timestamp)) from RDB$DATABASE';
  d := StrToDateTime('1/1/1998');
  IBSQL1.ExecQuery;
  Comparer.DoubleCompare(IncYear(d, 1), IBSQL1.Fields[0].AsDateTime, 0, 'Expected ' +
    DateTimeToStr(IncYear(d, 1)) + ' Recieved ' + IBSQL1.Fields[0].AsString);
  IBSQL1.Close;
  IBSQL1.SQL.Text := 'SELECT EF_DateAdd(''Year'', -1, cast(''1998-01-01'' as timestamp)) from RDB$DATABASE';
  IBSQL1.ExecQuery;
  Comparer.DoubleCompare(IncYear(d, -1), IBSQL1.Fields[0].AsDateTime, 0, 'Expected ' +
    DateTimeToStr(IncYear(d, 1)) + ' Recieved ' + IBSQL1.Fields[0].AsString);
  IBSQL1.Close;
  IBTransaction1.Rollback;
end;

procedure TdmTimefuncs.AddWeek;
var
  d : TDateTime;
begin
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := 'SELECT EF_DateAdd(''Week'', 1, cast(''1998-01-01'' as timestamp)) from RDB$DATABASE';
  d := StrToDateTime('1/1/1998');
  IBSQL1.ExecQuery;
  Comparer.DoubleCompare(IncWeek(d, 1), IBSQL1.Fields[0].AsDateTime, 0, 'Expected ' +
    DateTimeToStr(IncWeek(d)) + ' Recieved ' + IBSQL1.Fields[0].AsString);
  IBSQL1.Close;
  IBSQL1.SQL.Text := 'SELECT EF_DateAdd(''Week'', -1, cast(''1998-01-01'' as timestamp)) from RDB$DATABASE';
  IBSQL1.ExecQuery;
  Comparer.DoubleCompare(IncWeek(d, -1), IBSQL1.Fields[0].AsDateTime, 0, 'Expected ' +
    DateTimeToStr(IncWeek(d)) + ' Recieved ' + IBSQL1.Fields[0].AsString);
  IBTransaction1.Rollback;
end;

procedure TdmTimefuncs.AddDay;
var
  d : TDateTime;
begin
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := 'SELECT EF_DateAdd(''Day'', 1, cast(''1998-01-01'' as timestamp)) from RDB$DATABASE';
  d := StrToDateTime('1/1/1998');
  IBSQL1.ExecQuery;
  Comparer.DoubleCompare(IncDay(d, 1), IBSQL1.Fields[0].AsDateTime, 0, 'Expected ' +
    DateTimeToStr(IncDay(d)) + ' Recieved ' + IBSQL1.Fields[0].AsString);
  IBSQL1.Close;
  IBSQL1.SQL.Text := 'SELECT EF_DateAdd(''Day'', -1, cast(''1998-01-01'' as timestamp)) from RDB$DATABASE';
  IBSQL1.ExecQuery;
  Comparer.DoubleCompare(IncDay(d, -1), IBSQL1.Fields[0].AsDateTime, 0, 'Expected ' +
    DateTimeToStr(IncDay(d)) + ' Recieved ' + IBSQL1.Fields[0].AsString);
  IBTransaction1.Rollback;
end;

procedure TdmTimefuncs.AddHour;
var
  d : TDateTime;
begin
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := 'SELECT EF_DateAdd(''Hour'', 1, cast(''1998-01-01'' as timestamp)) from RDB$DATABASE';
  d := StrToDateTime('1/1/1998');
  IBSQL1.ExecQuery;
  Comparer.DoubleCompare(IncHour(d, 1), IBSQL1.Fields[0].AsDateTime, 0, 'Expected ' +
    DateTimeToStr(IncHour(d)) + ' Recieved ' + IBSQL1.Fields[0].AsString);
  IBSQL1.Close;
  IBSQL1.SQL.Text := 'SELECT EF_DateAdd(''Hour'', -1, cast(''1998-01-01'' as timestamp)) from RDB$DATABASE';
  IBSQL1.ExecQuery;
  Comparer.DoubleCompare(IncHour(d, -1), IBSQL1.Fields[0].AsDateTime, 0, 'Expected ' +
    DateTimeToStr(IncHour(d)) + ' Recieved ' + IBSQL1.Fields[0].AsString);
  IBTransaction1.Rollback;
end;

procedure TdmTimefuncs.AddMinute;
var
  d : TDateTime;
begin
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := 'SELECT EF_DateAdd(''Minute'', 1, cast(''1998-01-01'' as timestamp)) from RDB$DATABASE';
  d := StrToDateTime('1/1/1998');
  IBSQL1.ExecQuery;
  Comparer.DoubleCompare(IncMinute(d, 1), IBSQL1.Fields[0].AsDateTime, 0, 'Expected ' +
    DateTimeToStr(IncMinute(d)) + ' Recieved ' + IBSQL1.Fields[0].AsString);
  IBSQL1.Close;
  IBSQL1.SQL.Text := 'SELECT EF_DateAdd(''Minute'', -1, cast(''1998-01-01'' as timestamp)) from RDB$DATABASE';
  IBSQL1.ExecQuery;
  Comparer.DoubleCompare(IncMinute(d, -1), IBSQL1.Fields[0].AsDateTime, 0, 'Expected ' +
    DateTimeToStr(IncMinute(d)) + ' Recieved ' + IBSQL1.Fields[0].AsString);
  IBTransaction1.Rollback;
end;

procedure TdmTimefuncs.AddSecond;
var
  d : TDateTime;
begin
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := 'SELECT EF_DateAdd(''Second'', 1, cast(''1998-01-01'' as timestamp)) from RDB$DATABASE';
  d := StrToDateTime('1/1/1998');
  IBSQL1.ExecQuery;
  Comparer.DoubleCompare(IncSecond(d, 1), IBSQL1.Fields[0].AsDateTime, 0, 'Expected ' +
    DateTimeToStr(IncSecond(d)) + ' Recieved ' + IBSQL1.Fields[0].AsString);
  IBSQL1.Close;
  IBSQL1.SQL.Text := 'SELECT EF_DateAdd(''Second'', -1, cast(''1998-01-01'' as timestamp)) from RDB$DATABASE';
  IBSQL1.ExecQuery;
  Comparer.DoubleCompare(IncSecond(d, -1), IBSQL1.Fields[0].AsDateTime, 0, 'Expected ' +
    DateTimeToStr(IncSecond(d)) + ' Recieved ' + IBSQL1.Fields[0].AsString);
  IBTransaction1.Rollback;
end;

procedure TdmTimefuncs.DiffYear;
var
  d1 : TDateTime;
  aType : String;
  s, s1 : String;
begin
  d1 := now;
  aType := 'Year';
  DateTimeToString(s, 'yyyy-m-d hh:nn:ss.zzz', d1);
  DateTimeToString(s1, 'yyyy-m-d hh:nn:ss.zzz', IncDay(IncYear(d1, 5), 1));
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := format(sDateDiffSQL, [aType, s, s1]);
  IBSQL1.ExecQuery;
  Comparer.IntegerCompare(5, IBSQL1.Fields[0].AsInteger, 'Expected 5 Recieved ' +
    IBSQL1.Fields[0].AsString);
  IBSQL1.Close;
  DateTimeToString(s1, 'yyyy-m-d hh:nn:ss.zzz', IncDay(IncYear(d1, -5), -1));
  IBSQL1.SQL.Text := format(sDateDiffSQL, [aType, s, s1]);
  IBSQL1.ExecQuery;
  Comparer.IntegerCompare(-5, IBSQL1.Fields[0].AsInteger, 'Expected -5 Recieved ' +
    IBSQL1.Fields[0].AsString);
  IBTransaction1.Rollback;
end;

procedure TdmTimefuncs.DiffMonth;
var
  d1 : TDateTime;
  aType : String;
  s, s1 : String;
begin
  d1 := now;
  aType := 'Month';
  DateTimeToString(s, 'yyyy-m-d hh:nn:ss.zzz', d1);
  DateTimeToString(s1, 'yyyy-m-d hh:nn:ss.zzz', IncDay(IncMonth(d1, 5), 4));
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := format(sDateDiffSQL, [aType, s, s1]);
  IBSQL1.ExecQuery;
  Comparer.IntegerCompare(5, IBSQL1.Fields[0].AsInteger, 'Expected 5 Recieved ' +
    IBSQL1.Fields[0].AsString);
  IBSQL1.Close;
  DateTimeToString(s1, 'yyyy-m-d hh:nn:ss.zzz', IncDay(IncMonth(d1, -5), -4));
  IBSQL1.SQL.Text := format(sDateDiffSQL, [aType, s, s1]);
  IBSQL1.ExecQuery;
  Comparer.IntegerCompare(-5, IBSQL1.Fields[0].AsInteger, 'Expected -5 Recieved ' +
    IBSQL1.Fields[0].AsString);
  IBTransaction1.Rollback;
end;

procedure TdmTimefuncs.DiffWeek;
var
  d1 : TDateTime;
  aType : String;
  s, s1 : String;
begin
  d1 := now;
  aType := 'Week';
  DateTimeToString(s, 'yyyy-m-d hh:nn:ss.zzz', d1);
  DateTimeToString(s1, 'yyyy-m-d hh:nn:ss.zzz', IncDay(IncWeek(d1, 5), 1));
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := format(sDateDiffSQL, [aType, s, s1]);
  IBSQL1.ExecQuery;
  Comparer.IntegerCompare(5, IBSQL1.Fields[0].AsInteger, 'Expected 5 Recieved ' +
    IBSQL1.Fields[0].AsString);
  IBSQL1.Close;
  DateTimeToString(s1, 'yyyy-m-d hh:nn:ss.zzz', IncDay(IncWeek(d1, -5), -1));
  IBSQL1.SQL.Text := format(sDateDiffSQL, [aType, s, s1]);
  IBSQL1.ExecQuery;
  Comparer.IntegerCompare(-5, IBSQL1.Fields[0].AsInteger, 'Expected -5 Recieved ' +
    IBSQL1.Fields[0].AsString);
  IBTransaction1.Rollback;
end;

procedure TdmTimefuncs.DiffDay;
var
  d1 : TDateTime;
  aType : String;
  s, s1 : String;
begin
  d1 := now;
  aType := 'Day';
  DateTimeToString(s, 'yyyy-m-d hh:nn:ss.zzz', d1);
  DateTimeToString(s1, 'yyyy-m-d hh:nn:ss.zzz', IncHour(IncDay(d1, 5), 10));
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := format(sDateDiffSQL, [aType, s, s1]);
  IBSQL1.ExecQuery;
  Comparer.IntegerCompare(5, IBSQL1.Fields[0].AsInteger, 'Expected 5 Recieved ' +
    IBSQL1.Fields[0].AsString);
  IBSQL1.Close;
  DateTimeToString(s1, 'yyyy-m-d hh:nn:ss.zzz', IncHour(IncDay(d1, -5), -10));
  IBSQL1.SQL.Text := format(sDateDiffSQL, [aType, s, s1]);
  IBSQL1.ExecQuery;
  Comparer.IntegerCompare(-5, IBSQL1.Fields[0].AsInteger, 'Expected -5 Recieved ' +
    IBSQL1.Fields[0].AsString);
  IBTransaction1.Rollback;
end;

procedure TdmTimefuncs.DiffHour;
var
  d1 : TDateTime;
  aType : String;
  s, s1 : String;
begin
  d1 := now;
  aType := 'Hour';
  DateTimeToString(s, 'yyyy-m-d hh:nn:ss.zzz', d1);
  DateTimeToString(s1, 'yyyy-m-d hh:nn:ss.zzz', IncMinute(IncHour(d1, 5), 10));
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := format(sDateDiffSQL, [aType, s, s1]);
  IBSQL1.ExecQuery;
  Comparer.IntegerCompare(5, IBSQL1.Fields[0].AsInteger, 'Expected 5 Recieved ' +
    IBSQL1.Fields[0].AsString);
  IBSQL1.Close;
  DateTimeToString(s1, 'yyyy-m-d hh:nn:ss.zzz', IncMinute(IncHour(d1, -5), -10));
  IBSQL1.SQL.Text := format(sDateDiffSQL, [aType, s, s1]);
  IBSQL1.ExecQuery;
  Comparer.IntegerCompare(-5, IBSQL1.Fields[0].AsInteger, 'Expected -5 Recieved ' +
    IBSQL1.Fields[0].AsString);
  IBTransaction1.Rollback;
end;

procedure TdmTimefuncs.DiffMinute;
var
  d1 : TDateTime;
  aType : String;
  s, s1 : String;
begin
  d1 := now;
  aType := 'Minute';
  DateTimeToString(s, 'yyyy-m-d hh:nn:ss.zzz', d1);
  DateTimeToString(s1, 'yyyy-m-d hh:nn:ss.zzz', IncSecond(IncMinute(d1, 5), 10));
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := format(sDateDiffSQL, [aType, s, s1]);
  IBSQL1.ExecQuery;
  Comparer.IntegerCompare(5, IBSQL1.Fields[0].AsInteger, 'Expected 5 Recieved ' +
    IBSQL1.Fields[0].AsString);
  IBSQL1.Close;
  DateTimeToString(s1, 'yyyy-m-d hh:nn:ss.zzz', IncSecond(IncMinute(d1, -5), -10));
  IBSQL1.SQL.Text := format(sDateDiffSQL, [aType, s, s1]);
  IBSQL1.ExecQuery;
  Comparer.IntegerCompare(-5, IBSQL1.Fields[0].AsInteger, 'Expected -5 Recieved ' +
    IBSQL1.Fields[0].AsString);
  IBTransaction1.Rollback;
end;

procedure TdmTimefuncs.DiffSecond;
var
  d1 : TDateTime;
  aType : String;
  s, s1 : String;
begin
  d1 := now;
  aType := 'Second';
  DateTimeToString(s, 'yyyy-m-d hh:nn:ss.zzz', d1);
  DateTimeToString(s1, 'yyyy-m-d hh:nn:ss.zzz', IncMilliSecond(IncSecond(d1, 5), 10));
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := format(sDateDiffSQL, [aType, s, s1]);
  IBSQL1.ExecQuery;
  Comparer.IntegerCompare(5, IBSQL1.Fields[0].AsInteger, 'Expected 5 Recieved ' +
    IBSQL1.Fields[0].AsString);
  IBSQL1.Close;
  DateTimeToString(s1, 'yyyy-m-d hh:nn:ss.zzz', IncMilliSecond(IncSecond(d1, -5), -10));
  IBSQL1.SQL.Text := format(sDateDiffSQL, [aType, s, s1]);
  IBSQL1.ExecQuery;
  Comparer.IntegerCompare(-5, IBSQL1.Fields[0].AsInteger, 'Expected -5 Recieved ' +
    IBSQL1.Fields[0].AsString);
  IBTransaction1.Rollback;
end;

end.
