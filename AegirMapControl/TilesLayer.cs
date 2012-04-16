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

using de.Vanaheimr.Aegir.Tiles;
using de.ahzf.Vanaheimr.Aegir;

#endregion

namespace de.Vanaheimr.Aegir
{

    /// <summary>
    /// A feature layer for visualizing a map based of tiles.
    /// </summary>
    public class TilesLayer : AFeatureLayer
    {

        #region Data

        private readonly ConcurrentStack<Image> TilesOnMap;

        #endregion

        #region Properties

        #region TileServer

        /// <summary>
        /// The TileServer to use for fetching the
        /// map tiles from the image providers.
        /// </summary>
        public TileServer TileServer { get; set; }

        #endregion

        #region MapProvider

        private String _MapProvider;

        /// <summary>
        /// The map tiles provider for this map.
        /// </summary>
        public String MapProvider
        {
            
            get
            {
                return _MapProvider;
            }

            set
            {

                if (value != null && value != "")
                {

                    var OldMapProvider = _MapProvider;

                    _MapProvider = value;

                    Redraw();

                    if (MapProviderChanged != null)
                        MapProviderChanged(this, OldMapProvider, _MapProvider);

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

        #region TilesLayer()

        /// <summary>
        /// Creates a new feature layer for visualizing a map based of tiles.
        /// </summary>
        public TilesLayer()
        {
            this.Background     = new SolidColorBrush(Colors.Transparent);
            this._MapProvider   = de.Vanaheimr.Aegir.Tiles.ArcGIS_WorldImagery_Provider.Name;
            this.SizeChanged   += ProcessMapSizeChangedEvent;
            this.TilesOnMap     = new ConcurrentStack<Image>();
        }

        #endregion

        #endregion


        #region (private) ProcessMapSizeChangedEvent(Sender, SizeChangedEventArgs)

        /// <summary>
        /// Whenever the size of the map canvas was changed
        /// this method will be called.
        /// </summary>
        /// <param name="Sender">The sender of the event.</param>
        /// <param name="SizeChangedEventArgs">The event arguments.</param>
        private void ProcessMapSizeChangedEvent(Object Sender, SizeChangedEventArgs SizeChangedEventArgs)
        {
            Redraw();
        }

        #endregion


        #region RedrawLayer()

        /// <summary>
        /// Paints the map.
        /// </summary>
        /// <returns>True if the map was repainted; false otherwise.</returns>
        public override Boolean Redraw()
        {

            if (!IsCurrentlyPainting)
            {

                IsCurrentlyPainting = true;

                if (!DesignerProperties.GetIsInDesignMode(this))
                {

                    if (this.TileServer == null)
                        this.TileServer = new TileServer();

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

                                var _TileStream = TileServer.GetTileStream(MapProvider, ZoomLevel, (UInt32) _ActualXTile, (UInt32) _ActualYTile);

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




        #region ProcessMouseLeftButtonDown

        public void ProcessMouseLeftButtonDown(Object Sender, MouseButtonEventArgs MouseButtonEventArgs)
        {

            // We'll need this for when the Form starts to move
            var MousePosition = MouseButtonEventArgs.GetPosition(this);
            //LastClickPositionX = MousePosition.X;
            //LastClickPositionY = MousePosition.Y;

            //DrawingOffset_AtMovementStart_X = DrawingOffsetX;
            //DrawingOffset_AtMovementStart_Y = DrawingOffsetY;

        }

        #endregion



        public override Feature AddFeature(string Name, double Latitude, double Longitude, double Width, double Height, Color Color)
        {
            throw new NotImplementedException();
        }

    }

}
