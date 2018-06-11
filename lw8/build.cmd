@echo off

if "%~1" EQU "" goto error

echo Creating project structure...
mkdir lw8_%~1\"Frontend"
mkdir lw8_%~1\"Backend"
mkdir lw8_%~1\"TextListener"
mkdir lw8_%~1\"TextRankCalc"
mkdir lw8_%~1\"VowelConsCounter"
mkdir lw8_%~1\"VowelConsRater"
mkdir lw8_%~1\"TextStatistics"
mkdir lw8_%~1\"TextProcessingLimiter"
mkdir lw8_%~1\"TextSuccessMarker"
mkdir lw8_%~1"\"config"

echo Compiling project...

start /wait /d src\Backend dotnet publish
start /wait /d src\Frontend dotnet publish
start /wait /d src\TextListener dotnet publish
start /wait /d src\TextRankCalc dotnet publish
start /wait /d src\VowelConsCounter dotnet publish
start /wait /d src\VowelConsRater dotnet publish
start /wait /d src\TextStatistics dotnet publish
start /wait /d src\TextProcessingLimiter dotnet publish
start /wait /d src\TextSuccessMarker dotnet publish

echo Copying files...
xcopy src\Frontend\bin\Debug\netcoreapp2.0\publish lw8_"%~1"\"Frontend"
xcopy src\Backend\bin\Debug\netcoreapp2.0\publish lw8_"%~1"\"Backend"
xcopy src\TextListener\bin\Debug\netcoreapp2.0\publish lw8_"%~1"\"TextListener"
xcopy src\TextRankCalc\bin\Debug\netcoreapp2.0\publish lw8_"%~1"\"TextRankCalc"
xcopy src\VowelConsCounter\bin\Debug\netcoreapp2.0\publish lw8_"%~1"\"VowelConsCounter"
xcopy src\VowelConsRater\bin\Debug\netcoreapp2.0\publish lw8_"%~1"\"VowelConsRater"
xcopy src\TextStatistics\bin\Debug\netcoreapp2.0\publish lw8_"%~1"\"TextStatistics"
xcopy src\TextProcessingLimiter\bin\Debug\netcoreapp2.0\publish lw8_"%~1"\"TextProcessingLimiter"
xcopy src\TextSuccessMarker\bin\Debug\netcoreapp2.0\publish lw8_"%~1"\"TextSuccessMarker"
xcopy config lw8_"%~1"\config
xcopy run.cmd lw8_"%~1"
xcopy stop.cmd lw8_"%~1"

echo Success!
exit 0

:error
echo Invalid arguments count!
exit 1