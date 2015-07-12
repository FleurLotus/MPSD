@echo off
@SET MSBUILD="C:\Program Files (x86)\MSBuild\12.0\Bin\msbuild.exe"

@pushd ..\CommonLibraries

@call :build Common.sln
@popd
@call :build  MagicPictureSetDownloader.sln

@goto end

:build
@ECHO **** Building %1 ****
@call :build2 Debug %1
@call :build2 Release %1
@exit /B 0

:build2
@ECHO --^> %1
@%MSBUILD% /v:q /t:Rebuild  /p:Configuration=%1 %2
@exit /B 0

:end
pause