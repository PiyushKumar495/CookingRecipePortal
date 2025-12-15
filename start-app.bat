@echo off
echo Starting Recipe Portal Application...
echo.

echo Starting .NET Backend...
start "Backend" cmd /k "cd CookingRecipePortal && dotnet run"

echo Waiting for backend to start...
timeout /t 10 /nobreak > nul

echo Starting Angular Frontend...
start "Frontend" cmd /k "cd recipe-frontend && ng serve --port 4200"

echo.
echo Recipe Portal is starting up!
echo Backend: https://localhost:7297
echo Frontend: http://localhost:4200
echo.
echo Press any key to exit...
pause > nul