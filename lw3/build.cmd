@echo off

if "%~1" EQU "" goto error

echo Creating project structure...
mkdir %~1\"Frontend"
mkdir %~1\"Backend"
mkdir %~1\"TextListener"
mkdir "%~1"\"config"

echo Compiling project...
start /wait /d src\Backend dotnet publish
start /wait /d src\Frontend dotnet publish
start /wait /d src\TextListener dotnet publish

echo Copying files...
xcopy src\Frontend\bin\Debug\netcoreapp2.0\publish "%~1"\"Frontend"
xcopy src\Backend\bin\Debug\netcoreapp2.0\publish "%~1"\"Backend"
xcopy src\TextListener\bin\Debug\netcoreapp2.0\publish "%~1"\"TextListener"
xcopy config "%~1"\config
xcopy run.cmd "%~1"
xcopy stop.cmd "%~1"

echo Success!
exit 0

:error
echo Invalid arguments count!
exit 1