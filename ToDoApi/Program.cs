using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using ToDoApi;

var builder = WebApplication.CreateBuilder(args);

// 1. חיבור ל-DB
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ToDoDB"))));

// 2. Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. הגדרת CORS (פעם אחת בלבד!)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 4. הגדרת JWT Authentication
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

// ----- סדר ה-Middleware (חשוב מאוד!) -----

// תמיד ראשון כדי שכל הבקשות יאושרו
app.UseCors("AllowAll"); 

// Swagger תמיד זמין ב-Development (או בכלל ב-Render לבדיקות)
app.UseSwagger();
app.UseSwaggerUI();

// אימות חייב לבוא לפני הרשאות
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

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("supersecretkey123456789012345678"));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var token = new JwtSecurityToken(
        expires: DateTime.Now.AddHours(1),
        signingCredentials: creds);
    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

    return Results.Ok(new { token = tokenString });
});

// ===== ToDo Routes =====

app.MapGet("/api/todoitems", async (ToDoDbContext db) =>
    await db.Todoitems.ToListAsync()).RequireAuthorization();

app.MapPost("/api/todoitems", async (Todoitem item, ToDoDbContext db) =>
{
    db.Todoitems.Add(item);
    await db.SaveChangesAsync();
    return Results.Ok(item);
}).RequireAuthorization();

app.MapPut("/api/todoitems/{id}", async (int id, Todoitem updatedItem, ToDoDbContext db) =>
{
    var item = await db.Todoitems.FindAsync(id);
    if (item == null) return Results.NotFound();
    item.Name = updatedItem.Name;
    item.IsComplete = updatedItem.IsComplete;
    await db.SaveChangesAsync();
    return Results.Ok(item);
}).RequireAuthorization();

app.MapDelete("/api/todoitems/{id}", async (int id, ToDoDbContext db) =>
{
    var item = await db.Todoitems.FindAsync(id);
    if (item == null) return Results.NotFound();
    db.Todoitems.Remove(item);
    await db.SaveChangesAsync();
    return Results.NoContent();
}).RequireAuthorization();

app.Run();