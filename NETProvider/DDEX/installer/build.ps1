$wix = 'C:\Program Files (x86)\WiX Toolset v3.11\bin\'
$baseDir = Split-Path -Parent (Split-Path -parent $PSCommandPath)

& $wix\candle.exe "-dBaseDir=$baseDir" -out $baseDir\installer\out\Install.wixobj $baseDir\installer\Install.wxs
& $wix\light.exe -ext $wix\WixUIExtension.dll -ext $wix\WixUtilExtension.dll -out $baseDir\installer\out\DDEX.msi $baseDir\installer\out\Install.wixobj
