using Discord.Commands;
using System.Reflection;

namespace AlcoholicsDiscordBot.Disc.Managers;
public static class CommandManager
{
  /// <summary>Command service retrieved from DI</summary>
  private static CommandService CommandService { get => ServiceManager.GetService<CommandService>(); }


  /// <summary>
  /// Loads commands 
  /// Unsure if this is correct, but it was how you used to do it
  /// </summary>
  /// <returns></returns>
  public static async Task LoadCommandsAsync()
  { 
    await CommandService.AddModulesAsync(Assembly.GetEntryAssembly(), ServiceManager.ServiceProvider);
    foreach(CommandInfo info in CommandService.Commands)
      Console.WriteLine($"Command: {info.Name} loaded");
  }
}
