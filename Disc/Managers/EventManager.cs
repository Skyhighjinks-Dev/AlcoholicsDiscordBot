using Discord.Commands;
using Discord.WebSocket;

namespace AlcoholicsDiscordBot.Disc.Managers;
public static class EventManager
{
  private static DiscordSocketClient Client { get => ServiceManager.GetService<DiscordSocketClient>(); }
  private static CommandService CommandService { get => ServiceManager.GetService<CommandService>(); }


  public static async Task LoadEventsAsync()
  { 
    Client.Ready += OnClientReadyAsync;
    Client.Disconnected += OnClientDisconnectAsync;
    Client.LoggedOut += OnClientLogoutAsync;

    await Client.SetStatusAsync(Discord.UserStatus.AFK);
    await Client.SetGameAsync($"Probably drinking, mistakes will happen.");
  }

  private static async Task OnClientReadyAsync()
  { 
    Console.WriteLine($"{Client.CurrentUser.Username} is alive");
  }

  private static async Task OnClientDisconnectAsync(Exception nException)
  { 
    Console.WriteLine($"Client has disconnected! Reason:\n{nException.ToString()}");
  }

  private static async Task OnClientLogoutAsync()
  { 
    Console.WriteLine($"Client has logged out");
  }
}
