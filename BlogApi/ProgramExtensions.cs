using BlogApi.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System.Text;

namespace BlogApi
{
    public static class ProgramExtensions
    {
        public static void AddJwt(this IServiceCollection services)
        {
            services.AddOptions();
            var signinkey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("samalamadumalamayouassumingimahuming"));
            var signingCredentials = new SigningCredentials(signinkey,SecurityAlgorithms.HmacSha256);
            var issuer = "Issuer";
            var audience = "audience";


            services.Configure<JwtIssuerOptions>(opts =>
            {
                opts.Issuer = issuer;
                opts.Audience = audience;
                opts.SigningCredentials = signingCredentials;

            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingCredentials.Key,
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
            };

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opts =>
                {
                    opts.TokenValidationParameters = tokenValidationParameters;
                    opts.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = (context) =>
                        {
                            var token = context.HttpContext.Request.Headers["Authorization"];
                            if (token.Count > 0 && token[0].StartsWith("Token ", StringComparison.OrdinalIgnoreCase))
                            {
                                context.Token = token[0].Substring("Token ".Length).Trim(); 
                            }
                           return Task.CompletedTask;
                        }
                    };
                });
        }
    
        public static void AddSerilogLogging(this ILoggingBuilder logger)
        {

            logger.ClearProviders();

            var log = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                //just for local debug
                .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss} [{Level}] {SourceContext} {Message}{NewLine}{Exception}", theme: AnsiConsoleTheme.Code)
                .CreateLogger();

            logger.AddSerilog(log);
            Log.Logger = log;
        }
    }
}
