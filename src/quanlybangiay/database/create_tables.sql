-- SQL script to create required tables for the admin/shop system
-- Run on SQL Server (T-SQL)

SET NOCOUNT ON;

-- Roles (minimal)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Roles]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.Roles (
        RoleId INT IDENTITY(1,1) PRIMARY KEY,
        RoleName NVARCHAR(50) NOT NULL
    );
END

-- Categories (minimal)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Categories]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.Categories (
        CategoryId INT IDENTITY(1,1) PRIMARY KEY,
        CategoryName NVARCHAR(150) NOT NULL,
        Slug VARCHAR(220) NULL,
        IsActive BIT NOT NULL DEFAULT(1),
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL
    );
END

-- Addresses (minimal)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Addresses]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.Addresses (
        AddressId INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NULL,
        FullName NVARCHAR(200) NULL,
        Line1 NVARCHAR(500) NULL,
        Line2 NVARCHAR(500) NULL,
        City NVARCHAR(100) NULL,
        District NVARCHAR(100) NULL,
        Ward NVARCHAR(100) NULL,
        Phone VARCHAR(20) NULL
    );
END

-- Promotions (minimal)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Promotions]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.Promotions (
        PromotionId INT IDENTITY(1,1) PRIMARY KEY,
        Code VARCHAR(50) NOT NULL,
        Description NVARCHAR(500) NULL,
        DiscountAmount DECIMAL(18,2) NULL
    );
END

-- Users
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.Users (
        UserId INT IDENTITY(1,1) PRIMARY KEY,
        RoleId INT NOT NULL,
        FullName NVARCHAR(100) NULL,
        Email VARCHAR(150) NOT NULL,
        PasswordHash VARCHAR(255) NULL,
        Phone VARCHAR(20) NULL,
        Gender TINYINT NULL,
        DateOfBirth DATE NULL,
        AvatarUrl VARCHAR(255) NULL,
        IsActive BIT NOT NULL DEFAULT(1),
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL,
        CONSTRAINT UQ_Users_Email UNIQUE (Email),
        CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES dbo.Roles(RoleId)
    );
END

-- Products
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.Products (
        ProductId INT IDENTITY(1,1) PRIMARY KEY,
        CategoryId INT NOT NULL,
        ProductName NVARCHAR(200) NOT NULL,
        Slug VARCHAR(220) NOT NULL,
        Brand NVARCHAR(100) NULL,
        ShortDescription NVARCHAR(500) NULL,
        Description NVARCHAR(MAX) NULL,
        BasePrice DECIMAL(18,2) NOT NULL DEFAULT 0,
        DiscountPrice DECIMAL(18,2) NULL,
        ThumbnailUrl VARCHAR(255) NULL,
        Status TINYINT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL,
        CONSTRAINT UQ_Products_Slug UNIQUE (Slug),
        CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId) REFERENCES dbo.Categories(CategoryId)
    );
END

-- ProductVariants
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductVariants]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.ProductVariants (
        VariantId INT IDENTITY(1,1) PRIMARY KEY,
        ProductId INT NOT NULL,
        SKU VARCHAR(50) NOT NULL,
        SizeValue VARCHAR(10) NULL,
        ColorName NVARCHAR(50) NULL,
        ColorCode VARCHAR(10) NULL,
        StockQty INT NOT NULL DEFAULT 0,
        AdditionalPrice DECIMAL(18,2) NULL,
        WeightGram INT NULL,
        IsDefault BIT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT UQ_ProductVariants_SKU UNIQUE (SKU),
        CONSTRAINT FK_ProductVariants_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(ProductId)
    );
END

