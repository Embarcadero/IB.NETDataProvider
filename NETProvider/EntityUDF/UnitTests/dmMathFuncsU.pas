unit dmMathFuncsU;

interface

uses
  System.SysUtils, System.Classes, dmDataConvU, IBX.IBScript, IBX.IBTable, IBX.IBStoredProc,
  IBX.IBCustomDataSet, IBX.IBUpdateSQL, IBX.IBSQL, Data.DB, IBX.IBQuery, IBX.IBDatabase;

type

//  fn_BitOr,
//  fn_BitXor,
//  fn_BitNot,

  TdmMathFuncs = class(TdmDataConv)
  private
    { Private declarations }
  public
    { Public declarations }
    procedure Abs;
    procedure Ceiling;
    procedure Floor;
    procedure Power;
    procedure BitAnd;
    procedure BitOr;
    procedure BitXor;
    procedure BitNot;
    procedure fn_Mod;
    procedure BINSHL;
    procedure BINSHR;

    procedure Round;
    procedure Truncate;
  end;

var
  dmMathFuncs: TdmMathFuncs;

implementation

{%CLASSGROUP 'Vcl.Controls.TControl'}

{$R *.dfm}

{ TdmMathFuncs }

procedure TdmMathFuncs.Ceiling;
begin
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := 'select EF_Ceiling(35.689) from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.DoubleCompare(36, IBSQL1.Fields[0].AsDouble, 0.00001, 'Expected 36 Recieved ' +
    IBSQL1.Fields[0].AsString);

  IBSQL1.Close;
  IBSQL1.SQL.Text := 'select EF_Ceiling(-35.689) from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.DoubleCompare(-35, IBSQL1.Fields[0].AsDouble, 0.00001, 'Expected -35 Recieved ' +
    IBSQL1.Fields[0].AsString);
  IBTransaction1.Commit;
end;

procedure TdmMathFuncs.Abs;
begin
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := 'select EF_Abs(31.5) from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.DoubleCompare(31.5, IBSQL1.Fields[0].AsDouble, 0, 'Expected 31.5 Recieved ' +
    IBSQL1.Fields[0].AsString);
  IBSQL1.Close;
  IBSQL1.SQL.Text := 'select EF_Abs(-31.5) from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.DoubleCompare(31.5, IBSQL1.Fields[0].AsDouble, 0, 'Expected 31.5 Recieved ' +
    IBSQL1.Fields[0].AsString);
  IBSQL1.Close;
  IBSQL1.SQL.Text := 'select EF_Abs(-3100123456.5) from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.DoubleCompare(3100123456.5, IBSQL1.Fields[0].AsDouble, 0, 'Expected 3100123456.5 Recieved ' +
    IBSQL1.Fields[0].AsString);
end;

procedure TdmMathFuncs.Floor;
begin
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := 'select EF_Floor(35.689) from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.DoubleCompare(35, IBSQL1.Fields[0].AsDouble, 0.00001, 'Expected 35 Recieved ' +
    IBSQL1.Fields[0].AsString);

  IBSQL1.Close;
  IBSQL1.SQL.Text := 'select EF_Floor(-35.689) from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.DoubleCompare(-36, IBSQL1.Fields[0].AsDouble, 0.00001, 'Expected -36 Recieved ' +
    IBSQL1.Fields[0].AsString);
  IBTransaction1.Commit;
end;

procedure TdmMathFuncs.BINSHL;
begin
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := 'select EF_BINSHL(30564323213, 12) from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.Int64Compare(125191467880448, IBSQL1.Fields[0].AsInt64, 'Expected 125,191,467,880,448 Recieved ' +
    QuotedStr(IBSQL1.Fields[0].AsString));
  IBSQL1.Close;
  IBTransaction1.Commit;
end;

procedure TdmMathFuncs.BINSHR;
begin
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := 'select EF_BINSHR(30564323213, 12) from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.IntegerCompare(7461992, IBSQL1.Fields[0].AsInteger, 'Expected 7,461,992 Recieved ' +
    QuotedStr(IBSQL1.Fields[0].AsString));
  IBSQL1.Close;
  IBTransaction1.Commit;
end;

