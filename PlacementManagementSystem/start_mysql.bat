@echo off
echo ===================================================
echo Starting Local MySQL 8.0 Server for Placement Portal
echo Port: 3307 (User-space instance)
echo ===================================================
"C:\Program Files\MySQL\MySQL Server 8.0\bin\mysqld.exe" --port=3307 --datadir="%~dp0..\mysql_data" --console
pause
