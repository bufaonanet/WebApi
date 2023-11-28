using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebApi.Data;
using WebApi.Extensions;
using WebApi.Interfaces;

var builder = WebApplication.CreateBuilder(args);
{
    var builderConnection = new SqlConnectionStringBuilder(builder.
        Configuration.GetConnectionString("DefaultConnection"));
    builderConnection.Password = builder.Configuration.GetSection("DBPassword").Value;
    
    builder.Services.AddDbContext<DataContext>(options =>
        options.UseSqlServer(builderConnection.ConnectionString));

    builder.Services.AddControllers()
        .AddNewtonsoftJson();

    builder.Services.AddCors();

    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

    builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

    var secretKey = builder.Configuration.GetSection("AppSettings:Key").Value;
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(opt =>
        {
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = key
            };
        });
    
   
    
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApi", Version = "v1" });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header usando o esquema Bearer."
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
                new string[] { }
            }
        });
    });
    
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();
{
    app.ConfigureExceptionHandler(app.Environment);

    app.UseSwagger();
    app.UseSwaggerUI();
    
    app.UseHsts();
   
    app.UseHttpsRedirection();

    app.UseCors(m =>
        m.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseDefaultFiles();
    app.UseStaticFiles();

    app.MapControllers();
    app.Run();
}