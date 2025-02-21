using System.Text;
using System.Text.Json.Serialization;
using Application.Mapping;
using Application.Services.Implementation;
using Application.Services.Interfaces;
using Domain.Interfaces;
using Domain.Models;
using FluentValidation.AspNetCore;
using Infrastructure;
using Infrastructure.Repository.Implementation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;

namespace Application;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        // CORS Configuration
        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin", builder =>
            {
                builder.WithOrigins("http://127.0.0.1:5500")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });

        // DbContext
        services.AddDbContext<ApplicationContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Identity
        services.AddIdentity<AppUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationContext>()
            .AddDefaultTokenProviders();

        // Services
        services.AddScoped<IUserRequestService, UserRequestService>();
        services.AddScoped<IUserRequestRepository, UserRequestRepository>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IAttachmentService, AttachmentService>();
        services.AddScoped<AttachmentRepository>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IChatRepository, ChatRepository>();
        services.AddScoped<IOperatorRepository, OperatorRepository>();
        services.AddScoped<IEmailSender, EmailSenderService>();
        services.AddSingleton<ProfanityFilterService>();

        services.AddScoped<OperatorService>();
        services.AddScoped<AdminService>();
        services.AddSignalR();

        // FluentValidation
        services.AddFluentValidation(config =>
        {
            config.AutomaticValidationEnabled = true;
            config.RegisterValidatorsFromAssembly(typeof(Program).Assembly);
        });

        // JWT Authentication
        services.AddAuthentication(options =>
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
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                    ClockSkew = TimeSpan.Zero
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var claims = context.Principal?.Claims.Select(c => $"{c.Type}: {c.Value}");
                        Console.WriteLine("Token validated. Claims: " + string.Join(", ", claims));
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    }
                };
            });

        // OData Configuration
        var modelBuilder = new ODataConventionModelBuilder();
        modelBuilder.EntitySet<UserRequest>("UserRequests");
        modelBuilder.EntitySet<AttachmentUser>("Attachments");

        services.AddControllers()
            .AddOData(options =>
                options.Select().Filter().OrderBy().Expand().Count().SetMaxTop(100)
                    .AddRouteComponents("odata", modelBuilder.GetEdmModel()))
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        // AutoMapper
        services.AddAutoMapper(typeof(UserMappingProfile));
        services.AddAutoMapper(typeof(UserRequestMappingProfile));
        services.AddAutoMapper(typeof(RequestMappingProfile));

        // Swagger
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Введите JWT-токен в формате Bearer [токен]"
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
                    new string[] { }
                }
            });
        });

        return services;
    }
}