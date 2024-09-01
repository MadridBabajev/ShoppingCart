# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory inside the container
WORKDIR /src

# Copy the solution and project files
# Base projects
COPY *.sln .
COPY Base.DAL/*.csproj ./Base.DAL/
COPY Base.DAL.Contracts/*.csproj ./Base.DAL.Contracts/
COPY Base.Domain/*.csproj ./Base.Domain/
COPY Base.Domain.Contracts/*.csproj ./Base.Domain.Contracts/

# App projects
COPY Helpers/*.csproj ./Helpers/
COPY App.DAL.Contracts/*.csproj ./App.DAL.Contracts/
COPY App.DAL/*.csproj ./App.DAL/
COPY App.Domain/*.csproj ./App.Domain/
COPY WebApp/*.csproj ./WebApp/

# Restore the dependencies
RUN dotnet restore

# Copy the rest of the files and build the app
COPY . .

WORKDIR /src/WebApp
RUN dotnet publish -c Release -o out

# Use the official .NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app
EXPOSE 80

# Copy the build output to the runtime image
COPY --from=build /src/WebApp/out .

# Start the application
ENTRYPOINT ["dotnet", "WebApp.dll"]
