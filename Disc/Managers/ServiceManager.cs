using Microsoft.Extensions.DependencyInjection;

namespace AlcoholicsDiscordBot.Disc.Managers;

public static class ServiceManager
{
  public static IServiceProvider ServiceProvider { get; private set; } = null!;

  public static void SetProvider(ServiceCollection nServiceCollection)
      => ServiceProvider = nServiceCollection.BuildServiceProvider();

  public static T GetService<T>() => ServiceProvider.GetService<T>()!;
}