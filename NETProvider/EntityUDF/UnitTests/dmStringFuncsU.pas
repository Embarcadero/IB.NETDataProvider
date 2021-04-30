unit dmStringFuncsU;

interface

uses
  System.SysUtils, System.Classes, dmDataConvU, IBX.IBScript, IBX.IBTable, IBX.IBStoredProc,
  IBX.IBCustomDataSet, IBX.IBUpdateSQL, IBX.IBSQL, Data.DB, IBX.IBQuery, IBX.IBDatabase;

type

//  fn_substr,
//  NewGuid,

  TdmStringFuncs = class(TdmDataConv)
  private
    { Private declarations }
  public
    { Public declarations }
    procedure Reverse;
    procedure Position;
    procedure StringLength;
    procedure ToLower;
    procedure TrimLeading;
    procedure TrimTrailing;
    procedure TrimFull;
    procedure Left;
    Procedure Right;
    procedure Replace;
    procedure SubStr;
    procedure GUID;
    procedure UUID_TO_CHAR;
    procedure CHAR_TO_UUID;
  end;

implementation

{%CLASSGROUP 'Vcl.Controls.TControl'}

{$R *.dfm}
uses System.Types;

{ TdmStringFuncs }

procedure TdmStringFuncs.Replace;
begin
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := 'select Cast(EF_Replace(''Billy Wilder'',  ''il'', ''oog'') as VarChar(100)) from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.StringCompare('Boogly Woogder', IBSQL1.Fields[0].AsString.Trim, 'Expected ''Boogly Woogder'' Recieved ' +
    QuotedStr(IBSQL1.Fields[0].AsString.Trim));
  IBSQL1.Close;

  IBSQL1.SQL.Text := 'select Cast(EF_Replace(''Billy Wilder'',  ''il'', '''') as VarChar(100)) from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.StringCompare('Bly Wder', IBSQL1.Fields[0].AsString.Trim, 'Expected ''Bly Wder'' Recieved ' +
    QuotedStr(IBSQL1.Fields[0].AsString.Trim));
  IBSQL1.Close;

  IBSQL1.SQL.Text := 'select Cast(EF_Replace(''Billy Wilder'',  ''xyz'', ''abc'') as VarChar(100)) from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.StringCompare('Billy Wilder', IBSQL1.Fields[0].AsString.Trim, 'Expected ''Billy Wilder'' Recieved ' +
    QuotedStr(IBSQL1.Fields[0].AsString.Trim));
  IBSQL1.Close;

  IBSQL1.SQL.Text := 'select Cast(EF_Replace(''Billy Wilder'',  '''', ''abc'') as VarChar(100)) from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.StringCompare('Billy Wilder', IBSQL1.Fields[0].AsString.Trim, 'Expected ''Billy Wilder'' Recieved ' +
    QuotedStr(IBSQL1.Fields[0].AsString.Trim));
  IBSQL1.Close;

  IBTransaction1.Rollback;
end;

procedure TdmStringFuncs.Reverse;
begin
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := 'select EF_Reverse(''abCDef'') from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.StringCompare('feDCba', IBSQL1.Fields[0].AsString.Trim, 'Expected ''feDCba'' Recieved ' +
    QuotedStr(IBSQL1.Fields[0].AsString.Trim));
  IBTransaction1.Rollback;
end;

procedure TdmStringFuncs.Right;
begin
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := 'select Cast(EF_Right(''abCDef'', 3) as VarChar(10)) from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.StringCompare('Def', IBSQL1.Fields[0].AsString.Trim, 'Expected ''Def'' Recieved ' +
    QuotedStr(IBSQL1.Fields[0].AsString));
  IBTransaction1.Rollback;
end;

procedure TdmStringFuncs.StringLength;
begin
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := 'select EF_Length(''Hello!'') from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.IntegerCompare(6, IBSQL1.Fields[0].AsInteger, 'Expected 6 Recieved ' +
    IBSQL1.Fields[0].AsString);
  IBSQL1.Close;

  IBSQL1.SQL.Text := 'select EF_Length(''Grüß di!'') from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.IntegerCompare(8, IBSQL1.Fields[0].AsInteger, 'Expected 8 Recieved ' +
    IBSQL1.Fields[0].AsString);
  IBSQL1.Close;

  IBSQL1.SQL.Text := 'select EF_Length(Cast(''Grüß di!'' as Char(24))) from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.IntegerCompare(24, IBSQL1.Fields[0].AsInteger, 'Expected 24 Recieved ' +
    IBSQL1.Fields[0].AsString);
  IBSQL1.Close;

  IBTransaction1.Rollback;
end;

procedure TdmStringFuncs.SubStr;
begin
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := 'select Cast(EF_substr(''pinhead!'', 4, 2) as VarChar(8)) from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.StringCompare('he', IBSQL1.Fields[0].AsString, 'Expected he Recieved ' +
    IBSQL1.Fields[0].AsString);
  IBSQL1.Close;

  IBTransaction1.Rollback;
end;

procedure TdmStringFuncs.ToLower;
begin
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := 'select EF_Lower(''Hello!'') from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.StringCompare('hello!', IBSQL1.Fields[0].AsString.Trim, 'Expected ''hello!'' Recieved ' +
    QuotedStr(IBSQL1.Fields[0].AsString.Trim));
  IBSQL1.Close;

  IBSQL1.SQL.Text := 'select EF_Lower(''GrüÜ di!'') from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.StringCompare('grüü di!', IBSQL1.Fields[0].AsString.Trim, 'Expected ''grüü di!'' Recieved ' +
    QuotedStr(IBSQL1.Fields[0].AsString.Trim));
  IBSQL1.Close;
  IBTransaction1.Rollback;
