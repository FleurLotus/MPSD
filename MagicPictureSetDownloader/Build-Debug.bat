@echo off

SET LOCAL=%~dp0

pushd %LOCAL%

call SetBuildEnvironment.bat

pushd ..\CommonLibraries

echo *****************************************************
echo **               BUILDING DEBUG COMMON             **
echo *****************************************************

call :build Common.sln
popd

echo *****************************************************
echo **                BUILDING DEBUG GUI               **
echo *****************************************************

call :build  MagicPictureSetDownloader.sln
popd

goto end

:build
%MSBUILD% /v:q /t:Rebuild  /p:Configuration=Debug %1
exit /B 0


:end
pause