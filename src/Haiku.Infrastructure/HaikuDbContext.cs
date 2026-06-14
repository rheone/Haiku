using Haiku.Domain.Entities;
using Haiku.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Haiku.Infrastructure;

/// <summary>
/// Entity Framework Core database context for the Haiku application.
/// Configures entity mappings, relationships, indexes, and table names via <see cref="OnModelCreating"/>.
/// </summary>
public class HaikuDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HaikuDbContext"/> class.
    /// </summary>
    /// <param name="options">The options to configure the database provider and connection.</param>
    public HaikuDbContext(DbContextOptions<HaikuDbContext> options)
        : base(options) { }

    /// <summary>
    /// Gets the set of registered user accounts.
    /// </summary>
    /// <value>The <see cref="DbSet{User}"/> for querying and saving user accounts.</value>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// Gets the set of poems authored by users.
    /// </summary>
    /// <value>The <see cref="DbSet{Poem}"/> for querying and saving poems.</value>
    public DbSet<Poem> Poems => Set<Poem>();

    /// <summary>
    /// Gets the set of tags used to categorize poems.
    /// </summary>
    /// <value>The <see cref="DbSet{Tag}"/> for querying and saving tags.</value>
    public DbSet<Tag> Tags => Set<Tag>();

    /// <summary>
    /// Gets the many-to-many join set linking poems to tags.
    /// </summary>
    /// <value>The <see cref="DbSet{PoemTag}"/> for querying and saving poem-tag associations.</value>
    public DbSet<PoemTag> PoemTags => Set<PoemTag>();

    /// <summary>
    /// Gets the set of upvote/downvote records on poems.
    /// </summary>
    /// <value>The <see cref="DbSet{Vote}"/> for querying and saving votes.</value>
    public DbSet<Vote> Votes => Set<Vote>();

    /// <summary>
    /// Gets the set of love (heart) records on poems.
    /// </summary>
    /// <value>The <see cref="DbSet{Love}"/> for querying and saving loves.</value>
    public DbSet<Love> Loves => Set<Love>();

    /// <summary>
    /// Gets the set of bookmark records saved by users.
    /// </summary>
    /// <value>The <see cref="DbSet{Bookmark}"/> for querying and saving bookmarks.</value>
    public DbSet<Bookmark> Bookmarks => Set<Bookmark>();

    /// <summary>
    /// Gets the set of follow relationships between users.
    /// </summary>
    /// <value>The <see cref="DbSet{Follow}"/> for querying and saving follow relationships.</value>
    public DbSet<Follow> Follows => Set<Follow>();

    /// <summary>
    /// Gets the set of approved custom dictionary words.
    /// </summary>
    /// <value>The <see cref="DbSet{CustomDictionaryWord}"/> for querying and saving custom dictionary words.</value>
    public DbSet<CustomDictionaryWord> CustomDictionaryWords => Set<CustomDictionaryWord>();

    /// <summary>
    /// Gets the set of user-submitted suggestions for new dictionary words.
    /// </summary>
    /// <value>The <see cref="DbSet{CustomDictionarySuggestion}"/> for querying and saving dictionary suggestions.</value>
    public DbSet<CustomDictionarySuggestion> CustomDictionarySuggestions => Set<CustomDictionarySuggestion>();

    /// <summary>
    /// Gets the set of moderation actions (warnings, suspensions, bans).
    /// </summary>
    /// <value>The <see cref="DbSet{ModerationAction}"/> for querying and saving moderation actions.</value>
    public DbSet<ModerationAction> ModerationActions => Set<ModerationAction>();

    /// <summary>
    /// Gets the set of password reset tokens.
    /// </summary>
    /// <value>The <see cref="DbSet{PasswordResetToken}"/> for querying and saving password reset tokens.</value>
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();

    /// <summary>
    /// Gets the set of privilege assignments granted to users.
    /// </summary>
    /// <value>The <see cref="DbSet{UserPrivilege}"/> for querying and saving user privilege assignments.</value>
    public DbSet<UserPrivilege> UserPrivileges => Set<UserPrivilege>();

    /// <summary>
    /// Gets the set of email verification tokens for new user registrations.
    /// </summary>
    /// <value>The <see cref="DbSet{UserVerificationToken}"/> for querying and saving verification tokens.</value>
    public DbSet<UserVerificationToken> UserVerificationTokens => Set<UserVerificationToken>();

    /// <summary>
    /// Gets the set of daily tag usage counts for trending analytics.
    /// </summary>
    /// <value>The <see cref="DbSet{TagDailyCount}"/> for querying and saving daily tag usage counts.</value>
    public DbSet<TagDailyCount> TagDailyCounts => Set<TagDailyCount>();

    /// <summary>
    /// Gets the set of theme definitions.
    /// </summary>
    /// <value>
    /// <placeholder>The set of theme definitions.</placeholder>
    /// </value>
    public DbSet<Theme> Themes => Set<Theme>();

    /// <summary>
    /// Gets the set of theme keyword associations.
    /// </summary>
    /// <value>
    /// <placeholder>The set of theme keyword associations.</placeholder>
    /// </value>
    public DbSet<ThemeKeyword> ThemeKeywords => Set<ThemeKeyword>();

    /// <summary>
    /// Configures entity table mappings, relationships, unique indexes, and delete behaviors
    /// for all entity types in the Haiku domain model.
    /// </summary>
    /// <param name="modelBuilder">The model builder used to configure the entity mappings.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Users: unique indexes on login identifiers prevent duplicate registration.
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Username).IsUnique();
        });

        // Poems: Restrict on Author FK prevents accidental cascade-delete of all
        // poems when a user is removed. The filtered index on CreatedAt excludes
        // drafts, hidden content, and soft-deleted rows so browse/list queries
        // only see published, public poems.
        modelBuilder.Entity<Poem>(entity =>
        {
            entity.ToTable("Poems");
            entity.HasOne(e => e.Author).WithMany().HasForeignKey(e => e.AuthorId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => e.CreatedAt).HasFilter("[IsDraft] = 0 AND [IsHidden] = 0 AND [DeletedAt] IS NULL");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.ToTable("Tags");
            entity.HasIndex(e => e.Name).IsUnique();
        });

        modelBuilder.Entity<PoemTag>(entity =>
        {
            entity.ToTable("PoemTags");
            entity.HasKey(e => new { e.PoemId, e.TagId });
            entity.HasOne(e => e.Poem).WithMany().HasForeignKey(e => e.PoemId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Tag).WithMany().HasForeignKey(e => e.TagId).OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.TagId);
        });

        // Votes and Loves: Cascade on the poem FK cleans up interactions when a
        // poem is deleted. Restrict on the user FK prevents data loss from
        // accidental user deletion. The composite unique index enforces one
        // vote/love per user per poem.
        modelBuilder.Entity<Vote>(entity =>
        {
            entity.ToTable("Votes");
            entity.HasOne(e => e.Poem).WithMany().HasForeignKey(e => e.PoemId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => new { e.PoemId, e.UserId }).IsUnique();
            entity.HasIndex(e => e.PoemId);
        });

        modelBuilder.Entity<Love>(entity =>
        {
            entity.ToTable("Loves");
            entity.HasOne(e => e.Poem).WithMany().HasForeignKey(e => e.PoemId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => new { e.PoemId, e.UserId }).IsUnique();
            entity.HasIndex(e => e.PoemId);
        });

        // Bookmarks: Restrict on the user FK prevents data loss; Cascade on the poem
        // FK cleans up bookmarks when a poem is removed. Composite unique index
        // enforces one bookmark per user per poem.
        modelBuilder.Entity<Bookmark>(entity =>
        {
            entity.ToTable("Bookmarks");
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Poem).WithMany().HasForeignKey(e => e.PoemId).OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.UserId, e.PoemId }).IsUnique();
        });

        // Follows: Both FKs use Restrict to prevent circular cascade chains
        // (e.g. deleting user A could cascade through follows and back to user B).
        modelBuilder.Entity<Follow>(entity =>
        {
            entity.ToTable("Follows");
            entity.HasOne(e => e.Follower).WithMany().HasForeignKey(e => e.FollowerId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Followee).WithMany().HasForeignKey(e => e.FolloweeId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => new { e.FollowerId, e.FolloweeId }).IsUnique();
        });

        modelBuilder.Entity<CustomDictionaryWord>(entity =>
        {
            entity.ToTable("CustomDictionaryWords");
            entity.HasOne(e => e.AddedBy).WithMany().HasForeignKey(e => e.AddedByUserId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => e.Word).IsUnique();
        });

        modelBuilder.Entity<CustomDictionarySuggestion>(entity =>
        {
            entity.ToTable("CustomDictionarySuggestions");
            entity
                .HasOne(e => e.SuggestedBy)
                .WithMany()
                .HasForeignKey(e => e.SuggestedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.ReviewedBy).WithMany().HasForeignKey(e => e.ReviewedByUserId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<ModerationAction>(entity =>
        {
            entity.ToTable("ModerationActions");
            entity
                .HasOne(e => e.ActionedBy)
                .WithMany()
                .HasForeignKey(e => e.ActionedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.ToTable("PasswordResetTokens");
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.Token);
        });

        modelBuilder.Entity<UserPrivilege>(entity =>
        {
            entity.ToTable("UserPrivileges");
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.GrantedBy).WithMany().HasForeignKey(e => e.GrantedByUserId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => new { e.UserId, e.Privilege }).IsUnique();
        });

        modelBuilder.Entity<UserVerificationToken>(entity =>
        {
            entity.ToTable("UserVerificationTokens");
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.Token);
        });

        modelBuilder.Entity<TagDailyCount>(entity =>
        {
            entity.ToTable("TagDailyCounts");
            entity.HasKey(e => new { e.TagId, e.Date });
            entity.HasOne(e => e.Tag).WithMany().HasForeignKey(e => e.TagId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Theme>(entity =>
        {
            entity.ToTable("Themes");
            entity.HasIndex(e => e.Key).IsUnique();
            entity
                .HasMany(e => e.Keywords)
                .WithOne(e => e.Theme)
                .HasForeignKey(e => e.ThemeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ThemeKeyword>(entity =>
        {
            entity.ToTable("ThemeKeywords");
            entity.HasIndex(e => new { e.ThemeId, e.Keyword }).IsUnique();
        });
    }
}
