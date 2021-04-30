library EntityFrameworkUDF;

{ Important note about DLL memory management: ShareMem must be the
  first unit in your library's USES clause AND your project's (select
  Project-View Source) USES clause if your DLL exports any procedures or
  functions that pass strings as parameters or function results. This
  applies to all strings passed to and from your DLL--even those that
  are nested in records and classes. ShareMem is the interface unit to
  the BORLNDMM.DLL shared memory manager, which must be deployed along
  with your DLL. To avoid using BORLNDMM.DLL, pass string information
  using PChar or ShortString parameters. }

uses
  System.SysUtils,
  System.Classes,
  udf_glob in 'udf_glob.pas',
  StrFncs in 'StrFncs.pas',
  StdFuncs in 'StdFuncs.pas',
  StdConsts in 'StdConsts.pas',
  MathFncs in 'MathFncs.pas',
  TimeFncs in 'TimeFncs.pas';

{$R *.res}

exports

// Math functions
  fn_Abs,
  fn_Ceiling,
  fn_Floor,
  fn_Round,
  fn_Power,
  fn_Truncate,
  fn_BitAnd,
  fn_BitOr,
  fn_BitXor,
  fn_BitNot,
  fn_Mod,
  fn_BinSHL,
  fn_BinSHR,

// String Functions
  Reverse,
  Position,
  StringLength,
  ToLower,
  Trim,
  Left,
  Right,
  Replace,
  SubStr,
  NewGuid,
  UUID_TO_CHAR,
  CHAR_TO_UUID,

// Time Functions
  UTCCurrentTime,
  DateAdd,
  DateDiff;

begin
end.
