-- =====================================================
-- AUTHENTICATION DATABASE (DM02) CREATION SCRIPT
-- =====================================================
-- This script creates all authentication tables in the DM02 database
-- for security isolation from the main application data

USE [DM02];
GO

-- =====================================================
-- 1. USERS TABLE
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Users] (
        [Id] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Username] nvarchar(50) NOT NULL,
        [Email] nvarchar(255) NULL,
        [PasswordHash] nvarchar(255) NOT NULL,
        [FullName] nvarchar(255) NULL,
        [Role] nvarchar(20) NOT NULL DEFAULT 'user',
        [IsActive] bit NOT NULL DEFAULT 1,
        [CreatedAt] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [CreatedByUserId] int NULL,
        [LastLogin] datetime2 NULL,
        [PasswordChangedAt] datetime2 NULL,
        [FailedLoginAttempts] int NOT NULL DEFAULT 0,
        [LockedUntil] datetime2 NULL
    );
END
GO

-- Users table indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Username')
    CREATE UNIQUE INDEX [IX_Users_Username] ON [dbo].[Users] ([Username]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Email')
    CREATE UNIQUE INDEX [IX_Users_Email] ON [dbo].[Users] ([Email]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Role')
    CREATE INDEX [IX_Users_Role] ON [dbo].[Users] ([Role]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_IsActive')
    CREATE INDEX [IX_Users_IsActive] ON [dbo].[Users] ([IsActive]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_CreatedByUserId')
    CREATE INDEX [IX_Users_CreatedByUserId] ON [dbo].[Users] ([CreatedByUserId]);
GO

-- Users table foreign key
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Users_Users_CreatedByUserId')
    ALTER TABLE [dbo].[Users] 
    ADD CONSTRAINT [FK_Users_Users_CreatedByUserId] 
    FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[Users] ([Id]) 
    ON DELETE SET NULL;
GO

-- =====================================================
-- 2. USER SESSIONS TABLE
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UserSessions' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[UserSessions] (
        [Id] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [UserId] int NOT NULL,
        [TokenHash] nvarchar(255) NOT NULL,
        [ExpiresAt] datetime2 NOT NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [IpAddress] nvarchar(45) NULL,
        [UserAgent] nvarchar(max) NULL,
        [IsActive] bit NOT NULL DEFAULT 1
    );
END
GO

-- UserSessions table indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UserSessions_TokenHash')
    CREATE UNIQUE INDEX [IX_UserSessions_TokenHash] ON [dbo].[UserSessions] ([TokenHash]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UserSessions_UserId')
    CREATE INDEX [IX_UserSessions_UserId] ON [dbo].[UserSessions] ([UserId]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UserSessions_ExpiresAt')
    CREATE INDEX [IX_UserSessions_ExpiresAt] ON [dbo].[UserSessions] ([ExpiresAt]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UserSessions_IsActive')
    CREATE INDEX [IX_UserSessions_IsActive] ON [dbo].[UserSessions] ([IsActive]);
GO

-- UserSessions table foreign key
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_UserSessions_Users_UserId')
    ALTER TABLE [dbo].[UserSessions] 
    ADD CONSTRAINT [FK_UserSessions_Users_UserId] 
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) 
    ON DELETE CASCADE;
GO

-- =====================================================
-- 3. PASSWORD RESET TOKENS TABLE
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PasswordResetTokens' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[PasswordResetTokens] (
        [Id] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [UserId] int NOT NULL,
        [TokenHash] nvarchar(255) NOT NULL,
        [ExpiresAt] datetime2 NOT NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [UsedAt] datetime2 NULL,
        [IsUsed] bit NOT NULL DEFAULT 0
    );
END
GO

-- PasswordResetTokens table indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PasswordResetTokens_TokenHash')
    CREATE UNIQUE INDEX [IX_PasswordResetTokens_TokenHash] ON [dbo].[PasswordResetTokens] ([TokenHash]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PasswordResetTokens_UserId')
    CREATE INDEX [IX_PasswordResetTokens_UserId] ON [dbo].[PasswordResetTokens] ([UserId]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PasswordResetTokens_ExpiresAt')
    CREATE INDEX [IX_PasswordResetTokens_ExpiresAt] ON [dbo].[PasswordResetTokens] ([ExpiresAt]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PasswordResetTokens_IsUsed')
    CREATE INDEX [IX_PasswordResetTokens_IsUsed] ON [dbo].[PasswordResetTokens] ([IsUsed]);
GO

-- PasswordResetTokens table foreign key
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_PasswordResetTokens_Users_UserId')
    ALTER TABLE [dbo].[PasswordResetTokens] 
    ADD CONSTRAINT [FK_PasswordResetTokens_Users_UserId] 
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) 
    ON DELETE CASCADE;
GO

-- =====================================================
-- 4. AUDIT LOG TABLE
-- =====================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='AuditLogs' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[AuditLogs] (
        [Id] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [UserId] int NULL,
        [Action] nvarchar(100) NOT NULL,
        [ResourceType] nvarchar(50) NULL,
        [ResourceId] int NULL,
        [IpAddress] nvarchar(45) NULL,
        [UserAgent] nvarchar(max) NULL,
        [Details] nvarchar(max) NULL, -- JSON data
        [CreatedAt] datetime2 NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

-- AuditLogs table indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AuditLogs_UserId')
    CREATE INDEX [IX_AuditLogs_UserId] ON [dbo].[AuditLogs] ([UserId]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AuditLogs_Action')
    CREATE INDEX [IX_AuditLogs_Action] ON [dbo].[AuditLogs] ([Action]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AuditLogs_ResourceType')
    CREATE INDEX [IX_AuditLogs_ResourceType] ON [dbo].[AuditLogs] ([ResourceType]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AuditLogs_ResourceId')
    CREATE INDEX [IX_AuditLogs_ResourceId] ON [dbo].[AuditLogs] ([ResourceId]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AuditLogs_CreatedAt')
    CREATE INDEX [IX_AuditLogs_CreatedAt] ON [dbo].[AuditLogs] ([CreatedAt]);
GO

-- AuditLogs table foreign key
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AuditLogs_Users_UserId')
    ALTER TABLE [dbo].[AuditLogs] 
    ADD CONSTRAINT [FK_AuditLogs_Users_UserId] 
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) 
    ON DELETE SET NULL;
GO

-- =====================================================
-- SAMPLE DATA
-- =====================================================

-- Insert default admin user (only if not exists)
IF NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [Username] = 'admin')
BEGIN
    INSERT INTO [dbo].[Users] (
        [Username], 
        [Email], 
        [PasswordHash], 
        [FullName], 
        [Role], 
        [IsActive], 
        [CreatedAt], 
        [UpdatedAt]
    ) VALUES (
        'admin',
        'admin@cobblestonefruit.com',
        '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewdBPj4Qj4Qj4Qj4', -- Password: admin123
        'System Administrator',
        'admin',
        1,
        GETUTCDATE(),
        GETUTCDATE()
    );
END
GO

-- Insert the chrism user (only if not exists)
IF NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [Username] = 'chrism')
BEGIN
    INSERT INTO [dbo].[Users] (
        [Username], 
        [Email], 
        [PasswordHash], 
        [FullName], 
        [Role], 
        [IsActive], 
        [CreatedAt], 
        [UpdatedAt]
    ) VALUES (
        'chrism',
        'chrism@cobblestonefruit.com',
        '$2a$12$/QTokY5aJjNOYAtSYwmS3.ut0Adp5110Dslr6aJ6a4HSyvRdxE0He', -- Password: 1835
        'Chris M',
        'admin',
        1,
        GETUTCDATE(),
        GETUTCDATE()
    );
END
GO

-- =====================================================
-- VERIFICATION QUERIES
-- =====================================================

-- Verify tables were created
SELECT 
    TABLE_NAME,
    TABLE_TYPE
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_SCHEMA = 'dbo' 
AND TABLE_NAME IN ('Users', 'UserSessions', 'PasswordResetTokens', 'AuditLogs')
ORDER BY TABLE_NAME;

-- Verify users were inserted
SELECT 
    Id,
    Username,
    Email,
    FullName,
    Role,
    IsActive,
    CreatedAt
FROM [dbo].[Users]
ORDER BY Id;

-- Verify indexes were created
SELECT 
    i.name AS IndexName,
    t.name AS TableName,
    i.is_unique,
    i.is_primary_key
FROM sys.indexes i
INNER JOIN sys.tables t ON i.object_id = t.object_id
WHERE t.name IN ('Users', 'UserSessions', 'PasswordResetTokens', 'AuditLogs')
AND i.name LIKE 'IX_%'
ORDER BY t.name, i.name;

PRINT 'Authentication database (DM02) setup completed successfully!';
