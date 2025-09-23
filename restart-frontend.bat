@echo off
echo Rebuilding and restarting frontend container with hot reload...

echo.
echo Stopping frontend container...
docker compose stop frontend

echo.
echo Removing frontend container...
docker compose rm -f frontend

echo.
echo Rebuilding frontend image...
docker compose build frontend

echo.
echo Starting frontend with development configuration...
docker compose -f docker-compose.yml -f docker-compose.dev.yml up frontend -d

echo.
echo Frontend container restarted with hot reload enabled!
echo You can check logs with: docker compose logs -f frontend
echo.
pause