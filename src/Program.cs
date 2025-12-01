using Wolverine;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDatabases(builder.Configuration);
builder.Services.AddWolverine(options =>
{
    options.PublishMessage<Iowa.Packages.Delete.Messager.Message>().ToLocalQueue("package-delete");
    options.PublishMessage<Iowa.Packages.Post.Messager.Message>().ToLocalQueue("package-post");
    options.PublishMessage<Iowa.Packages.Update.Messager.Message>().ToLocalQueue("package-update");
});
builder.Services.AddSignalR(x => x.EnableDetailedErrors = true);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<Iowa.Packages.Hub>("packages-hub");

app.Run();
