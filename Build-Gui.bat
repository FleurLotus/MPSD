@echo off

SET LOCAL=%~dp0

pushd %LOCAL%

echo *****************************************************
echo **               BUILDING RELEASE GUI              **
echo *****************************************************

dotnet build -c Release -v m .\MagicPictureSetDownloader\MagicPictureSetDownloader.sln 

popd

pause