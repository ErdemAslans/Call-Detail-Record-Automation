# Authentication & Authorization

**Last Updated**: January 2026  
**Focus**: JWT tokens, Identity, Authorization patterns  

---

## 🔐 Authentication Overview

Cdr.Api uses **ASP.NET Identity** + **JWT Bearer Tokens** for authentication.

### Two-Store Architecture
- **SQL Server**: User credentials stored (via Identity)
- **JWT Tokens**: Stateless authentication (no session)

---

## 👤 User Model (SQL Server)

```csharp
public class User : IdentityUser<Guid>
{
    // Inherited from IdentityUser:
    // - Id (Guid)
    // - Email, NormalizedEmail
    // - PasswordHash, SecurityStamp
    // - PhoneNumber, EmailConfirmed, PhoneNumberConfirmed
    // - TwoFactorEnabled, LockoutEnabled, LockoutEnd
    // - AccessFailedCount, ConcurrencyStamp
    // - UserName
    
    // Custom properties can be added here
}
```

### Password Requirements (Program.cs)
```csharp
builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
{
    options.User.RequireUniqueEmail = true;
    
    options.Password.RequireDigit = true;              // ✓ Must contain digit
    options.Password.RequireLowercase = true;         // ✓ Must contain lowercase
    options.Password.Password.RequireUppercase = true;// ✓ Must contain uppercase
    options.Password.RequireNonAlphanumeric = true;   // ✓ Must contain special char
    options.Password.RequiredLength = 8;              // ✓ Min 8 characters
}).AddEntityFrameworkStores<CdrContext>()
  .AddDefaultTokenProviders();
```

---

## 🎫 JWT Token Architecture

### Token Generation Flow

```csharp
public class TokenService : ITokenService
{
    private readonly JwtConfigModel _jwtConfig;
    
    public async Task<string> GenerateAccessToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Name, user.UserName!),
            
            // Add roles
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtConfig.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtConfig.Issuer,
            audience: _jwtConfig.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_jwtConfig.ExpiresInMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

### Token Claims Breakdown

| Claim | Değer | Kullanım |
|-------|-------|----------|
| `sub` | User.Id | User identification |
| `email` | User.Email | Email-based queries |
| `name` | User.UserName | Display purposes |
| `role` | Role names | Authorization checks |
| `iat` | Issued at | Token creation time |
| `exp` | Expiration | Token validity |

### Token Lifespan
```json
{
  "JwtConfig": {
    "ExpiresInMinutes": 60,  // Token valid for 1 hour
    "Key": "very-long-secret-key-...",
    "Issuer": "cdr-api",
    "Audience": "cdr-web"
  }
}
```

---

## 🔑 Login & Token Refresh

### Login Endpoint
```csharp
[HttpPost("login")]
public async Task<IActionResult> LoginAsync([FromBody] LoginModel model)
{
    try
    {
        var response = await _accountService.LoginAsync(model);
        return Ok(response);  // { token, refreshToken }
    }
    catch (UnauthorizedAccessException ex)
    {
        return Unauthorized(new { message = ex.Message });
    }
}
```

### Request Format
```json
POST /api/account/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePass123!"
}
```

### Response Format
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "8a5f3e2d-9c1b-4a7e-8f2c-6d9e1b3a5f7c"
}
```

### Client-Side Token Usage (Vue)
```typescript
// Store token in localStorage
localStorage.setItem('access_token', response.token);

// Include in subsequent requests
headers.Authorization = `Bearer ${localStorage.getItem('access_token')}`;
```

---

## 🛡️ Token Validation

### JWT Bearer Configuration

```csharp
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var jwtConfig = builder.Configuration.GetSection("JwtConfig")
        .Get<JwtConfigModel>();
        
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,  // ✓ Verify signature
        ValidIssuer = jwtConfig?.Issuer,  // ✓ Check issuer
        ValidAudience = jwtConfig?.Audience,  // ✓ Check audience
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtConfig?.Key)),
        ClockSkew = TimeSpan.FromMinutes(jwtConfig?.ExpiresInMinutes ?? 0)
    };
});
```

### Validation Checks
1. **Signature Verification**: Token signed with correct secret?
2. **Issuer Check**: Token issued by expected server?
3. **Audience Check**: Token intended for this API?
4. **Expiration Check**: Token still valid?
5. **Clock Skew**: Allow small time differences

---

## 👮 Authorization

### Role-Based Authorization

```csharp
[Authorize]  // Authenticated users only
public class ReportController : ControllerBase
{
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> DeleteReportAsync(int id)
    {
        // Only Admin or Manager roles
    }
    
    [AllowAnonymous]  // Override [Authorize] on controller
    public async Task<IActionResult> GetPublicInfoAsync()
    {
        // No authentication required
    }
}
```

### Accessing Claims in Code

```csharp
[HttpGet("profile")]
[Authorize]
public async Task<IActionResult> GetProfileAsync()
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
    var roles = User.FindAll(ClaimTypes.Role);
    
    return Ok(new { userId, userEmail, roles });
}
```

---

## 🔄 Refresh Token Pattern

### Refresh Token Storage

```csharp
// In CdrContext (SQL Server)
public DbSet<RefreshToken> RefreshTokens { get; set; }
```

### Refresh Endpoint (Example Implementation)

```csharp
[HttpPost("refresh")]
public async Task<IActionResult> RefreshTokenAsync(string refreshToken)
{
    var user = await _accountService.ValidateRefreshTokenAsync(refreshToken);
    if (user == null)
    {
        return Unauthorized(new { message = "Invalid refresh token" });
    }
    
    var newAccessToken = await _tokenService.GenerateAccessToken(user);
    var newRefreshToken = _tokenService.GenerateRefreshToken();
    
    // Store new refresh token in database
    await _accountService.SaveRefreshTokenAsync(user.Id, newRefreshToken);
    
    return Ok(new { token = newAccessToken, refreshToken = newRefreshToken });
}
```

---

## ⚠️ Security Considerations

### Password Security
- ✅ PBKDF2 hashing (Identity default)
- ✅ Configurable complexity requirements
- ✅ Unique email enforcement

### Token Security
- ✅ HMAC-SHA256 signature
- ✅ Asymmetric key storage (appsettings.json)
- ✅ 60-minute expiration (short-lived)
- ❌ **NOT IMPLEMENTED**: Token refresh rotation, token blacklist

### CORS Security
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("https://doascdr.fw.dohas.com.tr")  // Whitelist
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});
```
⚠️ Only single origin whitelisted

### Attack Preventions
- **Brute Force**: Consider account lockout policy (currently default 5 failed attempts)
- **Token Theft**: HTTPS enforced, Secure cookies flag (if applicable)
- **CSRF**: Token-based approach (custom CSRF handling needed)

### Recommendations
- [OWASP Authentication Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Authentication_Cheat_Sheet.html)
- Store tokens in httpOnly cookies (not localStorage) for production
- Implement token refresh token rotation
- Add audit logging for authentication events

