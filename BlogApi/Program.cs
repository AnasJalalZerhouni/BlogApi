using BlogApi.Infrastructure;
using BlogApi.Infrastructure.Errors;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services= builder.Services;

var connectionString = builder.Configuration.GetConnectionString("BlogConnectionString");
services.AddDbContext<BlogContext>(opts =>
{
    opts.UseSqlServer(connectionString);
});

services.AddLocalization(x => x.ResourcesPath = "Resources");

services.AddSwaggerGen();
services.AddCors();

services.AddMvc(opt =>
{
    opt.Filters.Add(typeof(ValidatorActionFilter));

}).AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.DefaultIgnoreCondition =
        System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
}).AddFluentValidation(cfg =>
{
    cfg.RegisterValidatorsFromAssemblyContaining<Program>();
});



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseCors(builder =>
    builder
     .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod()
);

app.MapControllers();

app.Services.CreateScope()
    .ServiceProvider
    .GetRequiredService<BlogContext>()
    .Database.EnsureCreated();

app.Run();
