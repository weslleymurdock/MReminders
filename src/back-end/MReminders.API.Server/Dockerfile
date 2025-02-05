# Acesse https://aka.ms/customizecontainer para saber como personalizar seu contêiner de depuração e como o Visual Studio usa este Dockerfile para criar suas imagens para uma depuração mais rápida.

# Esta fase é usada durante a execução no VS no modo rápido (Padrão para a configuração de Depuração)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app

COPY certs/certificate.pem /etc/ssl/certs/certificate.pem
COPY certs/pk.key /etc/ssl/private/pk.key

# Atualiza os certificados confiáveis
USER root
RUN update-ca-certificates

EXPOSE 8080
EXPOSE 8081

# Esta fase é usada para compilar o projeto de serviço
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/back-end/MReminders.API.Server/MReminders.API.Server.csproj", "src/back-end/MReminders.API.Server/"]
COPY ["src/back-end/MReminders.API.Application/MReminders.API.Application.csproj", "src/back-end/MReminders.API.Application/"]
COPY ["src/back-end/MReminders.API.Domain/MReminders.API.Domain.csproj", "src/back-end/MReminders.API.Domain/"]
COPY ["src/back-end/MReminders.API.Infrastructure/MReminders.API.Infrastructure.csproj", "src/back-end/MReminders.API.Infrastructure/"]
RUN dotnet restore "./src/back-end/MReminders.API.Server/MReminders.API.Server.csproj"
COPY . .
WORKDIR "/src/src/back-end/MReminders.API.Server"
RUN dotnet build "./MReminders.API.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build
# Esta fase é usada para publicar o projeto de serviço a ser copiado para a fase final
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MReminders.API.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
 
# Esta fase é usada na produção ou quando executada no VS no modo normal (padrão quando não está usando a configuração de Depuração)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish . 
# Copie o restante da aplicação para o contêiner
COPY --from=base /etc/ssl/certs/certificate.pem /etc/ssl/certs/certificate.pem
COPY --from=base /etc/ssl/private/pk.key /etc/ssl/private/pk.key
USER root
# Atualize os certificados confiáveis
RUN update-ca-certificates

ENTRYPOINT ["dotnet", "MReminders.API.Server.dll"]