@cls
@echo off

setlocal enabledelayedexpansion

set rev=1.0
set current=%~dp0
set datadir=%~dp0data\
set logdir=C:\ETC\LOG\DM\
set logfile=%logdir%%computername%_DataMigration.log

set ret=1
set ret2=0
set ret3=9
set interface=
set dhcp2=

if not exist "%logdir%" md "%logdir%"

set ml=0
for /f "tokens=*" %%a in ('whoami /groups ^| findstr /i /c:"Mandatory Label\High Mandatory Level"') do set ml=1
for /f "tokens=*" %%a in ('whoami /groups ^| findstr /i /c:"Mandatory Label\System Mandatory Level"') do set ml=1

if not "%ml%"=="1" (
    rem if "%~3"=="10" (
    rem     powershell "$proc = Start-Process cmd.exe '/c """%current%%~nx0""" %*' -verb runas -wait -passthru; exit $proc.ExitCode"
    rem     exit !errorlevel!
    rem )
    powershell "$proc = Start-Process cmd.exe '/c """%current%%~nx0""" %*' -verb runas -windowstyle hidden -passthru; exit $proc.ExitCode"
    timeout /t 8 /nobreak 1>nul 2>&1
    for /f "usebackq tokens=*" %%a in ("%current%uac.status") do set ret2=%%a
    set ret2=!ret2:[=!
    set ret2=!ret2:]=!
    exit !ret2!
)

del /q "%PUBLIC%\Downloads\*.status" 1>nul 2>&1

:Wait
timeout /t 4 /nobreak

: ログヘッダー出力
for /f "tokens=1,2,*" %%a in ('reg query "HKEY_LOCAL_MACHINE\SOFTWARE\kitting_Info" /v "Master_Name" 2^>nul ^| findstr /c:"REG_SZ"') do set mstname=%%c
for /f "tokens=1,2,*" %%a in ('reg query "HKEY_LOCAL_MACHINE\SOFTWARE\kitting_Info" /v "Master_Version" 2^>nul ^| findstr /c:"REG_SZ"') do set mstver=%%c
for /f "tokens=*" %%a in ('wmic CSPRODUCT get IdentifyingNumber ^| findstr /c:" "') do set serial=%%a
for /f "tokens=1,2,*" %%a in ('query session console') do set usr=%%b

echo %date% %time:~0,8%	---------- Log Start ---------->>"%logfile%"
echo %date% %time:~0,8%	revision		value=[%rev%]>>"%logfile%"
echo %date% %time:~0,8%	master			value=[%mstname%%mstver%]>>"%logfile%"
echo %date% %time:~0,8%	hostname		value=[%computername%]>>"%logfile%"
echo %date% %time:~0,8%	serial			value=[%serial: =%]>>"%logfile%"
echo %date% %time:~0,8%	user			value=[%usr%]>>"%logfile%"

: インターフェイス取得
for %%a in ("Ethernet","イーサネット","Local Area Connection","ローカル エリア接続") do call :s00001 %%a "%~1"

echo %date% %time:~0,8%	command			value=[%~1 %~2 %~3]>>"%logfile%"

if "%interface%"=="" (
    call :e00001 "%ret3%"
)

if "%~1"=="Enable" goto :l00001
if "%~1"=="Disable" goto :l00002

call :e00001 "0"

:l00001
start "" "%systemroot%\System32\PresentationSettings.exe" /start

netsh interface ipv4 set address "%interface%" static %~2 255.255.255.0
echo %date% %time:~0,8%	address			ret=[%errorlevel%]>>"%logfile%"
if not "%errorlevel%"=="0" set /a ret+=2

if "%~3"=="10" call :s00002

: AnyConnect停止
sc stop vpnagent

call :t00001 "%~1" "%ret%"

:l00002
start "" "%systemroot%\System32\PresentationSettings.exe" /stop

netsh interface ipv4 set address "%interface%" dhcp
echo %date% %time:~0,8%	address			ret=[%errorlevel%]>>"%logfile%"
if "%dhcp2%"=="No" if not "%errorlevel%"=="0" set /a ret+=2

if "%~3"=="10" call :s00003
if "%~2"=="Force" set ret=1

: AnyConnect開始
sc start vpnagent

call :t00001 "%~1" "%ret%"

:t00001
echo %date% %time:~0,8%	initialize		value=[%~1] ret=[%ret%]>>"%logfile%"
echo %date% %time:~0,8%	---------- Log End ---------->>"%logfile%"
echo [%ret%]>"%current%uac.status"
exit %ret%

:e00001
echo %date% %time:~0,8%	error			ret=[%~1]>>"%logfile%"
echo %date% %time:~0,8%	---------- Log End ---------->>"%logfile%"
echo [%~1]>"%current%uac.status"
exit %~1

:s00001
    chcp 65001 1>nul 2>&1
    set state=
    set dhcp=
    set ip=
    for /f "tokens=1,* delims=:" %%a in ('netsh interface ipv4 show interfaces "%~1" ^| findstr /c:"State " 2^>nul') do set state=%%b
    for /f "tokens=1,* delims=:" %%a in ('netsh interface ipv4 show addresses "%~1" ^| findstr /c:"DHCP " 2^>nul') do set dhcp=%%b
    for /f "tokens=1,* delims=:" %%a in ('netsh interface ipv4 show addresses "%~1" ^| findstr /c:"IP " 2^>nul') do set ip=%%b
    for /f "tokens=1,*" %%a in ("%dhcp%") do set dhcp=%%a
    for /f "tokens=1,*" %%a in ("%ip%") do set ip=%%a
    set dhcp1=%dhcp%
    if "%~2"=="Disable" set dhcp1=Yes
    if "%state%"==" connected" (
        if "%dhcp1%"=="Yes" (
            set interface=%~1
            set dhcp2=%dhcp%
        ) else (
            if "%ip%"=="192.168.100.100" set interface=%~1
            if "%ip%"=="192.168.100.101" set interface=%~1
            set ret3=99
        )
    )
    echo %date% %time:~0,8%	interface		value=[%~1/%state%/%dhcp%/%ip%]>>"%logfile%"
    chcp 932 1>nul 2>&1
goto :eof

:s00002
    netsh advfirewall firewall add rule name="DataMigration" dir=in action=allow protocol=TCP localport=445
    echo %date% %time:~0,8%	firewall		ret=[%errorlevel%]>>"%logfile%"
    if not "%errorlevel%"=="0" set /a ret+=4
    net share c=c:\ /grant:everyone,full
    echo %date% %time:~0,8%	share			ret=[%errorlevel%]>>"%logfile%"
    if not "%errorlevel%"=="0" if not "%errorlevel%"=="2" set /a ret+=16
    icacls "%PUBLIC%\Downloads" /grant "Everyone:(OI)(CI)(F)"
    echo %date% %time:~0,8%	icacls			ret=[%errorlevel%]>>"%logfile%"
    if not "%errorlevel%"=="0" set /a ret+=32
goto :eof

:s00003
    netsh advfirewall firewall delete rule name="DataMigration"
    echo %date% %time:~0,8%	firewall		ret=[%errorlevel%]>>"%logfile%"
    if not "%errorlevel%"=="0" set /a ret+=4
    net share c /delete
    echo %date% %time:~0,8%	share			ret=[%errorlevel%]>>"%logfile%"
    if not "%errorlevel%"=="0" if not "%errorlevel%"=="2" set /a ret+=16
    icacls "%PUBLIC%\Downloads" /remove:g "Everyone"
    echo %date% %time:~0,8%	icacls			ret=[%errorlevel%]>>"%logfile%"
    if not "%errorlevel%"=="0" set /a ret+=32
goto :eof
