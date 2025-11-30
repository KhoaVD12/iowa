using Wolverine;

namespace Iowa.Wolverine;

public static class Extensions
{
    public static IServiceCollection AddWolverines(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddWolverine(options =>
        {
            options.PublishMessage<Subscriptions.Post.Messager.Message>().ToLocalQueue("subscription-created");
        });
            
        return services;
    }
}
