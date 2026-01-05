FROM mcr.microsoft.com/dotnet/sdk:10.0 as build
WORKDIR /app
EXPOSE 80

COPY iowa.sln iowa.sln
COPY src/Iowa.csproj src/Iowa.csproj
COPY test/Test.csproj test/Test.csproj
COPY provider/Provider.csproj provider/Provider.csproj

RUN dotnet restore iowa.sln

COPY src src
WORKDIR /app/src
RUN dotnet publish -c Release -o /app/src/out
COPY src/Databases/App/Tables/Package/Packages.xlsx /app/src/out/Databases/App/Tables/Package/Packages.xlsx
COPY src/Databases/App/Tables/Provider/Providers.xlsx /app/src/out/Databases/App/Tables/Provider/Providers.xlsx

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /app/src/out .
ENTRYPOINT [ "dotnet", "Iowa.dll" ]