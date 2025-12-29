#!/bin/bash
set -e

echo "🚀 Iniciando ZetaFin API..."

# Aguardar PostgreSQL ficar disponível
echo "⏳ Aguardando PostgreSQL..."
until curl -s "http://postgres:5432" > /dev/null 2>&1 || \
      PGPASSWORD=$DB_PASSWORD psql -h "$DB_HOST" -U "$DB_USER" -d postgres -c '\q' 2>/dev/null; do
  echo "PostgreSQL não está pronto - aguardando..."
  sleep 2
done

echo "✅ PostgreSQL disponível!"

# Executar migrations
echo "🔄 Executando migrations..."
dotnet ZetaFin.API.dll --migrate

# Iniciar aplicação
echo "🎯 Iniciando aplicação..."
exec dotnet ZetaFin.API.dll