param(
	[Parameter(Mandatory=$True)]$Configuration)

$ErrorActionPreference = 'Stop'

$baseDir = Split-Path -Parent $PSCommandPath

$7zipPath = "$env:ProgramFiles\7-Zip\7z.exe"

Set-Alias 7z $7zipPath

. "$baseDir\include.ps1"

$outDir = "$baseDir\out"
$version = ''

if ($env:build_wix) {
	$wix = $env:build_wix
}
else {
	$wix = 'C:\Program Files (x86)\WiX Toolset v3.11\bin'
}

function Clean() {
	if (Test-Path $outDir) {
		rm -Force -Recurse $outDir
	}
	mkdir $outDir | Out-Null
}

function Build() {
	function b($target, $check=$True) {
		dotnet msbuild /t:$target /p:Configuration=$Configuration "$baseDir\src\NETProvider.sln" /v:m /m
		if ($check) {
			Check-ExitCode
		}
	}
	b 'Clean'
	# this sometimes fails on CI
	b 'Restore' $False
	b 'Restore'
	b 'Build'
	$script:version = (Get-Item $baseDir\src\InterBaseSql.Data.InterBaseClient\bin\$Configuration\net452\InterBaseSql.Data.InterBaseClient.dll).VersionInfo.ProductVersion -replace '(\d+)\.(\d+)\.(\d+)(-[a-z0-9]+)?(.*)','$1.$2.$3$4'
}

function Pack() {
	7z a -mx=9 -bsp0 $outDir\InterBaseSql.Data.InterBaseClient-$version-net452.7z $baseDir\src\InterBaseSql.Data.InterBaseClient\bin\$Configuration\net452\InterBaseSql.Data.InterBaseClient.dll $baseDir\src\InterBaseSql.Data.InterBaseClient\bin\$Configuration\net452\InterBaseSql.Data.InterBaseClient.pdb
	7z a -mx=9 -bsp0 $outDir\InterBaseSql.Data.InterBaseClient-$version-net5.0.7z $baseDir\src\InterBaseSql.Data.InterBaseClient\bin\$Configuration\net5.0\InterBaseSql.Data.InterBaseClient.dll $baseDir\src\InterBaseSql.Data.InterBaseClient\bin\$Configuration\net5.0\InterBaseSql.Data.InterBaseClient.pdb
	7z a -mx=9 -bsp0 $outDir\InterBaseSql.Data.InterBaseClient-$version-net6.0.7z $baseDir\src\InterBaseSql.Data.InterBaseClient\bin\$Configuration\net6.0\InterBaseSql.Data.InterBaseClient.dll $baseDir\src\InterBaseSql.Data.InterBaseClient\bin\$Configuration\net6.0\InterBaseSql.Data.InterBaseClient.pdb

#	7z a -mx=9 -bsp0 $outDir\EntityFramework.InterBase-$version-net452.7z $baseDir\src\EntityFramework.InterBase\bin\$Configuration\net452\EntityFramework.InterBase.dll $baseDir\src\EntityFramework.InterBase\bin\$Configuration\net452\EntityFramework.InterBase.pdb
#	7z a -mx=9 -bsp0 $outDir\EntityFramework.InterBase-$version-netstandard2.1.7z $baseDir\src\EntityFramework.InterBase\bin\$Configuration\netstandard2.1\EntityFramework.InterBase.dll $baseDir\src\EntityFramework.InterBase\bin\$Configuration\netstandard2.1\EntityFramework.InterBase.pdb

	7z a -mx=9 -bsp0 $outDir\InterBaseSql.EntityFrameworkCore.InterBase-$version-net6.0.7z $baseDir\src\InterBaseSql.EntityFrameworkCore.InterBase\bin\$Configuration\net6.0\InterBaseSql.EntityFrameworkCore.InterBase.dll $baseDir\src\InterBaseSql.EntityFrameworkCore.InterBase\bin\$Configuration\net6.0\InterBaseSql.EntityFrameworkCore.InterBase.pdb
}

function NuGets() {
	cp $baseDir\src\InterBaseSql.Data.InterBaseClient\bin\$Configuration\InterBaseSql.Data.InterBaseClient.$version.nupkg $outDir
	cp $baseDir\src\EntityFramework.InterBase\bin\$Configuration\EntityFramework.InterBase.$version.nupkg $outDir
	cp $baseDir\src\InterBaseSql.EntityFrameworkCore.InterBase\bin\$Configuration\InterBaseSql.EntityFrameworkCore.InterBase.$version.nupkg $outDir

	cp $baseDir\src\InterBaseSql.Data.InterBaseClient\bin\$Configuration\InterBaseSql.Data.InterBaseClient.$version.snupkg $outDir
	cp $baseDir\src\EntityFramework.InterBase\bin\$Configuration\EntityFramework.InterBase.$version.snupkg $outDir
	cp $baseDir\src\InterBaseSql.EntityFrameworkCore.InterBase\bin\$Configuration\InterBaseSql.EntityFrameworkCore.InterBase.$version.snupkg $outDir
}

function WiX() {
	$wixVersion = $version -replace '(.+?)(-[a-z0-9]+)?','$1'
	& $wix\candle.exe "-dBaseDir=$baseDir" "-dVersion=$wixVersion" "-dConfiguration=$Configuration" -ext $wix\WixUtilExtension.dll -out $outDir\Installer.wixobj $baseDir\installer\Installer.wxs
	& $wix\light.exe -ext $wix\WixUIExtension.dll -ext $wix\WixUtilExtension.dll -out $outDir\InterBaseSql.Data.InterBaseClient-$version.msi $outDir\Installer.wixobj
	rm $outDir\Installer.wixobj
	rm $outDir\InterBaseSql.Data.InterBaseClient-$version.wixpdb
}

Clean
Build
Pack
NuGets
WiX
