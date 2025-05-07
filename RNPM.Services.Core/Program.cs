using System.Text;
using RNPM.API.Data;
using RNPM.API.Services;
using RNPM.APIServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using AutoMapper;
using RNPM.Common.Base;
using RNPM.Common.Base.Infrastructure.Filters;
using RNPM.Common.Data;
using RNPM.Common.Interfaces;
using RNPM.Common.Models;
using Swashbuckle.AspNetCore.Filters;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);
    
    //builder.Host.UseSerilog((ctx, lc) => lc
      //  .ReadFrom.Configuration(ctx.Configuration));

// Add services to the container.

    builder.Services.AddControllers()
        .AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        })
        .AddXmlSerializerFormatters();
    
    //var assembly = Assembly.Load("RNPM.API.Base");
    builder.Services.AddAutoMapper(typeof(MappingProfile));
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
    builder.Services.AddTransient<IDateTimeService, DateTimeService>();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddSwaggerGen(c =>
    {
        //c.AddServer(new OpenApiServer { Url = builder.Configuration["ApiSettings:CoreUrl"] });
        c.OperationFilter<SwaggerDefaultValues>();
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "bearer"
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement()
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
    
    // For Identity
    builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireDigit = false;
        })
        .AddEntityFrameworkStores<RnpmDbContext>()
        .AddDefaultTokenProviders();
    
    builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
        opt.TokenLifespan = TimeSpan.FromMinutes(5));

    // Adding Authentication  
    builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })

        // Adding Jwt Bearer  
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidAudience = builder.Configuration["JWT:ValidAudience"],
                ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"] ?? string.Empty))
            };
        });
        builder.Services.AddDbContext<RnpmDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


        builder.Services.AddAuthorization();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        using var scope = app.Services.CreateScope();
        //var provider = scope.ServiceProvider.GetRequiredService<IApiVersionDescriptionProvider>();
        var context = scope.ServiceProvider.GetRequiredService<RnpmDbContext>();
        await InitializeDatabase(app, context);
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, ex.Message);
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}


static async Task InitializeDatabase(WebApplication app, RnpmDbContext dbContext)
{
    Log.Information("chatsva");
    using var serviceScope = app.Services.CreateScope();
    var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
    var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    dbContext.Database.Migrate();
    
    //SeedData.SeedClaims(dbContext);
    SeedData.SeedRoles(dbContext, roleManager);
    SeedData.SeedUsers(dbContext, roleManager, userManager);
}