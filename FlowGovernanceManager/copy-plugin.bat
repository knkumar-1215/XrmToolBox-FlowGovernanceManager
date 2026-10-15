@echo off
taskkill /F /IM XrmToolBox.exe >nul 2>&1
copy /Y "%~1" "C:\Users\nagen\AppData\Roaming\MscrmTools\XrmToolBox\Plugins\"
exit /b 0