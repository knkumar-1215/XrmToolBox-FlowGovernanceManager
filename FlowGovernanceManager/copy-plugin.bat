@echo off
taskkill /F /IM XrmToolBox.exe >nul 2>&1

set PLUGIN_DIR=C:\Users\nagen\AppData\Roaming\MscrmTools\XrmToolBox\Plugins
set BUILD_DIR=%~dp1

copy /Y "%~1" "%PLUGIN_DIR%\"
copy /Y "%BUILD_DIR%MiniExcel.dll" "%PLUGIN_DIR%\"

echo Done.