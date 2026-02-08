using Cdr.Api.Context;
using Cdr.Api.Entities.Cdr;
using Cdr.Api.Helpers;
using Cdr.Api.Interfaces;
using Cdr.Api.Models;
using Cdr.Api.Models.Entities;
using Cdr.Api.Repositories;
using Cdr.Api.Services;
using Cdr.Api.Services.Interfaces;
using Hangfire;
using Hangfire.SqlServer;
using Interfaces.Notification;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Hangfire configuration
builder.Services.AddHangfire(configuration => configuration
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));

builder.Services.AddHangfireServer(options =>
{
    options.Queues = new[] { "default", "reports" };
});

builder.Services.AddDbContext<CdrContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
}).AddEntityFrameworkStores<CdrContext>()
  .AddDefaultTokenProviders();

// start:Token configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
    {
        var jwtConfig = builder.Configuration.GetSection("JwtConfig").Get<JwtConfigModel>();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // ClockSkew sadece sunucu saatleri arasındaki senkronizasyon farkı için
            // Token ömrüyle karıştırılmamalı - 30 saniye yeterli
            ClockSkew = TimeSpan.FromSeconds(30),
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfig?.Issuer,
            ValidAudience = jwtConfig?.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig?.Key))
        };
    });
// end:Token configuration

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.Configure<JwtConfigModel>(builder.Configuration.GetSection("JwtConfig"));
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDb"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));
builder.Services.Configure<CdrReportingSettings>(builder.Configuration.GetSection("CdrReporting"));

builder.Services.AddSingleton<IJwtConfig>(sp => sp.GetRequiredService<IOptions<JwtConfigModel>>().Value);
builder.Services.AddSingleton<IMongoDbSettings>(sp => sp.GetRequiredService<IOptions<MongoDbSettings>>().Value);
builder.Services.AddSingleton<INotification<EmailMessage>, EmailNotification>();

builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICdrRecordsService, CdrRecordsService>();
builder.Services.AddScoped<IOperatorService, OperatorService>();
builder.Services.AddScoped<ICdrReportService, CdrReportService>();
builder.Services.AddScoped<ICdrReportEmailService, CdrReportEmailService>();
builder.Services.AddScoped<ICdrReportJobService, CdrReportJobService>();

builder.Services.AddScoped<ICdrRecordsRepository, CdrRecordsRepository>();
builder.Services.AddScoped<IOperatorRepository, OperatorRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IBreakRepository, BreakRepository>();
builder.Services.AddScoped<IEmailDeliveryAuditRepository, EmailDeliveryAuditRepository>();
builder.Services.AddScoped<IHolidayCalendarRepository, HolidayCalendarRepository>();
builder.Services.AddScoped<IReportExecutionLogRepository, ReportExecutionLogRepository>();
builder.Services.AddScoped<IReadonlyMongoRepository<CdrRecord>>(sp =>
{
    var context = sp.GetRequiredService<MongoDbContext>();
    var mongoDbSettings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new ReadonlyMongoRepository<CdrRecord>(context, mongoDbSettings.CollectionName);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("https://doascdr.fw.dohas.com.tr", "http://localhost:5173")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

// Localization configuration
builder.Services.AddLocalization();
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("tr-TR"),
        new CultureInfo("en-US"),
        new CultureInfo("tr"),
        new CultureInfo("en")
    };
    
    options.DefaultRequestCulture = new RequestCulture("tr-TR");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.ApplyCurrentCultureToResponseHeaders = true;
    
    // Accept-Language header provider (highest priority)
    options.RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new AcceptLanguageHeaderRequestCultureProvider()
    };
});

using var scope = builder.Services.BuildServiceProvider().CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<CdrContext>();
// Ensure the database is created and migrations are applied
var pendingMigrations = dbContext.Database.GetPendingMigrations();
if (pendingMigrations.Any())
{
    dbContext.Database.Migrate();
}


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

// Enable request localization (must be before UseCors)
app.UseRequestLocalization();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseHangfireDashboard();

// Execute Hangfire jobs
HangfireJobs.ExecuteJobs();

app.Run();