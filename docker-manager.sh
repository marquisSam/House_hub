#!/bin/bash

# House Hub Docker Management Scripts

case "$1" in
  "dev")
    echo "Starting House Hub in development mode..."
    docker-compose -f docker-compose.yml -f docker-compose.dev.yml up --build
    ;;
  "prod")
    echo "Starting House Hub in production mode..."
    docker-compose -f docker-compose.yml -f docker-compose.prod.yml --profile production up --build -d
    ;;
  "stop")
    echo "Stopping all House Hub services..."
    docker-compose down
    ;;
  "clean")
    echo "Cleaning up House Hub containers and volumes..."
    docker-compose down -v --remove-orphans
    docker system prune -f
    ;;
  "logs")
    docker-compose logs -f
    ;;
  "backend-logs")
    docker-compose logs -f backend
    ;;
  "frontend-logs")
    docker-compose logs -f frontend
    ;;
  "db-logs")
    docker-compose logs -f database
    ;;
  *)
    echo "Usage: $0 {dev|prod|stop|clean|logs|backend-logs|frontend-logs|db-logs}"
    echo ""
    echo "Commands:"
    echo "  dev           - Start in development mode with hot reload"
    echo "  prod          - Start in production mode"
    echo "  stop          - Stop all services"
    echo "  clean         - Stop and remove all containers and volumes"
    echo "  logs          - Show logs for all services"
    echo "  backend-logs  - Show backend logs only"
    echo "  frontend-logs - Show frontend logs only"
    echo "  db-logs       - Show database logs only"
    exit 1
    ;;
esac