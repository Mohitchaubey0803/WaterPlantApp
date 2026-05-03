-- =============================================
-- AquaPure Water Plant - Database Setup
-- Run this in SQL Server Management Studio
-- =============================================

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'WaterPlantDB')
BEGIN
    CREATE DATABASE WaterPlantDB;
    PRINT 'Database WaterPlantDB created.';
END
ELSE
    PRINT 'Database WaterPlantDB already exists.';
GO

USE WaterPlantDB;
GO

-- ── DROP EXISTING TABLES ──────────────────────
IF OBJECT_ID('dbo.StoreProducts','U') IS NOT NULL DROP TABLE dbo.StoreProducts;
IF OBJECT_ID('dbo.Products','U')      IS NOT NULL DROP TABLE dbo.Products;
IF OBJECT_ID('dbo.Stores','U')        IS NOT NULL DROP TABLE dbo.Stores;
GO

-- ── STORES ───────────────────────────────────
CREATE TABLE dbo.Stores (
    StoreId           INT IDENTITY(1,1)  PRIMARY KEY,
    StoreName         NVARCHAR(100)      NOT NULL,
    StoreCode         NVARCHAR(20)       NOT NULL UNIQUE,
    Address           NVARCHAR(250)      NOT NULL,
    City              NVARCHAR(100)      NOT NULL,
    State             NVARCHAR(100)      NOT NULL,
    PinCode           NVARCHAR(10)       NOT NULL,
    PhoneNumber       NVARCHAR(20)       NOT NULL,
    EmailAddress      NVARCHAR(150)      NULL,
    ManagerName       NVARCHAR(100)      NOT NULL,
    ManagerPhone      NVARCHAR(20)       NULL,
    StoreType         NVARCHAR(50)       NOT NULL DEFAULT 'Retail',
    OperatingHours    NVARCHAR(100)      NULL,
    IsActive          BIT                NOT NULL DEFAULT 1,
    WaterCapacityLtrs INT                NULL,
    Description       NVARCHAR(500)      NULL,
    Latitude          DECIMAL(10,7)      NULL,
    Longitude         DECIMAL(10,7)      NULL,
    CreatedAt         DATETIME2          NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt         DATETIME2          NOT NULL DEFAULT GETUTCDATE()
);
GO

CREATE INDEX IX_Stores_City      ON dbo.Stores(City);
CREATE INDEX IX_Stores_IsActive  ON dbo.Stores(IsActive);
GO

-- ── PRODUCTS ─────────────────────────────────
CREATE TABLE dbo.Products (
    ProductId     INT IDENTITY(1,1) PRIMARY KEY,
    ProductName   NVARCHAR(100)     NOT NULL,
    ProductCode   NVARCHAR(20)      NOT NULL UNIQUE,
    SizeLtrs      DECIMAL(5,2)      NOT NULL,
    PricePerUnit  DECIMAL(10,2)     NOT NULL,
    Description   NVARCHAR(300)     NULL,
    IsActive      BIT               NOT NULL DEFAULT 1,
    CreatedAt     DATETIME2         NOT NULL DEFAULT GETUTCDATE()
);
GO

-- ── STORE PRODUCTS ────────────────────────────
CREATE TABLE dbo.StoreProducts (
    StoreProductId INT IDENTITY(1,1) PRIMARY KEY,
    StoreId        INT               NOT NULL,
    ProductId      INT               NOT NULL,
    StockQty       INT               NOT NULL DEFAULT 0,
    IsAvailable    BIT               NOT NULL DEFAULT 1,
    CONSTRAINT FK_SP_Store   FOREIGN KEY (StoreId)   REFERENCES dbo.Stores(StoreId),
    CONSTRAINT FK_SP_Product FOREIGN KEY (ProductId) REFERENCES dbo.Products(ProductId),
    CONSTRAINT UQ_StoreProduct UNIQUE (StoreId, ProductId)
);
GO

-- ── SEED: 4 STORES ────────────────────────────
INSERT INTO dbo.Stores
    (StoreName, StoreCode, Address, City, State, PinCode, PhoneNumber, EmailAddress, ManagerName, ManagerPhone, StoreType, OperatingHours, WaterCapacityLtrs, Description, Latitude, Longitude)
VALUES
    ('AquaPure - Main Branch',   'AP-001', '12, MG Road, Sector 14',    'Gurugram', 'Haryana', '122001', '+91-9876540001', 'main@aquapure.in',  'Rajesh Kumar',  '+91-9876540011', 'Retail',       '8:00 AM - 9:00 PM', 10000, 'Our flagship store with full range of products.',  28.4595, 77.0266),
    ('AquaPure - Sector 29',     'AP-002', '45, DLF Phase 2, Sec 29',   'Gurugram', 'Haryana', '122022', '+91-9876540002', 'sec29@aquapure.in', 'Sunita Sharma', '+91-9876540012', 'Retail',       '9:00 AM - 8:00 PM', 7000,  'Located in the heart of Sector 29.',               28.4721, 77.0480),
    ('AquaPure - Sohna Road',    'AP-003', '78, Sohna Road, Near Mall', 'Gurugram', 'Haryana', '122018', '+91-9876540003', 'sohna@aquapure.in', 'Amit Verma',    '+91-9876540013', 'Retail',       '8:30 AM - 8:30 PM', 8000,  'Convenient location with ample parking.',          28.4200, 77.0350),
    ('AquaPure - Warehouse Hub', 'AP-004', 'Plot 9, Industrial Area',   'Gurugram', 'Haryana', '122016', '+91-9876540004', 'hub@aquapure.in',   'Priya Singh',   '+91-9876540014', 'Distribution', '7:00 AM - 6:00 PM', 25000, 'Main distribution and wholesale hub.',             28.5000, 77.0600);
GO

-- ── SEED: 4 PRODUCTS ──────────────────────────
INSERT INTO dbo.Products (ProductName, ProductCode, SizeLtrs, PricePerUnit, Description)
VALUES
    ('AquaPure 500ml Bottle', 'PRD-001', 0.5,  10.00, 'Chilled purified drinking water 500ml'),
    ('AquaPure 1L Bottle',    'PRD-002', 1.0,  20.00, 'Purified drinking water 1 Litre'),
    ('AquaPure 5L Jar',       'PRD-003', 5.0,  60.00, 'Refillable 5 litre water jar'),
    ('AquaPure 20L Can',      'PRD-004', 20.0, 80.00, 'Home/office 20 litre water can');
GO

-- ── LINK ALL PRODUCTS TO ALL STORES ───────────
INSERT INTO dbo.StoreProducts (StoreId, ProductId, StockQty)
SELECT s.StoreId, p.ProductId, 100
FROM   dbo.Stores s CROSS JOIN dbo.Products p;
GO

PRINT '=============================================';
PRINT 'WaterPlantDB setup complete!';
PRINT '4 Stores + 4 Products inserted successfully.';
PRINT '=============================================';
