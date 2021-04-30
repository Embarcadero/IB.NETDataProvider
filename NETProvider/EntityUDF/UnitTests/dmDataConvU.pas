unit dmDataConvU;

interface

uses
  SysUtils, Classes, IBX.IBStoredProc, IBX.IBCustomDataSet, IBX.IBUpdateSQL, IBX.IBSQL, DB,
  IBX.IBQuery, IBX.IBDatabase, ComparerU, IBX.IBTable, IBX.IBScript;

type

  TdmDataConv = class(TDataModule)
    IBDatabase1: TIBDatabase;
    IBTransaction1: TIBTransaction;
    IBQuery1: TIBQuery;
    IBDataSet1: TIBDataSet;
    IBSQL1: TIBSQL;
    IBUpdateSQL1: TIBUpdateSQL;
    IBStoredProc1: TIBStoredProc;
    IBTable1: TIBTable;
    scp1: TIBScript;
    scpProcs: TIBScript;
  private
    FComparer: TComparer;
    { Private declarations }
  protected
    procedure DropTestDatabase;
    procedure CreateTestDatabase( OwnerName, Password, CharSet : string;
                                  Dialect, PageSize : integer);
    procedure ConnectToDatabase( Dialect : integer; CharacterSet : String = '';
       user : string = ''; password : string = '');
    procedure ActivateTransaction;
    procedure CreateTestTable ( const TestTableName, FieldType : String);
    procedure DropTestTable (const TestTableName : String);
  public
    { Public declarations }
    constructor Create(AOwner : TComponent); override;
    property Comparer : TComparer read FComparer;
  end;

var
  CharLength : integer = 0;
  DBFileName : string = 'test.ib';

implementation

uses test_env, Windows;

{$R *.dfm}

{ TDataModule1 }

procedure TdmDataConv.ActivateTransaction;
begin
  IBTransaction1.DefaultDatabase := IBDatabase1;
  IBTransaction1.Params.Clear;
  IBTransaction1.Params.append('read_committed');
  IBTransaction1.Params.append('rec_version');
  IBTransaction1.Params.append('nowait');
  IBTransaction1.DefaultAction := TARollback;
  IBTransaction1.StartTransaction;
end;

procedure TdmDataConv.ConnectToDatabase(Dialect: integer;
   CharacterSet, user, password : String);
begin
  IBDatabase1.Connected := false;
  IBDatabase1.DataBaseName := WHERE_GDB + DBFileName;
  IBDatabase1.Params.Clear;
  if user = '' then
    IBDatabase1.Params.append('user_name=sysdba')
  else
    IBDatabase1.Params.append('user_name=' + user);
  if password = '' then
    IBDatabase1.Params.append('password=masterkey')
  else
    IBDatabase1.Params.append('password=' + password);
  if CharacterSet <> '' then
    IBDatabase1.Params.append('lc_ctype=' + CharacterSet);
  IBDatabase1.SQLDialect := Dialect;
  IBDatabase1.LoginPrompt := false;
  IBDatabase1.Open;
end;

constructor TdmDataConv.Create(AOwner: TComponent);
begin
  inherited;
  FComparer := TComparer.Create(self);
  if not FileExists(PATH_TO_GDB + DBFileName) then
  begin
    CreateTestDatabase('sysdba', 'masterkey', 'NONE', 3, 8192);
    IBTransaction1.StartTransaction;
    scp1.ExecuteScript;
    scpProcs.ExecuteScript;
    IBTransaction1.Commit;
  end
  else
    ConnectToDatabase(3);
end;

procedure TdmDataConv.CreateTestDatabase(OwnerName, Password, CharSet: string;
  Dialect, PageSize: integer);
begin
  if FileExists(PATH_TO_GDB + DBFileName) then
    DeleteFile(PWideChar(PATH_TO_GDB + DBFileName));
  IBDatabase1.DataBaseName := WHERE_GDB + DBFileName;
  IBDatabase1.Params.Clear;
  IBDatabase1.Params.append('USER ''' + OwnerName + '''');
  IBDatabase1.Params.append('PASSWORD ''' + Password + '''');
  if PageSize > 0 then
     IBDatabase1.Params.append('PAGE_SIZE = ' + IntToStr(PageSize) );
  if CharSet <> '' then
     IBDatabase1.Params.append('DEFAULT CHARACTER SET ' + CharSet);
  IBDatabase1.SQLDialect := Dialect;
  IBDatabase1.LoginPrompt := false;
  IBDatabase1.ServerType := Server_Type;

  IBDatabase1.CreateDatabase;
  if Charset <> 'NONE' then
  begin
    IBDatabase1.Connected := false;
    IBDatabase1.Params.Clear;
    IBDatabase1.Params.append('USER_name=' + OwnerName);
    IBDatabase1.Params.append('PASSWORD=' + Password );
    IBDatabase1.Params.append('lc_ctype=' + CharSet);
    IBDatabase1.Connected := true;
  end;

end;

procedure TdmDataConv.CreateTestTable(const TestTableName, FieldType: String);
begin
  if IBQuery1.Active then
    IBQuery1.Close;
  IBQuery1.Transaction := IBTransaction1;
  IBQuery1.Database := IBDatabase1;
  IBQuery1.SQL.Text := 'Create table ' + TestTableName + ' ( id SmallInt, Datum ' + FieldType + ' )';
  IBQuery1.ExecSQL;
  IBTransaction1.CommitRetaining;
end;

procedure TdmDataConv.DropTestDatabase;
begin
  ConnectToDatabase(3);

  IBDatabase1.DropDatabase;
end;

procedure TdmDataConv.DropTestTable(const TestTableName: String);
begin
  if IBSQL1.Open then
    IBSQL1.Close;
  IBSQL1.Transaction := IBTransaction1;
  IBSQL1.Database := IBDatabase1;
  IBSQL1.SQL.Text := 'Drop table ' + TestTableName;
  IBSQL1.ExecQuery;
  IBTransaction1.CommitRetaining;
end;

end.
