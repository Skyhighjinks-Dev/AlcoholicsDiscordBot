using AlcoholicsDiscordBot.Disc.Managers;
using AlcoholicsDiscordBot.Util;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlcoholicsDiscordBot.Disc;
public class Bot
{
  /// <summary>Discord Socket Client (the bot)</summary>
  private DiscordSocketClient Client { get; set; }

  /// <summary>Command service - Note to self, check if needed as Im going off the old way of doing discord bots</summary>
  private CommandService CommandService { get; set; }

  private bool KillBot { get; set; } = true;
  private int DelaySecondInMiliSec { get; set; }


  /// <summary>
  /// Constructor
  /// </summary>
  /// <param name="nToken">Discord bot token</param>
  /// <param name="nCollection">Service collection with previous injected types</param>
  /// <param name="nKillCheckInMs">Kill check time in miliseconds (checks if kill command has been activated)</param>
  /// <exception cref="InvalidOperationException">Thrown when no token is provided</exception>
  public Bot(ServiceCollection nCollection, int nKillCheckInMs = 100)
  { 
    // Force 100ms as a minimum to avoid 
    if(nKillCheckInMs < 100)
      nKillCheckInMs = 100;

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

    IConfiguration config = ServiceManager.GetService<IConfiguration>();
    if(config == null || string.IsNullOrEmpty(config[Configuration.Credentials.TokenKey]))
      throw new InvalidOperationException("A token must be provided!");

    this.KillBot = false;

    await CommandManager.LoadCommandsAsync();
    await EventManager.LoadEventsAsync();
    await this.Client.LoginAsync(Discord.TokenType.Bot, config[Configuration.Credentials.TokenKey]);
    await this.Client.StartAsync();

    // Loop with delay to ensure bot hasn't been told to die
    while(!this.KillBot || ((!nCancelToken?.IsCancellationRequested) ?? true))
      await Task.Delay(this.DelaySecondInMiliSec, nCancelToken ?? CancellationToken.None);

    await this.Client.LogoutAsync();
    await this.Client.StopAsync();
  }


  /// <summary>
  /// Stops the application
  /// </summary>
  /// <returns></returns>
  public void Stop() => this.KillBot = true;
}
