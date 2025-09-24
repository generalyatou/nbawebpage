using Microsoft.EntityFrameworkCore;
using NBA.Business.Services;
using NBA.data;
using NBA.data.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionStringValue = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddPooledDbContextFactory<NBAContext>(opt =>
opt.UseSqlServer(connectionStringValue));

// Your services
builder.Services.AddScoped<INbaDetailsService, NBADetailsService>();
builder.Services.AddScoped<INbaDetailsMapping, NBADetailsMapping>();

var app = builder.Build();

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseRouting();
app.UseCors();
app.MapControllers();

app.Run();
