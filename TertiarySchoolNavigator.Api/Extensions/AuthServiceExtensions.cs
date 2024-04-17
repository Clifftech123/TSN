using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TertiarySchoolNavigator.Api.Domain;
using TertiarySchoolNavigator.Api.Models;
using TertiarySchoolNavigator.Api.Models.AuthModels;

namespace TertiarySchoolNavigator.Api.Extensions
{
    public static class AuthServiceExtensions
    {

        // Configure the SQL Server connection with the connection string from the appsettings.json file
        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration)
            =>
            services.AddDbContext<AppDbContext>(
                o => o.UseSqlServer(configuration.GetConnectionString("sqlConnection"),
                    b => b.MigrationsAssembly("TertialSchoolNavigator")));


        // Configure Identity Framework with the default settings and the custom User and Role classes
        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentity<User, Role>(o =>
            {
                o.User.RequireUniqueEmail = true;
                o.Password.RequireDigit = true;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequiredLength = 8;
                o.Password.RequireNonAlphanumeric = false;
            });


            // Add Identity Framework to the project and configure the default token providers
            builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
            builder.AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
        }


        // Configure JWT with the settings from the appsettings.json file
        public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration.GetSection(nameof(JWTSettings)).Get<JWTSettings>();
            var builder = services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = settings.Audience,
                    ValidIssuer = settings.ValidIssuer,
                };
            });
        }


        // Configure CORS to allow requests from any origin, header, and method
        public static void ConfigureCors(this IServiceCollection services) =>
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());
            });
    }
}

