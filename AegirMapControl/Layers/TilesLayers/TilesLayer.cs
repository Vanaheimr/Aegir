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

        private readonly ConcurrentStack<Image> TilesOnMap;

        private Object LockObject;

        private UInt64 CurrentVersion = 0;

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

            this.LockObject         = new Object();
            this.Background         = new SolidColorBrush(Colors.Transparent);
            this.TilesOnMap         = new ConcurrentStack<Image>();

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

        /// <summary>
        /// Paints the map.
        /// </summary>
        /// <returns>True if the map was repainted; false otherwise.</returns>
        public override Boolean Redraw()
        {

            if (this.IsVisible && !DesignerProperties.GetIsInDesignMode(this))
            {

                if (Monitor.TryEnter(LockObject))
                {

                    CurrentVersion++;

                    Debug.WriteLine(Thread.CurrentThread.ManagedThreadId);

                    #region Collect old tiles for deletion

                    var OldTilesToDelete = TilesOnMap.ToArray();
                    TilesOnMap.Clear();

                    #endregion

                    #region Paint new map as background task

                    //Task.Factory.StartNew(() =>
                    //{

                        var _NumberOfXTiles = (Int32)Math.Floor(base.ActualWidth / 256);
                        var _NumberOfYTiles = (Int32)Math.Floor(base.ActualHeight / 256);
                        var _NumberOfTiles  = (Int32)Math.Pow(2, this.MapControl.ZoomLevel);
                        var ___x = (Int32)this.MapControl.ScreenOffsetX % (_NumberOfTiles * 256) / 256;
                        var ___y = (Int32)this.MapControl.ScreenOffsetY % (_NumberOfTiles * 256) / 256;
                        var ListOfTasks = new List<Task>();

                        for (var _x = -1; _x < _NumberOfXTiles + 2; _x++)
                        {

                            Int32 _ActualXTile;
                            Int32 _ActualYTile;

                            _ActualXTile = ((_x - ___x) % _NumberOfTiles);
                            if (_ActualXTile < 0) _ActualXTile += _NumberOfTiles;

                            for (var _y = -1; _y < _NumberOfYTiles + 2; _y++)
                            {

                                _ActualYTile = (Int32)((_y - ___y) % _NumberOfTiles);
                                if (_ActualYTile < 0) _ActualYTile += _NumberOfTiles;

                                ListOfTasks.Add(TileClient.GetTile(this.MapControl.ZoomLevel,
                                                                   (UInt32)_ActualXTile,
                                                                   (UInt32)_ActualYTile,
                                                                   new Tuple<Int64, Int64, UInt64>(
                                                                       this.MapControl.ScreenOffsetX % 256 + _x * 256,
                                                                       this.MapControl.ScreenOffsetY % 256 + _y * 256,
                                                                       CurrentVersion)).

                                    ContinueWith(TileTask => PaintTile(TileTask.Result.Item1,
                                                                        (TileTask.Result.Item2 as Tuple<Int64, Int64, UInt64>).Item1,
                                                                        (TileTask.Result.Item2 as Tuple<Int64, Int64, UInt64>).Item2,
                                                                        (TileTask.Result.Item2 as Tuple<Int64, Int64, UInt64>).Item3)));

                            }

                        }

                        Task.Factory.ContinueWhenAll(ListOfTasks.ToArray(), (jj) =>

                        this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                        {
                            foreach (var Image in OldTilesToDelete)
                                this.Children.Remove(Image);
                        })));

                   // });

                    #endregion

                    Monitor.Exit(LockObject);

                }

                return true;

            }

            return false;

        }

        #endregion



        private void PaintTile(Byte[] Tile, Int64 ScreenX, Int64 ScreenY, UInt64 PaintingVersion)
        {

            if (Tile == null || Tile.Length == 0)
                return;

            if (PaintingVersion < CurrentVersion)
                return;

            var _BitmapImage = new BitmapImage();
            _BitmapImage.BeginInit();
            _BitmapImage.CacheOption  = BitmapCacheOption.OnLoad;
            _BitmapImage.StreamSource = new MemoryStream(Tile);
            _BitmapImage.EndInit();
            _BitmapImage.Freeze(); // To allow access from UI thread!

            //ScreenX = 100;
            //ScreenY = 100;

            this.Dispatcher.Invoke(DispatcherPriority.Send, (Action<Object>)(_ImageSource =>
            {

                try
                {

                    var _Image = new Image() {
                      //  Stretch = Stretch.Uniform,
                        Source  = _ImageSource as ImageSource,
                        Width   = 256,
                        Height  = 256,//(_ImageSource as BitmapImage).PixelWidth
                        DataContext = PaintingVersion
                    };

                    //_Image
                    this.Children.Add(_Image);
                    TilesOnMap.Push(_Image);

                    Canvas.SetLeft(_Image, ScreenX);
                    Canvas.SetTop (_Image, ScreenY);

                }
                catch (NotSupportedException)
                {
                }

            }), _BitmapImage);

        }


    }

}
