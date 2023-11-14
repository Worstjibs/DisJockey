﻿using Discord;
using DisJockey.BotService.Modules;

namespace DisJockey.BotService.Services;

public interface IMusicService
{
    Task LoadPullUpSound();
    Task PlayTrackAsync(string query, IInteractionContext context, SearchMode? searchMode = null);
    Task StopAsync(IInteractionContext context);
    Task SkipAsync(IInteractionContext context);
    Task PullUpTrackAsync(IInteractionContext context);
}