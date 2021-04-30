object dmDataConv: TdmDataConv
  OldCreateOrder = False
  Height = 310
  Width = 389
  object IBDatabase1: TIBDatabase
    Params.Strings = (
      'sql_dialect=1')
    DefaultTransaction = IBTransaction1
    ServerType = 'IBServer'
    SQLDialect = 1
    Left = 24
    Top = 16
  end
  object IBTransaction1: TIBTransaction
    DefaultDatabase = IBDatabase1
    Left = 96
    Top = 16
  end
  object IBQuery1: TIBQuery
    BufferChunks = 1000
    CachedUpdates = False
    ParamCheck = True
    Left = 176
    Top = 16
  end
  object IBDataSet1: TIBDataSet
    Database = IBDatabase1
    Transaction = IBTransaction1
    BufferChunks = 32
    CachedUpdates = False
    ParamCheck = True
    UniDirectional = False
    Left = 96
    Top = 64
  end
  object IBSQL1: TIBSQL
    Database = IBDatabase1
    Transaction = IBTransaction1
    Left = 96
    Top = 120
  end
  object IBUpdateSQL1: TIBUpdateSQL
    Left = 248
    Top = 16
  end
  object IBStoredProc1: TIBStoredProc
    Database = IBDatabase1
    Transaction = IBTransaction1
    Left = 96
    Top = 168
  end
  object IBTable1: TIBTable
    Database = IBDatabase1
    Transaction = IBTransaction1
    BufferChunks = 1000
    CachedUpdates = False
    UniDirectional = False
    Left = 96
    Top = 216
  end
  object scp1: TIBScript
    Database = IBDatabase1
    Transaction = IBTransaction1
    Terminator = ';'
    Script.Strings = (
      '/* Use these '#39'wrappers'#39' to put functionality into'
      '   any of your databases. */'
      ''
      '/* Date/Time routines */'
      ''
      'declare external function EF_UTCCurrentTime'
      '  returns'
      '  timestamp /* free_it */'
      '  entry_point '#39'UTCCurrentTime'#39' module_name '#39'EntityFrameworkUDF'#39';'
      ''
      'declare external function EF_DateAdd'
      '  cstring(7),'
      '  numeric(18, 0),'
      #9'timestamp'
      '  returns'
      '  timestamp /* free_it */'
      '  entry_point '#39'DateAdd'#39' module_name '#39'EntityFrameworkUDF'#39';'
      ''
      'declare external function EF_DateDiff'
      '  cstring(12),'
      '  timestamp,'
      #9'timestamp'
      '  returns'
      '  numeric(18,0) by value /* free_it */'
      '  entry_point '#39'DateDiff'#39' module_name '#39'EntityFrameworkUDF'#39';'
      ''
      '/* Mathematical functions */'
      ''
      'declare external function EF_Abs'
      '  double precision'
      '  returns'
      '  double precision by value'
      '  entry_point '#39'fn_Abs'#39' module_name '#39'EntityFrameworkUDF'#39';'
      ''
      'declare external function EF_Ceiling'
      '  double precision'
      '  returns'
      '  numeric(18,0) by value'
      '  entry_point '#39'fn_Ceiling'#39' module_name '#39'EntityFrameworkUDF'#39';'
      ''
      'declare external function EF_Floor'
      '  double precision'
      '  returns'
      '  numeric(18, 0) by value'
      '  entry_point '#39'fn_Floor'#39' module_name '#39'EntityFrameworkUDF'#39';'
      ''
      'declare external function EF_Round'
      '  double precision,'
      #9'integer'
      '  returns'
      '  double precision by value'
      '  entry_point '#39'fn_Round'#39' module_name '#39'EntityFrameworkUDF'#39';'
      ''
      'declare external function EF_Power'
      '  double precision,'
      #9'double precision'
      '  returns'
      '  double precision by value'
      '  entry_point '#39'fn_Power'#39' module_name '#39'EntityFrameworkUDF'#39';'
      #9
      'declare external function EF_Trunc'
      '  double precision,'
      #9'integer'
      '  returns'
      '  double precision by value'
      '  entry_point '#39'fn_Truncate'#39' module_name '#39'EntityFrameworkUDF'#39';'
      ''
      'declare external function EF_BitAnd'
      '  numeric(18, 0),'
      #9'numeric(18, 0)'
      '  returns'
      '  NUMERIC(18, 0) by value'
      '  entry_point '#39'fn_BitAnd'#39' module_name '#39'EntityFrameworkUDF'#39';'
      ''
      'declare external function EF_BitNot'
      #9'numeric(18, 0)'
      '  returns'
      '  NUMERIC(18, 0) by value'
      '  entry_point '#39'fn_BitNot'#39' module_name '#39'EntityFrameworkUDF'#39';'
      ''
      'declare external function EF_BitOr'
      '  numeric(18, 0),'
      #9'numeric(18, 0)'
      '  returns'
      '  NUMERIC(18, 0) by value'
      '  entry_point '#39'fn_BitOr'#39' module_name '#39'EntityFrameworkUDF'#39';'
      ''
      'declare external function EF_BitXor'
      '  numeric(18, 0),'
      #9'numeric(18, 0)'
      '  returns'
      '  NUMERIC(18, 0) by value'
      '  entry_point '#39'fn_BitXor'#39' module_name '#39'EntityFrameworkUDF'#39';'
      ''
      'declare external function EF_Mod'
      '  numeric(18, 0),'
      #9'numeric(18, 0)'
      '  returns'
      '  NUMERIC(18, 0) by value'
      '  entry_point '#39'fn_Mod'#39' module_name '#39'EntityFrameworkUDF'#39';'
      ''
      'declare external function EF_BINSHL'
      '  numeric(18, 0),'
      #9'numeric(18, 0)'
      '  returns'
      '  NUMERIC(18, 0) by value'
      '  entry_point '#39'fn_BinSHL'#39' module_name '#39'EntityFrameworkUDF'#39';'
      ''
      'declare external function EF_BINSHR'
      '  numeric(18, 0),'
      #9'numeric(18, 0)'
      '  returns'
      '  NUMERIC(18, 0) by value'
      '  entry_point '#39'fn_BinSHR'#39' module_name '#39'EntityFrameworkUDF'#39';'
      ''
      '/* String functions */'
      ''
      'declare external function EF_Reverse'
      '  cstring(2048)'
      '  returns cstring(2048) /* free_it */'
      '  entry_point '#39'Reverse'#39' module_name '#39'EntityFrameworkUDF'#39';'
      ''
      'declare external function EF_Position'
      '  cstring(2048),'
      #9'cstring(2048),'
      #9'integer'
      '  returns integer by value /* free_it */'
      '  entry_point '#39'Position'#39' module_name '#39'EntityFrameworkUDF'#39';'
      ''
      'declare external function EF_Length'
      '  cstring(2048)'
      '  returns integer by value /* free_it */'
      '  entry_point '#39'StringLength'#39' module_name '#39'EntityFrameworkUDF'#39';'
      ''
      'declare external function EF_Lower'
      '  cstring(2048)'
      '  returns cstring(2048) /* free_it */'
      '  entry_point '#39'ToLower'#39' module_name '#39'EntityFrameworkUDF'#39';'
      ''
      'declare external function EF_Trim'
      '  cstring(8),'
      '  cstring(2048)'
      '  returns cstring(2048) /* free_it */'
      '  entry_point '#39'Trim'#39' module_name '#39'EntityFrameworkUDF'#39';'
      ''
      'declare external function EF_Left'
      '  cstring(2048),'
      #9'integer'
      '  returns cstring(2048) /* free_it */'
      '  entry_point '#39'Left'#39' module_name '#39'EntityFrameworkUDF'#39';'
      ''
      'declare external function EF_Right'
      '  cstring(2048),'
      #9'integer'
      '  returns cstring(2048) /* free_it */'
      '  entry_point '#39'Right'#39' module_name '#39'EntityFrameworkUDF'#39';'
      ''
      'declare external function EF_Replace'
      '  cstring(2048),'
      #9'cstring(2048),'
      #9'cstring(2048)'
      '  returns cstring(2048) /* free_it */'
      '  entry_point '#39'Replace'#39' module_name '#39'EntityFrameworkUDF'#39';'
      ''
      'declare external function EF_SubStr'
      '  cstring(2048),'
      #9'integer,'
      #9'integer'
      '  returns cstring(2048) /* free_it */'
      '  entry_point '#39'SubStr'#39' module_name '#39'EntityFrameworkUDF'#39';'
      ''
      'declare external function EF_NewGUID'
      '  returns cstring(16) character set OCTETS /* free_it */'
      '  entry_point '#39'NewGuid'#39' module_name '#39'EntityFrameworkUDF'#39';'
      ''
      'declare external function EF_UUID_TO_CHAR'
      '  cstring(16) character set OCTETS'
      '  returns cstring(36)  /* free_it */'
      '  entry_point '#39'UUID_TO_CHAR'#39' module_name '#39'EntityFrameworkUDF'#39';'
      #9
      'declare external function EF_CHAR_TO_UUID'
      '  cstring(36) '
      '  returns cstring(16) character set OCTETS /* free_it */'
      '  entry_point '#39'CHAR_TO_UUID'#39' module_name '#39'EntityFrameworkUDF'#39';'#9
      '')
    Left = 264
    Top = 112
  end
  object scpProcs: TIBScript
    Database = IBDatabase1
    Transaction = IBTransaction1
    Terminator = ';'
    Script.Strings = (
      'Create PROCEDURE UUIDTEST '
      '('
      '  UUID CHAR(16) CHARACTER SET OCTETS'
      ')'
      'RETURNS'
      '('
      '  GUIDSTR VARCHAR(38)'
      ')'
      'AS'
      'begin'
      '  GuidStr = EF_UUID_TO_CHAR(:UUID);'
      '  suspend;'
      'end')
    Left = 256
    Top = 192
  end
end
