@echo off

set LIBS_DIRECTORY=%~dp0\x64\lib

for %%G in (FAudio FNA3D libtheorafile SDL2) do (
	:: generate def file
	echo LIBRARY %%G > %LIBS_DIRECTORY%\%%G.def
	echo EXPORTS >> %LIBS_DIRECTORY%\%%G.def
	for /f "skip=19 tokens=4" %%A in ('dumpbin /exports %LIBS_DIRECTORY%\..\%%G.dll') do @echo %%A >> %LIBS_DIRECTORY%\%%G.def
	:: generate lib file from def file
	lib /def:%LIBS_DIRECTORY%\%%G.def /out:%LIBS_DIRECTORY%\%%G.lib /machine:x64
)

:: delete leftover exp files
del /f %LIBS_DIRECTORY%\*.exp
del /f %LIBS_DIRECTORY%\*.def