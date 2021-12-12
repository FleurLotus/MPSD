@echo off
@SET MSBUILD="C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\msbuild.exe"

@ECHO --^> MagicPictureSetDownloader
@%MSBUILD% /v:q /t:Rebuild  /p:Configuration=Release MagicPictureSetDownloader.sln

pause