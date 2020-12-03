@cls
@echo off

set rev=1.0
set current=%~dp0
set datadir=%~dp0data\
set logdir=C:\ETC\LOG\DM\
set logfile=%logdir%%computername%_DataMigration.log
set rclogfile=%logdir%%computername%_Rc_%date:/=%%time::=%.log
set rclogfile2=%logdir%%computername%_RcErr_%date:/=%%time::=%.log
set dest=\\192.168.100.101\C\Users\
set src=%USERPROFILE%\
set errflg=1

if not exist "%logdir%" md "%logdir%"

start "" "%systemroot%\System32\PresentationSettings.exe" /start

echo %date% %time:~0,8%	---------- Log Start ---------->>"%logfile%"

rem データ移行進行中ステータス
del /q "%dest%Public\Downloads\*.status" 1>nul 2>&1
echo.>"%dest%Public\Downloads\transfer.status"

echo %date% %time:~0,8%	tr_run			value=[%errorlevel%]>>"%logfile%"

ver | findstr /c:"Version 10.0"
if "%errorlevel%"=="0" (
    call :Win10
) else (
    powershell -ExecutionPolicy RemoteSigned -File "%current%PST_COPY.ps1"
    call :Win7
)

findstr /c:" ERROR " "%rclogfile%" 1>nul 2>&1
if "%errorlevel%"=="0" set errflg=99

if not "%errflg%"=="1" call :e00001 "%errflg%"

rem データ移行完了中ステータス
del /q "%dest%Public\Downloads\*.status" 1>nul 2>&1
echo.>"%dest%Public\Downloads\success.status"

echo %date% %time:~0,8%	tr_end			value=[%errorlevel%]>>"%logfile%"

echo %date% %time:~0,8%	transfer		ret=[%errflg%]>>"%logfile%"
echo %date% %time:~0,8%	---------- Log End ---------->>"%logfile%"

if not exist "%dest%Public\Downloads\DM" md "%dest%Public\Downloads\DM"
copy /y "%logdir%" "%dest%Public\Downloads\DM"

exit %errflg%

:Win7
call :s00001 "Desktop"
call :s00001 "Documents"
call :s00001 "Pictures"
call :s00001 "Videos"
call :s00001 "Music"
call :s00001 "Contacts"
call :s00001 "Favorites"
call :s00001 "Links"
call :s00001 "Downloads"
call :s00002 "AppData\Roaming\Microsoft\IMJP10" "AppData\Roaming\Microsoft\IME\15.0\IMEJP\UserDict"
call :s00001 "AppData\Roaming\Microsoft\Signatures"
goto :eof

:Win10
call :s00001 "Desktop"
call :s00001 "Documents"
call :s00001 "Pictures"
call :s00001 "Videos"
call :s00001 "Music"
call :s00001 "Contacts"
call :s00001 "Favorites"
call :s00001 "Links"
call :s00001 "Downloads"
call :s00001 "3D Objects"
call :s00002 "AppData\Roaming\Microsoft\IME" "AppData\Roaming\Microsoft\IME"
call :s00001 "AppData\Roaming\Microsoft\Signatures"
goto :eof

:s00001
    chcp 65001 1>nul 2>&1
    if not exist "%src%%~1" md "%src%%~1" 1>nul 2>&1
    echo %date% %time:~0,8%	robocopy		src=[%src%%~1] dest=[%dest%%username%\%~1]>>"%logfile%"
    robocopy "%src%%~1" "%dest%%username%\%~1" /V /FP /TEE /S /E /NP /XO /XN /XC /XJF /XJD /XA:SH /R:0 /W:0 /LOG+:"%rclogfile%"
    echo %date% %time:~0,8%	robocopy		src=[%src%%~1] dest=[%dest%%username%\%~1] ret=[%errorlevel%]>>"%logfile%"
    if %errorlevel% geq 8 set /a errflg+=1
    chcp 932 1>nul 2>&1
goto :eof

:s00002
    chcp 65001 1>nul 2>&1
    if not exist "%src%%~1" md "%src%%~1" 1>nul 2>&1
    echo %date% %time:~0,8%	robocopy		src=[%src%%~1] dest=[%dest%%username%\%~1]>>"%logfile%"
    robocopy "%src%%~1" "%dest%%username%\%~2" /V /FP /TEE /S /E /NP /XJF /XJD /XA:SH /R:0 /W:0 /LOG+:"%rclogfile%"
    echo %date% %time:~0,8%	robocopy		src=[%src%%~1] dest=[%dest%%username%\%~1] ret=[%errorlevel%]>>"%logfile%"
    if %errorlevel% geq 8 set /a errflg+=1
    chcp 932 1>nul 2>&1
goto :eof

:s00003
echo %~2>>"%rclogfile2%"
for /f "usebackq tokens=* skip=%~1" %%a in ("%rclogfile%") do echo %%a>>"%rclogfile2%"&goto :eof
goto :eof

:e00001
rem データ移行エラーステータス
del /q "%dest%Public\Downloads\*.status" 1>nul 2>&1
echo.>"%dest%Public\Downloads\error.status"

echo %date% %time:~0,8%	tr_end			value=[%errorlevel%]>>"%logfile%"

echo %date% %time:~0,8%	transfer		ret=[%~1]>>"%logfile%"
echo %date% %time:~0,8%	---------- Log End ---------->>"%logfile%"

if not exist "%dest%Public\Downloads\DM" md "%dest%Public\Downloads\DM"
copy /y "%logdir%" "%dest%Public\Downloads\DM"

rem findstr /c:" ERROR " "%rclogfile%">"%rclogfile2%"
for /f "tokens=1,* delims=:" %%a in ('findstr /c:" ERROR " /n "%rclogfile%"') do call :s00003 "%%a" "%%b"

start notepad "%rclogfile2%"
exit %~1
