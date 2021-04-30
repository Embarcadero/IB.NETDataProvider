program TesterBase;

uses
  madExcept,
  madLinkDisAsm,
  Vcl.Forms,
  TestBaseU in 'TestBaseU.pas' {Form2},
  ComparerU in 'ComparerU.pas',
  test_env in 'test_env.pas',
  dmTimeFuncsU in 'dmTimeFuncsU.pas' {dmTimefuncs: TDataModule},
  dmStringFuncsU in 'dmStringFuncsU.pas' {dmStringFuncs: TDataModule},
  dmMathFuncsU in 'dmMathFuncsU.pas' {dmMathFuncs: TDataModule},
  dmDataConvU in 'dmDataConvU.pas' {dmDataConv: TDataModule};

{$R *.res}

begin
  Application.Initialize;
  Application.MainFormOnTaskbar := True;
  Application.CreateForm(TForm2, Form2);
  Application.Run;
end.
