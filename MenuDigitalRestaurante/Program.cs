using Application.Interfaces.CategoryInterfaces;
using Application.Interfaces.DishInterfaces;
using Application.Mappers;
using Application.UseCase.DishUse;
using Application.Validations;
using Infrastructure.Command;
using Infrastructure.Persistence;
using Infrastructure.Querys;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Custom
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("MenuDigitalConnection")));

builder.Services.AddScoped<IDishServices, DishServices>();
builder.Services.AddScoped<IDishCommand, DishCommand>();
builder.Services.AddScoped<IDishQuery, DishQuery>();
builder.Services.AddScoped<IDishMapper, DishMapper>();
builder.Services.AddScoped<IDishValidator, DishValidator>();
builder.Services.AddScoped<ICategoryMapper, CategoryMapper>();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Restaurante API",
        Description = "API para la gestión de platos en un restaurante",
        Contact = new OpenApiContact
        {
            Name = "Contact Restaurant  API Support",
            Url = new Uri("https://example.com/contact")
        }
    });
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<Infrastructure.Middleware.ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();
