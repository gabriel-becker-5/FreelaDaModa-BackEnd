// Migrations
// dotnet ef migrations add InitialCreate --project 03-Infrastructure --startup-project 01-Presentation
// dotnet ef database update --project 03-Infrastructure --startup-project 01-Presentation

// Configurar UserSecrets
// "ConnectionStrings:DefaultConnection": "Server=xxx;Database=freeladamoda;User=xxx;Password=xxx",
// "Jwt:Secret": "JWT_SECRET_KEY"
// "AdminSeed:Email": "EMAIL_ADMIN"
// "AdminSeed:Password": "PASSWORD_ADMIN"

using _02_Application.Interfaces;
using _02_Application.Services;
using _03_Infrastructure.Data;
using _03_Infrastructure.Repositories;
using _03_Infrastructure.Repositories.Vaga;
using _03_Infrastructure.Seed;
using _03_Infrastructure.Services;
using _04_Domain.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// CORS - Cross-Origin Resource Sharing
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://127.0.0.1:5500")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// Interfaces
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddSingleton<IPasswordHasher, IdentityPasswordHasher>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFreelancerFieldsService, FreelancerFieldsService>();
builder.Services.AddScoped<IVagaRepository, VagaRepository>();
builder.Services.AddScoped<IVagaService, VagaService>();

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("ConnectionString não configurada.");

builder.Services.AddDbContext<AppDbContext>(options =>
options.UseMySQL(connectionString));
    options.UseMySQL(
        builder.Configuration.GetConnectionString("DefaultConnection")));

string jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? throw new InvalidOperationException("Jwt:Secret não configurado.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            NameClaimType = JwtRegisteredClaimNames.Name,
            RoleClaimType = "roles"
        };
    });

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddFixedWindowLimiter("login", limiter =>
    {
        limiter.PermitLimit = 25; // Observação: o limite é por IP público. Valor não pode ser muito baixo.
        limiter.Window = TimeSpan.FromMinutes(1);
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Freela da Moda",
        Version = "v1",
        Description = "API REST Freela da Moda",
        Contact = new() { Name = "Equipe Entra 21: Charles, Gabriel, Karina, Lucas" }
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter the JWT token as follows: Bearer {your token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });

    string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

var app = builder.Build();

// Aplica migrações automáticas no arranque
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Erro ao aplicar migrations na base de dados.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (IServiceScope scope = app.Services.CreateScope())
{
    IUserRepository userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
    IPasswordHasher passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    await SeedData.Initializer(userRepository, passwordHasher, builder.Configuration);
}

app.Run();