end;

procedure TdmStringFuncs.TrimFull;
begin
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := 'select Cast(EF_Trim(''Both'', ''    Hello!    '') as VarChar(24)) from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.StringCompare('Hello!', IBSQL1.Fields[0].AsString, 'Expected ''Hello!'' Received ' +
    IBSQL1.Fields[0].AsString);
  IBSQL1.Close;
  IBTransaction1.Rollback;
end;

procedure TdmStringFuncs.TrimLeading;
begin
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := 'select Cast(EF_Trim(''Leading'', ''    Hello!    '') as VarChar(24)) from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.StringCompare('Hello!    ', IBSQL1.Fields[0].AsString, 'Expected ''Hello!    '' Received ' +
    IBSQL1.Fields[0].AsString);
  IBSQL1.Close;
  IBTransaction1.Rollback;
end;

procedure TdmStringFuncs.TrimTrailing;
begin
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := 'select Cast(EF_Trim(''Trailing'', ''    Hello!    '') as VarChar(24)) from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.StringCompare('    Hello!', IBSQL1.Fields[0].AsString, 'Expected ''    Hello!'' Received ' +
    IBSQL1.Fields[0].AsString);
  IBSQL1.Close;
  IBTransaction1.Rollback;
end;

procedure TdmStringFuncs.UUID_TO_CHAR;
var
  aGuid : TGuid;
begin
  aGuid := TGuid.NewGuid;
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := 'select GUIDSTR from UUIDTest(:g)';
  IBSQL1.Params[0].AsBytes := aGuid.ToByteArray();
  IBSQL1.ExecQuery;
  // Note need ot strip off the {}
  Comparer.StringCompare(aGuid.ToString.Substring(1,36), IBSQL1.FieldByName('GUIDSTR').AsString, 'Expected ' +
    aGuid.ToString.Substring(1,36) + ' Received ' + IBSQL1.FieldByName('GUIDSTR').AsString);
end;

procedure TdmStringFuncs.CHAR_TO_UUID;
var
  aGuid : TGuid;
begin
  aGuid := TGuid.NewGuid;
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := 'select EF_CHAR_TO_UUID(''' + aGuid.ToString.Substring(1,36) + ''') GUIDSTR from rdb$database';
  IBSQL1.ExecQuery;
  // Note need ot strip off the {}
  Comparer.BytesCompare(aGuid.ToByteArray, IBSQL1.FieldByName('GUIDSTR').AsBytes, 'Expected ' +
    TEncoding.ANSI.GetString(aGuid.ToByteArray) + ' Received ' +
    TEncoding.ANSI.GetString(IBSQL1.FieldByName('GUIDSTR').AsBytes));
end;

procedure TdmStringFuncs.GUID;
begin
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := 'select EF_NewGUID() from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.IntegerCompare(16, IBSQL1.Fields[0].AsString.Length, 'Expected 16 Recieved ' +
    IBSQL1.Fields[0].AsString.Length.ToString);
  try
    TGuid.Create(IBSQL1.Fields[0].AsBytes).ToString;
  except
    Comparer.LogFailure('Resulting bytes could not be converted into a GUID');
  end;
  IBTransaction1.Rollback;
end;

procedure TdmStringFuncs.Left;
begin
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := 'select Cast(EF_Left(''abCDef'', 3) as VarChar(10)) from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.StringCompare('abC', IBSQL1.Fields[0].AsString, 'Expected ''abC'' Recieved ' +
    QuotedStr(IBSQL1.Fields[0].AsString));
  IBTransaction1.Rollback;
end;

procedure TdmStringFuncs.Position;
begin
  IBTransaction1.StartTransaction;
  IBSQL1.SQL.Text := 'select EF_Length(''Hello!'') from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.IntegerCompare(6, IBSQL1.Fields[0].AsInteger, 'Expected 6 Recieved ' +
    IBSQL1.Fields[0].AsString);
  IBSQL1.Close;

  IBSQL1.SQL.Text := 'select EF_position(''be'', ''To be or not to be'', 4) from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.IntegerCompare(4, IBSQL1.Fields[0].AsInteger, 'Expected 4 Recieved ' +
    IBSQL1.Fields[0].asString);
  IBSQL1.Close;

  IBSQL1.SQL.Text := 'select EF_position(''be'', ''To be or not to be'', 8) from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.IntegerCompare(17, IBSQL1.Fields[0].AsInteger, 'Expected 17 Recieved ' +
    IBSQL1.Fields[0].asString);
  IBSQL1.Close;

  IBSQL1.SQL.Text := 'select EF_position(''be'', ''To be or not to be'', 18) from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.IntegerCompare(0, IBSQL1.Fields[0].AsInteger, 'Expected 0 Recieved ' +
    IBSQL1.Fields[0].AsString);
  IBSQL1.Close;

  IBSQL1.SQL.Text := 'select EF_position(''be'', ''Alas, poor Yorick!'', 0) from rdb$database';
  IBSQL1.ExecQuery;
  Comparer.IntegerCompare(0, IBSQL1.Fields[0].AsInteger, 'Expected 0 Recieved ' +
    IBSQL1.Fields[0].AsString);

  IBTransaction1.Rollback;
end;

end.
