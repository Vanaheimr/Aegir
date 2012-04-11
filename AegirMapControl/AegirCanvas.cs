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
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using de.Vanaheimr.Aegir.Tiles;
using System.Collections.Generic;
using System.ComponentModel;

#endregion

namespace de.Vanaheimr.Aegir
{

    /// <summary>
    /// A canvas for visualizing a map.
    /// </summary>
    public class AegirMapCanvas : Canvas
    {

        public delegate void GeoPositionChangedEventHandler(object sender, Tuple<Double, Double> GeoPosition);

        #region Data

        //private Point  Mousy;

        private Int32 DrawingOffsetX;
        private Int32 DrawingOffsetY;

        private Tuple<Double, Double> lastClick;
        private Int32 DrawingOffset_AtMovementStart_X;
        private Int32 DrawingOffset_AtMovementStart_Y;
        private Double LastMousePositionDuringMovementX;
        private Double LastMousePositionDuringMovementY;

        private const UInt32 MinZoomLevel =  2;
        private const UInt32 MaxZoomLevel = 23;

        #endregion

        #region Properties

        #region TileServer

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
                    _MapProvider = value;
                    Paint();
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

        /// <summary>
        /// An event getting fired whenever the position of the mouse
        /// on the map changes.
        /// </summary>
        public event GeoPositionChangedEventHandler GeoPositionChanged;

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

        }

        #endregion

        #endregion


        #region ZoomIn()

        /// <summary>
        /// Zoom into the map.
        /// </summary>
        public void ZoomIn()
        {
            
            ZoomLevel++;

            if (ZoomLevel > MaxZoomLevel)
                ZoomLevel = MaxZoomLevel;

            Paint();

        }

        #endregion

        #region ZoomOut()

        /// <summary>
        /// Zoom out of the map.
        /// </summary>
        public void ZoomOut()
        {

            ZoomLevel--;

            if (ZoomLevel < MinZoomLevel)
                ZoomLevel = MinZoomLevel;

            Paint();

        }

        #endregion


        #region Paint()

        public void Paint()
        {

            if (!DesignerProperties.GetIsInDesignMode(this))
            {

                if (this.TileServer == null)
                    this.TileServer = new TileServer();

                var _ToDelete = new List<Image>();

                foreach (var Child in this.Children)
                {
                    if (Child is Image)
                        _ToDelete.Add((Image) Child);
                }

                foreach (var Image in _ToDelete)
                    this.Children.Remove(Image);

                Task.Factory.StartNew(() => {

                    var _NumberOfXTiles = (Int32) Math.Floor(base.ActualWidth  / 256);
                    var _NumberOfYTiles = (Int32) Math.Floor(base.ActualHeight / 256);
                    var _NumberOfTiles  = (Int32) Math.Pow(2, ZoomLevel);
                    var ___x = (Int32) DrawingOffsetX % (_NumberOfTiles * 256) / 256;
                    var ___y = (Int32) DrawingOffsetY % (_NumberOfTiles * 256) / 256;

                    Int32 _ActualXTile;
                    Int32 _ActualYTile;

                    for (var _x = -1; _x <= _NumberOfXTiles + 1; _x++)
                    {

                        _ActualXTile = ((_x - ___x) % _NumberOfTiles);
                        if (_ActualXTile < 0) _ActualXTile += _NumberOfTiles;

                        for (var _y = -1; _y <= _NumberOfYTiles + 1; _y++)
                        {

                            _ActualYTile = (Int32) ((_y - ___y) % _NumberOfTiles);
                            if (_ActualYTile < 0) _ActualYTile += _NumberOfTiles;

                            var _TileStream = TileServer.GetTile(MapProvider, ZoomLevel, (UInt32) _ActualXTile, (UInt32) _ActualYTile);

                            this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action<Object>)((_TileStream2) =>
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
                            
                                Canvas.SetLeft(_Image, DrawingOffsetX % 256 + _x * 256);
                                Canvas.SetTop (_Image, DrawingOffsetY % 256 + _y * 256);

                            }), _TileStream);

                        }

                    }

                });

            }

        }

        #endregion


        public void MapCanvas1_MouseDown(object sender, MouseButtonEventArgs e)
        {
         //   Paint();
        }


        #region SizeChangedEvent

        public void SizeChangedEvent(object sender, SizeChangedEventArgs e)
        {
            Paint();
        }

        #endregion



        #region canvas1_MouseLeftButtonDown

        public void canvas1_MouseLeftButtonDown(object sender, MouseButtonEventArgs eventArgs)
        {

            // We'll need this for when the Form starts to move
            var MousePosition = eventArgs.GetPosition(this);
            lastClick = new Tuple<Double, Double>(MousePosition.X, MousePosition.Y);

            DrawingOffset_AtMovementStart_X = DrawingOffsetX;
            DrawingOffset_AtMovementStart_Y = DrawingOffsetY;

        }

        #endregion

        #region AllCanvas_MouseMove

        public void AllCanvas_MouseMove(Object sender, MouseEventArgs eventArgs)
        {

            var MousePosition = eventArgs.GetPosition(this);

            if (LastMousePositionDuringMovementX != MousePosition.X ||
                LastMousePositionDuringMovementY != MousePosition.Y)
            {

                var ___x = Math.Floor(MousePosition.X / 256);
                var ___y = Math.Floor(MousePosition.Y / 256);

                //var _TilePos  = TileToWorldPos((Double)___x, (Double)___y, Zoom);
                //PositionTextBlock.Text = ToGeo(MouseToWorldPos(MousePosition.X - DrawingOffsetX,
                //                                               MousePosition.Y - DrawingOffsetY,
                //                                               Zoom));

                if (GeoPositionChanged != null)
                {
                    GeoPositionChanged(this, MouseToWorldPos(MousePosition.X - DrawingOffsetX,
                                                               MousePosition.Y - DrawingOffsetY,
                                                               ZoomLevel));
                }


                if (eventArgs.LeftButton == MouseButtonState.Pressed)
                {

                    var pos = eventArgs.GetPosition(this);

                    var _MapSizeAtZoomlevel = (Int32)(Math.Pow(2, ZoomLevel) * 256);

                    DrawingOffset_AtMovementStart_X = DrawingOffset_AtMovementStart_X % _MapSizeAtZoomlevel;
                    DrawingOffset_AtMovementStart_Y = DrawingOffset_AtMovementStart_Y % _MapSizeAtZoomlevel;

                    DrawingOffsetX = (Int32)(Math.Round(DrawingOffset_AtMovementStart_X + pos.X - lastClick.Item1) % _MapSizeAtZoomlevel);
                    DrawingOffsetY = (Int32)(Math.Round(DrawingOffset_AtMovementStart_Y + pos.Y - lastClick.Item2) % _MapSizeAtZoomlevel);

                    if (DrawingOffsetY < -_MapSizeAtZoomlevel + this.ActualHeight + 1)
                        DrawingOffsetY = -_MapSizeAtZoomlevel + (Int32)this.ActualHeight + 1;

                    if (DrawingOffsetY > 0)
                        DrawingOffsetY = 0;

                    //Moves++;

                    Paint();

                }

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

        public Tuple<Double, Double> MouseToWorldPos(Double MouseX, Double MouseY, UInt32 zoom)
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
