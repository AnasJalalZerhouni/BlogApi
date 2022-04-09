using BlogApi;
using BlogApi.Features.Profiles;
using BlogApi.Infrastructure;
using BlogApi.Infrastructure.Errors;
using BlogApi.Infrastructure.Security;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var services= builder.Services;


services.AddMediatR(Assembly.GetExecutingAssembly());
services.AddTransient(typeof(IPipelineBehavior<,>),typeof(ValidationPipelineBehavior<,>));
services.AddTransient(typeof(IPipelineBehavior<,>),typeof(DBContextTransactionPipelineBehavior<,>));

var connectionString = builder.Configuration.GetConnectionString("BlogConnectionString");
services.AddDbContext<BlogContext>(opts =>
{
    opts.UseSqlServer(connectionString);
    opts.EnableSensitiveDataLogging();
});
 
services.AddLocalization(x => x.ResourcesPath = "Resources");

services.AddSwaggerGen(x =>
{
    x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT"
    });


    x.CustomSchemaIds(y => y.FullName.Replace("+", "."));

    x.DocInclusionPredicate((version, apiDescription) => true);
    x.TagActionsBy(y => new List<string>()
                {
                    y.GroupName ?? throw new InvalidOperationException()
                });
});
services.AddCors();

services.AddMvc(opt =>
{
    opt.Conventions.Add(new GroupByApiRootConvention());
    opt.Filters.Add(typeof(ValidatorActionFilter));
    opt.EnableEndpointRouting = false;

}).AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.DefaultIgnoreCondition =
        System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
}).AddFluentValidation(cfg =>
{
    cfg.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
});

services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


services.AddScoped<IProfileReader, ProfileReader>();
services.AddScoped<IPasswordHasher, PasswordHasher>();
services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();
services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

services.AddJwt();


var app = builder.Build();


app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseCors(builder =>
    builder
     .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod()
);

app.UseAuthentication();

app.UseMvc();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Services.CreateScope()
    .ServiceProvider
    .GetRequiredService<BlogContext>()
    .Database.EnsureCreated();

app.Run();
