@echo off

echo Running project...
start "Frontend" /d "Frontend" dotnet Frontend.dll
start "Backend" /d "Backend" dotnet Backend.dll
start "TextListener" /d "TextListener" dotnet TextListener.dll
start "TextRankCalc" /d "TextRankCalc" dotnet TextRankCalc.dll
start "VowelConsCounter" /d "VowelConsCounter" dotnet VowelConsCounter.dll
start "VowelConsRater" /d "VowelConsRater" dotnet VowelConsRater.dll
start "TextStatistics" /d "TextStatistics" dotnet TextStatistics.dll
start "TextProcessingLimiter" /d "TextProcessingLimiter" dotnet TextProcessingLimiter.dll
start "TextSuccessMarker" /d "TextSuccessMarker" dotnet TextSuccessMarker.dll

set file=config\components_config.json
for /f "tokens=1,2" %%i in (%file%) do (@echo %%i %%j
for /l %%n in (2, 1, %%j) do start "%%i" /d "%%i" dotnet %%i.dll
)

exit 0