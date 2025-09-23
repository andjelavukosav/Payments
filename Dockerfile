# =============================
# 1️⃣ SDK image za build i EF tools
# =============================
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /src
COPY . .

# Postavljamo working dir za projekat
WORKDIR /src/src
RUN dotnet restore Explorer.API/Explorer.API.csproj
RUN dotnet build Explorer.API/Explorer.API.csproj -c Release
RUN dotnet publish Explorer.API/Explorer.API.csproj -c Release -o /app/publish

# =============================
# 2️⃣ Target za migracije (samo EF, ne startuje server)
# =============================
FROM build AS migration

# Instaliramo EF tool globalno
RUN dotnet new tool-manifest --force
RUN dotnet tool install --global dotnet-ef --version 7.*
ENV PATH="$PATH:/root/.dotnet/tools"

# ENV varijable za migracije
ENV STARTUP_PROJECT=Explorer.API
ENV MIGRATION=init
ENV DATABASE_SCHEMA=""
ENV DATABASE_HOST=""
ENV DATABASE_PASSWORD=""
ENV DATABASE_USERNAME=""
ENV STAKEHOLDERS_TARGET_PROJECT=Explorer.Stakeholders.Infrastructure
ENV TOURS_TARGET_PROJECT=Explorer.Tours.Infrastructure
ENV BLOG_TARGET_PROJECT=Explorer.Blog.Infrastructure

# CMD za migracije
CMD ["sh", "-c", "\
dotnet tool restore && \
dotnet ef database update -s \"$STARTUP_PROJECT/$STARTUP_PROJECT.csproj\" -p \"Modules/Stakeholders/$STAKEHOLDERS_TARGET_PROJECT/$STAKEHOLDERS_TARGET_PROJECT.csproj\" -c StakeholdersContext --configuration Release && \
dotnet ef database update -s \"$STARTUP_PROJECT/$STARTUP_PROJECT.csproj\" -p \"Modules/Tours/$TOURS_TARGET_PROJECT/$TOURS_TARGET_PROJECT.csproj\" -c ToursContext --configuration Release && \
dotnet ef database update -s \"$STARTUP_PROJECT/$STARTUP_PROJECT.csproj\" -p \"Modules/Blog/$BLOG_TARGET_PROJECT/$BLOG_TARGET_PROJECT.csproj\" -c BlogContext --configuration Release \
"]

# =============================
# 3️⃣ Target za runtime server
# =============================
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime

WORKDIR /app

# Kopiramo publish folder iz build stage-a
COPY --from=build /app/publish .

# Startujemo server
CMD ["dotnet", "Explorer.API.dll"]
