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

#endregion

namespace de.Vanaheimr.Aegir
{

    /// <summary>
    /// A canvas for visualizing a map.
    /// </summary>
    public class AegirMapCanvas : Canvas
    {

        #region Data

        private UInt32 MapMoves;

        private Int32  DrawingOffsetX;
        private Int32  DrawingOffsetY;
        private Int32  DrawingOffset_AtMovementStart_X;
        private Int32  DrawingOffset_AtMovementStart_Y;

        private Double LastClickPositionX;
        private Double LastClickPositionY;
        private Double LastMousePositionDuringMovementX;
        private Double LastMousePositionDuringMovementY;

        private const    UInt32  MinZoomLevel =  1;
        private const    UInt32  MaxZoomLevel = 23;

        private readonly ConcurrentStack<Image> TilesOnMap;
        private volatile Boolean                IsCurrentlyPainting;

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

                    PaintMap();

                    if (MapProviderChanged != null)
                        MapProviderChanged(this, OldMapProvider, _MapProvider);

                }

            }

        }

        #endregion

        #region ZoomLevel

        /// <summary>
        /// The zoom level of the map.
        /// </summary>
        public UInt32 ZoomLevel { get; set; }

        #endregion

        #endregion

        #region Events

        #region GeoPositionChanged

        /// <summary>
        /// An event handler getting fired whenever the position
        /// of the mouse on the map changed.
        /// </summary>
        public delegate void GeoPositionChangedEventHandler(AegirMapCanvas Sender, Tuple<Double, Double> GeoPosition);

        /// <summary>
        /// An event getting fired whenever the position of the mouse
        /// on the map changed.
        /// </summary>
        public event GeoPositionChangedEventHandler GeoPositionChanged;

        #endregion

        #region ZoomLevelChanged

        /// <summary>
        /// An event handler getting fired whenever the
        /// zoomlevel of the map changed.
        /// </summary>
        public delegate void ZoomLevelChangedEventHandler(AegirMapCanvas Sender, UInt32 OldZoomLevel, UInt32 NewZoomLevel);

        /// <summary>
        /// An event getting fired whenever the zoomlevel
        /// of the map changed.
        /// </summary>
        public event ZoomLevelChangedEventHandler ZoomLevelChanged;

        #endregion

        #region MapProviderChanged

        /// <summary>
        /// An event handler getting fired whenever the
        /// map provider of the map changed.
        /// </summary>
        public delegate void MapProviderChangedEventHandler(AegirMapCanvas Sender, String OldMapProvider, String NewMapProvider);

        /// <summary>
        /// An event getting fired whenever the map provider
        /// of the map changed.
        /// </summary>
        public event MapProviderChangedEventHandler MapProviderChanged;

        #endregion

        #region MapMoved

        /// <summary>
        /// An event handler getting fired whenever the
        /// map provider of the map changed.
        /// </summary>
        public delegate void MapMovedEventHandler(AegirMapCanvas Sender, UInt32 Movements);

        /// <summary>
        /// An event getting fired whenever the map provider
        /// of the map changed.
        /// </summary>
        public event MapMovedEventHandler MapMoved;

        #endregion

        #endregion

        #region Constructor(s)

        #region AegirMapCanvas()

        /// <summary>
        /// Creates a new canvas for visualizing a map.
        /// </summary>
        public AegirMapCanvas()
        {

            this.DrawingOffsetX = 0;
            this.DrawingOffsetY = 0;
            this.Background     = new SolidColorBrush(Colors.Transparent);
            this._MapProvider   = de.Vanaheimr.Aegir.Tiles.ArcGIS_WorldImagery_Provider.Name;
            this.ZoomLevel      = MinZoomLevel;

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
            PaintMap();
        }

        #endregion


        #region ZoomIn()

        /// <summary>
        /// Zoom into the map.
        /// </summary>
        public void ZoomIn()
        {
            
            if (ZoomLevel < MaxZoomLevel)
            {

                ZoomLevel++;

                PaintMap();

                if (ZoomLevelChanged != null)
                    ZoomLevelChanged(this, ZoomLevel - 1, ZoomLevel);

            }

        }

        #endregion

        #region ZoomOut()

        /// <summary>
        /// Zoom out of the map.
        /// </summary>
        public void ZoomOut()
        {

            if (ZoomLevel > MinZoomLevel)
            {

                ZoomLevel--;

                PaintMap();

                if (ZoomLevelChanged != null)
                    ZoomLevelChanged(this, ZoomLevel - 1, ZoomLevel);

            }

        }

        #endregion

        #region PaintMap()

        /// <summary>
        /// Paints the map.
        /// </summary>
        /// <returns>True if the map was repainted; false otherwise.</returns>
        public Boolean PaintMap()
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

                    #region Run in background

                    Task.Factory.StartNew(() => {

                        var _NumberOfXTiles = (Int32) Math.Floor(base.ActualWidth  / 256);
                        var _NumberOfYTiles = (Int32) Math.Floor(base.ActualHeight / 256);
                        var _NumberOfTiles  = (Int32) Math.Pow(2, ZoomLevel);
                        var ___x = (Int32) DrawingOffsetX % (_NumberOfTiles * 256) / 256;
                        var ___y = (Int32) DrawingOffsetY % (_NumberOfTiles * 256) / 256;

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

                                var _TileStream = TileServer.GetTile(MapProvider, ZoomLevel, (UInt32) _ActualXTile, (UInt32) _ActualYTile);

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
                            
                                    Canvas.SetLeft(_Image, DrawingOffsetX % 256 + _x * 256);
                                    Canvas.SetTop (_Image, DrawingOffsetY % 256 + _y * 256);

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


        #region AegirMapCanvas_MouseLeftButtonDown

        public void AegirMapCanvas_MouseLeftButtonDown(Object Sender, MouseButtonEventArgs MouseButtonEventArgs)
        {

            // We'll need this for when the Form starts to move
            var MousePosition = MouseButtonEventArgs.GetPosition(this);
            LastClickPositionX = MousePosition.X;
            LastClickPositionY = MousePosition.Y;

            DrawingOffset_AtMovementStart_X = DrawingOffsetX;
            DrawingOffset_AtMovementStart_Y = DrawingOffsetY;

        }

        #endregion

        #region AllCanvas_MouseMove

        public void AllCanvas_MouseMove(Object Sender, MouseEventArgs MouseEventArgs)
        {

            Int32 MapSizeAtZoomlevel;

            var MousePosition = MouseEventArgs.GetPosition(this);

            if (LastMousePositionDuringMovementX != MousePosition.X ||
                LastMousePositionDuringMovementY != MousePosition.Y)
            {

                #region Send GeoPositionChanged event

                if (GeoPositionChanged != null)
                {
                    GeoPositionChanged(this, MouseToWorldPosition(MousePosition.X - DrawingOffsetX,
                                                                  MousePosition.Y - DrawingOffsetY,
                                                                  ZoomLevel));
                }

                #endregion

                #region The left mouse button is still pressed => dragging the map!

                if (MouseEventArgs.LeftButton == MouseButtonState.Pressed)
                {

                    MapSizeAtZoomlevel              = (Int32) (Math.Pow(2, ZoomLevel) * 256);

                    DrawingOffset_AtMovementStart_X = DrawingOffset_AtMovementStart_X % MapSizeAtZoomlevel;
                    DrawingOffset_AtMovementStart_Y = DrawingOffset_AtMovementStart_Y % MapSizeAtZoomlevel;

                    DrawingOffsetX                  = (Int32) (Math.Round(DrawingOffset_AtMovementStart_X + MousePosition.X - LastClickPositionX) % MapSizeAtZoomlevel);
                    DrawingOffsetY                  = (Int32) (Math.Round(DrawingOffset_AtMovementStart_Y + MousePosition.Y - LastClickPositionY) % MapSizeAtZoomlevel);

                    #region Avoid endless vertical scrolling

                    var MapVerticalStart = (Int32) (-MapSizeAtZoomlevel + this.ActualHeight + 1);

                    if (DrawingOffsetY < MapVerticalStart)
                        DrawingOffsetY = MapVerticalStart;

                    if (DrawingOffsetY > 0)
                        DrawingOffsetY = 0;

                    #endregion

                    if (PaintMap())
                    {

                        MapMoves++;

                        if (MapMoved != null)
                            MapMoved(this, MapMoves);

                    }

                }

                #endregion

                LastMousePositionDuringMovementX = MousePosition.X;
                LastMousePositionDuringMovementY = MousePosition.Y;

            }

        }

        #endregion


        #region Map Area

        public Tuple<Double, Double> WorldCoordinates_2_Tile(Double lat, Double lon, Int32 zoom)
        {
            return new Tuple<Double, Double>(Math.Floor(((lon + 180.0) / 360.0) * (1 << zoom)),
                                     Math.Floor((1.0 - Math.Log(
                                                              Math.Tan(lat * Math.PI / 180.0) +
                                                              1.0 / Math.Cos(lat * Math.PI / 180.0)
                                                          ) / Math.PI)
                                                    / 2.0 * (1 << zoom))
                                    );
        }

        public Tuple<Double, Double> TileToWorldPos(Double tile_x, Double tile_y, UInt32 zoom)
        {

            double n = Math.PI - ((2.0 * Math.PI * tile_y) / Math.Pow(2.0, zoom));

            return new Tuple<Double, Double>(((tile_x / Math.Pow(2.0, zoom) * 360.0) - 180.0),
                                     (180.0 / Math.PI * Math.Atan(Math.Sinh(n))));

        }

        public Tuple<Double, Double> MouseToWorldPosition(Double MouseX, Double MouseY, UInt32 zoom)
        {

            var AllSize = Math.Pow(2.0, zoom) * 256;


            double n = Math.PI - ((2.0 * Math.PI * MouseY) / AllSize);

            return new Tuple<Double, Double>(((MouseX / AllSize * 360.0) - 180.0),
                                     (180.0 / Math.PI * Math.Atan(Math.Sinh(n))));

        }

        ////Usage:
        //var point = LatLonToPoint(51.51202,0.02435,17)
        //var tile = point.toTile
        //// ==> Tile(65544,43582,17)
        //var uri = tile.toURI
        //// ==> http://tile.openstreetmap.org/17/65544/43582.png

        #endregion

    }

}
