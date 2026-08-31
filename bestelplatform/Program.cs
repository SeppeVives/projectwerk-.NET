using bestelplatform.Data.bestelplatform;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add services to the container.
builder.Services
    .AddDbContext<BestelplatformDbContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)))
    .AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();

app.MapControllers();

app.Run();