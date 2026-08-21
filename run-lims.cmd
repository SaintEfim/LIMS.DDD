@echo off
setlocal enabledelayedexpansion

set "LIMS_NEED_BUILD=Y"
set "LIMS_ROOT_PATH="

if /i "%~1"=="N" set "LIMS_NEED_BUILD=N"
if /i "%~1"=="n" set "LIMS_NEED_BUILD=N"
if "%~1"=="0" set "LIMS_NEED_BUILD=N"

if "%LIMS_NEED_BUILD%"=="N" (
    if not "%~2"=="" set "LIMS_ROOT_PATH=%~2"
) else (
    if not "%~1"=="" set "LIMS_ROOT_PATH=%~1"
)

if "%LIMS_ROOT_PATH%"=="" set "LIMS_ROOT_PATH=%~dp0"

set "LIMS_BUILD_CONFIGURATION=Debug"
set "ASPNETCORE_ENVIRONMENT=Development"
set "TARGET_FRAMEWORK=net10.0"

echo ============================================================
echo  LIMS.DDD Startup
echo  Root:          %LIMS_ROOT_PATH%
echo  Build:         %LIMS_NEED_BUILD%
echo  Configuration: %LIMS_BUILD_CONFIGURATION%
echo ============================================================
echo.

set "USE_WT=0"
where wt.exe >nul 2>nul
if not errorlevel 1 set "USE_WT=1"

if "%USE_WT%"=="1" (
    echo Launcher: Windows Terminal
) else (
    echo Launcher: cmd.exe windows
)
echo.

REM ---- Запуск сервисов с явными портами ----
call :run_service Guides.Service                        Guides.Service                        "Units [Guides]"            1003
call :run_service LIMS.Service.Methodologies.API        LIMS.Service.Methodologies.API        "Methodologies"             1001
call :run_service LIMS.Service.LaboratoryOperations.API LIMS.Service.LaboratoryOperations.API "Laboratory Operations"     1002

echo.
echo ============================================================
echo  All services started.
echo ============================================================
pause
goto :eof

:run_service
set "FOLDER=%~1"
set "PROJECT=%~2"
set "TITLE=%~3"
set "PORT=%~4"
set "SRC_DIR=%LIMS_ROOT_PATH%src\%FOLDER%"
set "EXE_PATH=%SRC_DIR%\bin\%LIMS_BUILD_CONFIGURATION%\%TARGET_FRAMEWORK%\%PROJECT%.exe"

echo --- [%TITLE%] on port %PORT% ---

if not exist "%SRC_DIR%" (
    echo   [SKIP] Not found: %SRC_DIR%
    echo.
    goto :eof
)

if "%LIMS_NEED_BUILD%"=="Y" goto :build_%PROJECT%
goto :check_%PROJECT%

:build_Guides.Service
:build_LIMS.Service.Methodologies.API
:build_LIMS.Service.LaboratoryOperations.API
echo   Building...
pushd "%SRC_DIR%"
dotnet build --configuration %LIMS_BUILD_CONFIGURATION% --verbosity quiet
if errorlevel 1 (
    echo   [ERROR] Build failed!
    popd
    echo.
    goto :eof
)
popd

:check_Guides.Service
:check_LIMS.Service.Methodologies.API
:check_LIMS.Service.LaboratoryOperations.API
if not exist "%EXE_PATH%" (
    echo   [ERROR] Exe not found: %EXE_PATH%
    echo.
    goto :eof
)

echo   Starting on http://localhost:%PORT%...
if "%USE_WT%"=="1" (
    start "" wt.exe new-tab --title "%TITLE%" -d "%SRC_DIR%" cmd /K "set ASPNETCORE_ENVIRONMENT=%ASPNETCORE_ENVIRONMENT% && "%EXE_PATH%" --urls=http://localhost:%PORT%"
) else (
    start "%TITLE%" cmd /K "cd /d "%SRC_DIR%" && set ASPNETCORE_ENVIRONMENT=%ASPNETCORE_ENVIRONMENT% && "%EXE_PATH%" --urls=http://localhost:%PORT%"
)
echo.
goto :eof