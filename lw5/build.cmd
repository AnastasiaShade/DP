@echo off

if "%~1" EQU "" goto error

echo Creating project structure...
mkdir lw5_%~1\"Frontend"
mkdir lw5_%~1\"Backend"
mkdir lw5_%~1\"TextListener"
mkdir lw5_%~1\"TextRankCalc"
mkdir lw5_%~1\"VowelConsCounter"
mkdir lw5_%~1\"VowelConsRater"
mkdir lw5_%~1"\"config"

echo Compiling project...
start /wait /d src\Backend dotnet publish
start /wait /d src\Frontend dotnet publish
start /wait /d src\TextListener dotnet publish
start /wait /d src\TextRankCalc dotnet publish
start /wait /d src\VowelConsCounter dotnet publish
start /wait /d src\VowelConsRater dotnet publish

echo Copying files...
xcopy src\Frontend\bin\Debug\netcoreapp2.0\publish lw5_"%~1"\"Frontend"
xcopy src\Backend\bin\Debug\netcoreapp2.0\publish lw5_"%~1"\"Backend"
xcopy src\TextListener\bin\Debug\netcoreapp2.0\publish lw5_"%~1"\"TextListener"
xcopy src\TextRankCalc\bin\Debug\netcoreapp2.0\publish lw5_"%~1"\"TextRankCalc"
xcopy src\VowelConsCounter\bin\Debug\netcoreapp2.0\publish lw5_"%~1"\"VowelConsCounter"
xcopy src\VowelConsRater\bin\Debug\netcoreapp2.0\publish lw5_"%~1"\"VowelConsRater"
xcopy config lw5_"%~1"\config
xcopy run.cmd lw5_"%~1"
xcopy stop.cmd lw5_"%~1"

echo Success!
exit 0

:error
echo Invalid arguments count!
exit 1