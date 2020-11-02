REM =============================
REM If you want to clean the registry of the Assembly automatically, please create a file named
REM CleanAsm.bat and place in Debug/Release folder. This file will be called automatically at pre-build action.
REM 
REM Sample of the CleanAsm.bat
REM 	REM Content of CleanAsm.bat
REM 	IF NOT EXIST "%windir%\Microsoft.NET\Framework" (
REM 	exit 0
REM 	)
REM 	IF NOT EXIST "%windir%\Microsoft.NET\Framework\v2.0.50727" (
REM 	exit 0
REM 	)
REM 	set path=%path%;%windir%\Microsoft.NET\Framework\v2.0.50727
REM 	IF EXIST "Kofax.Connector.Text.dll" (
REM 	RegAsm.exe Kofax.Connector.Text.dll /u
REM		del Kofax.Connector.Text.dll /s /q
REM		)
IF EXIST "CleanAsm.bat" (
call CleanAsm.bat
)