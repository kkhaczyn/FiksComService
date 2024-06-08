using FiksComService.Application;
using FiksComService.Models.Database;
using FiksComService.Repositories;
using log4net.Config;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.config"));
builder.Logging.ClearProviders();
builder.Logging.AddLog4Net("log4net.config");

builder.Services.AddIdentity<User, Role>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = true;
}).AddEntityFrameworkStores<ApplicationContext>();

var connectionString = builder.Environment.IsProduction()
    ? builder.Configuration.GetConnectionString("CSFiksComDocker")
    : builder.Configuration.GetConnectionString("CSFiksCom");

builder.Services.AddControllers();
builder.Services.AddDbContextFactory<ApplicationContext>(options
    => options.UseNpgsql(connectionString));
builder.Services.AddSingleton<IComponentRepository, ComponentRepository>();
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
builder.Services.AddSingleton<IOrderDetailRepository, OrderDetailRepository>();
builder.Services.AddSingleton<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddSingleton<IComponentTypeRepository, ComponentTypeRepository>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddCors(options =>
{
    options.AddPolicy("default", policy =>
    {
        policy.WithOrigins("http://localhost")
        .WithOrigins("http://51.103.240.161")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseCors("default");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationContext>();
    context.Database.Migrate();
}

QuestPDF.Settings.License = LicenseType.Community;

app.Run();
