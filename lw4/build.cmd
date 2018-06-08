@echo off

if "%~1" EQU "" goto error

echo Creating project structure...
mkdir lw4_%~1\"Frontend"
mkdir lw4_%~1\"Backend"
mkdir lw4_%~1\"TextListener"
mkdir lw4_%~1\"TextRankCalc"
mkdir lw4_%~1"\"config"

echo Compiling project...
start /wait /d src\Backend dotnet publish
start /wait /d src\Frontend dotnet publish
start /wait /d src\TextListener dotnet publish
start /wait /d src\TextRankCalc dotnet publish

echo Copying files...
xcopy src\Frontend\bin\Debug\netcoreapp2.0\publish lw4_"%~1"\"Frontend"
xcopy src\Backend\bin\Debug\netcoreapp2.0\publish lw4_"%~1"\"Backend"
xcopy src\TextListener\bin\Debug\netcoreapp2.0\publish lw4_"%~1"\"TextListener"
xcopy src\TextRankCalc\bin\Debug\netcoreapp2.0\publish lw4_"%~1"\"TextRankCalc"
xcopy config lw4_"%~1"\config
xcopy run.cmd lw4_"%~1"
xcopy stop.cmd lw4_"%~1"

echo Success!
exit 0

:error
echo Invalid arguments count!
exit 1