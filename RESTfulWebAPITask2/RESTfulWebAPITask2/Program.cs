using Microsoft.EntityFrameworkCore;
using RESTfulWebAPITask2;
using RESTfulWebAPITask2.Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<ICartItemService, CartItemService>();
// Register the database context
builder.Services.AddDbContext<CartDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CartDbConnection")));


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

app.UseAuthorization();

app.MapControllers();


app.Run();

