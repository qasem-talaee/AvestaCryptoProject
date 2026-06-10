using System.Text.Json.Serialization;
using Avesta.Application.Interfaces;
using Avesta.Application.Repository;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
// CORS
services.AddCors();

// Controllers
services.AddControllers().AddJsonOptions(option =>
{
    option.JsonSerializerOptions.PropertyNamingPolicy = null;
    option.JsonSerializerOptions.DefaultIgnoreCondition =
        JsonIgnoreCondition.WhenWritingNull;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

services.AddScoped<IService, ServiceRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

// Static files
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.Run();