procedure TdmMathFuncs.fn_Mod;
begin
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := 'select EF_MOD(30564323213, 653) from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.IntegerCompare(642, IBSQL1.Fields[0].AsInteger, 'Expected 642 Recieved ' +
    QuotedStr(IBSQL1.Fields[0].AsString));
  IBSQL1.Close;
  IBTransaction1.Commit;
end;

procedure TdmMathFuncs.BitAnd;
begin
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := 'select EF_BitAnd(536, 653) from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.IntegerCompare(520, IBSQL1.Fields[0].AsInteger, 'Expected 520 Recieved ' +
    QuotedStr(IBSQL1.Fields[0].AsString));
  IBSQL1.Close;
  IBTransaction1.Commit;
end;

procedure TdmMathFuncs.BitNot;
begin
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := 'select EF_BitNot(536) from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.IntegerCompare(-537, IBSQL1.Fields[0].AsInteger, 'Expected -537 Recieved ' +
    QuotedStr(IBSQL1.Fields[0].AsString));
  IBSQL1.Close;
  IBTransaction1.Commit;
end;

procedure TdmMathFuncs.BitOr;
begin
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := 'select EF_BitOr(536, 653) from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.IntegerCompare(669, IBSQL1.Fields[0].AsInteger, 'Expected 669 Recieved ' +
    QuotedStr(IBSQL1.Fields[0].AsString));
  IBSQL1.Close;
  IBTransaction1.Commit;
end;

procedure TdmMathFuncs.BitXor;
begin
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := 'select EF_BitXor(536, 653) from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.IntegerCompare(149, IBSQL1.Fields[0].AsInteger, 'Expected 149 Recieved ' +
    QuotedStr(IBSQL1.Fields[0].AsString));
  IBSQL1.Close;
  IBTransaction1.Commit;
end;

procedure TdmMathFuncs.Power;
begin
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := 'select EF_Power(3, 2) from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.DoubleCompare(9, IBSQL1.Fields[0].AsDouble, 0, 'Expected 9 Recieved ' +
    IBSQL1.Fields[0].AsString);
  IBSQL1.Close;
  IBSQL1.SQL.Text := 'select EF_Power(2, 3) from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.DoubleCompare(8, IBSQL1.Fields[0].AsDouble, 0, 'Expected 8 Recieved ' +
    IBSQL1.Fields[0].AsString);
  IBTransaction1.Commit;
end;

procedure TdmMathFuncs.Round;
begin
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := 'select EF_Round(123.654, 1) from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.DoubleCompare(123.7, IBSQL1.Fields[0].AsDouble, 0, 'Expected 123.7 Recieved ' +
    IBSQL1.Fields[0].AsString);
  IBSQL1.Close;
  IBSQL1.SQL.Text := 'select EF_Round(8341.7, -3) from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.DoubleCompare(8000.0 , IBSQL1.Fields[0].AsDouble, 0, 'Expected 8000.0 Recieved ' +
    IBSQL1.Fields[0].AsString);
  IBSQL1.Close;
  IBSQL1.SQL.Text := 'select EF_Round(45.1212, 0) from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.DoubleCompare(45.0 , IBSQL1.Fields[0].AsDouble, 0, 'Expected 45.0 Recieved ' +
    IBSQL1.Fields[0].AsString);
  IBTransaction1.Commit;
end;

procedure TdmMathFuncs.Truncate;
begin
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := 'select EF_Trunc(789.2225, 2) from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.DoubleCompare(789.22, IBSQL1.Fields[0].AsDouble, 0.0001, 'Expected 789.22 Recieved ' +
    IBSQL1.Fields[0].AsString);
  IBSQL1.Close;
  IBSQL1.SQL.Text := 'select EF_Trunc(345.4, -2) from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.DoubleCompare(300, IBSQL1.Fields[0].AsDouble, 0.0001, 'Expected 300 Recieved ' +
    IBSQL1.Fields[0].AsString);
  IBSQL1.Close;
  IBSQL1.SQL.Text := 'select EF_Trunc(-163.41, 0) from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.DoubleCompare(-163, IBSQL1.Fields[0].AsDouble, 0.0001, 'Expected -163 Recieved ' +
    IBSQL1.Fields[0].AsString);
  IBTransaction1.Commit;
end;

end.
