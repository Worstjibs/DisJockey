﻿using Discord;
using Discord.Interactions;
using DisJockey.BotService.Services;

namespace DisJockey.BotService.Modules;

public class MusicModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IMusicService _musicService;

    public MusicModule(IMusicService musicService)
    {
        _musicService = musicService;
    }

    [SlashCommand("play", description: "Plays music", runMode: RunMode.Async)]
    public async Task Play(string query, [MinLength(0)] SearchMode? searchMode = null) =>
        await DeferActionAsync(async () => await _musicService.PlayTrackAsync(query, Context, searchMode));

    [SlashCommand("stop", description: "Stops music", runMode: RunMode.Async)]
    public async Task Stop() => await DeferActionAsync(async () => await _musicService.StopAsync(Context).ConfigureAwait(false));

    [SlashCommand("skip", description: "Skips the current track", runMode: RunMode.Async)]
    public async Task Skip() => await DeferActionAsync(async () => await _musicService.SkipAsync(Context).ConfigureAwait(false));

    [SlashCommand("pull-it", description: "If it's nice, play it twice", runMode: RunMode.Async)]
    public async Task PullIt() => await DeferActionAsync(async () => await _musicService.PullUpTrackAsync(Context).ConfigureAwait(false));

    private async Task DeferActionAsync(Func<Task> deferredAction)
    {
        await DeferAsync().ConfigureAwait(false);

        await deferredAction().ConfigureAwait(false);
    }
}
