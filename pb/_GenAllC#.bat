@echo off 
for /f "delims=" %%i in ('dir /b/a "*.proto"') do   protoc %%i --csharp_out=.
pause