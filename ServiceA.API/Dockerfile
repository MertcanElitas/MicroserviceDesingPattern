FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ServiceA.API/ServiceA.API.csproj", "ServiceA.API/"]
RUN dotnet restore "ServiceA.API/ServiceA.API.csproj"
COPY . .
WORKDIR "/src/ServiceA.API"
RUN dotnet build "ServiceA.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ServiceA.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ServiceA.API.dll"]
