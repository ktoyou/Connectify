#!/bin/bash

set -e

echo "🚀 Building Docker images..."
docker-compose -f docker-compose.debug.yml build

echo "🔄 Starting containers..."
docker-compose -f docker-compose.debug.yml up -d

echo "⏳ Waiting for services to start..."
sleep 8

container_name="connectify-debug"
http_port=5000

echo ""
echo "✅ Containers are up and running!"
echo "---------------------------------------------"
echo "Backend (HTTP):  http://localhost:${http_port}"
echo "Swagger UI:      http://localhost:${http_port}/swagger"
echo "---------------------------------------------"
echo ""
