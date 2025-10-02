# AnimeHub Publishing Plan

## Overview
To publish the AnimeHub ASP.NET Core MVC application for free online access, we'll use Azure App Service (free tier) for hosting and PlanetScale (free MySQL) for the database.

## Prerequisites
- Azure account (free tier available with $200 credit)
- PlanetScale account (free MySQL database)

## Steps

### 1. Set Up Production Database
- Sign up for PlanetScale (planetscale.com)
- Create a new database
- Get the connection string (it will look like: `mysql://user:password@host/database?sslaccept=strict`)

### 2. Update Application Configuration
- Update `appsettings.json` with the PlanetScale connection string
- Ensure `ASPNETCORE_ENVIRONMENT` is set to `Production` in Azure

### 3. Create Azure Resources
- Create Azure App Service (Web App) with Free tier (F1)
- Configure runtime stack to .NET 8
- Set up deployment (GitHub Actions or Visual Studio publish)

### 4. Deploy Application
- Use Visual Studio "Publish" feature or Azure CLI
- Publish to the created App Service
- The app will run migrations on startup

### 5. Test and Configure
- Access the site via Azure-provided URL
- Test search functionality
- Optionally configure custom domain

## Free Resources Used
- Azure App Service Free tier: 1 GB RAM, 1 hour/day compute
- PlanetScale Free tier: 1 database, 1 GB storage

## Notes
- Free tiers have limitations (e.g., Azure free tier has compute time limits)
- For production use, consider paid tiers for reliability
- Database seeding happens on first run, ensure connection is correct