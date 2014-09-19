/*
 * Copyright (c) 2011-2013 Achim 'ahzf' Friedland <achim@graph-database.org>
 * This file is part of Aegir <http://www.github.com/Vanaheimr/Aegir>
 * 
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 3 of the License, or
 * (at your option) any later version.
 * 
 * You may obtain a copy of the License at
 *   http://www.gnu.org/licenses/gpl.html
 * 
 * This program is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * General Public License for more details.
 */

#region Usings

using System;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using org.GraphDefined.Vanaheimr.Aegir.Tiles;
using org.GraphDefined.Vanaheimr.Aegir.Controls;

#endregion

namespace org.GraphDefined.Vanaheimr.Aegir
{

    /// <summary>
    /// A feature layer for visualizing a map based of tiles.
    /// </summary>
    public class TilesLayer : AMapLayer
    {

        #region Data

        private          Int64   GlobalVersionCounter = 0;
        private          Object  AutoTilesRefreshLock;
        private readonly Timer   TilesRefreshTimer;

        #endregion

        #region Properties

        #region TilesClient

        /// <summary>
        /// The TileClient to use for fetching the
        /// map tiles from the image providers.
        /// </summary>
        public AegirTilesClient TilesClient { get; private set; }

        #endregion

        #region CurrentMapProvider

        /// <summary>
        /// The map tiles provider for this tiles layer.
        /// </summary>
        public MapTilesProvider CurrentMapProvider
        {

            get
            {
                return this.TilesClient.CurrentProvider;
            }

            set
            {

                if (value != null)
                {

                    var OldMapProvider = this.TilesClient.CurrentProvider;

                    this.TilesClient.CurrentProvider = value;

                    this.Children.Clear();
                    Redraw();

                    if (MapProviderChanged != null)
                        MapProviderChanged(this, OldMapProvider, this.TilesClient.CurrentProvider);

                }

            }

        }

        #endregion

        #region CurrentMapProviderId

        /// <summary>
        /// The identification string of the map tiles
        /// provider for this tiles layer.
        /// </summary>
        public String CurrentMapProviderId
        {

            get
            {
                return this.TilesClient.CurrentProviderId;
            }

            set
            {

                if (value != null && value.Trim() != "")
                {

                    var OldMapProvider = this.TilesClient.CurrentProvider;

                    this.TilesClient.CurrentProviderId = value;

                    this.Children.Clear();
                    Redraw();

                    if (MapProviderChanged != null)
                        MapProviderChanged(this, OldMapProvider, this.TilesClient.CurrentProvider);

                }

            }

        }

        #endregion

        #region MapProviders

        /// <summary>
        /// Return an enumeration of all registered map provider identifications.
        /// </summary>
        public IEnumerable<MapTilesProvider> MapProviders
        {
            get
            {
                return TilesClient.Providers;
            }
        }

        #endregion

        #region MapProviderIds

        /// <summary>
        /// Return an enumeration of all registered map provider identifications.
        /// </summary>
        public IEnumerable<String> MapProviderIds
        {
            get
            {
                return TilesClient.ProviderIds;
            }
        }

        #endregion

        #endregion

        #region Events

        #region MapProviderChanged

        /// <summary>
        /// An event handler getting fired whenever the
        /// map provider of the map changed.
        /// </summary>
        public delegate void MapProviderChangedEventHandler(TilesLayer Sender, MapTilesProvider OldMapProvider, MapTilesProvider NewMapProvider);

        /// <summary>
        /// An event getting fired whenever the map provider
        /// of the map changed.
        /// </summary>
        public event MapProviderChangedEventHandler MapProviderChanged;

        #endregion

        #endregion

        #region Constructor(s)

        #region (private) TilesLayer(Id, MapControl, ZIndex)

        /// <summary>
        /// Creates a new feature layer for visualizing a map based of tiles.
        /// </summary>
        /// <param name="Id">The unique identification of the layer.</param>
        /// <param name="ZIndex">The Z-Index of the layer.</param>
        /// <param name="MapControl">The MapControl of the layer.</param>
        private TilesLayer(String      Id,
                           Int32       ZIndex,
                           MapControl  MapControl)

            : base(Id, MapControl, ZIndex)

        {

            // Do not react on mouse events!
            IsHitTestVisible      = false;
            TilesClient           = new AegirTilesClient();
            AutoTilesRefreshLock  = new Object();

        }

        #endregion

        #region TilesLayer(Id, MapControl, ZIndex)

        /// <summary>
        /// Creates a new feature layer for visualizing a map based of tiles.
        /// </summary>
        /// <param name="Id">The unique identification of the layer.</param>
        /// <param name="MapControl">The MapControl of the layer.</param>
        /// <param name="ZIndex">The Z-Index of the layer.</param>
        public TilesLayer(String Id,
                          MapControl  MapControl,
                          Int32       ZIndex)

