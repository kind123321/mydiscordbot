using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Web;

public class MusicModule : ModuleBase<SocketCommandContext>
{

    private readonly YoutubeClient _ytClient;

    public MusicModule(YoutubeClient ytClient)
    {
        _ytClient = ytClient;
    }

    [Command("play")]
    public async Task PlayMusicAsync([Remainder] string songName)
    {
        string videoId = null;
        if (Uri.TryCreate(songName, UriKind.Absolute, out Uri uri) && uri.Host == "www.youtube.com")
        {
            var queryParams = HttpUtility.ParseQueryString(uri.Query);
            videoId = queryParams["v"];
        }
        else if (Uri.TryCreate(songName, UriKind.Absolute, out uri) && uri.Host == "youtu.be")
        {
            videoId = uri.AbsolutePath.TrimStart('/');
        }
        else
        {
            videoId = (await _ytClient.Search.GetVideosAsync(songName).FirstOrDefaultAsync())?.Id;
        }

        if (videoId == null)
        {
            await Context.Channel.SendMessageAsync("Video not found!");
            return;
        }

        var streamManifest = await _ytClient.Videos.Streams.GetManifestAsync(videoId);
        var streamInfo = streamManifest.GetAudioStreams().GetWithHighestBitrate();
        var stream = await _ytClient.Videos.Streams.GetAsync(streamInfo);

        var voiceChannel = (Context.User as IGuildUser)?.VoiceChannel;
        if (voiceChannel == null)
        {
            await Context.Channel.SendMessageAsync("You must be in a voice channel!");
            return;
        }

        var audioClient = await voiceChannel.ConnectAsync();
        var audioStream = audioClient.CreatePCMStream(AudioApplication.Music);
        await stream.CopyToAsync(audioStream);
        await audioStream.FlushAsync();
        await audioClient.StopAsync();
    }
}
public class MusicBot
{
   

    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly YoutubeClient _ytClient;
    static async Task Main(string[] args)
    {
      

        var bot = new MusicBot();
        await bot.MainAsync("MTA3NjgzNTMzNjg5NTg3MzAyNA.GkABFJ.KYsnRvMZ3kVW4NxdiJyGKU_9UjDv6fOxmDh9fc");



    }

    public MusicBot()
    {
        _client = new DiscordSocketClient();
        _commands = new CommandService();
        _ytClient = new YoutubeClient();

        var config = new DiscordSocketConfig { GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages };
        _client = new DiscordSocketClient(config);
       


    }

    public async Task MainAsync(string token)
    {

        _client.Log += Log;
        _commands.Log += Log;

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        var services = new ServiceCollection()
            .AddSingleton(_ytClient)
            .BuildServiceProvider();

        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);

        _client.MessageReceived += HandleCommandAsync;

        await Task.Delay(-1);
    }

    private async Task HandleCommandAsync(SocketMessage messageParam)
    {
        Console.WriteLine();
        var message = messageParam as SocketUserMessage;
        if (message == null) return;

        var context = new SocketCommandContext(_client, message);

        int argPos = 0;
        if (message.HasStringPrefix("&", ref argPos))
        {
            var result = await _commands.ExecuteAsync(context, argPos, null);

            if (!result.IsSuccess)
            {
                Console.WriteLine(result.ErrorReason); 
            }
        }
    }

    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
}