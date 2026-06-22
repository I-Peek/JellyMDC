using System;
using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.MetadataOverride.Configuration;

/// <summary>
/// Die Plugin-Konfiguration. Jellyfin serialisiert sie automatisch nach XML auf die Platte,
/// sodass deine Overrides Neustarts und Updates ueberstehen.
/// </summary>
public class PluginConfiguration : BasePluginConfiguration
{
    public OverrideEntry[] Overrides { get; set; } = Array.Empty<OverrideEntry>();
}

/// <summary>
/// Ein einzelner Override fuer genau einen Film.
/// Leere Textfelder bzw. leere Listen werden NICHT angewendet (Jellyfins Originalwert bleibt dann stehen).
/// </summary>
public class OverrideEntry
{
    /// <summary>TMDB-ID des Films (z. B. 27205). Mindestens eine ID muss gesetzt sein.</summary>
    public string TmdbId { get; set; } = string.Empty;

    /// <summary>IMDB-ID des Films (z. B. tt1375666). Alternative zur TMDB-ID.</summary>
    public string ImdbId { get; set; } = string.Empty;

    /// <summary>Freie Notiz, nur fuer die Anzeige in der Liste (z. B. der Originaltitel).</summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>Ueberschreibt den Titel (Name).</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Ueberschreibt den Sortiertitel (ForcedSortName).</summary>
    public string SortTitle { get; set; } = string.Empty;

    /// <summary>Ueberschreibt die Beschreibung (Overview).</summary>
    public string Overview { get; set; } = string.Empty;

    /// <summary>Ersetzt die Genres komplett (wenn die Liste nicht leer ist).</summary>
    public string[] Genres { get; set; } = Array.Empty<string>();

    /// <summary>Ersetzt die Tags komplett (wenn die Liste nicht leer ist).</summary>
    public string[] Tags { get; set; } = Array.Empty<string>();
}