            : this(Id, ZIndex, MapControl)

        {

            TilesRefreshTimer  = new Timer(TilesAutoRefresh, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(100));

        }

        #endregion

        #region TilesLayer(Id, MapControl, ZIndex, params MapTilesProviders)

        /// <summary>
        /// Creates a new feature layer for visualizing a map based of tiles.
        /// </summary>
        /// <param name="Id">The unique identification of the layer.</param>
        /// <param name="MapControl">The MapControl of the layer.</param>
        /// <param name="ZIndex">The Z-Index of the layer.</param>
        /// <param name="MapTilesProviders">A list of map providers. The first will be activated.</param>
        public TilesLayer(String                     Id,
                          MapControl                 MapControl,
                          Int32                      ZIndex,
                          params MapTilesProvider[]  MapTilesProviders)

            : this(Id, ZIndex, MapControl)

        {

            if (MapTilesProviders != null)
                foreach (var MapTilesProvider in MapTilesProviders)
                    TilesClient.Register(MapTilesProvider, Activate: false);

            // Mark the firt MapProvider as 'active'
            if (MapTilesProviders.Any())
                TilesClient.CurrentProvider = MapTilesProviders.First();

            TilesRefreshTimer  = new Timer(TilesAutoRefresh, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(100));

        }

        #endregion

        #endregion


        private Double Normalize(Double Value, Int32 Factor)
        {

            var _ActualXTile = Value % Factor;

            return (_ActualXTile < 0) ? _ActualXTile += Factor : _ActualXTile;

        }

        #region (override) Move(X, Y)

        /// <summary>
        /// Move tiles and delete those outside the visible canvas.
        /// </summary>
        /// <param name="X">X</param>
        /// <param name="Y">Y</param>
        public override void Move(Double X, Double Y)
        {

            var ToDelete = new List<Image>();
            var List     = new List<String>();

            foreach (Image Tile in this.Children)
            {

                var NewX = Canvas.GetLeft(Tile) + X;
                var NewY = Canvas.GetTop (Tile) + Y;

                List.Add(Canvas.GetLeft(Tile) + " / " + Canvas.GetTop(Tile));

                // Find all tiles outside the visible canvas
                if (NewX < -256 || NewX > this.ActualWidth ||
                    NewY < -256 || NewY > this.ActualHeight)
                    ToDelete.Add(Tile);

                else
                {
                    // Move tiles within the visible canvas
                    Canvas.SetLeft(Tile, NewX);
                    Canvas.SetTop (Tile, NewY);
                }

            }

            // Delete all tiles outside the visible canvas
            ToDelete.ForEach(Tile => this.Children.Remove(Tile));

            Redraw();

        }

        #endregion



        private void TilesAutoRefresh(Object State)
        {

            if (Monitor.TryEnter(AutoTilesRefreshLock))
            {

                var NumberOfXTiles            = (Int32) Math.Floor(base.ActualWidth  / 256) + 1;
                var NumberOfYTiles            = (Int32) Math.Floor(base.ActualHeight / 256) + 1;
                var NumberOfTilesAtZoomLevel  = (Int32) Math.Pow(2, this.MapControl.ZoomLevel);
                var LeftUpperTile             = new Point(this.MapControl.ScreenOffset.X % (NumberOfTilesAtZoomLevel * 256) / 256,
                                                          this.MapControl.ScreenOffset.Y % (NumberOfTilesAtZoomLevel * 256) / 256);

                var _NumberOfXTiles = (Normalize(LeftUpperTile.X, NumberOfTilesAtZoomLevel) == 0) ? NumberOfXTiles : NumberOfXTiles + 1;
                var _NumberOfYTiles = (Normalize(LeftUpperTile.Y, NumberOfTilesAtZoomLevel) == 0) ? NumberOfYTiles : NumberOfYTiles + 1;

                    this.Dispatcher.Invoke(DispatcherPriority.Send, (Action)(() => {
                        if (this.Children.Count != (_NumberOfXTiles * _NumberOfYTiles))
                            Redraw();
                    }));

                Monitor.Exit(AutoTilesRefreshLock);

            }

        }


        #region Redraw_old()

