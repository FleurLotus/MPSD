@echo off

SET LOCAL=%~dp0

pushd %LOCAL%

echo *****************************************************
echo **               BUILDING DEBUG COMMON             **
echo *****************************************************

dotnet build -c Debug -v m .\CommonLibraries\Common.sln 

echo *****************************************************
echo **                BUILDING DEBUG GUI               **
echo *****************************************************

dotnet build -c Debug -v m .\MagicPictureSetDownloader\MagicPictureSetDownloader.sln 

popd