-- Orders
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.Orders (
        OrderId BIGINT IDENTITY(1,1) PRIMARY KEY,
        OrderCode VARCHAR(20) NOT NULL,
        UserId INT NULL,
        AddressId INT NULL,
        PromotionId INT NULL,
        GuestName NVARCHAR(100) NULL,
        GuestEmail NVARCHAR(150) NULL,
        GuestPhone NVARCHAR(20) NULL,
        GuestAddress NVARCHAR(500) NULL,
        Subtotal DECIMAL(18,2) NOT NULL DEFAULT 0,
        ShippingFee DECIMAL(18,2) NOT NULL DEFAULT 0,
        DiscountAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
        TotalAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
        PaymentStatus TINYINT NULL,
        OrderStatus TINYINT NULL,
        Note NVARCHAR(500) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL,
        CONSTRAINT UQ_Orders_OrderCode UNIQUE (OrderCode),
        CONSTRAINT FK_Orders_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId),
        CONSTRAINT FK_Orders_Addresses FOREIGN KEY (AddressId) REFERENCES dbo.Addresses(AddressId),
        CONSTRAINT FK_Orders_Promotions FOREIGN KEY (PromotionId) REFERENCES dbo.Promotions(PromotionId)
    );
END

-- Orders: allow guest checkout (existing databases)
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND type in (N'U'))
BEGIN
    -- Make UserId nullable (drop FK first, alter column, re-add FK)
    IF EXISTS (
        SELECT 1 FROM sys.foreign_keys
        WHERE name = 'FK_Orders_Users' AND parent_object_id = OBJECT_ID('dbo.Orders')
    )
    BEGIN
        -- Check if column is currently NOT NULL
        IF EXISTS (
            SELECT 1 FROM sys.columns
            WHERE object_id = OBJECT_ID('dbo.Orders')
              AND name = 'UserId'
              AND is_nullable = 0
        )
        BEGIN
            ALTER TABLE dbo.Orders DROP CONSTRAINT FK_Orders_Users;
            ALTER TABLE dbo.Orders ALTER COLUMN UserId INT NULL;
            ALTER TABLE dbo.Orders ADD CONSTRAINT FK_Orders_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId);
        END
    END

    -- Make AddressId nullable
    IF EXISTS (
        SELECT 1 FROM sys.foreign_keys
        WHERE name = 'FK_Orders_Addresses' AND parent_object_id = OBJECT_ID('dbo.Orders')
    )
    BEGIN
        IF EXISTS (
            SELECT 1 FROM sys.columns
            WHERE object_id = OBJECT_ID('dbo.Orders')
              AND name = 'AddressId'
              AND is_nullable = 0
        )
        BEGIN
            ALTER TABLE dbo.Orders DROP CONSTRAINT FK_Orders_Addresses;
            ALTER TABLE dbo.Orders ALTER COLUMN AddressId INT NULL;
            ALTER TABLE dbo.Orders ADD CONSTRAINT FK_Orders_Addresses FOREIGN KEY (AddressId) REFERENCES dbo.Addresses(AddressId);
        END
    END

    -- Add guest info columns if not present
    IF COL_LENGTH('dbo.Orders', 'GuestName') IS NULL
        ALTER TABLE dbo.Orders ADD GuestName NVARCHAR(100) NULL;

    IF COL_LENGTH('dbo.Orders', 'GuestEmail') IS NULL
        ALTER TABLE dbo.Orders ADD GuestEmail NVARCHAR(150) NULL;

    IF COL_LENGTH('dbo.Orders', 'GuestPhone') IS NULL
        ALTER TABLE dbo.Orders ADD GuestPhone NVARCHAR(20) NULL;

    IF COL_LENGTH('dbo.Orders', 'GuestAddress') IS NULL
        ALTER TABLE dbo.Orders ADD GuestAddress NVARCHAR(500) NULL;
END

-- OrderItems
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderItems]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.OrderItems (
        OrderItemId BIGINT IDENTITY(1,1) PRIMARY KEY,
        OrderId BIGINT NOT NULL,
        VariantId INT NOT NULL,
        ProductName NVARCHAR(200) NOT NULL,
        SizeValue VARCHAR(10) NULL,
        ColorName NVARCHAR(50) NULL,
        UnitPrice DECIMAL(18,2) NOT NULL,
        Quantity INT NOT NULL,
        LineTotal DECIMAL(18,2) NOT NULL,
        CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId) REFERENCES dbo.Orders(OrderId),
        CONSTRAINT FK_OrderItems_ProductVariants FOREIGN KEY (VariantId) REFERENCES dbo.ProductVariants(VariantId)
    );
