FROM microsoft/dotnet:2.1-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY XUnitTestProject1/XUnitTestProject1.csproj XUnitTestProject1/
RUN dotnet restore XUnitTestProject1/XUnitTestProject1.csproj
COPY . .
WORKDIR /src/XUnitTestProject1
RUN dotnet build XUnitTestProject1.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish XUnitTestProject1.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "XUnitTestProject1.dll"]
