# Inventory API

## Overview

This project implements a simple inventory management system using ASP.NET Core, CQRS, and clean architecture principles.

## Features

- Product management (adding, editing, deleting, (de)activating)
- Customer management (adding)
- Order processing (creating, changing status)
- Discount rules
- Location-based pricing
- Validation and error handling
- Logging messages and correlation id for tracing

## Requirements

- .NET 8+
- Database provider (SQLite, can be changed)

## Setup
dotnet restore
dotnet build
dotnet run --project src/Inventory.Api


## Project Structure

See `/docs/NOTES.md` for architectural notes.

## Testing
dotnet test


## License

MIT

