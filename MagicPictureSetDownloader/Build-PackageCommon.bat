@echo off

SET LOCAL=%~dp0

pushd %LOCAL%

call SetBuildEnvironment.bat

echo *****************************************************
echo **                 PACKAGING COMMON                **
echo *****************************************************


pushd ..\CommonLibraries

dotnet pack --configuration Release --output ..\LocalPackages Common.sln
popd
popd

pause