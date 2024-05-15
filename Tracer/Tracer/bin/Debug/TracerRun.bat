@echo off
setlocal enabledelayedexpansion enableextensions
set RESULTCODE=0
set "PATH=%~dp0"

cd /d %PATH%

if "%1"=="0" (

	echo /**********StartButton**********/
	Tracer.exe /s 
	
)

if "%1"=="1" (

    echo /**********StopButton**********/
    
    Tracer.exe /p
)


if "%1"=="2" (

    echo /**********ClearButton**********/
    
    Tracer.exe /c
)



echo "End of batch program."
exit /B %RESULTCODE%
