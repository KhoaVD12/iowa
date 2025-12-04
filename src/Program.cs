using Iowa.Authentication;
using Iowa.Databases;
using Iowa.Wolverine;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddDatabases(builder.Configuration);
builder.Services.AddAuthentication(builder.Configuration);
builder.Services.AddWolverines(builder.Configuration);

builder.Services.AddSignalR(x => x.EnableDetailedErrors = true);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();


app.MapHub<Iowa.Subscriptions.Hub>("subscriptions-hub");
app.MapHub<Iowa.Packages.Hub>("packages-hub");
app.MapHub<Iowa.Providers.Hub>("providers-hub");

app.Run();
