@echo off
title renderserver thingy
taskkill /f /im renderserver.exe
renderserver /regserver
:start
thumbserver
goto start