@echo off
echo =========================================================
echo Launching College Placement Management System (ASP.NET Core)
echo =========================================================
cd /d "%~dp0"
dotnet run --urls "http://localhost:5000"
pause
