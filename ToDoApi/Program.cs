using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using ToDoApi;

var builder = WebApplication.CreateBuilder(args);

// חיבור ל-DB
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ToDoDB"))));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// JWT Authentication
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
// 1. הגדרת הפוליסה (לפני ה-var app = builder.Build();)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

// 2. הפעלת ה-CORS (חייב להיות אחרי Build ולפני MapControllers)
app.UseCors("AllowAll");

app.UseAuthorization();
// ... שאר הקוד


// Swagger
app.UseSwagger();
app.UseSwaggerUI();

// CORS
app.UseCors("AllowAll");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// ===== Auth Routes =====

// רישום
app.MapPost("/api/auth/register", async (User user, ToDoDbContext db) =>
{
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Ok();
});

// לוגין
app.MapPost("/api/auth/login", async (User loginUser, ToDoDbContext db) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u =>
        u.Username == loginUser.Username && u.Password == loginUser.Password);

    if (user == null) return Results.Unauthorized();

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("supersecretkey123456789012345678"));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var token = new JwtSecurityToken(
        expires: DateTime.Now.AddHours(1),
        signingCredentials: creds);
    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

    return Results.Ok(new { token = tokenString });
});

// ===== ToDo Routes =====

// שליפת כל המשימות
app.MapGet("/api/todoitems", async (ToDoDbContext db) =>
    await db.Todoitems.ToListAsync()).RequireAuthorization();

// הוספת משימה
app.MapPost("/api/todoitems", async (Todoitem item, ToDoDbContext db) =>
{
    db.Todoitems.Add(item);
    await db.SaveChangesAsync();
    return Results.Ok(item);
}).RequireAuthorization();

// עדכון משימה
app.MapPut("/api/todoitems/{id}", async (int id, Todoitem updatedItem, ToDoDbContext db) =>
{
    var item = await db.Todoitems.FindAsync(id);
    if (item == null) return Results.NotFound();
    item.Name = updatedItem.Name;
    item.IsComplete = updatedItem.IsComplete;
    await db.SaveChangesAsync();
    return Results.Ok(item);
}).RequireAuthorization();

// מחיקת משימה
app.MapDelete("/api/todoitems/{id}", async (int id, ToDoDbContext db) =>
{
    var item = await db.Todoitems.FindAsync(id);
    if (item == null) return Results.NotFound();
    db.Todoitems.Remove(item);
    await db.SaveChangesAsync();
    return Results.NoContent();
}).RequireAuthorization();

app.Run();