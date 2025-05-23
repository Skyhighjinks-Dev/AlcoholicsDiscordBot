using Discord.WebSocket;

namespace AlcoholicsDiscordBot.Disc.Managers;
public static class EventManager
{
  /// <summary>Discord Socket Client retrieved from dependency injection</summary>
  private static DiscordSocketClient Client { get => ServiceManager.GetService<DiscordSocketClient>(); }


  /// <summary>
  /// Loads/registers all events
  /// </summary>
  /// <returns></returns>
  public static async Task LoadEventsAsync()
  { 
    Client.Ready += OnClientReadyAsync;
    Client.Disconnected += OnClientDisconnectAsync;
    Client.LoggedOut += OnClientLogoutAsync;

    await Client.SetStatusAsync(Discord.UserStatus.AFK);
    await Client.SetGameAsync($"Probably drinking, mistakes will happen.");
  }


  /// <summary>
  /// Event raised on client ready to interact with Discord
  /// </summary>
  private static async Task OnClientReadyAsync()
  { 
    Console.WriteLine($"{Client.CurrentUser.Username} is alive");
  }


  /// <summary>
  /// Event raised when the bot is disconnected for any reason
  /// </summary>
  /// <param name="nException">Exception thrown by discord</param>
  private static async Task OnClientDisconnectAsync(Exception nException)
  { 
    Console.WriteLine($"Client has disconnected! Reason:\n{nException.ToString()}");
  }


  /// <summary>
  /// Event raised when bot is logged out
  /// </summary>
  private static async Task OnClientLogoutAsync()
  { 
    Console.WriteLine($"Client has logged out");
  }
}
