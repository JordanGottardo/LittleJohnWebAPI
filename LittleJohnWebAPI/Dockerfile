# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY LittleJohnWebAPI.csproj ./LittleJohnWebAPI/
RUN dotnet restore ./LittleJohnWebAPI/LittleJohnWebAPI.csproj

# copy everything else and build app
COPY . ./LittleJohnWebAPI/
WORKDIR /source/LittleJohnWebAPI
RUN dotnet publish -c Release -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "LittleJohnWebAPI.dll"]
EXPOSE 5100