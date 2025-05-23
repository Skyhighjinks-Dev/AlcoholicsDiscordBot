using Discord.Commands;
using System.Reflection;

namespace AlcoholicsDiscordBot.Disc.Managers;
public static class CommandManager
{
  private static CommandService CommandService { get => ServiceManager.GetService<CommandService>(); }

  public static async Task LoadCommandsAsync()
  { 
    await CommandService.AddModulesAsync(Assembly.GetEntryAssembly(), ServiceManager.ServiceProvider);
    foreach(CommandInfo info in CommandService.Commands)
      Console.WriteLine($"Command: {info.Name} loaded");
  }
}
