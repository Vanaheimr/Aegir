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
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.Concurrent;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using eu.Vanaheimr.Illias.Commons;
using eu.Vanaheimr.Aegir.Tiles;
using eu.Vanaheimr.Aegir.Controls;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;


#endregion

namespace eu.Vanaheimr.Aegir
{

    /// <summary>
    /// A feature layer for visualizing a map based of tiles.
    /// </summary>
    public class TilesLayer : AMapLayer
    {

        #region Data

        /// <summary>
        /// An internal collection of all reflected map providers.
        /// </summary>
//        private AutoDiscovery<IMapTilesProvider> MapProviders;

        private readonly List<Image> TilesOnMap;
        private Image[] VeryOldTilesToDelete = new Image[0];
        private Image[] OldTilesToDelete = new Image[0];

        private Object AutoTilesRefreshLock;

        private readonly Timer TilesRefreshTimer;

        #endregion

        #region Properties

        #region TileClient

        /// <summary>
        /// The TileClient to use for fetching the
        /// map tiles from the image providers.
        /// </summary>
        public AegirTilesClient TileClient { get; set; }

        #endregion

        #region MapProvider

        /// <summary>
        /// The map tiles provider for this map.
        /// </summary>
        public String MapProvider
        {

            get
            {
                return this.TileClient.CurrentProviderId;
            }

            set
            {

                if (value != null && value != "")
                {

                    var OldMapProvider = this.TileClient.CurrentProviderId;

                    this.TileClient.CurrentProviderId = value;

                    this.Children.Clear();
                    Redraw();

                    if (MapProviderChanged != null)
                        MapProviderChanged(this, OldMapProvider, this.TileClient.CurrentProviderId);

                }

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
        public delegate void MapProviderChangedEventHandler(TilesLayer Sender, String OldMapProvider, String NewMapProvider);

        /// <summary>
        /// An event getting fired whenever the map provider
        /// of the map changed.
        /// </summary>
        public event MapProviderChangedEventHandler MapProviderChanged;

        #endregion

        #endregion

        #region Constructor(s)

        #region TilesLayer(Id, MapControl, ZIndex)

        /// <summary>
        /// Creates a new feature layer for visualizing a map based of tiles.
        /// </summary>
        public TilesLayer(String Id, MapControl MapControl, Int32 ZIndex)
            : base(Id, MapControl, ZIndex)
        {

            this.AutoTilesRefreshLock         = new Object();
            this.Background         = new SolidColorBrush(Colors.Transparent);
            this.TilesOnMap         = new List<Image>();

            #region Register mouse events

            this.PreviewMouseMove           += MapControl.ProcessMouseMove;
            this.MouseLeftButtonDown        += MapControl.ProcessMouseLeftButtonDown;
            this.PreviewMouseLeftButtonDown += MapControl.ProcessMouseLeftDoubleClick;
            this.MouseWheel                 += MapControl.ProcessMouseWheel;

            #endregion

            this.SizeChanged += (s, o) => Redraw();

            #region Find map providers and add context menu

            this.ContextMenu = new ContextMenu();

            // Find map providers via reflection
            //MapProviders = 
            //    //new AutoDiscovery<IMapTilesProvider>(Autostart: true,
            //    //                                                IdentificatorFunc: (MapProviderClass) => MapProviderClass.Id);

            //// Add all map providers to the mapping canvas context menu
            //foreach (var _MapProvider in MapProviders.RegisteredNames)
            //{

            //    var _MapProviderMenuItem = new MenuItem()
            //    {
            //        Header = _MapProvider,
            //        HeaderStringFormat = _MapProvider,
            //        IsCheckable = true
            //    };

            //    _MapProviderMenuItem.Click += new RoutedEventHandler(ChangeMapProvider);

            //    this.ContextMenu.Items.Add(_MapProviderMenuItem);

            //}

            //ChangeMapProvider(MapProvider);

            #endregion


            this.TileClient = new AegirTilesClient();
            this.TileClient.Register(new OSMProvider());

            //this.Loaded += (s, o) => { PaintTiles(); PaintTiles(); };

            this.TilesRefreshTimer = new Timer(TilesAutoRefresh, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(100));

        }

        #endregion

        #endregion


        #region (private) ChangeMapProvider(Sender, RoutedEventArgs)

        /// <summary>
        /// Changes the current map provider.
        /// </summary>
        /// <param name="Sender">The sender of the event.</param>
        /// <param name="RoutedEventArgs">The event arguments.</param>
        private void ChangeMapProvider(Object Sender, RoutedEventArgs RoutedEventArgs)
        {

            var MenuItem = Sender as MenuItem;

            if (MenuItem != null)
                ChangeMapProvider(MenuItem.HeaderStringFormat);

        }

        #endregion

        #region (private) ChangeMapProvider(MapProviderName)

        /// <summary>
        /// Changes the current map provider.
        /// </summary>
        /// <param name="MapProviderName">The well-known name of the map provider.</param>
        private void ChangeMapProvider(String MapProviderName)
        {

            var OldMapProvider = MapProvider;

            if (MapProviderName != null && MapProviderName != "")
            {

                foreach (var Item in this.ContextMenu.Items)
                {

                    var MenuItem = Item as MenuItem;

                    if (MenuItem != null)
                        MenuItem.IsChecked = (MenuItem.HeaderStringFormat == MapProviderName);

                }

                MapProvider = MapProviderName;

            }

        }

        #endregion


        #region RedrawLayer()

        public Boolean lala_old()
        {

            if (this.IsVisible && !DesignerProperties.GetIsInDesignMode(this))
            {

                if (Monitor.TryEnter(AutoTilesRefreshLock))
                {

                    try
                    {

                        Debug.WriteLine(Thread.CurrentThread.ManagedThreadId + "-" + this.Children.Count);

                        #region Collect old tiles for deletion

                        //VeryOldTilesToDelete = OldTilesToDelete;
                        //OldTilesToDelete = TilesOnMap.ToArray();
                        //TilesOnMap.Clear();

                        //this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                        //    {
                        //        this.Children.Clear();
                        //    }));

                        #endregion

                        #region Paint new map as background task

                        //Task.Factory.StartNew(() =>
                        //{

                        var _NumberOfXTiles = (Int32) Math.Floor(base.ActualWidth / 256) + 1;
                        var _NumberOfYTiles = (Int32) Math.Floor(base.ActualHeight / 256) + 1;
                        var _NumberOfTiles  = (Int32) Math.Pow(2, this.MapControl.ZoomLevel);
                        var ___x = (Int32) this.MapControl.ScreenOffsetX % (_NumberOfTiles * 256) / 256;
                        var ___y = (Int32) this.MapControl.ScreenOffsetY % (_NumberOfTiles * 256) / 256;
                        var ListOfTasks = new List<Task>();
                  //      this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() => TilesOnMap.Clear()));

                        for (var _x = 0; _x < _NumberOfXTiles + 1; _x++)
                        {

                            Int32 _ActualXTile;
                            Int32 _ActualYTile;

                            _ActualXTile = ((_x - ___x) % _NumberOfTiles);
                            if (_ActualXTile < 0) _ActualXTile += _NumberOfTiles;

                            for (var _y = 0; _y < _NumberOfYTiles + 1; _y++)
                            {

                                _ActualYTile = (Int32)((_y - ___y) % _NumberOfTiles);
                                if (_ActualYTile < 0) _ActualYTile += _NumberOfTiles;

                                ListOfTasks.Add(TileClient.GetTile(this.MapControl.ZoomLevel,
                                                                   (UInt32)_ActualXTile,
                                                                   (UInt32)_ActualYTile,
                                                                   new Tuple<Int64, Int64>(
                                                                       this.MapControl.ScreenOffsetX % 256 + _x * 256,
                                                                       this.MapControl.ScreenOffsetY % 256 + _y * 256)).

                                    ContinueWith(TileTask => PaintTile(TileTask.Result.Item1,
                                                                      (TileTask.Result.Item2 as Tuple<Int64, Int64, UInt64>).Item1,
                                                                      (TileTask.Result.Item2 as Tuple<Int64, Int64, UInt64>).Item2)));

                            }

                        }

                        //Task.Factory.ContinueWhenAll(ListOfTasks.ToArray(), Tasks =>
                        //    this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                        //    {
                        //        var Listi = new List<Image>();
                        //        foreach (Image ImageChild in this.Children)
                        //            if (!TilesOnMap.Contains(ImageChild))
                        //                Listi.Add(ImageChild);

                        //        Listi.ForEach(ll => this.Children.Remove(ll));
                        //        //    this.Children.Remove(Image);
                        //    })));

                        // });

                        #endregion

                    }

                    finally
                    {
                        Monitor.Exit(AutoTilesRefreshLock);
                    }

                }

                return true;

            }

            return false;

        }

        #endregion


        private Double Normalize(Double Value, Int32 Factor)
        {

            var _ActualXTile = Value % Factor;

            return (_ActualXTile < 0) ? _ActualXTile += Factor : _ActualXTile;

        }

        public override void Move(Double X_Movement, Double Y_Movement)
        {

            #region Move tiles and delete those outside the visible canvas

            var ToDelete = new List<Image>();
            var List = new List<String>();

            foreach (Image Tile in this.Children)
            {

                var NewX = Canvas.GetLeft(Tile) + X_Movement;
                var NewY = Canvas.GetTop (Tile) + Y_Movement;

                List.Add(Canvas.GetLeft(Tile) + " / " + Canvas.GetTop(Tile));

                // Find all tiles outside the visible canvas
                if (NewX < -256 || NewX > this.ActualWidth ||
                    NewY < -256 || NewY > this.ActualHeight)
                    ToDelete.Add(Tile);

                else
                {
                    // Move tiles within the visible canvas
                    Canvas.SetLeft(Tile, NewX);
                    Canvas.SetTop(Tile, NewY);
                }

            }

            // Delete all tiles outside the visible canvas
            ToDelete.ForEach(Tile => this.Children.Remove(Tile));

            #endregion

            Redraw();

        }

        private void TilesAutoRefresh(Object State)
        {

            if (Monitor.TryEnter(AutoTilesRefreshLock))
            {

                var NumberOfXTiles            = (Int32) Math.Floor(base.ActualWidth  / 256) + 1;
                var NumberOfYTiles            = (Int32) Math.Floor(base.ActualHeight / 256) + 1;
                var NumberOfTilesAtZoomLevel  = (Int32) Math.Pow(2, this.MapControl.ZoomLevel);
                var LeftUpperTile             = new Point(this.MapControl.ScreenOffsetX % (NumberOfTilesAtZoomLevel * 256) / 256,
                                                          this.MapControl.ScreenOffsetY % (NumberOfTilesAtZoomLevel * 256) / 256);

                var _NumberOfXTiles = (Normalize(LeftUpperTile.X, NumberOfTilesAtZoomLevel) == 0) ? NumberOfXTiles : NumberOfXTiles + 1;
                var _NumberOfYTiles = (Normalize(LeftUpperTile.Y, NumberOfTilesAtZoomLevel) == 0) ? NumberOfYTiles : NumberOfYTiles + 1;

                    this.Dispatcher.Invoke(DispatcherPriority.Send, (Action)(() => {
                        if (this.Children.Count != (_NumberOfXTiles * _NumberOfYTiles))
                            Redraw();
                    }));

                Monitor.Exit(AutoTilesRefreshLock);

            }

        }


        #region Redraw()

        /// <summary>
        /// Paints the tiles layer.
        /// </summary>
        /// <returns>True if the map was repainted; false otherwise.</returns>
        public override Boolean Redraw()
        {

            if (this.IsVisible && !DesignerProperties.GetIsInDesignMode(this))
            {

                var _NumberOfXTiles = 0;
                var _NumberOfYTiles = 0;

                var NumberOfXTiles            = (Int32) Math.Floor(base.ActualWidth  / 256) + 1;
                var NumberOfYTiles            = (Int32) Math.Floor(base.ActualHeight / 256) + 1;
                var NumberOfTilesAtZoomLevel  = (Int32) Math.Pow(2, this.MapControl.ZoomLevel);
                var LeftUpperTile             = new Point(this.MapControl.ScreenOffsetX % (NumberOfTilesAtZoomLevel * 256) / 256,
                                                          this.MapControl.ScreenOffsetY % (NumberOfTilesAtZoomLevel * 256) / 256);

                var UselessTilesOnScreen = new List<Image>();

                foreach (Image Tile in this.Children)
                    UselessTilesOnScreen.Add(Tile);


                for (var CurrentX = 0; CurrentX < NumberOfXTiles + 1; CurrentX++)
                {

                    for (var CurrentY = 0; CurrentY < NumberOfYTiles + 1; CurrentY++)
                    {

                        var NewX = this.MapControl.ScreenOffsetX % 256 + CurrentX * 256;
                        var NewY = this.MapControl.ScreenOffsetY % 256 + CurrentY * 256;

                        #region Is this tile already on the screen?

                        var FoundAndHowOften = 0;

                        foreach (Image Tile in this.Children)
                        {

                            if (NewX == Canvas.GetLeft(Tile) &&
                                NewY == Canvas.GetTop(Tile))
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

                            TileClient.GetTile(this.MapControl.ZoomLevel,
                                               (UInt32) Normalize(CurrentX - LeftUpperTile.X, NumberOfTilesAtZoomLevel),
                                               (UInt32) Normalize(CurrentY - LeftUpperTile.Y, NumberOfTilesAtZoomLevel),
                                               new Tuple<Int64, Int64>(
                                                   NewX,
                                                   NewY)).

                                ContinueWith(TileTask => PaintTile(TileTask.Result.Item1,
                                                                  (TileTask.Result.Item2 as Tuple<Int64, Int64>).Item1,
                                                                  (TileTask.Result.Item2 as Tuple<Int64, Int64>).Item2));

                    }

                }

                Debug.WriteLine("Deleting tiles: " + UselessTilesOnScreen.Count);

                foreach (var Tile in UselessTilesOnScreen)
                    this.Children.Remove(Tile);

                _NumberOfXTiles = (Normalize(LeftUpperTile.X, NumberOfTilesAtZoomLevel) == 0) ? NumberOfXTiles : NumberOfXTiles + 1;
                _NumberOfYTiles = (Normalize(LeftUpperTile.Y, NumberOfTilesAtZoomLevel) == 0) ? NumberOfYTiles : NumberOfYTiles + 1;

                Debug.WriteLine("Number of visible tiles: " + this.Children.Count + " (" + (_NumberOfXTiles * _NumberOfYTiles) + ")");

            }

            return true;

        }

        #endregion

        #region (private) PaintTile(TileStream, ScreenX, ScreenY)

        private void PaintTile(MemoryStream TileStream, Int64 ScreenX, Int64 ScreenY)
        {

            if (TileStream == null || TileStream.Length == 0)
                return;

            try
            {

                var _BitmapImage = new BitmapImage();
                _BitmapImage.BeginInit();
                _BitmapImage.CacheOption  = BitmapCacheOption.OnLoad;
                _BitmapImage.StreamSource = TileStream;
                _BitmapImage.EndInit();
                _BitmapImage.Freeze(); // To allow access from UI thread!

                this.Dispatcher.Invoke(DispatcherPriority.Send, (Action<Object>)(_ImageSource =>
                {

                    //Debug.WriteLine(Thread.CurrentThread.ManagedThreadId + "-paint->" + this.Children.Count);

                    try
                    {

                        var _Image = new Image() {
                            Source = _ImageSource as ImageSource,
                            Width  = 256,
                            Height = 256,
                        };

                        this.Children.Add(_Image);

                        Canvas.SetLeft(_Image, ScreenX);
                        Canvas.SetTop(_Image, ScreenY);

                    }
                    catch (NotSupportedException)
                    {
                    }

                }), _BitmapImage);

            }
            catch (Exception e)
            {
                Debug.WriteLine("PaintTile: " + e.Message);
            }

        }

        #endregion


    }

}
