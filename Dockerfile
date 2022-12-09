FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Exchange.Api/Exchange.Api.csproj", "Exchange.Api/"]
RUN dotnet restore "Exchange.Api/Exchange.Api.csproj"
COPY . .
WORKDIR "/src/Exchange.Api"
RUN dotnet build "Exchange.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Exchange.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Exchange.Api.dll"]
