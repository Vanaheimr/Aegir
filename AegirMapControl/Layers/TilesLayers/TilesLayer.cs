/*
 * Copyright (c) 2010-2012, Achim 'ahzf' Friedland <code@ahzf.de>
 * This file is part of Aegir <http://www.github.com/Vanaheimr/Aegir>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
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

using de.ahzf.Vanaheimr.Aegir.Tiles;
using de.ahzf.Illias.Commons;
using de.ahzf.Vanaheimr.Aegir.Controls;


#endregion

namespace de.ahzf.Vanaheimr.Aegir
{

    /// <summary>
    /// A feature layer for visualizing a map based of tiles.
    /// </summary>
    public class TilesLayer : ALayer
    {

        #region Data

        /// <summary>
        /// An internal collection of all reflected map providers.
        /// </summary>
        private AutoDiscovery<IMapProvider> MapProviders;

        private readonly ConcurrentStack<Image> TilesOnMap;

        #endregion

        #region Properties

        #region TileClient

        /// <summary>
        /// The TileClient to use for fetching the
        /// map tiles from the image providers.
        /// </summary>
        public ITileClient TileClient { get; set; }

        #endregion

        #region MapProvider

        private String CurrentMapProvider;

        /// <summary>
        /// The map tiles provider for this map.
        /// </summary>
        public String MapProvider
        {
            
            get
            {
                return CurrentMapProvider;
            }

            set
            {

                if (value != null && value != "")
                {

                    var OldMapProvider = CurrentMapProvider;

                    CurrentMapProvider = value;

                    Redraw();

                    if (MapProviderChanged != null)
                        MapProviderChanged(this, OldMapProvider, CurrentMapProvider);

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

        #region TilesLayer(Id, ZoomLevel, ScreenOffsetX, ScreenOffsetY, MapControl, ZIndex)

        /// <summary>
        /// Creates a new feature layer for visualizing a map based of tiles.
        /// </summary>
        public TilesLayer(String Id, UInt32 ZoomLevel, Int64 ScreenOffsetX, Int64 ScreenOffsetY, MapControl MapControl, Int32 ZIndex)
            : base(Id, ZoomLevel, ScreenOffsetX, ScreenOffsetY, MapControl, ZIndex)
        {

            this.Background         = new SolidColorBrush(Colors.Transparent);
            this.TilesOnMap         = new ConcurrentStack<Image>();
            this.CurrentMapProvider = de.ahzf.Vanaheimr.Aegir.Tiles.ArcGIS_WorldImagery_Provider.Name;

            #region Register mouse events

            this.PreviewMouseMove           += MapControl.ProcessMouseMove;
            this.MouseLeftButtonDown        += MapControl.ProcessMouseLeftButtonDown;
            this.PreviewMouseLeftButtonDown += MapControl.ProcessMouseLeftDoubleClick;
            this.MouseWheel                 += MapControl.ProcessMouseWheel;

            #endregion

            #region Find map providers and add context menu

            this.ContextMenu = new ContextMenu();

            // Find map providers via reflection
            MapProviders = new AutoDiscovery<IMapProvider>(Autostart: true,
                                                           IdentificatorFunc: (MapProviderClass) => MapProviderClass.Name);

            // Add all map providers to the mapping canvas context menu
            foreach (var _MapProvider in MapProviders.RegisteredNames)
            {

                var _MapProviderMenuItem = new MenuItem()
                {
                    Header = _MapProvider,
                    HeaderStringFormat = _MapProvider,
                    IsCheckable = true
                };

                _MapProviderMenuItem.Click += new RoutedEventHandler(ChangeMapProvider);

                this.ContextMenu.Items.Add(_MapProviderMenuItem);

            }

            ChangeMapProvider(MapProvider);

            #endregion

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

            if (this.IsVisible && !IsCurrentlyPainting)
            {

                IsCurrentlyPainting = true;

                if (!DesignerProperties.GetIsInDesignMode(this))
                {

                    if (this.TileClient == null)
                        this.TileClient = new TileServer();

                    #region Collect old tiles for deletion

                    var OldTilesToDelete = TilesOnMap.ToArray();
                    TilesOnMap.Clear();

                    #endregion

                    #region Paint new map as background task

                    Task.Factory.StartNew(() => {

                        var _NumberOfXTiles = (Int32) Math.Floor(base.ActualWidth  / 256);
                        var _NumberOfYTiles = (Int32) Math.Floor(base.ActualHeight / 256);
                        var _NumberOfTiles  = (Int32) Math.Pow(2, ZoomLevel);
                        var ___x = (Int32) ScreenOffsetX % (_NumberOfTiles * 256) / 256;
                        var ___y = (Int32) ScreenOffsetY % (_NumberOfTiles * 256) / 256;

                        Parallel.For(-1, _NumberOfXTiles + 2, _x =>
                        {

                            Int32 _ActualXTile;
                            Int32 _ActualYTile;

                            _ActualXTile = ((_x - ___x) % _NumberOfTiles);
                            if (_ActualXTile < 0) _ActualXTile += _NumberOfTiles;

                            Parallel.For(-1, _NumberOfYTiles + 2, _y => 
                            {

                                _ActualYTile = (Int32) ((_y - ___y) % _NumberOfTiles);
                                if (_ActualYTile < 0) _ActualYTile += _NumberOfTiles;

                                var _TileStream = TileClient.GetTileStream(MapProvider, ZoomLevel, (UInt32) _ActualXTile, (UInt32) _ActualYTile);

                                this.Dispatcher.Invoke(DispatcherPriority.Send, (Action<Object>)((_TileStream2) =>
                                {

                                    var _BitmapImage = new BitmapImage();
                                    _BitmapImage.BeginInit();
                                    _BitmapImage.CacheOption  = BitmapCacheOption.OnLoad;
                                    _BitmapImage.StreamSource = (Stream) _TileStream;
                                    _BitmapImage.EndInit();
                                    _BitmapImage.Freeze();

                                    var _Image = new Image()
                                    {
                                        Stretch = Stretch.Uniform,
                                        Source  = _BitmapImage,
                                        Width   = _BitmapImage.PixelWidth
                                    };

                                    this.Children.Add(_Image);
                                    TilesOnMap.Push(_Image);
                            
                                    Canvas.SetLeft(_Image, ScreenOffsetX % 256 + _x * 256);
                                    Canvas.SetTop (_Image, ScreenOffsetY % 256 + _y * 256);

                                }), _TileStream);

                            });

                        });

                        this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() => {
                            foreach (var Image in OldTilesToDelete)
                                this.Children.Remove(Image);
                            }));

                    });

                    #endregion

                }

                IsCurrentlyPainting = false;

                return true;

            }

            return false;

        }

        #endregion


    }

}
