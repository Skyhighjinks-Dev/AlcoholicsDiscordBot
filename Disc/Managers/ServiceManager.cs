using Microsoft.Extensions.DependencyInjection;

namespace AlcoholicsDiscordBot.Disc.Managers;


/// <summary>
/// Dependency injection handler
/// </summary>
public static class ServiceManager
{
  /// <summary>Service provider for dependency injection</summary>
  public static IServiceProvider ServiceProvider { get; private set; } = null!;

  /// <summary>
  /// Builds and sets service provider
  /// </summary>
  /// <param name="nServiceCollection"></param>
  public static void SetProvider(ServiceCollection nServiceCollection)
      => ServiceProvider = nServiceCollection.BuildServiceProvider();


  /// <summary>
  /// Retrieves a service from the service provider
  /// </summary>
  /// <typeparam name="T">Type of service to retrieve</typeparam>
  /// <returns>A found service matching the type passed in</returns>
  public static T GetService<T>() => ServiceProvider.GetService<T>()!;
}