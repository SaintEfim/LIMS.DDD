@echo off
setlocal EnableExtensions

set "LIMS_ROOT_PATH=%~dp0"
set "LIMS_NEED_BUILD=Y"
set "LIMS_START_DOCKER=Y"
set "LIMS_APPLY_MIGRATIONS=Y"
set "LIMS_BUILD_CONFIGURATION=Debug"
set "ASPNETCORE_ENVIRONMENT=Development"
set "LIMS_LOG_FILE=%~dp0run-lims.log"
set "LIMS_CURRENT_STAGE=Initialization"
set "LIMS_EXIT_CODE=1"
>"%LIMS_LOG_FILE%" echo [%date% %time%] LIMS startup started.

:parse_arguments
if "%~1"=="" goto arguments_parsed
if /i "%~1"=="--no-build" (
    set "LIMS_NEED_BUILD=N"
    shift
    goto parse_arguments
)
if /i "%~1"=="--no-docker" (
    set "LIMS_START_DOCKER=N"
    shift
    goto parse_arguments
)
if /i "%~1"=="--no-migrations" (
    set "LIMS_APPLY_MIGRATIONS=N"
    shift
    goto parse_arguments
)
if /i "%~1"=="--root" (
    if "%~2"=="" goto usage_error
    set "LIMS_ROOT_PATH=%~f2\"
    shift
    shift
    goto parse_arguments
)
if /i "%~1"=="N" (
    set "LIMS_NEED_BUILD=N"
    shift
    goto parse_arguments
)
if /i "%~1"=="0" (
    set "LIMS_NEED_BUILD=N"
    shift
    goto parse_arguments
)
goto usage_error

:arguments_parsed
if not exist "%LIMS_ROOT_PATH%LIMS.DDD.sln" (
    echo [ERROR] LIMS.DDD.sln was not found in "%LIMS_ROOT_PATH%".
    set "LIMS_CURRENT_STAGE=Project directory validation"
    goto startup_failed
)

call :configure_dotnet
if errorlevel 1 (
    echo [ERROR] .NET SDK was not found.
    echo Install the .NET 10 SDK or add its directory to PATH.
    set "LIMS_CURRENT_STAGE=.NET SDK validation"
    goto startup_failed
)
>>"%LIMS_LOG_FILE%" echo dotnet: %LIMS_DOTNET_PATH%

echo ============================================================
echo  LIMS.DDD Startup
echo  Root:          %LIMS_ROOT_PATH%
echo  Docker:        %LIMS_START_DOCKER%
echo  Migrations:    %LIMS_APPLY_MIGRATIONS%
echo  Build:         %LIMS_NEED_BUILD%
echo  Configuration: %LIMS_BUILD_CONFIGURATION%
echo  Environment:   %ASPNETCORE_ENVIRONMENT%
echo ============================================================
echo.

if "%LIMS_START_DOCKER%"=="Y" (
    set "LIMS_CURRENT_STAGE=Docker infrastructure startup"
    call :start_infrastructure
    if errorlevel 1 goto startup_failed
)

if "%LIMS_NEED_BUILD%"=="Y" (
    set "LIMS_CURRENT_STAGE=Solution build"
    call :build_solution
    if errorlevel 1 goto startup_failed
)

if "%LIMS_APPLY_MIGRATIONS%"=="Y" (
    set "LIMS_CURRENT_STAGE=Database migrations"
    call :apply_migrations
    if errorlevel 1 goto startup_failed
)

set "USE_WT=0"
where wt.exe >nul 2>nul
if not errorlevel 1 set "USE_WT=1"

if "%USE_WT%"=="1" (
    echo Launcher: Windows Terminal
) else (
    echo Launcher: cmd.exe windows
)
echo.

set "LIMS_CURRENT_STAGE=Service.Guides.API startup"
call :run_service Service.Guides.API "Units [Guides]"
if errorlevel 1 goto startup_failed
set "LIMS_CURRENT_STAGE=LIMS.Service.Methodologies.API startup"
call :run_service LIMS.Service.Methodologies.API "Methodologies"
if errorlevel 1 goto startup_failed
set "LIMS_CURRENT_STAGE=Service.Reports startup"
call :run_service Service.Reports "Reports"
if errorlevel 1 goto startup_failed
set "LIMS_CURRENT_STAGE=LIMS.Service.LaboratoryOperations.API startup"
call :run_service LIMS.Service.LaboratoryOperations.API "Laboratory Operations"
if errorlevel 1 goto startup_failed

