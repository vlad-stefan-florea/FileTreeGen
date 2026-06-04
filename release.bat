@echo off
setlocal
set "MODE=%~1"
set "VERSION=%~2"
powershell -ExecutionPolicy Bypass -File "%~dp0release_script.ps1" -Mode "%MODE%" -CustomVersion "%VERSION%"
start "" "%~dp0releases"