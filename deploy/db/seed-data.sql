-- ============================================================================
-- Haiku Seed Data Script
-- Inserts default data for local development and testing.
-- Designed to be idempotent (checks for existing data before inserting).
-- ============================================================================

USE [Haiku];
GO

-- ============================================================================
-- SEED ADMIN USER
-- Password: Admin@123 (BCrypt work factor 12)
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'admin@haiku.app')
BEGIN
    INSERT INTO Users (Id, Email, Username, DisplayName, PasswordHash, Bio, EmailVerifiedAt, IsDisabled)
    VALUES (
        '00000000-0000-0000-0000-000000000001',
        'admin@haiku.app',
        'admin',
        'Administrator',
        -- BCrypt hash of "Admin@123" with work factor 12
        '$2a$12$LJ3m4ys3Lk0TSwHnbfOMiOXPm1Qlq5GzKXq0Bx7n5Y5Z5p5b5W5S',
        'Haiku platform administrator.',
        SYSUTCDATETIME(),
        0
    );
END
GO

-- ============================================================================
-- SEED MODERATOR USER
-- Password: Moderator@123
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'moderator@haiku.app')
BEGIN
    INSERT INTO Users (Id, Email, Username, DisplayName, PasswordHash, Bio, EmailVerifiedAt)
    VALUES (
        '00000000-0000-0000-0000-000000000002',
        'moderator@haiku.app',
        'moderator',
        'Moderator',
        '$2a$12$LJ3m4ys3Lk0TSwHnbfOMiOXPm1Qlq5GzKXq0Bx7n5Y5Z5p5b5W5S',
        'Community moderator.',
        SYSUTCDATETIME()
    );
END
GO

-- ============================================================================
-- SEED REGULAR POET USERS
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'poet1@haiku.app')
BEGIN
    INSERT INTO Users (Id, Email, Username, DisplayName, PasswordHash, Bio, ProfileImageUrl, EmailVerifiedAt)
    VALUES
        ('00000000-0000-0000-0000-000000000010', 'poet1@haiku.app', 'silent_pond',   'Silent Pond',   '$2a$12$LJ3m4ys3Lk0TSwHnbfOMiOXPm1Qlq5GzKXq0Bx7n5Y5Z5p5b5W5S', 'Finding peace in nature, one haiku at a time.', NULL, SYSUTCDATETIME()),
        ('00000000-0000-0000-0000-000000000011', 'poet2@haiku.app', 'moon_watcher',  'Moon Watcher',  '$2a$12$LJ3m4ys3Lk0TSwHnbfOMiOXPm1Qlq5GzKXq0Bx7n5Y5Z5p5b5W5S', 'Watching the moon and writing since 2020.', NULL, SYSUTCDATETIME()),
        ('00000000-0000-0000-0000-000000000012', 'poet3@haiku.app', 'cherry_bloom',  'Cherry Bloom',  '$2a$12$LJ3m4ys3Lk0TSwHnbfOMiOXPm1Qlq5GzKXq0Bx7n5Y5Z5p5b5W5S', 'Spring is my season. Cherry blossoms forever.', NULL, SYSUTCDATETIME()),
        ('00000000-0000-0000-0000-000000000013', 'reader@haiku.app', 'casual_reader', 'Casual Reader', '$2a$12$LJ3m4ys3Lk0TSwHnbfOMiOXPm1Qlq5GzKXq0Bx7n5Y5Z5p5b5W5S', 'Just here to enjoy beautiful poetry.', NULL, SYSUTCDATETIME());
END
GO

-- ============================================================================
-- SEED ADMIN PRIVILEGES
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM UserPrivileges WHERE UserId = '00000000-0000-0000-0000-000000000001')
BEGIN
    INSERT INTO UserPrivileges (Id, UserId, Privilege, GrantedByUserId, GrantedAt)
    VALUES
        (NEWID(), '00000000-0000-0000-0000-000000000001', 'moderate_poems',   '00000000-0000-0000-0000-000000000001', SYSUTCDATETIME()),
        (NEWID(), '00000000-0000-0000-0000-000000000001', 'moderate_users',   '00000000-0000-0000-0000-000000000001', SYSUTCDATETIME()),
        (NEWID(), '00000000-0000-0000-0000-000000000001', 'manage_dictionary','00000000-0000-0000-0000-000000000001', SYSUTCDATETIME()),
        (NEWID(), '00000000-0000-0000-0000-000000000001', 'view_logs',       '00000000-0000-0000-0000-000000000001', SYSUTCDATETIME());
