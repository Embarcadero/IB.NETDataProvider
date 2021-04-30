param(
	[Parameter(Mandatory=$True)]$InterBaseSelection,
	[Parameter(Mandatory=$True)]$TestSuite)

$ErrorActionPreference = 'Stop'

$baseDir = Split-Path -Parent $PSCommandPath

. "$baseDir\include.ps1"

$testsBaseDir = "$baseDir\src\InterBaseSql.Data.InterBaseClient.Tests"
$testsNETDir = "$testsBaseDir\bin\$Configuration\net452"
$testsCOREDir = "$testsBaseDir\bin\$Configuration\netcoreapp3.1"

$startDir = $null
$InterBaseProcess = $null

if ($env:tests_InterBase_dir) {
	$InterBaseDir = $env:tests_InterBase_dir
}
else {
	$InterBaseDir = 'I:\Downloads\fb_tests'
}

function Prepare() {
	echo "=== $($MyInvocation.MyCommand.Name) ==="

	$script:startDir = $pwd
	$selectedConfiguration = $InterBaseConfiguration[$InterBaseSelection]
	$ibDownload = $selectedConfiguration.Download
	$ibDownloadName = $ibDownload -Replace '.+/([^/]+)\?dl=1','$1'
	if (Test-Path $InterBaseDir) {
		rm -Force -Recurse $InterBaseDir
	}
	mkdir $InterBaseDir | Out-Null
	cd $InterBaseDir
	echo "Downloading $ibDownload"
	(New-Object System.Net.WebClient).DownloadFile($ibDownload, (Join-Path (pwd) $ibDownloadName))
	echo "Extracting $ibDownloadName"
	7z x -bsp0 -bso0 $ibDownloadName
	cp -Recurse -Force .\embedded\* $testsNETDir
	cp -Recurse -Force .\embedded\* $testsCOREDir
	rmdir -Recurse .\embedded
	rm $ibDownloadName
	mv .\server\* .
	rmdir .\server

	ni InterBase.log -ItemType File | Out-Null

	echo "Starting InterBase"
	$process = Start-Process -FilePath $selectedConfiguration.Executable -ArgumentList $selectedConfiguration.Args -PassThru
	echo "Version: $($process.MainModule.FileVersionInfo.FileVersion)"
	$script:InterBaseProcess = $process

	echo "=== END ==="
}

function Cleanup() {
	echo "=== $($MyInvocation.MyCommand.Name) ==="

	cd $script:startDir
	$process = $script:InterBaseProcess
	$process.Kill()
	$process.WaitForExit()
	rm -Force -Recurse $InterBaseDir

	echo "=== END ==="
}

function Tests-All() {
	Tests-InterBaseClient-NET
	Tests-InterBaseClient-Core
	Tests-EF6
	Tests-EFCore
}

function Tests-InterBaseClient-NET() {
	echo "=== $($MyInvocation.MyCommand.Name) ==="

	cd $testsNETDir
	.\InterBaseSql.Data.InterBaseClient.Tests.exe --labels=All
	Check-ExitCode

	echo "=== END ==="
}

function Tests-InterBaseClient-Core() {
	echo "=== $($MyInvocation.MyCommand.Name) ==="

	cd $testsCOREDir
	dotnet InterBaseSql.Data.InterBaseClient.Tests.dll --labels=All
	Check-ExitCode

	echo "=== END ==="
}

function Tests-EF6() {
	echo "=== $($MyInvocation.MyCommand.Name) ==="

	cd "$baseDir\src\EntityFramework.InterBase.Tests\bin\$Configuration\net452"
	.\EntityFramework.InterBase.Tests.exe --labels=All
	Check-ExitCode

	cd "$baseDir\src\EntityFramework.InterBase.Tests\bin\$Configuration\netcoreapp3.1"
	dotnet EntityFramework.InterBase.Tests.dll --labels=All
	Check-ExitCode

	echo "=== END ==="
}

function Tests-EFCore() {
	echo "=== $($MyInvocation.MyCommand.Name) ==="

	if ($InterBaseSelection -eq 'FB25') {
		return
	}

	cd "$baseDir\src\InterBaseSql.EntityFrameworkCore.InterBase.Tests\bin\$Configuration\netcoreapp3.1"
	dotnet InterBaseSql.EntityFrameworkCore.InterBase.Tests.dll --labels=All
	Check-ExitCode

	cd "$baseDir\src\InterBaseSql.EntityFrameworkCore.InterBase.FunctionalTests"
	dotnet test --no-build -c $Configuration
	Check-ExitCode

	echo "=== END ==="
}

try {
	Prepare
	& $TestSuite
}
finally {
	Cleanup
}
