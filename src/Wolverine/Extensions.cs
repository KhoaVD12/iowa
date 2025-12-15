using Microsoft.EntityFrameworkCore;
using Wolverine;

namespace Iowa.Wolverine;

public static class Extensions
{
   
    public static IServiceCollection AddWolverines(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddWolverine(options =>
    {
        options.PublishMessage<Iowa.Subscriptions.Delete.Messager.Message>().ToLocalQueue("subcription-delete");
        options.PublishMessage<Iowa.Subscriptions.Post.Messager.Message>().ToLocalQueue("subcription-post");
        options.PublishMessage<Iowa.Subscriptions.Put.Messager.Message>().ToLocalQueue("subcription-update");

        options.PublishMessage<Iowa.Packages.Delete.Messager.Message>().ToLocalQueue("package-delete");
        options.PublishMessage<Iowa.Packages.Post.Messager.Message>().ToLocalQueue("package-post");
        options.PublishMessage<Iowa.Packages.Put.Messager.Message>().ToLocalQueue("package-update");

        options.PublishMessage<Iowa.Providers.Delete.Messager.Message>().ToLocalQueue("provider-delete");
        options.PublishMessage<Iowa.Providers.Post.Messager.Message>().ToLocalQueue("provider-post");
        options.PublishMessage<Iowa.Providers.Put.Messager.Message>().ToLocalQueue("provider-update");

        options.PublishMessage<Iowa.SubscriptionBySubscriptionPlan.Post.Messager.Message>().ToLocalQueue("subscription-by-subscription-plan-post");
    });

        return services;
    }
}
