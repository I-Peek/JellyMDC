using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.MetadataOverride.Configuration;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.MetadataOverride;

/// <summary>
/// Shared logic: looks up the captured snapshot for an item and re-stamps the saved values
/// over the freshly loaded data. Matching runs over TMDB-/IMDB-ID (survives renames) and,
/// as a fallback, over the Jellyfin item id (used mainly for collections).
/// </summary>
internal static class OverrideApplier
{
    public static ItemUpdateType Apply(BaseItem item, ILogger logger)
    {
        var config = Plugin.Instance?.Configuration;
        if (config?.Overrides == null || config.Overrides.Length == 0)
        {
            return ItemUpdateType.None;
        }

        var tmdbId = item.GetProviderId(MetadataProvider.Tmdb);
        var imdbId = item.GetProviderId(MetadataProvider.Imdb);
        var itemId = item.Id.ToString("N");

        var entry = config.Overrides.FirstOrDefault(o =>
            (!string.IsNullOrWhiteSpace(tmdbId) && string.Equals(o.TmdbId, tmdbId, StringComparison.OrdinalIgnoreCase))
            || (!string.IsNullOrWhiteSpace(imdbId) && string.Equals(o.ImdbId, imdbId, StringComparison.OrdinalIgnoreCase))
            || (!string.IsNullOrWhiteSpace(o.ItemId) && string.Equals(o.ItemId, itemId, StringComparison.OrdinalIgnoreCase)));

        if (entry is null)
        {
            return ItemUpdateType.None;
        }

        var changed = false;

        if (!string.IsNullOrWhiteSpace(entry.Title) && item.Name != entry.Title)
        {
            item.Name = entry.Title;
            changed = true;
        }

        if (!string.IsNullOrWhiteSpace(entry.SortTitle) && item.ForcedSortName != entry.SortTitle)
        {
            item.ForcedSortName = entry.SortTitle;
            changed = true;
        }

        if (!string.IsNullOrWhiteSpace(entry.Overview) && item.Overview != entry.Overview)
        {
            item.Overview = entry.Overview;
            changed = true;
        }

        if (entry.Genres.Length > 0 && !entry.Genres.SequenceEqual(item.Genres))
        {
            item.Genres = entry.Genres.ToArray();
            changed = true;
        }

        if (entry.Tags.Length > 0 && !entry.Tags.SequenceEqual(item.Tags))
        {
            item.Tags = entry.Tags.ToArray();
            changed = true;
        }

        if (changed)
        {
            logger.LogInformation("JellyMDC applied override to \"{Item}\"", item.Name);
            return ItemUpdateType.MetadataEdit;
        }

        return ItemUpdateType.None;
    }
}

/// <summary>Re-applies snapshots to movies. Runs late so our values win.</summary>
public class MovieOverrideProvider : ICustomMetadataProvider<Movie>, IHasOrder
{
    private readonly ILogger<MovieOverrideProvider> _logger;

    public MovieOverrideProvider(ILogger<MovieOverrideProvider> logger) => _logger = logger;

    public string Name => "JellyMDC";

    public int Order => 1000;

    public Task<ItemUpdateType> FetchAsync(Movie item, MetadataRefreshOptions options, CancellationToken cancellationToken)
        => Task.FromResult(OverrideApplier.Apply(item, _logger));
}

/// <summary>Re-applies snapshots to series.</summary>
public class SeriesOverrideProvider : ICustomMetadataProvider<Series>, IHasOrder
{
    private readonly ILogger<SeriesOverrideProvider> _logger;

    public SeriesOverrideProvider(ILogger<SeriesOverrideProvider> logger) => _logger = logger;

    public string Name => "JellyMDC";

    public int Order => 1000;

    public Task<ItemUpdateType> FetchAsync(Series item, MetadataRefreshOptions options, CancellationToken cancellationToken)
        => Task.FromResult(OverrideApplier.Apply(item, _logger));
}

/// <summary>Re-applies snapshots to episodes.</summary>
public class EpisodeOverrideProvider : ICustomMetadataProvider<Episode>, IHasOrder
{
    private readonly ILogger<EpisodeOverrideProvider> _logger;

    public EpisodeOverrideProvider(ILogger<EpisodeOverrideProvider> logger) => _logger = logger;

    public string Name => "JellyMDC";

    public int Order => 1000;

    public Task<ItemUpdateType> FetchAsync(Episode item, MetadataRefreshOptions options, CancellationToken cancellationToken)
        => Task.FromResult(OverrideApplier.Apply(item, _logger));
}

/// <summary>Re-applies snapshots to collections (box sets).</summary>
public class CollectionOverrideProvider : ICustomMetadataProvider<BoxSet>, IHasOrder
{
    private readonly ILogger<CollectionOverrideProvider> _logger;

    public CollectionOverrideProvider(ILogger<CollectionOverrideProvider> logger) => _logger = logger;

    public string Name => "JellyMDC";

    public int Order => 1000;

    public Task<ItemUpdateType> FetchAsync(BoxSet item, MetadataRefreshOptions options, CancellationToken cancellationToken)
        => Task.FromResult(OverrideApplier.Apply(item, _logger));
}
