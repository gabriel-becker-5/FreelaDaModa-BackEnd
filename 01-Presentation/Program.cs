// Migrations
// dotnet ef migrations add InitialCreate --project 03-Infrastructure --startup-project 01-Presentation
// dotnet ef database update --project 03-Infrastructure --startup-project 01-Presentation

// Configurar UserSecrets
// "ConnectionStrings:DefaultConnection": "Server=xxx;Database=freeladamoda;User=xxx;Password=xxx",
// "Jwt:Key": "JWT_SECRET_KEY"
// "AdminSeed:Email": "EMAIL_ADMIN"
// "AdminSeed:Password": "PASSWORD_ADMIN"

using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using _02_Application.Interfaces;
using _02_Application.Services;
using _02_Application.Services.Vaga;
using _03_Infrastructure;
using _03_Infrastructure.Repositories;
using _03_Infrastructure.Repositories.Vaga;
using _03_Infrastructure.Storage;
using _04_Domain.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Adicionar controllers
builder.Services.AddControllers();

// Configuração do CORS para permitir a comunicação com o front-end
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configuração da Infraestrutura (DbContext MySQL, Repositórios, PasswordHasher e TokenService)
builder.Services.AddInfrastructure(builder.Configuration);

// Obtém a pasta das imagens de perfil de usuário
builder.Services.Configure<DiskProfileImageStorageOptions>(
    builder.Configuration.GetSection("ProfileImageStorage"));

// Interfaces
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IRoleService, RoleService>();

builder.Services.AddScoped<IOrdemServicoRepository, OrdemServicoRepository>();
builder.Services.AddScoped<IOrdemServicoService, OrdemServicoService>();

builder.Services.AddScoped<IMensagemRepository, MensagemRepository>();
builder.Services.AddScoped<IMensagemService, MensagemService>();

builder.Services.AddScoped<IAvaliacaoRepository, AvaliacaoRepository>();
builder.Services.AddScoped<IAvaliacaoService, AvaliacaoService>();

builder.Services.AddScoped<IFreelancerFieldsService, FreelancerFieldsService>();
builder.Services.AddScoped<IVagaService, VagaService>();
builder.Services.AddScoped<IProfileImageStorage, DiskProfileImageStorage>();
builder.Services.AddScoped<IProfileImageService, ProfileImageService>();

// Serviços da camada de Aplicação
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<RegistrarVagaUseCase>(); // <-- Registo adicionado aqui

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// AUTENTICAÇÃO JWT
string jwtSecret = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key não configurado.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            NameClaimType = JwtRegisteredClaimNames.Name,
            RoleClaimType = "roles"
        };
    });

builder.Services.AddAuthorization();

// Rate Limiter
builder.Services.AddRateLimiter(options =>
{
    // Configurações do rate limiter se necessário
});

// SWAGGER / OPENAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Freela da Moda / API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insere o token JWT no formato: Bearer {seu_token}"
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
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
    });
}

app.UseHttpsRedirection();

// Ativar a política de CORS antes da autenticação e mapeamento
app.UseCors("PermitirFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
