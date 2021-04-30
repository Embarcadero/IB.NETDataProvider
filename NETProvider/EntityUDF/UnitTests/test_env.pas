unit test_env;

interface

uses IniFiles, SysUtils, IBX.IBUtils;

var
  WHERE_GDB: string;
  INIFileName: string;
  INIFile: TIniFile;
  PATH_TO_GDB: string;
  WHERE_SERVER: string;
  ProtocolName: string;
  SERVER_PROTOCOL: TIBProtocols;
  Server_Type: String;
  InstanceName: String;

implementation

procedure InitialGlobals;
begin
  INIFileName := ChangeFileExt(ParamStr(0), '.ini');
  INIFile := TIniFile.Create(INIFileName);
  PATH_TO_GDB := INIFile.ReadString('IB Environment', 'PATH_TO_GDB', ExtractFilePath(ParamStr(0)));
  WHERE_SERVER := INIFile.ReadString('IB Environment', 'WHERE_SERVER',
    'localhost');
  ProtocolName := INIFile.ReadString('IB Environment',
    'SERVER_PROTOCOL', 'TCP');
  Server_Type := INIFile.ReadString('IB Environment', 'SERVER_TYPE',
    'IBServer');
  InstanceName := INIFile.ReadString('IB_Environment', 'INSTANCE_NAME',
    'gds_db');

  INIFile.WriteString('IB Environment', 'PATH_TO_GDB', PATH_TO_GDB);
  INIFile.WriteString('IB Environment', 'WHERE_SERVER', WHERE_SERVER);
  INIFile.WriteString('IB Environment', 'SERVER_PROTOCOL', ProtocolName);
  INIFile.WriteString('IB Environment', 'SERVER_TYPE', Server_Type);
  INIFile.WriteString('IB_Environment', 'INSTANCE_NAME', InstanceName);

  if FileExists(PATH_TO_GDB + 'test.ib') then
    DeleteFile(PWideChar(PATH_TO_GDB + 'test.ib'));

  if ProtocolName = 'TCP' then
    SERVER_PROTOCOL := ibTCP
  else
    if ProtocolName = 'SPX' then
      SERVER_PROTOCOL := ibSPX
    else
      if ProtocolName = 'NamedPipe' then
        SERVER_PROTOCOL := ibNamedPipe
      else
        SERVER_PROTOCOL := ibLocal;

  WHERE_GDB := ComposeDatabaseName(WHERE_SERVER, '', SERVER_PROTOCOL, PATH_TO_GDB);

  INIFile.Free;
end;

initialization

InitialGlobals;

end.
