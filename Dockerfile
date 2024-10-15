# Use the official ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

# Use the SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["BE-Teaching/BE-Teaching.csproj", "BE-Teaching/"]
RUN dotnet restore "BE-Teaching/BE-Teaching.csproj"
COPY . .
WORKDIR "/src/BE-Teaching"
RUN dotnet build "BE-Teaching.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BE-Teaching.csproj" -c Release -o /app/publish

# Use the runtime image to run the app
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BE-Teaching.dll"]
