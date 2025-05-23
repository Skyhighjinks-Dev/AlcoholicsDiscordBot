using AlcoholicsDiscordBot.Disc;
using AlcoholicsDiscordBot.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlcoholicsDiscordBot
{
  internal class Program
  {
    static async Task Main(string[] args)
    {
      var configBuilder = new ConfigurationBuilder();
      configBuilder.SetBasePath(Directory.GetCurrentDirectory());
      configBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
      IConfiguration config = configBuilder.Build();

      ServiceCollection collection = new ServiceCollection();

      // Maybe keep - depends on how happy I am with it staying loaded in memory
      collection.AddSingleton<IConfiguration>(config);

      // Force token to be present
      if (string.IsNullOrEmpty(config[Configuration.Credentials.TokenKey]))
        throw new InvalidOperationException("A token must be provided!");

      Bot _ = new Bot(collection, 100);
      await _.StartMainAsync();
    }
  }
}
