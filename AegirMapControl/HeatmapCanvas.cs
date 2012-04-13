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
using System.Windows.Shapes;

#endregion

namespace de.Vanaheimr.Aegir
{

    /// <summary>
    /// A canvas for visualizing a heatmap.
    /// </summary>
    public class HeatmapCanvas : Canvas
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

        private readonly ConcurrentStack<Image> TilesOnMap;
        private volatile Boolean                IsCurrentlyPainting;

        #endregion

        #region Properties

        #region ZoomLevel

        private UInt32 _ZoomLevel;

        /// <summary>
        /// The zoom level of the map.
        /// </summary>
        public UInt32 ZoomLevel
        {

            get
            {
                return _ZoomLevel;
            }

            set
            {
                _ZoomLevel = value;
                PaintMap();
            }

        }

        #endregion

        #endregion

        #region Events

        #region GeoPositionChanged

        /// <summary>
        /// An event handler getting fired whenever the position
        /// of the mouse on the map changed.
        /// </summary>
        public delegate void GeoPositionChangedEventHandler(TilesCanvas Sender, Tuple<Double, Double> GeoPosition);

        /// <summary>
        /// An event getting fired whenever the position of the mouse
        /// on the map changed.
        /// </summary>
        public event GeoPositionChangedEventHandler GeoPositionChanged;

        #endregion

        #region MapProviderChanged

        /// <summary>
        /// An event handler getting fired whenever the
        /// map provider of the map changed.
        /// </summary>
        public delegate void MapProviderChangedEventHandler(TilesCanvas Sender, String OldMapProvider, String NewMapProvider);

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
        public delegate void MapMovedEventHandler(TilesCanvas Sender, UInt32 Movements);

        /// <summary>
        /// An event getting fired whenever the map provider
        /// of the map changed.
        /// </summary>
        public event MapMovedEventHandler MapMoved;

        #endregion

        #endregion

        #region Constructor(s)

        #region HeatmapCanvas()

        /// <summary>
        /// Creates a new canvas for visualizing a heatmap.
        /// </summary>
        public HeatmapCanvas()
        {

            this.DrawingOffsetX = 0;
            this.DrawingOffsetY = 0;
            this.Background     = new SolidColorBrush(Colors.Transparent);

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
            LastClickPositionX = MousePosition.X;
            LastClickPositionY = MousePosition.Y;

            DrawingOffset_AtMovementStart_X = DrawingOffsetX;
            DrawingOffset_AtMovementStart_Y = DrawingOffsetY;

        }

        #endregion

        #region MouseMoved

        public void SetDisplayOffset(Int32 OffsetX, Int32 OffsetY)
        {

            DrawingOffsetX = OffsetX;
            DrawingOffsetY = OffsetY;

            foreach (var Child in this.Children)
            {
                var Feature = (Feature) Child;
                var XY = GeoCalculations.WorldCoordinates_2_Screen(Feature.Latitude, Feature.Longitude, (Int32) _ZoomLevel);
                Canvas.SetLeft(Feature, DrawingOffsetX + XY.Item1 - Feature.Width / 2);
                Canvas.SetTop (Feature, DrawingOffsetY + XY.Item2 - Feature.Height / 2);
            }

        }

        #endregion

    }

}
