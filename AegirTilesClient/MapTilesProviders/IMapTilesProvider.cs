using System;

namespace eu.Vanaheimr.Aegir.Tiles
{

    public interface IMapTilesProvider
    {

        string Copyright { get; }
        string Description { get; }
        byte[] GetTile(uint ZoomLevel, uint X, uint Y);
        global::System.IO.Stream GetTileStream(uint ZoomLevel, uint X, uint Y);
        global::System.Collections.Generic.IEnumerable<string> Hosts { get; }
        string Id { get; }
        string InfoUri { get; }
        bool IsMemoryCacheable { get; }
        bool MemoryCacheEnabled { get; set; }
        string UriPattern { get; }

    }

}
