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
/// Main plugin. Holds the configuration and exposes the settings page,
/// including a quick-access entry in the dashboard sidebar.
/// </summary>
public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
{
    public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
        : base(applicationPaths, xmlSerializer)
    {
        Instance = this;
    }

    /// <summary>Static access to the running plugin instance (so the providers can reach the config).</summary>
    public static Plugin? Instance { get; private set; }

    public override string Name => "JellyMDC";

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
                    GetType().Namespace),

                // Shows JellyMDC as its own entry in the dashboard sidebar under "Plugins".
                EnableInMainMenu = true,
                DisplayName = "JellyMDC",
                MenuIcon = "save"
            }
        };
    }
}
