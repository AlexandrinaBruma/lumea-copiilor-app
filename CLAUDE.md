# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

**Lumea Copiilor Shop** — a WPF desktop application for managing a children's toy shop. Built with .NET 8.0-windows, C#, and SQL Server Express.

Solution file: `App/LumeaCopiilor/LumeaCopiilor.sln`

## Build & Run

```powershell
# From the solution directory
cd "App/LumeaCopiilor"

# Build
dotnet build

# Run
dotnet run
```

Or open `LumeaCopiilor.sln` in Visual Studio and press F5.

**Prerequisite**: SQL Server Express must be running locally with a database named `Lumea_Copiilor`. The connection string is hardcoded in every window/popup file:

```
Server=.\SQLEXPRESS;Database=Lumea_Copiilor;Integrated Security=True;TrustServerCertificate=True;
```

**Prerequisite**: The Montserrat font must be installed on the system (used globally via `App.xaml`).

## Architecture

The app uses **WPF code-behind** (no MVVM). Each `.xaml` file has a paired `.xaml.cs` that handles events and queries the database directly via `Microsoft.Data.SqlClient`. There is no service or repository layer.

### Window flow

```
HomeWindow (startup)
  └─ LoginWindow
       ├─ Role "A" → DashboardAdmin
       │    ├─ ProductPopup (double-click row) — view/edit/delete product
       │    ├─ UserPopup    (double-click row) — view/edit/delete user
       │    ├─ ImportatorPopup (double-click row) — view/edit/delete importer
       │    ├─ SearchPopup
       │    ├─ NewProductWindow — add product form
       │    └─ NewUserWindow   — add user form
       └─ Role "U" → DashboardUser
            ├─ UserAccountPage
            └─ productUserPopup → PurchaseWindow → ThankYouPage
  └─ AddUserAccount (self-registration)
```

Startup window is set in `App.xaml` via `StartupUri="HomeWindow.xaml"`.

### Popup pattern

All detail popups (`ProductPopup`, `UserPopup`, `ImportatorPopup`) follow the same pattern:
- Accept an entity ID in their constructor and call `LoadXxx()` immediately.
- Toggle between read-only and edit mode via `SetEditMode(bool)`, which swaps `TextBlock` visibility for editable input controls.
- FK ComboBoxes are lazy-loaded on first entry into edit mode (`_comboBoxesLoaded` flag).
- Parent window dims to `Opacity = 0.78` before `ShowDialog()` and restores to `1` after.

### Database schema

Tables used (inferred from queries):

| Table | Key columns |
|---|---|
| `Utilizator` | UtilizatorID, Username, Passwd, Name, Surname, Birthdate, Email, Phone_number, Gender ('M'/'F'), Registration_date, Role ('A'=Admin/'U'=User), City |
| `Product` | ProductID, Name, Min_age, Max_age, Fab_date, Exp_date, Price, Quantity, Origin_country→Country, Importator→Importator, Shop→Shop, Category→Category |
| `Category` | CategoryID, Name |
| `Importator` | ImportatorID, Company_name, City→City |
| `Shop` | ShopID, Street_address, City→City |
| `City` | CityID, Name, Country→Country |
| `Country` | CountryID, Name |

### Global styles (App.xaml)

All visual resources are defined in `App.xaml`. Reference them by key:

**Colors**: `LightColor` (#FFFFF8), `Peach` (#FF9A86), `DarkColor` (#410000), `Paragraph` (#733232), `Placeholder` (#AC9292), `Card` (#F5EAD6)

**Button styles**: `PrimaryButton`, `SecondaryButton`, `TertiaryButton`, `AdminButton`

**Other**: `ProductCard` (Border), `InputStyle` (TextBox), `ComboBoxStyle` — plus implicit styles for `TextBox`, `DataGrid`, `DataGridColumnHeader`, `DataGridRow`, `DataGridCell`, `xctk:DateTimePicker`, `xctk:IntegerUpDown`, `xctk:DecimalUpDown`

### NuGet dependencies

- `Microsoft.Data.SqlClient` 7.0.1 — database access
- `Extended.Wpf.Toolkit` 5.0.0 — `DateTimePicker`, `IntegerUpDown`, `DecimalUpDown` controls (namespace prefix `xctk`)

### Known incomplete features

`PurchaseWindow.SalveazaComanda()` has a `// TODO: Salvează în baza de date...` comment — the order is validated but not persisted to the database.
