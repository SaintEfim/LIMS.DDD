@echo off
setlocal

set "LIMS_NEED_BUILD=Y"
set "LIMS_ROOT_PATH="

if /i "%~1"=="N" set "LIMS_NEED_BUILD=N"
if /i "%~1"=="0" set "LIMS_NEED_BUILD=N"

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
echo  Environment:   %ASPNETCORE_ENVIRONMENT%
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

call :run_service Guides.Service                        Guides.Service                        "Units [Guides]"
call :run_service LIMS.Service.Methodologies.API        LIMS.Service.Methodologies.API        "Methodologies"
call :run_service LIMS.Service.LaboratoryOperations.API LIMS.Service.LaboratoryOperations.API "Laboratory Operations"

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
set "SRC_DIR=%LIMS_ROOT_PATH%src\%FOLDER%"

echo --- [%TITLE%] ---

if not exist "%SRC_DIR%" (
    echo   [SKIP] Not found: %SRC_DIR%
    echo.
    goto :eof
)

if "%LIMS_NEED_BUILD%"=="Y" (
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
)

echo   Starting...

if "%USE_WT%"=="1" (
    start "" wt.exe new-tab ^
        --title "%TITLE%" ^
        -d "%SRC_DIR%" ^
        cmd /K "set ASPNETCORE_ENVIRONMENT=%ASPNETCORE_ENVIRONMENT% && dotnet run --no-build"
) else (
    start "%TITLE%" ^
        cmd /K "cd /d "%SRC_DIR%" && set ASPNETCORE_ENVIRONMENT=%ASPNETCORE_ENVIRONMENT% && dotnet run --no-build"
)

echo.
goto :eof