using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.MetadataOverride.Configuration;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.MetadataOverride;

/// <summary>
/// Laeuft als Teil des Metadaten-Refreshs und schreibt deine Overrides als Letztes ueber die
/// frisch geladenen Daten. Dadurch gewinnen deine Werte zuverlaessig - auch nach "alles ersetzen"
/// oder nach einem Umbenennen der Datei, weil die Zuordnung ueber die TMDB-/IMDB-ID laeuft
/// und nicht ueber den Dateinamen.
/// </summary>
public class OverrideMetadataProvider : ICustomMetadataProvider<Movie>, IHasOrder
{
    private readonly ILogger<OverrideMetadataProvider> _logger;

    public OverrideMetadataProvider(ILogger<OverrideMetadataProvider> logger)
    {
        _logger = logger;
    }

    public string Name => "JellyMDC";

    // Hoher Wert => laeuft nach den normalen Fetchern, damit unsere Werte das letzte Wort haben.
    public int Order => 1000;

    public Task<ItemUpdateType> FetchAsync(Movie item, MetadataRefreshOptions options, CancellationToken cancellationToken)
    {
        var config = Plugin.Instance?.Configuration;
        if (config is null || config.Overrides.Length == 0)
        {
            return Task.FromResult(ItemUpdateType.None);
        }

        var tmdbId = item.GetProviderId(MetadataProvider.Tmdb);
        var imdbId = item.GetProviderId(MetadataProvider.Imdb);

        var entry = config.Overrides.FirstOrDefault(o =>
            (!string.IsNullOrWhiteSpace(tmdbId) && string.Equals(o.TmdbId, tmdbId, StringComparison.OrdinalIgnoreCase))
            || (!string.IsNullOrWhiteSpace(imdbId) && string.Equals(o.ImdbId, imdbId, StringComparison.OrdinalIgnoreCase)));

        if (entry is null)
        {
            return Task.FromResult(ItemUpdateType.None);
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
            _logger.LogInformation(
                "Override angewendet auf \"{Movie}\" (TMDB {Tmdb}, IMDB {Imdb})",
                item.Name,
                string.IsNullOrEmpty(tmdbId) ? "-" : tmdbId,
                string.IsNullOrEmpty(imdbId) ? "-" : imdbId);

            return Task.FromResult(ItemUpdateType.MetadataEdit);
        }

        return Task.FromResult(ItemUpdateType.None);
    }
}
