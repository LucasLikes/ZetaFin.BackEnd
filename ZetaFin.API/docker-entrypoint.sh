#!/bin/bash
set -e

echo "================================================"
echo "  ZetaFin API - Iniciando..."
echo "  Ambiente: $ASPNETCORE_ENVIRONMENT"
echo "================================================"

echo "Aguardando banco de dados..."
sleep 5

echo "Iniciando aplicação..."
exec dotnet ZetaFin.API.dll