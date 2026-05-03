# AquaPure Water Plant Web App
## Quick Start (3 Steps Only!)

### STEP 1 — Install .NET 8 SDK (if not already installed)
Download from: https://dotnet.microsoft.com/download/dotnet/8.0
Choose: ".NET 8.0 SDK" (not Runtime)

### STEP 2 — Setup Database
1. Open SQL Server Management Studio (SSMS)
2. Connect to your SQL Server
3. Open file: Database_Schema.sql
4. Press F5 to run it
5. You will see: "WaterPlantDB setup complete!"

### STEP 3 — Update Connection String (if needed)
Open appsettings.json and change the Server name:

  "Server=.\SQLEXPRESS"     ← for SQL Server Express
  "Server=localhost"         ← for full SQL Server
  "Server=(localdb)\MSSQLLocalDB"  ← for LocalDB

### STEP 4 — Run the App
Double-click: RUN_ME_FIRST.bat
OR open Command Prompt in this folder and type: dotnet run

### Your App URLs:
- Home:     http://localhost:5000
- Stores:   http://localhost:5000/Store        ← QR Code points here
- QR Code:  http://localhost:5000/Store/QRPage ← Download QR for bottle
- Admin:    http://localhost:5000/Store/Manage

## File Structure
WaterPlantApp/
├── WaterPlantApp.csproj       ← Project file
├── Program.cs                 ← App startup
├── appsettings.json           ← Config (edit connection string here)
├── Database_Schema.sql        ← Run this in SSMS first!
├── RUN_ME_FIRST.bat           ← Double-click to start
├── Controllers/
│   ├── HomeController.cs
│   └── StoreController.cs
├── Data/
│   └── AppDbContext.cs
├── Models/
│   └── Store.cs
├── Views/
│   ├── Home/Index.cshtml
│   ├── Store/Index.cshtml     ← Public store list (QR lands here)
│   ├── Store/Details.cshtml
│   ├── Store/QRPage.cshtml    ← Download QR code here
│   ├── Store/Manage.cshtml
│   ├── Store/Create.cshtml
│   ├── Store/Edit.cshtml
│   └── Shared/_Layout.cshtml
└── wwwroot/
    ├── css/site.css
    └── js/site.js
