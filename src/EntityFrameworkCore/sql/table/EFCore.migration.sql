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

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240411204710_Initial'
)
BEGIN
    CREATE TABLE [Address] (
        [AddressId] int NOT NULL IDENTITY,
        [Street] nvarchar(50) NULL,
        [City] nvarchar(50) NULL,
        [State] nvarchar(2) NULL,
        [Country] nvarchar(3) NULL,
        [ZipCode] nvarchar(10) NULL,
        CONSTRAINT [PK_Address] PRIMARY KEY ([AddressId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240411204710_Initial'
)
BEGIN
    CREATE TABLE [Customer] (
        [CustomerId] int NOT NULL IDENTITY,
        [CustomerResourceId] uniqueidentifier NOT NULL,
        [FirstName] nvarchar(50) NULL,
        [LastName] nvarchar(50) NULL,
        [Email] nvarchar(250) NULL,
        CONSTRAINT [PK_Customer] PRIMARY KEY ([CustomerId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240411204710_Initial'
)
BEGIN
    CREATE TABLE [Order] (
        [OrderId] int NOT NULL IDENTITY,
        [OrderResourceId] uniqueidentifier NOT NULL,
        [Status] nvarchar(20) NOT NULL,
        [CustomerId] int NULL,
        [AddressId] int NULL,
        [LastNotified] datetime2 NULL,
        CONSTRAINT [PK_Order] PRIMARY KEY ([OrderId]),
        CONSTRAINT [FK_Order_Address_AddressId] FOREIGN KEY ([AddressId]) REFERENCES [Address] ([AddressId]),
        CONSTRAINT [FK_Order_Customer_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customer] ([CustomerId])
    );
    DECLARE @defaultSchema AS sysname;
    SET @defaultSchema = SCHEMA_NAME();
    DECLARE @description AS sql_variant;
    SET @description = N'Orders';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'Order';
    SET @description = N'Primary Key';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'Order', 'COLUMN', N'OrderId';
    SET @description = N'Public unique identifier';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'Order', 'COLUMN', N'OrderResourceId';
    SET @description = N'Order status (created, paid, shipped, cancelled)';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'Order', 'COLUMN', N'Status';
    SET @description = N'Date customer was last notified for order';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'Order', 'COLUMN', N'LastNotified';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240411204710_Initial'
)
BEGIN
    CREATE TABLE [OrderItem] (
        [OrderItemId] int NOT NULL IDENTITY,
        [OrderId] int NOT NULL,
        [Sku] nvarchar(10) NULL,
        [Quantity] int NOT NULL,
        [UnitPrice] money NOT NULL,
        CONSTRAINT [PK_OrderItem] PRIMARY KEY ([OrderItemId]),
        CONSTRAINT [FK_OrderItem_Order_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Order] ([OrderId])
    );
    DECLARE @defaultSchema AS sysname;
    SET @defaultSchema = SCHEMA_NAME();
    DECLARE @description AS sql_variant;
    SET @description = N'Items that belong to an Order';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'OrderItem';
    SET @description = N'Primary Key';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'OrderItem', 'COLUMN', N'OrderItemId';
    SET @description = N'Item Sku';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'OrderItem', 'COLUMN', N'Sku';
    SET @description = N'Quantity of Sku';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'OrderItem', 'COLUMN', N'Quantity';
    SET @description = N'Per quantity price';
    EXEC sp_addextendedproperty 'MS_Description', @description, 'SCHEMA', @defaultSchema, 'TABLE', N'OrderItem', 'COLUMN', N'UnitPrice';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240411204710_Initial'
)
BEGIN
    CREATE INDEX [IX_Order_AddressId] ON [Order] ([AddressId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240411204710_Initial'
)
BEGIN
    CREATE INDEX [IX_Order_CustomerId] ON [Order] ([CustomerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240411204710_Initial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Order_OrderResourceId] ON [Order] ([OrderResourceId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240411204710_Initial'
)
BEGIN
    CREATE INDEX [IX_OrderItem_OrderId] ON [OrderItem] ([OrderId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240411204710_Initial'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240411204710_Initial', N'8.0.2');
END;
GO

COMMIT;
GO

