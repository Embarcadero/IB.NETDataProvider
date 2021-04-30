program TesterBaseTests;
{

  Delphi DUnit Test Project
  -------------------------
  This project contains the DUnit test framework and the GUI/Console test runners.
  Add "CONSOLE_TESTRUNNER" to the conditional defines entry in the project options
  to use the console test runner.  Otherwise the GUI test runner will be used by
  default.

}

{$IFDEF CONSOLE_TESTRUNNER}
{$APPTYPE CONSOLE}
{$ENDIF}

uses
  madExcept,
  madLinkDisAsm,
  DUnitTestRunner,
  TestdmMathFuncsU in 'TestdmMathFuncsU.pas',
  dmMathFuncsU in '..\dmMathFuncsU.pas',
  test_env in '..\test_env.pas',
  ComparerU in '..\ComparerU.pas',
  dmDataConvU in '..\dmDataConvU.pas' {dmDataConv: TDataModule},
  TestdmTimeFuncsU in 'TestdmTimeFuncsU.pas',
  dmTimeFuncsU in '..\dmTimeFuncsU.pas',
  dmStringFuncsU in '..\dmStringFuncsU.pas' {dmStringFuncs: TDataModule},
  TestdmStringFuncsU in 'TestdmStringFuncsU.pas';

{$R *.RES}

begin
  DUnitTestRunner.RunRegisteredTests;
end.

