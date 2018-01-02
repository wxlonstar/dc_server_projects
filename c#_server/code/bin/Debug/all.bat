
@echo off 

echo wscript.sleep 6000>sleep.vbs
#@cscript sleep.vbs >nul
start Global.exe

echo wscript.sleep 6000>sleep.vbs
#@cscript sleep.vbs >nul
start World.exe

#echo wscript.sleep 3000>sleep.vbs
#@cscript sleep.vbs >nul
start Fight.exe

#echo wscript.sleep 3000>sleep.vbs
#@cscript sleep.vbs >nul
start Server.exe

#echo wscript.sleep 3000>sleep.vbs
#@cscript sleep.vbs >nul
start Gate.exe

del /f /s /q sleep.vbs

tskill cmd



