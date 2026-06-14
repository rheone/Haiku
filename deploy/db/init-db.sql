-- ============================================================================
-- Haiku Database Bootstrap Script
-- Creates all tables, indexes, and constraints for the Haiku platform.
-- Designed for SQL Server 2022+.
-- ============================================================================

-- Create the database if it does not exist (when running outside container init)
IF DB_ID('Haiku') IS NULL
BEGIN
    CREATE DATABASE [Haiku];
END
GO

USE [Haiku];
GO

-- ============================================================================
-- USERS
-- ============================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        Email           NVARCHAR(320)    NOT NULL,
        Username        NVARCHAR(50)     NOT NULL,
        PasswordHash    NVARCHAR(200)    NOT NULL,
        DisplayName     NVARCHAR(100)    NOT NULL DEFAULT '',
        Bio             NVARCHAR(1000)   NULL,
        ProfileImageUrl NVARCHAR(500)    NULL,
        CreatedAt       DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),
        EmailVerifiedAt DATETIME2        NULL,
        IsDisabled      BIT              NOT NULL DEFAULT 0,
        DeletedAt       DATETIME2        NULL,
        DeletionChoice  VARCHAR(10)      NULL,

        CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_Users_Email UNIQUE (Email),
        CONSTRAINT UQ_Users_Username UNIQUE (Username),
        CONSTRAINT CK_Users_DeletionChoice CHECK (DeletionChoice IS NULL OR DeletionChoice IN ('Anonymize', 'Remove'))
    );
END
GO

-- ============================================================================
-- POEMS
-- ============================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Poems')
BEGIN
    CREATE TABLE Poems (
        Id              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        AuthorId        UNIQUEIDENTIFIER NOT NULL,
        Content         NVARCHAR(500)    NOT NULL,
        PoemType        VARCHAR(20)      NOT NULL,
        TotalSyllables  INT              NOT NULL DEFAULT 0,
        IsDraft         BIT              NOT NULL DEFAULT 0,
        IsHidden        BIT              NOT NULL DEFAULT 0,
        CreatedAt       DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),
        DeletedAt       DATETIME2        NULL,

        CONSTRAINT PK_Poems PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_Poems_Users FOREIGN KEY (AuthorId) REFERENCES Users(Id)
    );

    CREATE INDEX IX_Poems_CreatedAt
        ON Poems (CreatedAt DESC)
        WHERE IsDraft = 0 AND IsHidden = 0 AND DeletedAt IS NULL;

    CREATE INDEX IX_Poems_Author_CreatedAt
        ON Poems (AuthorId, CreatedAt DESC);
END
GO

-- ============================================================================
-- TAGS
-- ============================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Tags')
BEGIN
    CREATE TABLE Tags (
        Id   INT IDENTITY(1,1) NOT NULL,
        Name NVARCHAR(100)     NOT NULL,

        CONSTRAINT PK_Tags PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_Tags_Name UNIQUE (Name)
    );
END
GO

-- ============================================================================
-- POEM TAGS (Junction)
-- ============================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PoemTags')
BEGIN
    CREATE TABLE PoemTags (
        PoemId UNIQUEIDENTIFIER NOT NULL,
        TagId   INT              NOT NULL,

        CONSTRAINT PK_PoemTags PRIMARY KEY CLUSTERED (PoemId, TagId),
        CONSTRAINT FK_PoemTags_Poems FOREIGN KEY (PoemId) REFERENCES Poems(Id),
        CONSTRAINT FK_PoemTags_Tags FOREIGN KEY (TagId) REFERENCES Tags(Id)
    );

    CREATE INDEX IX_PoemTags_TagId ON PoemTags (TagId);
END
GO

-- ============================================================================
-- VOTES
-- ============================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Votes')
BEGIN
    CREATE TABLE Votes (
        Id        UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        PoemId    UNIQUEIDENTIFIER NOT NULL,
        UserId    UNIQUEIDENTIFIER NOT NULL,
        Value     TINYINT          NOT NULL,
        CreatedAt DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),

        CONSTRAINT PK_Votes PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_Votes_Poems FOREIGN KEY (PoemId) REFERENCES Poems(Id),
        CONSTRAINT FK_Votes_Users FOREIGN KEY (UserId) REFERENCES Users(Id),
        CONSTRAINT UQ_Votes_PoemId_UserId UNIQUE (PoemId, UserId),
        CONSTRAINT CK_Votes_Value CHECK (Value IN (1, -1))
    );

    CREATE INDEX IX_Votes_PoemId ON Votes (PoemId);
