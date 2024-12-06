# Use the official .NET runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Use the official .NET SDK as a parent image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["FlappyBird.csproj", "./"]
RUN dotnet restore "./FlappyBird.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "FlappyBird.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FlappyBird.csproj" -c Release -o /app/publish

# Use the base image to run the app
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FlappyBird.dll"]