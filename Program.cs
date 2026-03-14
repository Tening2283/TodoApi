using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TodoApi.Models;

var builder = WebApplication.CreateBuilder(args);

// -------------------------------------------------------------------
// Code exact du tutoriel Microsoft
// -------------------------------------------------------------------
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Contexte TodoContext avec base de données en mémoire (tutoriel)
builder.Services.AddDbContext<TodoContext>(opt =>
    opt.UseInMemoryDatabase("TodoList"));

// -------------------------------------------------------------------
// Ajouts pour l'exigence TP : authentification et sécurité
// -------------------------------------------------------------------

// Contexte AppDbContext (utilisateurs) — aussi en mémoire
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("AuthDb"));

// Configuration JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKey)
    };
});

builder.Services.AddAuthorization();

// Swagger avec support JWT (bouton Authorize dans l'UI)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TodoApi",
        Version = "v1",
        Description = "API Todo — ASP.NET Core + EF InMemory + JWT"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Entrez : Bearer {votre_token_jwt}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// -------------------------------------------------------------------
// Pipeline HTTP (structure exacte du tutoriel)
// -------------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TodoApi v1");
        c.RoutePrefix = string.Empty; // Swagger à la racine "/"
    });
}

app.UseHttpsRedirection();

// Ordre obligatoire : Authentication AVANT Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
