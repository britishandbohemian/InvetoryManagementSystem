IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230826072902_InventoryDatabase')
BEGIN
    CREATE TABLE [Categories] (
        [CategoryId] int NOT NULL IDENTITY,
        [CategoryName] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Categories] PRIMARY KEY ([CategoryId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230826072902_InventoryDatabase')
BEGIN
    CREATE TABLE [Orders] (
        [OrderId] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [OrderType] int NOT NULL,
        [OrderState] int NOT NULL,
        [OrderDate] datetime2 NOT NULL,
        CONSTRAINT [PK_Orders] PRIMARY KEY ([OrderId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230826072902_InventoryDatabase')
BEGIN
    CREATE TABLE [Suppliers] (
        [SupplierId] int NOT NULL IDENTITY,
        [SupplierName] nvarchar(100) NOT NULL,
        [EmailAddress] nvarchar(max) NOT NULL,
        [PhoneNumber] nvarchar(max) NOT NULL,
        [ContactPerson] nvarchar(max) NOT NULL,
        [Address] nvarchar(max) NOT NULL,
        [Supplier_Active] int NOT NULL,
        [Rating] int NOT NULL,
        CONSTRAINT [PK_Suppliers] PRIMARY KEY ([SupplierId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230826072902_InventoryDatabase')
BEGIN
    CREATE TABLE [SystemCustomizations] (
        [Id] int NOT NULL IDENTITY,
        [ThemeName] nvarchar(max) NOT NULL,
        [ImageLocation] nvarchar(max) NOT NULL,
        [ProductMarkupPercentage] int NOT NULL,
        [Currency] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_SystemCustomizations] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230826072902_InventoryDatabase')
BEGIN
    CREATE TABLE [Users] (
        [UserId] int NOT NULL IDENTITY,
        [Username] nvarchar(50) NOT NULL,
        [UserPassword] nvarchar(100) NOT NULL,
        [UserEmail] nvarchar(max) NOT NULL,
        [UserContact] nvarchar(max) NOT NULL,
        [Role] int NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([UserId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230826072902_InventoryDatabase')
BEGIN
    CREATE TABLE [Sales] (
        [SaleId] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [SaleDate] datetime2 NOT NULL,
        [OrderId] int NOT NULL,
        [NumberOfItems] int NOT NULL,
        [TotalAmountMadeFromSale] decimal(18,2) NOT NULL,
        CONSTRAINT [PK_Sales] PRIMARY KEY ([SaleId]),
        CONSTRAINT [FK_Sales_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([OrderId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230826072902_InventoryDatabase')
BEGIN
    CREATE TABLE [Components] (
        [ComponentId] int NOT NULL IDENTITY,
        [ComponentName] nvarchar(max) NOT NULL,
        [UnitsInInventory] int NULL,
        [ComponentDescription] nvarchar(max) NOT NULL,
        [CostPricePerUnit] int NOT NULL,
        [UnitOfMeasurement] int NOT NULL,
        [SellByDate] datetime2 NOT NULL,
        [MinimumThreshold] int NOT NULL,
        [MaximumThreshold] int NOT NULL,
        [PurchasePrice] decimal(18,2) NOT NULL,
        [Status] int NOT NULL,
        [SupplierId] int NOT NULL,
        [CategoryId] int NOT NULL,
        CONSTRAINT [PK_Components] PRIMARY KEY ([ComponentId]),
        CONSTRAINT [FK_Components_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([CategoryId]) ON DELETE CASCADE,
        CONSTRAINT [FK_Components_Suppliers_SupplierId] FOREIGN KEY ([SupplierId]) REFERENCES [Suppliers] ([SupplierId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230826072902_InventoryDatabase')
BEGIN
    CREATE TABLE [Products] (
        [ProductId] int NOT NULL IDENTITY,
        [CategoryId] int NOT NULL,
        [UnitsInInventory] int NULL,
        [ProductSellingPrice] decimal(18,2) NOT NULL,
        [ProductName] nvarchar(max) NOT NULL,
        [ProductCostPrice] decimal(18,2) NULL,
        [ProdcutMarkup] int NOT NULL,
        [Status] int NULL,
        [SellByDate] datetime2 NOT NULL,
        [SupplierId] int NULL,
        [SupplierId1] int NULL,
        CONSTRAINT [PK_Products] PRIMARY KEY ([ProductId]),
        CONSTRAINT [FK_Products_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([CategoryId]) ON DELETE CASCADE,
        CONSTRAINT [FK_Products_Suppliers_SupplierId] FOREIGN KEY ([SupplierId]) REFERENCES [Suppliers] ([SupplierId]),
        CONSTRAINT [FK_Products_Suppliers_SupplierId1] FOREIGN KEY ([SupplierId1]) REFERENCES [Suppliers] ([SupplierId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230826072902_InventoryDatabase')
BEGIN
    CREATE TABLE [SupplierOrder] (
        [OrderId] int NOT NULL IDENTITY,
        [TotalCostOfOrder] decimal(18,2) NOT NULL,
        [LeadTimeFromSupplier] int NOT NULL,
        [DateOfArrival] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL,
        [SupplierId] int NOT NULL,
        [UserId] int NOT NULL,
        CONSTRAINT [PK_SupplierOrder] PRIMARY KEY ([OrderId]),
        CONSTRAINT [FK_SupplierOrder_Suppliers_SupplierId] FOREIGN KEY ([SupplierId]) REFERENCES [Suppliers] ([SupplierId]) ON DELETE CASCADE,
        CONSTRAINT [FK_SupplierOrder_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230826072902_InventoryDatabase')
BEGIN
    CREATE TABLE [ProductComponents] (
        [ProductId] int NOT NULL,
        [ComponentId] int NOT NULL,
        [LinkId] int NOT NULL IDENTITY,
        [UnitsUsed] decimal(18,2) NOT NULL,
        [UnitOfMeasurement] int NOT NULL,
        CONSTRAINT [PK_ProductComponents] PRIMARY KEY ([ProductId], [ComponentId]),
        CONSTRAINT [FK_ProductComponents_Components_ComponentId] FOREIGN KEY ([ComponentId]) REFERENCES [Components] ([ComponentId]),
        CONSTRAINT [FK_ProductComponents_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([ProductId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230826072902_InventoryDatabase')
BEGIN
    CREATE TABLE [OrderComponents] (
        [OrderComponentId] int NOT NULL IDENTITY,
        [OrderId] int NOT NULL,
        [ComponentId] int NOT NULL,
        [UnitsOrdered] int NOT NULL,
        [TotalCostOfOrder] decimal(18,2) NOT NULL,
        [SupplierOrderOrderId] int NULL,
        CONSTRAINT [PK_OrderComponents] PRIMARY KEY ([OrderComponentId]),
        CONSTRAINT [FK_OrderComponents_Components_ComponentId] FOREIGN KEY ([ComponentId]) REFERENCES [Components] ([ComponentId]) ON DELETE CASCADE,
        CONSTRAINT [FK_OrderComponents_SupplierOrder_SupplierOrderOrderId] FOREIGN KEY ([SupplierOrderOrderId]) REFERENCES [SupplierOrder] ([OrderId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230826072902_InventoryDatabase')
BEGIN
    CREATE TABLE [OrderProducts] (
        [OrderProductId] int NOT NULL IDENTITY,
        [ProductId] int NOT NULL,
        [UnitsOrdered] int NOT NULL,
        [TotalCostOfOrder] decimal(18,2) NOT NULL,
        [OrderId] int NULL,
        [CustomerOrderDetailOrderId] int NULL,
        CONSTRAINT [PK_OrderProducts] PRIMARY KEY ([OrderProductId]),
        CONSTRAINT [FK_OrderProducts_Orders_CustomerOrderDetailOrderId] FOREIGN KEY ([CustomerOrderDetailOrderId]) REFERENCES [Orders] ([OrderId]),
        CONSTRAINT [FK_OrderProducts_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([ProductId]) ON DELETE CASCADE,
        CONSTRAINT [FK_OrderProducts_SupplierOrder_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [SupplierOrder] ([OrderId])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230826072902_InventoryDatabase')
BEGIN
    CREATE INDEX [IX_Components_CategoryId] ON [Components] ([CategoryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230826072902_InventoryDatabase')
BEGIN
    CREATE INDEX [IX_Components_SupplierId] ON [Components] ([SupplierId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230826072902_InventoryDatabase')
BEGIN
    CREATE UNIQUE INDEX [IX_OrderComponents_ComponentId] ON [OrderComponents] ([ComponentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230826072902_InventoryDatabase')
BEGIN
    CREATE INDEX [IX_OrderComponents_SupplierOrderOrderId] ON [OrderComponents] ([SupplierOrderOrderId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230826072902_InventoryDatabase')
BEGIN
    CREATE INDEX [IX_OrderProducts_CustomerOrderDetailOrderId] ON [OrderProducts] ([CustomerOrderDetailOrderId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230826072902_InventoryDatabase')
BEGIN
    CREATE INDEX [IX_OrderProducts_OrderId] ON [OrderProducts] ([OrderId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230826072902_InventoryDatabase')
BEGIN
    CREATE INDEX [IX_OrderProducts_ProductId] ON [OrderProducts] ([ProductId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230826072902_InventoryDatabase')
BEGIN
    CREATE INDEX [IX_ProductComponents_ComponentId] ON [ProductComponents] ([ComponentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230826072902_InventoryDatabase')
BEGIN
    CREATE INDEX [IX_Products_CategoryId] ON [Products] ([CategoryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230826072902_InventoryDatabase')
BEGIN
    CREATE INDEX [IX_Products_SupplierId] ON [Products] ([SupplierId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230826072902_InventoryDatabase')
BEGIN
    CREATE INDEX [IX_Products_SupplierId1] ON [Products] ([SupplierId1]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230826072902_InventoryDatabase')
BEGIN
    CREATE INDEX [IX_Sales_OrderId] ON [Sales] ([OrderId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230826072902_InventoryDatabase')
BEGIN
    CREATE INDEX [IX_SupplierOrder_SupplierId] ON [SupplierOrder] ([SupplierId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230826072902_InventoryDatabase')
BEGIN
    CREATE INDEX [IX_SupplierOrder_UserId] ON [SupplierOrder] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230826072902_InventoryDatabase')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230826072902_InventoryDatabase', N'7.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230826074112_Update')
BEGIN
    ALTER TABLE [SupplierOrder] ADD [StatusOfOrder] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230826074112_Update')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230826074112_Update', N'7.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230826074342_Update-Two')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230826074342_Update-Two', N'7.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230826075439_new')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230826075439_new', N'7.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230826080133_Updatedb')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230826080133_Updatedb', N'7.0.8');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230826080847_Update_2')
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Products]') AND [c].[name] = N'UnitsInInventory');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Products] DROP CONSTRAINT [' + @var0 + '];');
    EXEC(N'UPDATE [Products] SET [UnitsInInventory] = 0 WHERE [UnitsInInventory] IS NULL');
    ALTER TABLE [Products] ALTER COLUMN [UnitsInInventory] int NOT NULL;
    ALTER TABLE [Products] ADD DEFAULT 0 FOR [UnitsInInventory];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230826080847_Update_2')
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Components]') AND [c].[name] = N'UnitsInInventory');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Components] DROP CONSTRAINT [' + @var1 + '];');
    EXEC(N'UPDATE [Components] SET [UnitsInInventory] = 0 WHERE [UnitsInInventory] IS NULL');
    ALTER TABLE [Components] ALTER COLUMN [UnitsInInventory] int NOT NULL;
    ALTER TABLE [Components] ADD DEFAULT 0 FOR [UnitsInInventory];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230826080847_Update_2')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230826080847_Update_2', N'7.0.8');
END;
GO

COMMIT;
GO

