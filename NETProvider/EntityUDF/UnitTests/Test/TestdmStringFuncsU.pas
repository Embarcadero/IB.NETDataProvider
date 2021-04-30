unit TestdmStringFuncsU;
{

  Delphi DUnit Test Case
  ----------------------
  This unit contains a skeleton test case class generated by the Test Case Wizard.
  Modify the generated code to correctly setup and call the methods from the unit 
  being tested.

}

interface

uses
  TestFramework, IBX.IBCustomDataSet, IBX.IBDatabase, System.SysUtils,
  IBX.IBUpdateSQL, IBX.IBSQL, IBX.IBStoredProc, IBX.IBTable, dmStringFuncsU,
  IBX.IBQuery, IBX.IBScript, dmDataConvU, Data.DB, System.Classes;

type
  // Test methods for class TdmStringFuncs

  TestTdmStringFuncs = class(TTestCase)
  strict private
    FdmStringFuncs: TdmStringFuncs;
  public
    procedure SetUp; override;
    procedure TearDown; override;
    procedure HasErrors;
  published
    procedure TestReverse;
    procedure TestPosition;
    procedure TestStringLength;
    procedure TestToLower;
    procedure TestTrimLeading;
    procedure TestTrimTrailing;
    procedure TestTrimFull;
    procedure TestLeft;
    procedure TestRight;
    procedure TestReplace;
    procedure TestSubStr;
    procedure TestGUID;
    procedure TestUUID_TO_CHAR;
    procedure TestCHAR_TO_UUID;
  end;

implementation

procedure TestTdmStringFuncs.HasErrors;
begin
  if FdmStringFuncs.Comparer.Errors.Count > 0 then
    raise Exception.Create(FdmStringFuncs.Comparer.Errors.Text);
  if FdmStringFuncs.Comparer.Warnings.Count > 0 then
    Check(False, FdmStringFuncs.Comparer.Warnings.Text);
end;

procedure TestTdmStringFuncs.SetUp;
begin
  FdmStringFuncs := TdmStringFuncs.Create(nil);
end;

procedure TestTdmStringFuncs.TearDown;
begin
  FdmStringFuncs.Free;
  FdmStringFuncs := nil;
end;

procedure TestTdmStringFuncs.TestReverse;
begin
  FdmStringFuncs.Reverse;
  HasErrors;
  // TODO: Validate method results
end;

procedure TestTdmStringFuncs.TestStringLength;
begin
  FdmStringFuncs.StringLength;
  HasErrors;
end;

procedure TestTdmStringFuncs.TestToLower;
begin
  FdmStringFuncs.ToLower;
  HasErrors;
  // TODO: Validate method results
end;

procedure TestTdmStringFuncs.TestTrimLeading;
begin
  FdmStringFuncs.TrimLeading;
  HasErrors;
  // TODO: Validate method results
end;

procedure TestTdmStringFuncs.TestTrimTrailing;
begin
  FdmStringFuncs.TrimTrailing;
  HasErrors;
  // TODO: Validate method results
end;

procedure TestTdmStringFuncs.TestUUID_TO_CHAR;
begin
  FdmStringFuncs.UUID_TO_CHAR;
  HasErrors;
end;

procedure TestTdmStringFuncs.TestTrimFull;
begin
  FdmStringFuncs.TrimFull;
  HasErrors;
  // TODO: Validate method results
end;

procedure TestTdmStringFuncs.TestLeft;
begin
  FdmStringFuncs.Left;
  HasErrors;
  // TODO: Validate method results
end;

procedure TestTdmStringFuncs.TestRight;
begin
  FdmStringFuncs.Right;
  HasErrors;
  // TODO: Validate method results
end;

procedure TestTdmStringFuncs.TestReplace;
begin
  FdmStringFuncs.Replace;
  HasErrors;
  // TODO: Validate method results
end;

procedure TestTdmStringFuncs.TestSubStr;
begin
  FdmStringFuncs.SubStr;
  HasErrors;
  // TODO: Validate method results
end;

procedure TestTdmStringFuncs.TestCHAR_TO_UUID;
begin
  FdmStringFuncs.CHAR_TO_UUID;
  HasErrors;
end;

procedure TestTdmStringFuncs.TestGUID;
begin
  FdmStringFuncs.GUID;
  HasErrors;
  // TODO: Validate method results
end;

procedure TestTdmStringFuncs.TestPosition;
begin
  FdmStringFuncs.Position;
  HasErrors;
  // TODO: Validate method results
end;

initialization
  // Register any test cases with the test runner
  RegisterTest(TestTdmStringFuncs.Suite);
end.
