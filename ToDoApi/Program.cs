using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ToDoApi;

var builder = WebApplication.CreateBuilder(args);

// 1. חיבור ל-DB
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ToDoDB"))));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. הגדרת CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://todolist-umei.onrender.com") // הכתובת של ה-Frontend
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 3. הגדרת JWT Authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("supersecretkey123456789012345678"))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication(); 
app.UseAuthorization();

// ===== Auth Routes =====

app.MapPost("/api/auth/register", async (User user, ToDoDbContext db) =>
{
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapPost("/api/auth/login", async (User loginUser, ToDoDbContext db) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => 
        u.Username == loginUser.Username && u.Password == loginUser.Password);

    if (user == null) return Results.Unauthorized();

    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username ?? "")
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("supersecretkey123456789012345678"));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: "your-app",
        audience: "your-app",
        claims: claims,
        expires: DateTime.Now.AddHours(3),
        signingCredentials: creds
    );

    return Results.Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
});

// ===== ToDo Routes =====

app.MapGet("/api/todoitems", async (ClaimsPrincipal user, ToDoDbContext db) =>
{
    var userIdStr = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userIdStr == null) return Results.Unauthorized();
    int userId = int.Parse(userIdStr);

    var items = await db.Todoitems.Where(t => t.UserId == userId).ToListAsync();
    return Results.Ok(items);
}).RequireAuthorization();

app.MapPost("/api/todoitems", async (ClaimsPrincipal user, ToDoDbContext db, Todoitem item) =>
{
    var userIdStr = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userIdStr == null) return Results.Unauthorized();
    
    item.UserId = int.Parse(userIdStr);
    db.Todoitems.Add(item);
    await db.SaveChangesAsync();
    return Results.Ok(item);
}).RequireAuthorization();

app.MapPut("/api/todoitems/{id}", async (int id, ClaimsPrincipal user, ToDoDbContext db, Todoitem updatedItem) =>
{
    var userIdStr = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userIdStr == null) return Results.Unauthorized();
    int userId = int.Parse(userIdStr);

    var item = await db.Todoitems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
    if (item == null) return Results.NotFound();

    if (!string.IsNullOrEmpty(updatedItem.Name)) item.Name = updatedItem.Name;
    item.IsComplete = updatedItem.IsComplete;

    await db.SaveChangesAsync();
    return Results.Ok(item);
}).RequireAuthorization();

app.MapDelete("/api/todoitems/{id}", async (int id, ClaimsPrincipal user, ToDoDbContext db) =>
{
    var userIdStr = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userIdStr == null) return Results.Unauthorized();
    int userId = int.Parse(userIdStr);

    var item = await db.Todoitems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
    if (item == null) return Results.NotFound();
    
    db.Todoitems.Remove(item);
    await db.SaveChangesAsync();
    return Results.NoContent();
}).RequireAuthorization();

app.Run();