set "LIMS_CURRENT_STAGE=Application readiness check"
echo.
echo --- [Application readiness] ---
call :wait_for_url "http://localhost:1001/swagger/index.html" Methodologies 60
if errorlevel 1 goto startup_failed
call :wait_for_url "http://localhost:1002/swagger/index.html" "Laboratory Operations" 60
if errorlevel 1 goto startup_failed
call :wait_for_url "http://localhost:1003/swagger/index.html" Guides 60
if errorlevel 1 goto startup_failed
call :wait_for_url "http://localhost:1004/openapi/v1.json" Reports 60
if errorlevel 1 goto startup_failed

echo.
echo ============================================================
echo  All infrastructure and application services started.
echo ============================================================
>>"%LIMS_LOG_FILE%" echo [%date% %time%] All services were started successfully.
pause
exit /b 0

:configure_dotnet
set "LIMS_DOTNET_PATH="
for /f "delims=" %%I in ('where dotnet.exe 2^>nul') do if not defined LIMS_DOTNET_PATH set "LIMS_DOTNET_PATH=%%I"
if defined LIMS_DOTNET_PATH exit /b 0

if exist "%ProgramFiles%\dotnet\dotnet.exe" goto use_program_files_dotnet
if exist "%ProgramFiles(x86)%\dotnet\dotnet.exe" goto use_program_files_x86_dotnet
exit /b 1

:use_program_files_dotnet
set "DOTNET_ROOT=%ProgramFiles%\dotnet"
set "PATH=%ProgramFiles%\dotnet;%PATH%"
set "LIMS_DOTNET_PATH=%ProgramFiles%\dotnet\dotnet.exe"
exit /b 0

:use_program_files_x86_dotnet
set "DOTNET_ROOT=%ProgramFiles(x86)%\dotnet"
set "PATH=%ProgramFiles(x86)%\dotnet;%PATH%"
set "LIMS_DOTNET_PATH=%ProgramFiles(x86)%\dotnet\dotnet.exe"
exit /b 0

:start_infrastructure
call :configure_docker
if errorlevel 1 (
    echo [ERROR] Docker CLI was not found.
    echo Install Docker Desktop or add its directory to PATH.
    exit /b 1
)
>>"%LIMS_LOG_FILE%" echo docker: %LIMS_DOCKER_PATH%

docker info >nul 2>nul
if errorlevel 1 (
    echo [ERROR] Docker Engine is not running.
    exit /b 1
)

echo --- [Docker infrastructure] ---
docker compose -f "%LIMS_ROOT_PATH%docker-compose.yml" up -d
if errorlevel 1 (
    echo [ERROR] Failed to start Docker services.
    exit /b 1
)

call :wait_for_port localhost 5432 PostgreSQL 90
if errorlevel 1 exit /b 1
call :wait_for_port localhost 5672 RabbitMQ 90
if errorlevel 1 exit /b 1
call :wait_for_url "http://localhost:8081/realms/lims/.well-known/openid-configuration" Keycloak 120
if errorlevel 1 exit /b 1
echo.
exit /b 0

:configure_docker
set "LIMS_DOCKER_PATH="
for /f "delims=" %%I in ('where docker.exe 2^>nul') do if not defined LIMS_DOCKER_PATH set "LIMS_DOCKER_PATH=%%I"
if defined LIMS_DOCKER_PATH exit /b 0

if exist "%LOCALAPPDATA%\Programs\DockerDesktop\resources\bin\docker.exe" goto use_local_docker_desktop
if exist "%ProgramFiles%\Docker\Docker\resources\bin\docker.exe" goto use_program_files_docker_desktop
exit /b 1

:use_local_docker_desktop
set "PATH=%LOCALAPPDATA%\Programs\DockerDesktop\resources\bin;%PATH%"
set "LIMS_DOCKER_PATH=%LOCALAPPDATA%\Programs\DockerDesktop\resources\bin\docker.exe"
exit /b 0

:use_program_files_docker_desktop
set "PATH=%ProgramFiles%\Docker\Docker\resources\bin;%PATH%"
set "LIMS_DOCKER_PATH=%ProgramFiles%\Docker\Docker\resources\bin\docker.exe"
exit /b 0

:build_solution
echo --- [Build] ---
pushd "%LIMS_ROOT_PATH%"
dotnet build LIMS.DDD.sln --configuration %LIMS_BUILD_CONFIGURATION% --verbosity quiet
set "RESULT=%ERRORLEVEL%"
popd
if not "%RESULT%"=="0" (
    echo [ERROR] Solution build failed.
    exit /b %RESULT%
)
echo.
exit /b 0

:apply_migrations
echo --- [EF Core migrations] ---
pushd "%LIMS_ROOT_PATH%"
dotnet tool restore
if errorlevel 1 (
    echo [ERROR] Failed to restore local .NET tools.
    popd
    exit /b 1
)

