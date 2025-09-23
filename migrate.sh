#!/bin/sh

echo "Pokrećem migracije..."

dotnet tool restore

# Stakeholders migracije
dotnet ef database update \
    -s "$STARTUP_PROJECT/$STARTUP_PROJECT.csproj" \
    -p "Modules/Stakeholders/$STAKEHOLDERS_TARGET_PROJECT/$STAKEHOLDERS_TARGET_PROJECT.csproj" \
    -c StakeholdersContext \
    --configuration Release

# Tours migracije
dotnet ef database update \
    -s "$STARTUP_PROJECT/$STARTUP_PROJECT.csproj" \
    -p "Modules/Tours/$TOURS_TARGET_PROJECT/$TOURS_TARGET_PROJECT.csproj" \
    -c ToursContext \
    --configuration Release

# Blog migracije
dotnet ef database update \
    -s "$STARTUP_PROJECT/$STARTUP_PROJECT.csproj" \
    -p "Modules/Blog/$BLOG_TARGET_PROJECT/$BLOG_TARGET_PROJECT.csproj" \
    -c BlogContext \
    --configuration Release

echo "Migracije završene!"
