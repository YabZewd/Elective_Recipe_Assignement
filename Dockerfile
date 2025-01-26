FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /App

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Debug -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App


COPY --from=build /App/out ./

EXPOSE 5001

ENTRYPOINT ["dotnet", "RecipeApp.dll"]
