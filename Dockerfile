#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["ChatyChaty/ChatyChaty.csproj", "ChatyChaty/"]
COPY ["ChatyChaty.Infrastructure/ChatyChaty.Infrastructure.csproj", "ChatyChaty.Infrastructure/"]
COPY ["ChatyChaty.Domain/ChatyChaty.Domain.csproj", "ChatyChaty.Domain/"]
RUN dotnet restore "ChatyChaty/ChatyChaty.csproj"
COPY . .
WORKDIR "/src/ChatyChaty"
RUN dotnet build "ChatyChaty.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChatyChaty.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet ChatyChaty.dll