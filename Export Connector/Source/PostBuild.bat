setlocal
@echo off
IF NOT EXIST "%ProgramFiles%\Microsoft\ILMerge\ILMerge.exe" (
echo ***** =====================================================
echo ***** ILMerge tool does not exist.
echo ***** Check IlMerge tool from http://research.microsoft.com/en-us/people/mbarnett/ILMerge.aspx
echo ***** Keep build going.
echo ***** =====================================================
exit 1
)
echo current directory is [%CD%]
set path=%path%;%ProgramFiles%\Microsoft\ILMerge;
)
REM Sets ROOTDIR to KCEC-TEXT folder
set ROOTDIR=..\..\..\..
set PUBLISHDIR=%ROOTDIR%\Publish

if not exist %PUBLISHDIR% (
echo Creating %PUBLISHDIR% directory...
md %PUBLISHDIR%
)
copy %ROOTDIR%\Source\KCEC-Text.snk

if exist "%PUBLISHDIR%\Kofax.Connector.Text.dll" (
del "%PUBLISHDIR%\Kofax.Connector.Text.dll" /s /q
)

echo ***** =====================================================
echo ***** Merging libraries Kofax.Connector.Text.dll, Kofax.Connector.Common.dll, 
echo ***** Kofax.CapLib4.Interop.dll to single Kofax.Connector.Text.dll library...
echo ***** =====================================================
ilmerge /lib: Kofax.Text.Connector.dll Kofax.Connector.Common.dll Kofax.CapLib4.Interop.dll Kofax.ReleaseLib.Interop.dll /log:merge.log /internalize /out:Kofax.Connector.Text.dll

copy Kofax.Connector.Text.dll %PUBLISHDIR%
copy %ROOTDIR%\Source\INF\KCEC-Text.inf %PUBLISHDIR%
copy %ROOTDIR%\Source\Resources\KCEC-Text.ico %PUBLISHDIR%