END
GO

-- ============================================================================
-- LOVES
-- ============================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Loves')
BEGIN
    CREATE TABLE Loves (
        Id        UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        PoemId    UNIQUEIDENTIFIER NOT NULL,
        UserId    UNIQUEIDENTIFIER NOT NULL,
        CreatedAt DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),

        CONSTRAINT PK_Loves PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_Loves_Poems FOREIGN KEY (PoemId) REFERENCES Poems(Id),
        CONSTRAINT FK_Loves_Users FOREIGN KEY (UserId) REFERENCES Users(Id),
        CONSTRAINT UQ_Loves_PoemId_UserId UNIQUE (PoemId, UserId)
    );

    CREATE INDEX IX_Loves_PoemId ON Loves (PoemId);
END
GO

-- ============================================================================
-- FOLLOWS
-- ============================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Follows')
BEGIN
    CREATE TABLE Follows (
        Id          UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        FollowerId  UNIQUEIDENTIFIER NOT NULL,
        FolloweeId  UNIQUEIDENTIFIER NOT NULL,
        CreatedAt   DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),

        CONSTRAINT PK_Follows PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_Follows_Follower FOREIGN KEY (FollowerId) REFERENCES Users(Id),
        CONSTRAINT FK_Follows_Followee FOREIGN KEY (FolloweeId) REFERENCES Users(Id),
        CONSTRAINT UQ_Follows_FollowerId_FolloweeId UNIQUE (FollowerId, FolloweeId),
        CONSTRAINT CK_Follows_NoSelfFollow CHECK (FollowerId <> FolloweeId)
    );
END
GO

-- ============================================================================
-- BOOKMARKS
-- ============================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Bookmarks')
BEGIN
    CREATE TABLE Bookmarks (
        Id        UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        UserId    UNIQUEIDENTIFIER NOT NULL,
        PoemId    UNIQUEIDENTIFIER NOT NULL,
        CreatedAt DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),

        CONSTRAINT PK_Bookmarks PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_Bookmarks_Users FOREIGN KEY (UserId) REFERENCES Users(Id),
        CONSTRAINT FK_Bookmarks_Poems FOREIGN KEY (PoemId) REFERENCES Poems(Id),
        CONSTRAINT UQ_Bookmarks_UserId_PoemId UNIQUE (UserId, PoemId)
    );
END
GO

-- ============================================================================
-- CUSTOM DICTIONARY WORDS
-- ============================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CustomDictionaryWords')
BEGIN
    CREATE TABLE CustomDictionaryWords (
        Id              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        Word            NVARCHAR(200)    NOT NULL,
        SyllableCount   INT              NOT NULL,
        AddedByUserId   UNIQUEIDENTIFIER NOT NULL,
        AddedAt         DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),
        IsApproved      BIT              NOT NULL DEFAULT 1,

        CONSTRAINT PK_CustomDictionaryWords PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_CDW_Users FOREIGN KEY (AddedByUserId) REFERENCES Users(Id),
        CONSTRAINT UQ_CDW_Word UNIQUE (Word)
    );

    CREATE INDEX IX_CustomDictionaryWords_Word ON CustomDictionaryWords (Word);
END
GO

-- ============================================================================
-- CUSTOM DICTIONARY SUGGESTIONS
-- ============================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CustomDictionarySuggestions')
BEGIN
    CREATE TABLE CustomDictionarySuggestions (
        Id                      UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        Word                    NVARCHAR(200)    NOT NULL,
        SuggestedSyllableCount  INT              NOT NULL,
        SuggestedByUserId       UNIQUEIDENTIFIER NOT NULL,
        Justification           NVARCHAR(200)    NULL,
        ReviewedByUserId        UNIQUEIDENTIFIER NULL,
        ReviewedAt              DATETIME2        NULL,
        Status                  VARCHAR(20)      NOT NULL DEFAULT 'Pending',

        CONSTRAINT PK_CustomDictionarySuggestions PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_CDS_SuggestedBy FOREIGN KEY (SuggestedByUserId) REFERENCES Users(Id),
        CONSTRAINT FK_CDS_ReviewedBy FOREIGN KEY (ReviewedByUserId) REFERENCES Users(Id),
        CONSTRAINT CK_CDS_Status CHECK (Status IN ('Pending', 'Approved', 'Rejected'))
    );
