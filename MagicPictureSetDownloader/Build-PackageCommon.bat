@echo off
@SET MSBUILD="C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\msbuild.exe"

@pushd ..\CommonLibraries

ECHO --^> Package Common
@dotnet pack --configuration Release --output ..\LocalPackages Common.sln
@popd

pause