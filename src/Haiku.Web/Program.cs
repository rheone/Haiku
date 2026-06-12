using Haiku.Domain.Interfaces;
using Haiku.Infrastructure;
using Haiku.Infrastructure.Email;
using Haiku.Infrastructure.Repositories;
using Haiku.Services.Auth;
using Haiku.Services.Dictionary;
using Haiku.Services.Haiku;
using Haiku.Services.Moderation;
using Haiku.Web.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=localhost;Database=Haiku;Trusted_Connection=True;TrustServerCertificate=True;";

builder.Services.AddSingleton(new NHibernateSessionFactory(connectionString));
builder.Services.AddScoped<ScopedSession>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IHaikuRepository, HaikuRepository>();
builder.Services.AddScoped<IVoteRepository, VoteRepository>();
builder.Services.AddScoped<ILoveRepository, LoveRepository>();
builder.Services.AddScoped<IBookmarkRepository, BookmarkRepository>();
builder.Services.AddScoped<IDictionaryRepository, DictionaryRepository>();
builder.Services.AddScoped<IModerationRepository, ModerationRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<HaikuService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<ModerationService>();
builder.Services.AddScoped<DictionaryService>();
builder.Services.AddSingleton<SyllableEngine>();

var emailProvider = builder.Configuration.GetValue<string>("Email:Sender:Provider") ?? "Console";
if (emailProvider == "Smtp")
    builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
else
    builder.Services.AddScoped<IEmailSender, ConsoleEmailSender>();

builder.Services.AddAuthentication()
    .AddCookie(options =>
    {
        options.Cookie.Name = "haiku-auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.None
            : CookieSecurePolicy.Always;
        options.Cookie.SameSite = builder.Environment.IsDevelopment()
            ? SameSiteMode.Lax
            : SameSiteMode.Strict;
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
