using System;
using System.Collections.Generic;
using System.Globalization;
using Jellyfin.Plugin.MetadataOverride.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace Jellyfin.Plugin.MetadataOverride;

/// <summary>
/// Das Haupt-Plugin. Haelt die Konfiguration (deine Overrides) und stellt die Einstellungsseite bereit.
/// </summary>
public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
{
    public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
        : base(applicationPaths, xmlSerializer)
    {
        Instance = this;
    }

    /// <summary>
    /// Statischer Zugriff auf die laufende Plugin-Instanz (damit der Provider an die Overrides kommt).
    /// </summary>
    public static Plugin? Instance { get; private set; }

    public override string Name => "Metadata Override";

    public override Guid Id => Guid.Parse("7f3b2a14-9c6d-4e8f-b1a2-3c4d5e6f7a8b");

    public IEnumerable<PluginPageInfo> GetPages()
    {
        return new[]
        {
            new PluginPageInfo
            {
                Name = Name,
                EmbeddedResourcePath = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}.Configuration.configPage.html",
                    GetType().Namespace)
            }
        };
    }
}
