using Microsoft.EntityFrameworkCore;
using Wolverine;

namespace Iowa.Wolverine;

public static class Extensions
{
   
    public static IServiceCollection AddWolverines(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddWolverine(options =>
    {
        options.PublishMessage<Iowa.Packages.Delete.Messager.Message>().ToLocalQueue("subcription-delete");
        options.PublishMessage<Iowa.Packages.Post.Messager.Message>().ToLocalQueue("subcription-post");
        options.PublishMessage<Iowa.Packages.Update.Messager.Message>().ToLocalQueue("subcription-update");

        options.PublishMessage<Iowa.Packages.Delete.Messager.Message>().ToLocalQueue("package-delete");
        options.PublishMessage<Iowa.Packages.Post.Messager.Message>().ToLocalQueue("package-post");
        options.PublishMessage<Iowa.Packages.Update.Messager.Message>().ToLocalQueue("package-update");
    });

        return services;
    }
}