END
GO

-- ============================================================================
-- USER PRIVILEGES
-- ============================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserPrivileges')
BEGIN
    CREATE TABLE UserPrivileges (
        Id              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        UserId          UNIQUEIDENTIFIER NOT NULL,
        Privilege       VARCHAR(50)      NOT NULL,
        GrantedByUserId UNIQUEIDENTIFIER NOT NULL,
        GrantedAt       DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),

        CONSTRAINT PK_UserPrivileges PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_UP_Users FOREIGN KEY (UserId) REFERENCES Users(Id),
        CONSTRAINT FK_UP_GrantedBy FOREIGN KEY (GrantedByUserId) REFERENCES Users(Id),
        CONSTRAINT UQ_UserPrivileges_UserId_Privilege UNIQUE (UserId, Privilege)
    );
END
GO

-- ============================================================================
-- USER VERIFICATION TOKENS
-- ============================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserVerificationTokens')
BEGIN
    CREATE TABLE UserVerificationTokens (
        Id        UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        UserId    UNIQUEIDENTIFIER NOT NULL,
        Token     VARCHAR(128)     NOT NULL,
        CreatedAt DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),
        ExpiresAt DATETIME2        NOT NULL,
        UsedAt    DATETIME2        NULL,

        CONSTRAINT PK_UserVerificationTokens PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_UVT_Users FOREIGN KEY (UserId) REFERENCES Users(Id)
    );

    CREATE INDEX IX_UVT_Token ON UserVerificationTokens (Token);
END
GO

-- ============================================================================
-- PASSWORD RESET TOKENS
-- ============================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PasswordResetTokens')
BEGIN
    CREATE TABLE PasswordResetTokens (
        Id        UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        UserId    UNIQUEIDENTIFIER NOT NULL,
        Token     VARCHAR(128)     NOT NULL,
        CreatedAt DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),
        ExpiresAt DATETIME2        NOT NULL,
        UsedAt    DATETIME2        NULL,

        CONSTRAINT PK_PasswordResetTokens PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_PRT_Users FOREIGN KEY (UserId) REFERENCES Users(Id)
    );

    CREATE INDEX IX_PRT_Token ON PasswordResetTokens (Token);
END
GO

-- ============================================================================
-- MODERATION ACTIONS
-- ============================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ModerationActions')
BEGIN
    CREATE TABLE ModerationActions (
        Id              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        ActionType      VARCHAR(50)      NOT NULL,
        TargetType      VARCHAR(20)      NOT NULL,
        TargetId        UNIQUEIDENTIFIER NOT NULL,
        ActionedByUserId UNIQUEIDENTIFIER NOT NULL,
        Reason          NVARCHAR(500)    NOT NULL,
        CreatedAt       DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),

        CONSTRAINT PK_ModerationActions PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_MA_Users FOREIGN KEY (ActionedByUserId) REFERENCES Users(Id)
    );
END
GO

-- ============================================================================
-- TAG DAILY COUNTS
-- ============================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TagDailyCounts')
BEGIN
    CREATE TABLE TagDailyCounts (
        TagId INT      NOT NULL,
        Date  DATE     NOT NULL,
        Count INT      NOT NULL DEFAULT 0,

        CONSTRAINT PK_TagDailyCounts PRIMARY KEY CLUSTERED (TagId, Date),
        CONSTRAINT FK_TDC_Tags FOREIGN KEY (TagId) REFERENCES Tags(Id)
    );

    CREATE INDEX IX_TagDailyCounts_Date_Count ON TagDailyCounts (Date, Count DESC);
END
GO

PRINT 'Haiku database schema initialized successfully.';
GO