call :update_database "src\Service.Guides.Persistence\Service.Guides.Persistence.csproj" "src\Service.Guides.API\Service.Guides.API.csproj" GuidesDb
if errorlevel 1 (
    popd
    exit /b 1
)
call :update_database "src\LIMS.Service.Methodologies.Persistence\LIMS.Service.Methodologies.Persistence.csproj" "src\LIMS.Service.Methodologies.API\LIMS.Service.Methodologies.API.csproj" MethodologiesDb
if errorlevel 1 (
    popd
    exit /b 1
)
call :update_database "src\LIMS.Service.LaboratoryOperations.Persistence\LIMS.Service.LaboratoryOperations.Persistence.csproj" "src\LIMS.Service.LaboratoryOperations.API\LIMS.Service.LaboratoryOperations.API.csproj" LaboratoryOperationsDb
if errorlevel 1 (
    popd
    exit /b 1
)
popd
echo.
exit /b 0

:update_database
echo   Updating %~3...
dotnet tool run dotnet-ef database update --project "%~1" --startup-project "%~2" --configuration %LIMS_BUILD_CONFIGURATION% --no-build
if errorlevel 1 (
    echo [ERROR] Migration failed for %~3.
    exit /b 1
)
exit /b 0

:run_service
set "PROJECT_NAME=%~1"
set "TITLE=%~2"
set "PROJECT_DIR=%LIMS_ROOT_PATH%src\%PROJECT_NAME%"
set "PROJECT_FILE=%PROJECT_DIR%\%PROJECT_NAME%.csproj"

if not exist "%PROJECT_FILE%" (
    echo [ERROR] Project was not found: %PROJECT_FILE%
    exit /b 1
)

echo --- Starting [%TITLE%] ---
if "%USE_WT%"=="1" (
    start "" wt.exe new-tab --title "%TITLE%" -d "%PROJECT_DIR%" cmd /K "set ASPNETCORE_ENVIRONMENT=%ASPNETCORE_ENVIRONMENT% && dotnet run --project ""%PROJECT_FILE%"" --no-build --configuration %LIMS_BUILD_CONFIGURATION%"
) else (
    start "%TITLE%" cmd /K "cd /d ""%PROJECT_DIR%"" && set ASPNETCORE_ENVIRONMENT=%ASPNETCORE_ENVIRONMENT% && dotnet run --project ""%PROJECT_FILE%"" --no-build --configuration %LIMS_BUILD_CONFIGURATION%"
)
exit /b 0

:wait_for_port
echo   Waiting for %~3 on %~1:%~2...
powershell.exe -NoProfile -ExecutionPolicy Bypass -Command "$deadline = [DateTime]::UtcNow.AddSeconds(%~4); do { try { $client = [Net.Sockets.TcpClient]::new(); $task = $client.ConnectAsync('%~1', %~2); if ($task.Wait(1000) -and $client.Connected) { $client.Dispose(); exit 0 }; $client.Dispose() } catch {}; Start-Sleep -Milliseconds 500 } while ([DateTime]::UtcNow -lt $deadline); exit 1"
if errorlevel 1 (
    echo [ERROR] %~3 did not become ready in %~4 seconds.
    exit /b 1
)
echo   %~3 is ready.
exit /b 0

:wait_for_url
echo   Waiting for %~2...
powershell.exe -NoProfile -ExecutionPolicy Bypass -Command "$deadline = [DateTime]::UtcNow.AddSeconds(%~3); do { try { $response = Invoke-WebRequest -UseBasicParsing -Uri '%~1' -TimeoutSec 2; if ($response.StatusCode -eq 200) { exit 0 } } catch {}; Start-Sleep -Seconds 1 } while ([DateTime]::UtcNow -lt $deadline); exit 1"
if errorlevel 1 (
    echo [ERROR] %~2 did not become ready in %~3 seconds.
    exit /b 1
)
echo   %~2 is ready.
exit /b 0

:usage_error
set "LIMS_CURRENT_STAGE=Command line argument validation"
set "LIMS_EXIT_CODE=2"
echo Usage: run-lims.cmd [--no-build] [--no-docker] [--no-migrations] [--root PATH]
goto startup_failed

:startup_failed
>>"%LIMS_LOG_FILE%" echo [%date% %time%] FAILED: %LIMS_CURRENT_STAGE%. Exit code: %LIMS_EXIT_CODE%.
echo.
echo ============================================================
echo  LIMS startup failed.
echo  Stage: %LIMS_CURRENT_STAGE%
echo  Exit code: %LIMS_EXIT_CODE%
echo  Review the error above.
echo  Log: %LIMS_LOG_FILE%
echo ============================================================
pause
exit /b %LIMS_EXIT_CODE%