END
GO

-- ============================================================================
-- SEED MODERATOR PRIVILEGES
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM UserPrivileges WHERE UserId = '00000000-0000-0000-0000-000000000002')
BEGIN
    INSERT INTO UserPrivileges (Id, UserId, Privilege, GrantedByUserId, GrantedAt)
    VALUES
        (NEWID(), '00000000-0000-0000-0000-000000000002', 'moderate_poems',   '00000000-0000-0000-0000-000000000001', SYSUTCDATETIME()),
        (NEWID(), '00000000-0000-0000-0000-000000000002', 'moderate_users',   '00000000-0000-0000-0000-000000000001', SYSUTCDATETIME()),
        (NEWID(), '00000000-0000-0000-0000-000000000002', 'manage_dictionary','00000000-0000-0000-0000-000000000001', SYSUTCDATETIME());
END
GO

-- ============================================================================
-- SEED POEMS
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM Haikus WHERE Id = '10000000-0000-0000-0000-000000000001')
BEGIN
    INSERT INTO Haikus (Id, AuthorId, Content, PoemType, TotalSyllables, IsDraft, CreatedAt)
    VALUES
        ('10000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000010',
         'An old silent pond' + CHAR(10) + 'A frog jumps into the pond' + CHAR(10) + 'Splash! Silence again',
         'Haiku', 17, 0, DATEADD(HOUR, -2, SYSUTCDATETIME())),

        ('10000000-0000-0000-0000-000000000002', '00000000-0000-0000-0000-000000000011',
         'The moon tonight' + CHAR(10) + 'Even the stars envy its glow' + CHAR(10) + 'A perfect circle',
         'Haiku', 17, 0, DATEADD(HOUR, -5, SYSUTCDATETIME())),

        ('10000000-0000-0000-0000-000000000003', '00000000-0000-0000-0000-000000000012',
         'Cherry blossoms fall' + CHAR(10) + 'A gentle breeze carries them' + CHAR(10) + 'Spring says goodbye',
         'Haiku', 17, 0, DATEADD(HOUR, -12, SYSUTCDATETIME())),

        ('10000000-0000-0000-0000-000000000004', '00000000-0000-0000-0000-000000000010',
         'Autumn leaves descend' + CHAR(10) + 'A carpet of gold and red' + CHAR(10) + 'Footsteps crackle soft',
         'Haiku', 17, 0, DATEADD(HOUR, -24, SYSUTCDATETIME())),

        ('10000000-0000-0000-0000-000000000005', '00000000-0000-0000-0000-000000000011',
         'In the winter cold' + CHAR(10) + 'A single candle flickers' + CHAR(10) + 'Hope never goes out',
         'Haiku', 17, 0, DATEADD(HOUR, -36, SYSUTCDATETIME())),

        ('10000000-0000-0000-0000-000000000006', '00000000-0000-0000-0000-000000000012',
         'Rain on the window' + CHAR(10) + 'Each drop a tiny poem' + CHAR(10) + 'The world listens',
         'Haiku', 17, 0, DATEADD(HOUR, -48, SYSUTCDATETIME())),

        -- A Tanka example
        ('10000000-0000-0000-0000-000000000007', '00000000-0000-0000-0000-000000000010',
         'A single flower' + CHAR(10) + 'Blooms in the morning dew' + CHAR(10) + 'Bees begin their dance' + CHAR(10) + 'The sun warms the waiting earth' + CHAR(10) + 'Another day has begun',
         'Tanka', 31, 0, DATEADD(HOUR, -72, SYSUTCDATETIME())),

        -- A Monoku example
        ('10000000-0000-0000-0000-000000000008', '00000000-0000-0000-0000-000000000011',
         'Wind through the pine trees whispers secrets of the distant sea',
         'Monoku', 17, 0, DATEADD(HOUR, -96, SYSUTCDATETIME()));
