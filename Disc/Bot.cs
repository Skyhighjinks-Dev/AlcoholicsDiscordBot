using AlcoholicsDiscordBot.Disc.Managers;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AlcoholicsDiscordBot.Disc;
public class Bot
{
  private DiscordSocketClient Client { get; set; }
  private CommandService CommandService { get; set; }
  private string DiscordToken { get; set; }
  private bool KillBot { get; set; } = true;
  private int DelaySecondInMiliSec { get; set; }


  /// <summary>
  /// Constructor
  /// </summary>
  /// <param name="nToken">Discord bot token</param>
  /// <param name="nCollection">Service collection with previous injected types</param>
  /// <param name="nKillCheckInMs">Kill check time in miliseconds (checks if kill command has been activated)</param>
  /// <exception cref="InvalidOperationException">Thrown when no token is provided</exception>
  public Bot(string nToken, ServiceCollection nCollection, int nKillCheckInMs = 100)
  { 
    if(string.IsNullOrEmpty(nToken))
      throw new InvalidOperationException("Token must be provided!");

    if(nKillCheckInMs < 100)
      nKillCheckInMs = 100;

    this.DiscordToken = nToken; // Replace with a DB call
    this.DelaySecondInMiliSec = nKillCheckInMs;
    this.Client = new DiscordSocketClient(new DiscordSocketConfig() { LogLevel = Discord.LogSeverity.Debug });
    this.CommandService = new CommandService(new CommandServiceConfig()
    { 
      LogLevel = Discord.LogSeverity.Debug,
      CaseSensitiveCommands = false,
      DefaultRunMode = RunMode.Async,
      IgnoreExtraArgs = true
    });

    if(nCollection == null)
      nCollection = new ServiceCollection();

    nCollection.AddSingleton(this.Client)
               .AddSingleton(this.CommandService);

    ServiceManager.SetProvider(nCollection);
  }


  /// <summary>
  /// Spins up the bot and causes infinite loop (while requested to be alive)
  /// </summary>
  /// <param name="nCancelToken">Cancellation token</param>
  /// <returns>Unused</returns>
  public async Task StartMainAsync(CancellationToken? nCancelToken = null)
  {
    // Check if the bot is already running
    if (this.KillBot == false)
      return; 

    this.KillBot = false;

    await CommandManager.LoadCommandsAsync();
    await EventManager.LoadEventsAsync();
    await this.Client.LoginAsync(Discord.TokenType.Bot, this.DiscordToken);
    await this.Client.StartAsync();

    while(!this.KillBot)
      await Task.Delay(this.DelaySecondInMiliSec, nCancelToken ?? CancellationToken.None);

    await this.Client.LogoutAsync();
    await this.Client.StopAsync();
  }


  /// <summary>
  /// Logs t
  /// </summary>
  /// <returns></returns>
  public async Task StopAsync() => this.KillBot = true;
}
