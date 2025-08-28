@echo off

SET LOCAL=%~dp0

pushd %LOCAL%

echo *****************************************************
echo **                 PACKAGING COMMON                **
echo *****************************************************


dotnet pack -c Release -o .\LocalPackages .\CommonLibraries\Common.sln
popd

pause