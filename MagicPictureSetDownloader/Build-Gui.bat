@echo off

SET LOCAL=%~dp0

pushd %LOCAL%

call SetBuildEnvironment.bat

echo *****************************************************
echo **               BUILDING RELEASE GUI              **
echo *****************************************************

%MSBUILD% /v:q /t:Rebuild  /p:Configuration=Release MagicPictureSetDownloader.sln
popd

pause