FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY LoadBalance/LoadBalance.csproj LoadBalance/
RUN dotnet restore LoadBalance/LoadBalance.csproj
COPY . .
WORKDIR /src/LoadBalance
RUN dotnet build LoadBalance.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish LoadBalance.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "LoadBalance.dll"]