        /// <summary>
        /// Paints the tiles layer.
        /// </summary>
        /// <returns>True if the map was repainted; false otherwise.</returns>
        public override void Redraw()
        {

            if (this.IsVisible && !DesignerProperties.GetIsInDesignMode(this))
            {

                Interlocked.Increment(ref GlobalVersionCounter);

                var _NumberOfXTiles = 0;
                var _NumberOfYTiles = 0;

                var NumberOfXTiles            = (Int32) Math.Floor(base.ActualWidth  / 256) + 1;
                var NumberOfYTiles            = (Int32) Math.Floor(base.ActualHeight / 256) + 1;
                var NumberOfTilesAtZoomLevel  = (Int32) Math.Pow(2, this.MapControl.ZoomLevel);
                var LeftUpperTile             = new Point(this.MapControl.ScreenOffset.X % (NumberOfTilesAtZoomLevel * 256) / 256,
                                                          this.MapControl.ScreenOffset.Y % (NumberOfTilesAtZoomLevel * 256) / 256);

                var UselessTilesOnScreen = new List<Image>();

                foreach (Image Tile in this.Children)
                    UselessTilesOnScreen.Add(Tile);


                for (var CurrentX = 0; CurrentX < NumberOfXTiles + 1; CurrentX++)
                {

                    for (var CurrentY = 0; CurrentY < NumberOfYTiles + 1; CurrentY++)
                    {

                        var NewPosition = new Point(this.MapControl.ScreenOffset.X % 256 + CurrentX * 256,
                                                    this.MapControl.ScreenOffset.Y % 256 + CurrentY * 256);

                        #region Is this tile already on the screen?

                        var FoundAndHowOften = 0;

                        foreach (Image Tile in this.Children)
                        {

                            if (NewPosition.X == Canvas.GetLeft(Tile) &&
                                NewPosition.Y == Canvas.GetTop(Tile))
                            {
                                if (FoundAndHowOften == 0)
                                {
                                    FoundAndHowOften = 1;
                                    UselessTilesOnScreen.Remove(Tile);
                                }
                                else
                                    FoundAndHowOften++;
                            }

                        }

                        #endregion

                        if (FoundAndHowOften == 0)

                            TilesClient.GetTile(this.MapControl.ZoomLevel,
                                               (UInt32) Normalize(CurrentX - LeftUpperTile.X, NumberOfTilesAtZoomLevel),
                                               (UInt32) Normalize(CurrentY - LeftUpperTile.Y, NumberOfTilesAtZoomLevel),
                                               new Tuple<Point, Int64>(
                                                   NewPosition,
                                                   GlobalVersionCounter)).

                                ContinueWith(TileTask => PaintTile(TileTask.Result.Item1,
                                                                  (TileTask.Result.Item2 as Tuple<Point, Int64>).Item1,
                                                                  (TileTask.Result.Item2 as Tuple<Point, Int64>).Item2));

                    }

                }

                Debug.WriteLine("Deleting tiles: " + UselessTilesOnScreen.Count);

                foreach (var Tile in UselessTilesOnScreen)
                    this.Children.Remove(Tile);

                _NumberOfXTiles = (Normalize(LeftUpperTile.X, NumberOfTilesAtZoomLevel) == 0) ? NumberOfXTiles : NumberOfXTiles + 1;
                _NumberOfYTiles = (Normalize(LeftUpperTile.Y, NumberOfTilesAtZoomLevel) == 0) ? NumberOfYTiles : NumberOfYTiles + 1;

                Debug.WriteLine("Number of visible tiles: " + this.Children.Count + " (" + (_NumberOfXTiles * _NumberOfYTiles) + ")");

            }

//            return true;

        }

        #endregion

        #region (private) PaintTile(TileStream, Position, VersionCounter)

        private void PaintTile(MemoryStream TileStream, Point Position, Int64 VersionCounter)
        {

            if (TileStream == null || TileStream.Length == 0)
                return;

            if (this.GlobalVersionCounter-1 > VersionCounter)
                return;

            try
            {

                var TileBitmap = new BitmapImage();
                TileBitmap.BeginInit();
                TileBitmap.CacheOption  = BitmapCacheOption.OnLoad;
                TileBitmap.StreamSource = TileStream;
                TileBitmap.EndInit();
                TileBitmap.Freeze(); // To allow access from UI thread!

                this.Dispatcher.Invoke(DispatcherPriority.Send, (Action<Object>)(_ImageSource =>
                {

                    try
                    {

                        var TileImage = new Image() {
                            Source = _ImageSource as ImageSource,
                            Width  = 256,
                            Height = 256,
                        };

                        this.Children.Add(TileImage);

                        Canvas.SetLeft(TileImage, Position.X);
                        Canvas.SetTop (TileImage, Position.Y);

                    }
                    catch (NotSupportedException)
                    {
                    }

                }), TileBitmap);

            }
            catch (Exception e)
            {
                Debug.WriteLine("PaintTile: " + e.Message);
            }

        }

        #endregion

    }

}
