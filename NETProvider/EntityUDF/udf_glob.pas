unit udf_glob;

interface

uses
  Windows, System.Types, SysUtils, IBX.IBHeader;

(*
  * Define FREE_IT if you want to use the free_it clause with your stuff
  *  - uncomment below "commented" define
*)

{ $define FREE_IT }

type
  (*
    * TThreadLocalVariables
    *   This structure is set up to contain all structures for maintaining
    *   thread-local stuff.
    * Use this in conjunction with ThreadLocals....
    * This makes it easy to maintain an virtually unlimited number of thread
    * locals.
  *)
  TThreadLocalVariables = class
  protected
    FPAnsiChar: PAnsiChar;
    FPCharSize: DWord;
    FQuad: PISC_QUAD;
  public
    constructor Create;
    destructor Destroy; override;
  end;

  TLibEntryProc = procedure(Reason: Integer);

function MakeResultString(Source : PAnsiChar; OptionalDest: PAnsiChar = nil; Len: DWord = 0): PAnsiChar;
function MakeResultQuad(Source, OptionalDest: PISC_QUAD): PISC_QUAD;

function ThreadLocals: TThreadLocalVariables;

var
  hThreadLocalVariables: Integer; (* Index to thread-local storage *)

const
  UDF_SUCCESS = 0;
  UDF_FAILURE = 1;
  cSignificantlyLarger = 1024 * 4; // We don't want strings to be more than
  // 4k larger than what we're actually
  // passing back.

implementation

uses AnsiStrings;

// function malloc(Size: Integer): Pointer; cdecl; external 'msvcrt.dll';

function MakeResultString(Source, OptionalDest: PAnsiChar; Len: DWord): PAnsiChar;
begin
  result := OptionalDest;
  if (Len = 0) then
    Len := AnsiStrings.StrLen(Source) + 1;
  if (result = nil) then
  begin
{$IFDEF FREE_IT}
    result := ib_util_malloc(Len);
{$ELSE}
    (*
      * If the current PChar is smaller than than Source, or
      * it is significanly larger than Source, then reallocate it
      * in cSignificantlyLarger chunks.
    *)
    if (ThreadLocals.FPCharSize < Len) or (ThreadLocals.FPCharSize > Len + cSignificantlyLarger) then
    begin
      ThreadLocals.FPCharSize := 0;
      (*
        * Realistically, we'll never return strings longer than about
        * 2k, so I'd rather risk spending time in a loop that *adds* than
        * "compute" FPCharSize by performing division and modulo arithmetic.
        * Addition is very fast, and the while loop will in general, only
        * be at most 1 to 2 steps.
      *)
      while (ThreadLocals.FPCharSize < Len) do
        Inc(ThreadLocals.FPCharSize, cSignificantlyLarger);
      ReallocMem(ThreadLocals.FPAnsiChar, ThreadLocals.FPCharSize);
    end;
    result := ThreadLocals.FPAnsiChar;
{$ENDIF}
  end;
  if (Source <> result) then
  begin
    if (Source = nil) or (Len = 1) then
      result[0] := #0
    else
      Move(Source^, result^, Len);
  end;
end;

function MakeResultQuad(Source, OptionalDest: PISC_QUAD): PISC_QUAD;
begin
  result := OptionalDest;
  if (result = nil) then
{$IFDEF FREE_IT}
    result := ib_util_malloc(SizeOf(TISC_QUAD));
{$ELSE}
    result := ThreadLocals.FQuad;
{$ENDIF}
  if (Source <> nil) then
    Move(Source^, result^, SizeOf(TISC_QUAD));
end;

(* TThreadLocalVariables *)
constructor TThreadLocalVariables.Create;
begin
  ReallocMem(FPAnsiChar, cSignificantlyLarger);
  FPCharSize := cSignificantlyLarger;
  ReallocMem(FQuad, SizeOf(TISC_QUAD));
end;

destructor TThreadLocalVariables.Destroy;
begin
  ReallocMem(FPAnsiChar, 0);
  ReallocMem(FQuad, 0);
  inherited;
end;

function ThreadLocals: TThreadLocalVariables;
begin
  result := TLSGetValue(hThreadLocalVariables);
  if result = nil then
  begin
    result := TThreadLocalVariables.Create;
    TLSSetValue(hThreadLocalVariables, result);
  end;
end;

(*
  * LibEntry -
  *  Used for the initialization of all threads but the primary thread.
  *  Used for the finalization of all threads.
*)
procedure LibEntry(Reason: Integer);
begin
  if Reason = DLL_THREAD_DETACH then
  begin
    ThreadLocals.Free;
    TLSSetValue(hThreadLocalVariables, nil);
  end;
end;

initialization

(*
  * IsMultiThread *must* be set to true for the Delphi Memory Manager to
  * work correctly.
*)
IsMultiThread := True;
hThreadLocalVariables := TLSAlloc;
if (hThreadLocalVariables = -1) then
  raise Exception.Create('EntityUDFLIB: Error instantiating TLS');
DllProc := @LibEntry;

finalization

// Just make sure that the thread local variables in the main thread
// are freed.
ThreadLocals.Free;
TLSSetValue(hThreadLocalVariables, nil);
if (hThreadLocalVariables <> -1) then
  TLSFree(hThreadLocalVariables);

end.
