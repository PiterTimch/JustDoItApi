@echo off

echo Docker login...
docker login

echo Building Docker image api...
docker build -t task-api . 

echo Tagging Docker image api...
docker tag task-api:latest pedro007salo/task-api:latest

echo Pushing Docker image api to repository...
docker push pedro007salo/task-api:latest

echo Done ---api---!
pause
 