END
GO

-- ============================================================================
-- SEED TAGS
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM Tags WHERE Name = 'nature')
BEGIN
    INSERT INTO Tags (Name) VALUES
        ('nature'), ('moon'), ('spring'), ('autumn'), ('winter'),
        ('rain'), ('peace'), ('love'), ('silence'), ('beauty');
END
GO

-- ============================================================================
-- SEED HAIKU-TAG RELATIONSHIPS
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM HaikuTags WHERE HaikuId = '10000000-0000-0000-0000-000000000001')
BEGIN
    INSERT INTO HaikuTags (HaikuId, TagId)
    VALUES
        ('10000000-0000-0000-0000-000000000001', (SELECT Id FROM Tags WHERE Name = 'nature')),
        ('10000000-0000-0000-0000-000000000001', (SELECT Id FROM Tags WHERE Name = 'silence')),
        ('10000000-0000-0000-0000-000000000002', (SELECT Id FROM Tags WHERE Name = 'moon')),
        ('10000000-0000-0000-0000-000000000002', (SELECT Id FROM Tags WHERE Name = 'nature')),
        ('10000000-0000-0000-0000-000000000003', (SELECT Id FROM Tags WHERE Name = 'spring')),
        ('10000000-0000-0000-0000-000000000003', (SELECT Id FROM Tags WHERE Name = 'beauty')),
        ('10000000-0000-0000-0000-000000000004', (SELECT Id FROM Tags WHERE Name = 'autumn')),
        ('10000000-0000-0000-0000-000000000004', (SELECT Id FROM Tags WHERE Name = 'nature')),
        ('10000000-0000-0000-0000-000000000005', (SELECT Id FROM Tags WHERE Name = 'winter')),
        ('10000000-0000-0000-0000-000000000005', (SELECT Id FROM Tags WHERE Name = 'peace')),
        ('10000000-0000-0000-0000-000000000006', (SELECT Id FROM Tags WHERE Name = 'rain')),
        ('10000000-0000-0000-0000-000000000006', (SELECT Id FROM Tags WHERE Name = 'peace')),
        ('10000000-0000-0000-0000-000000000007', (SELECT Id FROM Tags WHERE Name = 'nature')),
        ('10000000-0000-0000-0000-000000000007', (SELECT Id FROM Tags WHERE Name = 'spring')),
        ('10000000-0000-0000-0000-000000000008', (SELECT Id FROM Tags WHERE Name = 'nature')),
        ('10000000-0000-0000-0000-000000000008', (SELECT Id FROM Tags WHERE Name = 'silence'));
END
GO

-- ============================================================================
-- SEED VOTES
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM Votes WHERE HaikuId = '10000000-0000-0000-0000-000000000001')
BEGIN
    INSERT INTO Votes (Id, HaikuId, UserId, Value, CreatedAt)
    VALUES
        (NEWID(), '10000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000011', 1,  DATEADD(MINUTE, -30, SYSUTCDATETIME())),
        (NEWID(), '10000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000012', 1,  DATEADD(MINUTE, -45, SYSUTCDATETIME())),
        (NEWID(), '10000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000013', 1,  DATEADD(MINUTE, -60, SYSUTCDATETIME())),
        (NEWID(), '10000000-0000-0000-0000-000000000002', '00000000-0000-0000-0000-000000000010', 1,  DATEADD(HOUR, -3, SYSUTCDATETIME())),
        (NEWID(), '10000000-0000-0000-0000-000000000002', '00000000-0000-0000-0000-000000000012', 1,  DATEADD(HOUR, -4, SYSUTCDATETIME())),
        (NEWID(), '10000000-0000-0000-0000-000000000003', '00000000-0000-0000-0000-000000000010', 1,  DATEADD(HOUR, -8, SYSUTCDATETIME())),
        (NEWID(), '10000000-0000-0000-0000-000000000003', '00000000-0000-0000-0000-000000000011', 1,  DATEADD(HOUR, -9, SYSUTCDATETIME())),
        (NEWID(), '10000000-0000-0000-0000-000000000003', '00000000-0000-0000-0000-000000000013', -1, DATEADD(HOUR, -10, SYSUTCDATETIME()));
