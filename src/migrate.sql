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
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251206170332_initial'
)
BEGIN
    CREATE TABLE [Discounts] (
        [Id] uniqueidentifier NOT NULL,
        [ProviderId] uniqueidentifier NOT NULL,
        [Code] nvarchar(max) NOT NULL,
        [DiscountType] nvarchar(max) NOT NULL,
        [DiscountValue] decimal(18,2) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [CreatedById] uniqueidentifier NOT NULL,
        [LastUpdated] datetime2 NULL,
        [UpdatedById] uniqueidentifier NULL,
        CONSTRAINT [PK_Discounts] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251206170332_initial'
)
BEGIN
    CREATE TABLE [Packages] (
        [Id] uniqueidentifier NOT NULL,
        [ProviderId] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [IconUrl] nvarchar(max) NOT NULL,
        [Price] decimal(18,2) NULL,
        [Currency] nvarchar(max) NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastUpdated] datetime2 NULL,
        [CreatedById] uniqueidentifier NOT NULL,
        [UpdatedById] uniqueidentifier NULL,
        CONSTRAINT [PK_Packages] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251206170332_initial'
)
BEGIN
    CREATE TABLE [PaymentHistories] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [ProviderId] uniqueidentifier NOT NULL,
        [PackageId] uniqueidentifier NOT NULL,
        [DiscountId] uniqueidentifier NULL,
        [ChartColor] nvarchar(max) NOT NULL,
        [Price] decimal(18,2) NOT NULL,
        [DiscountedPrice] decimal(18,2) NULL,
        [Currency] nvarchar(max) NOT NULL,
        [PaymentDate] datetime2 NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastUpdated] datetime2 NULL,
        [CreateById] uniqueidentifier NOT NULL,
        [UpdateById] uniqueidentifier NULL,
        CONSTRAINT [PK_PaymentHistories] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251206170332_initial'
)
BEGIN
    CREATE TABLE [Providers] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [IconUrl] nvarchar(max) NOT NULL,
        [WebsiteUrl] nvarchar(max) NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastUpdated] datetime2 NULL,
        [CreatedById] uniqueidentifier NOT NULL,
        [UpdatedById] uniqueidentifier NULL,
        CONSTRAINT [PK_Providers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251206170332_initial'
)
BEGIN
    CREATE TABLE [Subcriptions] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [ProviderId] uniqueidentifier NOT NULL,
        [PackageId] uniqueidentifier NOT NULL,
        [Price] decimal(18,2) NOT NULL,
        [DiscountedPrice] decimal(18,2) NULL,
        [Currency] nvarchar(max) NOT NULL,
        [ChartColor] nvarchar(max) NOT NULL,
        [DiscountId] uniqueidentifier NULL,
        [RenewalDate] datetime2 NOT NULL,
        [Status] nvarchar(max) NULL,
        [CreatedDate] datetime2 NOT NULL,
        [CreatedById] uniqueidentifier NOT NULL,
        [LastUpdated] datetime2 NULL,
        [UpdatedById] uniqueidentifier NULL,
        CONSTRAINT [PK_Subcriptions] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251206170332_initial'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251206170332_initial', N'10.0.0');
END;

COMMIT;
GO

