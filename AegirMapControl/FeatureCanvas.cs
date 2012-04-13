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
using de.Vanaheimr.Aegir.Controls;

#endregion

namespace de.Vanaheimr.Aegir
{

    /// <summary>
    /// A canvas for visualizing features.
    /// </summary>
    public class FeatureCanvas : Canvas
    {

        #region Data

        private Int32  DrawingOffsetX;
        private Int32  DrawingOffsetY;
        private Int32  DrawingOffset_AtMovementStart_X;
        private Int32  DrawingOffset_AtMovementStart_Y;

        private Double LastClickPositionX;
        private Double LastClickPositionY;

        private volatile Boolean IsCurrentlyPainting;

        #endregion

        #region Properties

        public MapControl AegirMapControl { get; set; }

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

        #region FeatureCanvas()

        /// <summary>
        /// Creates a new canvas for visualizing features.
        /// </summary>
        public FeatureCanvas()
        {

            this.DrawingOffsetX = 0;
            this.DrawingOffsetY = 0;
            this.Background     = new SolidColorBrush(Colors.Transparent);

            this.SizeChanged   += ProcessMapSizeChangedEvent;

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

                    Feature Feature;
                    Tuple<UInt32, UInt32> XY;

                    foreach (var Child in this.Children)
                    {

                        Feature = Child as Feature;

                        if (Feature != null)
                        {
                            XY = GeoCalculations.WorldCoordinates_2_Screen(Feature.Latitude, Feature.Longitude, (Int32)_ZoomLevel);
                            Canvas.SetLeft(Feature, DrawingOffsetX + XY.Item1 - Feature.Width  / 2);
                            Canvas.SetTop (Feature, DrawingOffsetY + XY.Item2 - Feature.Height / 2);
                        }

                    }

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

        #region SetDisplayOffset(OffsetX, OffsetY)

        public void SetDisplayOffset(Int32 OffsetX, Int32 OffsetY)
        {

            DrawingOffsetX = OffsetX;
            DrawingOffsetY = OffsetY;

            PaintMap();

        }

        #endregion


        #region AddFeature

        public Feature AddFeature(String Name, Double Latitude, Double Longitude, Double width, Double height, Color StrokeColor)
        {

            var XY = GeoCalculations.WorldCoordinates_2_Screen(Latitude, Longitude, (Int32) _ZoomLevel);

            var Feature              = new Feature(new EllipseGeometry() { RadiusX = width/2, RadiusY = height/2 });
            Feature.Name             = Name;
            Feature.Latitude         = Latitude;
            Feature.Longitude        = Longitude;
            Feature.Stroke           = new SolidColorBrush(StrokeColor);
            Feature.StrokeThickness  = 1;
            Feature.Width            = width;
            Feature.Height           = height;
            Feature.Fill             = new SolidColorBrush(Colors.Blue);
            Feature.ToolTip          = Name;

            this.Children.Add(Feature);
            
            Canvas.SetLeft(Feature, DrawingOffsetX + XY.Item1 - width / 2);
            Canvas.SetTop (Feature, DrawingOffsetY + XY.Item2 - height / 2);

            return Feature;

        }

        #endregion

    }

}