END
GO

-- ============================================================================
-- SEED LOVES
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM Loves WHERE HaikuId = '10000000-0000-0000-0000-000000000001')
BEGIN
    INSERT INTO Loves (Id, HaikuId, UserId, CreatedAt)
    VALUES
        (NEWID(), '10000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000012', DATEADD(MINUTE, -40, SYSUTCDATETIME())),
        (NEWID(), '10000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000013', DATEADD(MINUTE, -55, SYSUTCDATETIME())),
        (NEWID(), '10000000-0000-0000-0000-000000000002', '00000000-0000-0000-0000-000000000010', DATEADD(HOUR, -3, SYSUTCDATETIME())),
        (NEWID(), '10000000-0000-0000-0000-000000000003', '00000000-0000-0000-0000-000000000010', DATEADD(HOUR, -8, SYSUTCDATETIME())),
        (NEWID(), '10000000-0000-0000-0000-000000000003', '00000000-0000-0000-0000-000000000011', DATEADD(HOUR, -9, SYSUTCDATETIME()));
END
GO

-- ============================================================================
-- SEED FOLLOWS
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM Follows WHERE FollowerId = '00000000-0000-0000-0000-000000000013')
BEGIN
    INSERT INTO Follows (Id, FollowerId, FolloweeId, CreatedAt)
    VALUES
        (NEWID(), '00000000-0000-0000-0000-000000000013', '00000000-0000-0000-0000-000000000010', DATEADD(HOUR, -24, SYSUTCDATETIME())),
        (NEWID(), '00000000-0000-0000-0000-000000000013', '00000000-0000-0000-0000-000000000011', DATEADD(HOUR, -24, SYSUTCDATETIME())),
        (NEWID(), '00000000-0000-0000-0000-000000000012', '00000000-0000-0000-0000-000000000010', DATEADD(HOUR, -48, SYSUTCDATETIME())),
        (NEWID(), '00000000-0000-0000-0000-000000000011', '00000000-0000-0000-0000-000000000012', DATEADD(HOUR, -72, SYSUTCDATETIME())),
        (NEWID(), '00000000-0000-0000-0000-000000000010', '00000000-0000-0000-0000-000000000011', DATEADD(HOUR, -96, SYSUTCDATETIME()));
END
GO

-- ============================================================================
-- SEED CUSTOM DICTIONARY WORDS
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM CustomDictionaryWords WHERE Word = 'haiku')
BEGIN
    INSERT INTO CustomDictionaryWords (Id, Word, SyllableCount, AddedByUserId, IsApproved)
    VALUES
        (NEWID(), 'haiku',       2, '00000000-0000-0000-0000-000000000001', 1),
        (NEWID(), 'tanka',       2, '00000000-0000-0000-0000-000000000001', 1),
        (NEWID(), 'monoku',      3, '00000000-0000-0000-0000-000000000001', 1),
        (NEWID(), 'syllable',    3, '00000000-0000-0000-0000-000000000001', 1),
        (NEWID(), 'poetry',      3, '00000000-0000-0000-0000-000000000001', 1);
END
GO

-- ============================================================================
-- SEED TAG DAILY COUNTS (for trending)
-- ============================================================================
IF NOT EXISTS (SELECT 1 FROM TagDailyCounts WHERE Date = CAST(SYSUTCDATETIME() AS DATE))
BEGIN
    INSERT INTO TagDailyCounts (TagId, Date, Count)
    SELECT Id, CAST(SYSUTCDATETIME() AS DATE),
        CASE Name
            WHEN 'nature' THEN 15
            WHEN 'moon'   THEN 8
            WHEN 'spring' THEN 10
            WHEN 'autumn' THEN 6
            WHEN 'winter' THEN 5
            WHEN 'rain'   THEN 7
            WHEN 'peace'  THEN 12
            WHEN 'love'   THEN 20
            WHEN 'silence' THEN 9
            WHEN 'beauty' THEN 11
            ELSE 3
        END
    FROM Tags;
END
GO

PRINT 'Haiku seed data inserted successfully.';
GO
