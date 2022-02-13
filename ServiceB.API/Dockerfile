FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ServiceB.API/ServiceB.API.csproj", "ServiceB.API/"]
RUN dotnet restore "ServiceB.API/ServiceB.API.csproj"
COPY . .
WORKDIR "/src/ServiceB.API"
RUN dotnet build "ServiceB.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ServiceB.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ServiceB.API.dll"]