END

-- Payments
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payments]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.Payments (
        PaymentId BIGINT IDENTITY(1,1) PRIMARY KEY,
        OrderId BIGINT NOT NULL,
        Method VARCHAR(30) NULL,
        GatewayTransactionId VARCHAR(100) NULL,
        Amount DECIMAL(18,2) NOT NULL,
        PaidAt DATETIME2 NULL,
        Status TINYINT NULL,
        ResponsePayload NVARCHAR(MAX) NULL,
        CONSTRAINT FK_Payments_Orders FOREIGN KEY (OrderId) REFERENCES dbo.Orders(OrderId)
    );
END

-- Posts
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Posts]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.Posts (
        PostId INT IDENTITY(1,1) PRIMARY KEY,
        Title NVARCHAR(200) NOT NULL,
        Slug VARCHAR(220) NULL,
        ShortDescription NVARCHAR(500) NULL,
        Content NVARCHAR(MAX) NULL,
        ThumbnailUrl VARCHAR(255) NULL,
        IsActive BIT NOT NULL DEFAULT(1),
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL,
        CreatedBy INT NULL,
        UpdatedBy INT NULL,
        CONSTRAINT UQ_Posts_Slug UNIQUE (Slug),
        CONSTRAINT FK_Posts_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId),
        CONSTRAINT FK_Posts_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES dbo.Users(UserId)
    );
END

-- Settings (single-record site configuration)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Settings]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.Settings (
        SettingId INT IDENTITY(1,1) PRIMARY KEY,
        ShopName NVARCHAR(200) NULL,
        LogoUrl VARCHAR(255) NULL,
        Phone VARCHAR(20) NULL,
        Hotline VARCHAR(20) NULL,
        Email VARCHAR(150) NULL,
        Address NVARCHAR(500) NULL,
        WorkingHours NVARCHAR(500) NULL,
        FooterContent NVARCHAR(MAX) NULL,
        CopyrightText NVARCHAR(255) NULL,
        FacebookUrl VARCHAR(255) NULL,
        InstagramUrl VARCHAR(255) NULL,
        YoutubeUrl VARCHAR(255) NULL,
        TiktokUrl VARCHAR(255) NULL,
        ZaloUrl VARCHAR(255) NULL,
        MetaTitle NVARCHAR(200) NULL,
        MetaDescription NVARCHAR(500) NULL,
        MetaKeywords NVARCHAR(500) NULL,
        FaviconUrl VARCHAR(255) NULL,
        BannerUrl VARCHAR(255) NULL,
        UpdatedAt DATETIME2 NULL,
        CreatedBy INT NULL,
        UpdatedBy INT NULL,
        CONSTRAINT FK_Settings_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(UserId),
        CONSTRAINT FK_Settings_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES dbo.Users(UserId)
    );

    INSERT INTO dbo.Settings (ShopName) VALUES (N'Shop Giày');
END

-- Contacts (storefront contact form)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Contacts]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.Contacts (
        ContactId INT IDENTITY(1,1) PRIMARY KEY,
        FirstName NVARCHAR(100) NULL,
        LastName NVARCHAR(100) NULL,
        Email VARCHAR(150) NULL,
        Subject NVARCHAR(200) NULL,
        Message NVARCHAR(MAX) NULL,
        IP VARCHAR(45) NULL,
        Note NVARCHAR(500) NULL,
        IsActive BIT NOT NULL DEFAULT(1),
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedBy INT NULL,
        CONSTRAINT FK_Contacts_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES dbo.Users(UserId)
    );
END

-- Contacts: add UpdatedBy for existing databases
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Contacts]') AND type in (N'U'))
BEGIN
    IF COL_LENGTH('dbo.Contacts', 'UpdatedBy') IS NULL
    BEGIN
        ALTER TABLE dbo.Contacts ADD UpdatedBy INT NULL;
        ALTER TABLE dbo.Contacts ADD CONSTRAINT FK_Contacts_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES dbo.Users(UserId);
    END
END

PRINT 'Tables created (if not existed)';
