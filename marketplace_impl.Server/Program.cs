using marketplace_impl.Server.Configs;
using marketplace_impl.Server.Configs.Authentication;
using marketplace_impl.Server.Data;
using marketplace_impl.Server.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IdentityService>();
builder.Services.AddScoped<BusinessService>();
builder.Services.AddTransient<IAuthorizationHandler, AccessTraitAuthorizationHandler>();

#region Authentication
builder.Services.AddAuthentication(o => {
    o.DefaultScheme = Constants.AuthScheme;
})
    .AddScheme<AuthenticationSchemeOptions, AuthenticationHandler>(Constants.AuthScheme, o => { });

builder.Services.AddScoped<IPasswordHasher<ApplicationUser>, PasswordHasher<ApplicationUser>>();
#endregion

var config = builder.Configuration;

var connectionString = config.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

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

if (app.Environment.IsDevelopment() && app.Configuration.GetValue<bool>("ShouldInitializeDb") == true)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = services.GetRequiredService<IPasswordHasher<ApplicationUser>>();
        await DbInitializer.Initialize(context, passwordHasher);
    }
}

app.Run();
