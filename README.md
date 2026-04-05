# Kardex Bodega Web

`kardex-Web` is a Razor Pages inventory control application for a warehouse environment. It is built on ASP.NET Core 10 with MySQL and includes offline frontend assets, print support, and cookie-based login protection.

## Key features

- ASP.NET Core Razor Pages web application
- MySQL database access using `MySqlConnector`
- Cookie authentication with login/logout protection
- Offline frontend assets for Bootstrap, jQuery, DataTables, and FontAwesome
- Print-friendly output via `PrintController` and `wwwroot/css/print.css`
- Soft delete support through `Eliminado` flags on key tables
- Global CRUD screens and register pages for inventory entities

## Project structure

- `Program.cs` - app startup, authentication, and Razor Pages config
- `Controllers/PrintController.cs` - print page generation
- `Pages/Shared/_Layout.cshtml` - shared navigation and asset includes
- `wwwroot/lib` - local frontend libraries
- `wwwroot/css/site.css` - shared CSS and FontAwesome helpers
- `kardex-Web.csproj` - .NET 10 web project config

## Requirements

- .NET 10 SDK
- MySQL database
- `web.config` contains connection settings and optional configuration

## Setup

1. Restore packages:

```bash
dotnet restore
```

2. Configure your database connection in `web.config`.

3. Apply required database schema changes, including soft-delete support:

```sql
ALTER TABLE usuarios ADD Eliminado TINYINT(1) NOT NULL DEFAULT 0;
ALTER TABLE categoria ADD Eliminado TINYINT(1) NOT NULL DEFAULT 0;
ALTER TABLE area_trabajo ADD Eliminado TINYINT(1) NOT NULL DEFAULT 0;
ALTER TABLE proyecto ADD Eliminado TINYINT(1) NOT NULL DEFAULT 0;
ALTER TABLE periodo ADD Eliminado TINYINT(1) NOT NULL DEFAULT 0;
ALTER TABLE proveedor ADD Eliminado TINYINT(1) NOT NULL DEFAULT 0;
ALTER TABLE retira ADD Eliminado TINYINT(1) NOT NULL DEFAULT 0;
ALTER TABLE salida ADD Eliminado TINYINT(1) NOT NULL DEFAULT 0;
ALTER TABLE autoriza ADD Eliminado TINYINT(1) NOT NULL DEFAULT 0;
ALTER TABLE orden_compra ADD Eliminado TINYINT(1) NOT NULL DEFAULT 0;
```

4. Run the app:

```bash
dotnet run
```

## Usage

- Open the application in the browser at the URL shown by `dotnet run`.
- Use the login page to authenticate before accessing the protected area.
- Print pages use the `POST /print/html` endpoint and local `print.css` styling.

## Offline assets

The project includes these libraries locally under `wwwroot/lib`:

- `bootstrap`
- `jquery`
- `datatables`
- `fontawesome`

This keeps the UI working without external CDN dependencies.

## Notes

- Authentication is defined in `Program.cs` with cookie auth and anonymous access only for `/Login`, `/Logout`, `/Privacy`, and `/Error`.
- The print HTML is returned from `Controllers/PrintController.cs`.
- Button icons are enhanced with FontAwesome in the shared layout and button markup.

## Support

If you need to add or update functionality, start by reviewing `Pages/Shared/_Layout.cshtml` for navigation, `Program.cs` for auth/routes, and the `Pages/*` Razor pages for CRUD behavior.