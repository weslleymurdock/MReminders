#!/bin/sh

# Espera até que o SQL Server esteja disponível
echo "Esperando pelo SQL Server..."
while ! nc -z sqlserver 1433; do
  sleep 1
done

echo "SQL Server está disponível. Iniciando aplicação..."
# Inicia a aplicação
exec dotnet MReminders.API.Server.dll
