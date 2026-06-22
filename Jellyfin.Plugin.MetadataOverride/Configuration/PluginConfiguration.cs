using System;
using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.MetadataOverride.Configuration;

/// <summary>
/// Plugin configuration. Jellyfin serializes this to XML on disk, so the settings and
/// captured snapshots survive restarts and updates.
/// </summary>
public class PluginConfiguration : BasePluginConfiguration
{
    /// <summary>Default protected fields for all movies.</summary>
    public FieldSet MovieFields { get; set; } = new FieldSet();

    /// <summary>Default protected fields for all series AND their episodes.</summary>
    public FieldSet SeriesFields { get; set; } = new FieldSet();

    /// <summary>Default protected fields for all collections.</summary>
    public FieldSet CollectionFields { get; set; } = new FieldSet();

    /// <summary>Individual items that deviate from the type defaults.</summary>
    public ExceptionEntry[] Exceptions { get; set; } = Array.Empty<ExceptionEntry>();

    /// <summary>
    /// The captured snapshots. Filled by the "Capture snapshot now" button.
    /// The metadata provider re-applies these after every refresh.
    /// </summary>
    public OverrideEntry[] Overrides { get; set; } = Array.Empty<OverrideEntry>();
}

/// <summary>Which fields are protected.</summary>
public class FieldSet
{
    public bool Title { get; set; }

    public bool SortTitle { get; set; }

    public bool Overview { get; set; }

    public bool Genres { get; set; }

    public bool Tags { get; set; }
}

/// <summary>An individual item that uses its own field selection instead of the type default.</summary>
public class ExceptionEntry
{
    public string TmdbId { get; set; } = string.Empty;

    public string ImdbId { get; set; } = string.Empty;

    public string ItemId { get; set; } = string.Empty;

    public string Label { get; set; } = string.Empty;

    public FieldSet Fields { get; set; } = new FieldSet();
}

/// <summary>
/// A captured snapshot for a single item. Empty text fields / empty lists are NOT applied.
/// Matched by TMDB id, then IMDB id, then the Jellyfin item id (used for collections).
/// </summary>
public class OverrideEntry
{
    public string TmdbId { get; set; } = string.Empty;

    public string ImdbId { get; set; } = string.Empty;

    public string ItemId { get; set; } = string.Empty;

    public string Label { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string SortTitle { get; set; } = string.Empty;

    public string Overview { get; set; } = string.Empty;

    public string[] Genres { get; set; } = Array.Empty<string>();

    public string[] Tags { get; set; } = Array.Empty<string>();
}
