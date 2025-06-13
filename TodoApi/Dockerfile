


## Use official .NET SDK to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Add this line before RUN dotnet publish
COPY .env .env

# Copy rest of the project and build
COPY . ./
RUN dotnet publish -c Release -o out

# Use smaller runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Install wait-for-it script
RUN apt-get update && apt-get install -y wget netcat-openbsd && rm -rf /var/lib/apt/lists/*
RUN wget -O /wait-for-it.sh https://raw.githubusercontent.com/vishnubob/wait-for-it/master/wait-for-it.sh
RUN chmod +x /wait-for-it.sh

COPY --from=build /app/out ./

# Set env variables
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

# Wait for database and then start the application
ENTRYPOINT ["/wait-for-it.sh", "db:5432", "--timeout=60", "--", "dotnet", "TodoApi.dll"]