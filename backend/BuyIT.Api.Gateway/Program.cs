using BuyIT.Api.Gateway;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add services to the container.
var grpcClients = builder.Configuration.GetSection("AppSettings:GrpcClients").Get<Dictionary<string, string>>();


builder.Services.AddGrpcClient<AuthService.AuthServiceClient>(o =>
{
    o.Address = new Uri(grpcClients["AuthService"]);
});
builder.Services.AddGrpcClient<CouponService.CouponServiceClient>(o =>
{
    o.Address = new Uri(grpcClients["CouponService"]);
});

builder.Services.AddCors(o =>
{
    var corsOrigins = builder.Configuration.GetSection("AppSettings:CORS_ORIGINS").Get<string[]>();
    o.AddDefaultPolicy(builder =>
        builder.WithOrigins(corsOrigins).AllowAnyHeader().AllowAnyMethod().AllowCredentials());
});

builder.Services.AddControllers();
var jwtOptions = builder.Configuration.GetSection("AppSettings:JwtOptions").Get<JwtOptions>();
var key = Encoding.ASCII.GetBytes(jwtOptions.Secret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidAudience = jwtOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("admin"));
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BUYIT GATEWAY API", Version = "v1" });
    c.EnableAnnotations(); // Enable Swagger annotations

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token in the text input below.\n\nExample: \"Bearer 12345abcdef\"",
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
            new string[] {}
        }
    });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
