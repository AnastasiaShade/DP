@echo off

if "%~1" EQU "" goto error

echo Creating project structure...
mkdir lw7_%~1\"Frontend"
mkdir lw7_%~1\"Backend"
mkdir lw7_%~1\"TextListener"
mkdir lw7_%~1\"TextRankCalc"
mkdir lw7_%~1\"VowelConsCounter"
mkdir lw7_%~1\"VowelConsRater"
mkdir lw7_%~1\"TextStatistics"
mkdir lw7_%~1"\"config"

echo Compiling project...

start /wait /d src\Backend dotnet publish
start /wait /d src\Frontend dotnet publish
start /wait /d src\TextListener dotnet publish
start /wait /d src\TextRankCalc dotnet publish
start /wait /d src\VowelConsCounter dotnet publish
start /wait /d src\VowelConsRater dotnet publish
start /wait /d src\TextStatistics dotnet publish

echo Copying files...
xcopy src\Frontend\bin\Debug\netcoreapp2.0\publish lw7_"%~1"\"Frontend"
xcopy src\Backend\bin\Debug\netcoreapp2.0\publish lw7_"%~1"\"Backend"
xcopy src\TextListener\bin\Debug\netcoreapp2.0\publish lw7_"%~1"\"TextListener"
xcopy src\TextRankCalc\bin\Debug\netcoreapp2.0\publish lw7_"%~1"\"TextRankCalc"
xcopy src\VowelConsCounter\bin\Debug\netcoreapp2.0\publish lw7_"%~1"\"VowelConsCounter"
xcopy src\VowelConsRater\bin\Debug\netcoreapp2.0\publish lw7_"%~1"\"VowelConsRater"
xcopy src\TextStatistics\bin\Debug\netcoreapp2.0\publish lw7_"%~1"\"TextStatistics"
xcopy config lw7_"%~1"\config
xcopy run.cmd lw7_"%~1"
xcopy stop.cmd lw7_"%~1"

echo Success!
exit 0

:error
echo Invalid arguments count!
exit 1