using FiksComService.Application;
using FiksComService.Models.Database;
using FiksComService.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Logging.AddLog4Net("log4net.config");

builder.Services.AddIdentity<User, Role>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = true;
}).AddEntityFrameworkStores<ApplicationContext>();

builder.Services.AddControllers();
builder.Services.AddDbContextFactory<ApplicationContext>(options 
    => options.UseNpgsql(builder.Configuration.GetConnectionString("CSFiksCom")));
builder.Services.AddSingleton<IComponentRepository, ComponentRepository>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthorization();
app.UseAuthentication();
app.MapControllers();

using (var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationContext>();
    context.Database.Migrate();
}

app.Run();
