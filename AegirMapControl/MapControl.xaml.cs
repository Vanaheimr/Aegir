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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using de.ahzf.Illias.Commons;
using de.Vanaheimr.Aegir.Tiles;
using de.ahzf.Vanaheimr.Aegir;

#endregion

namespace de.Vanaheimr.Aegir.Controls
{

    /// <summary>
    /// Interaction logic for MapControl.xaml
    /// </summary>
    public partial class MapControl : UserControl
    {

        #region Data

        /// <summary>
        /// An internal collection of all reflected map providers.
        /// </summary>
        private AutoDiscovery<IMapProvider> MapProviders;

        private Int32 DrawingOffsetX;
        private Int32 DrawingOffsetY;
        private Int32 DrawingOffset_AtMovementStart_X;
        private Int32 DrawingOffset_AtMovementStart_Y;

        private Double LastClickPositionX;
        private Double LastClickPositionY;

        private Double LastMousePositionDuringMovementX;
        private Double LastMousePositionDuringMovementY;

        public const UInt32 MinZoomLevel = 1;
        public const UInt32 MaxZoomLevel = 23;

        #endregion

        #region Properties

        #region ZoomLevel

        /// <summary>
        /// The zoom level of the map.
        /// </summary>
        public UInt32 ZoomLevel
        {
            
            get
            {
                return TilesCanvas.ZoomLevel;
            }

            set
            {
                TilesCanvas.ZoomLevel   = value;
                HeatmapCanvas.ZoomLevel = value;
                FeatureCanvas.ZoomLevel = value;
            }

        }

        #endregion

        #region MapProvider

        /// <summary>
        /// The map tiles provider for this map.
        /// </summary>
        public String MapProvider
        {
            
            get
            {
                return TilesCanvas.MapProvider;
            }

            set
            {
                if (value != null && value != "")
                    TilesCanvas.MapProvider = value;
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
        public delegate void GeoPositionChangedEventHandler(MapControl Sender, Tuple<Double, Double> GeoPosition);

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
        public delegate void ZoomLevelChangedEventHandler(MapControl Sender, UInt32 OldZoomLevel, UInt32 NewZoomLevel);

        /// <summary>
        /// An event getting fired whenever the zoomlevel
        /// of the map changed.
        /// </summary>
        public event ZoomLevelChangedEventHandler ZoomLevelChanged;

        #endregion

        #region MapProviderChanged

        /// <summary>
        /// An event getting fired whenever the map provider
        /// of the map changed.
        /// </summary>
        public event TilesCanvas.MapProviderChangedEventHandler MapProviderChanged
        {

            add
            {
                this.TilesCanvas.MapProviderChanged += value;
            }

            remove
            {
                this.TilesCanvas.MapProviderChanged -= value;
            }

        }

        #endregion

        #region MapMoved

        /// <summary>
        /// An event getting fired whenever the zoomlevel
        /// of the map changes.
        /// </summary>
        public event TilesCanvas.MapMovedEventHandler MapMoved
        {

            add
            {
                this.TilesCanvas.MapMoved += value;
            }

            remove
            {
                this.TilesCanvas.MapMoved -= value;
            }

        }

        #endregion

        #endregion

        #region MapControl()

        /// <summary>
        /// Initialize the MapControl component.
        /// </summary>
        public MapControl()
        {

            InitializeComponent();
            AddMapCanvasContextMenu();
            ChangeMapProvider(TilesCanvas.MapProvider);

            this.ZoomLevel = MinZoomLevel;

        }

        #endregion


        #region (private) ZoomInButton_Click(Sender, RoutedEventArgs)

        /// <summary>
        /// Zoom into the map.
        /// </summary>
        /// <param name="Sender">The sender of the event.</param>
        /// <param name="RoutedEventArgs">The event arguments.</param>
        private void ZoomInButton_Click(Object Sender, RoutedEventArgs RoutedEventArgs)
        {

            if (ZoomLevel < MaxZoomLevel)
            {

                ZoomLevel++;

                TilesCanvas.ZoomLevel   = ZoomLevel;
                HeatmapCanvas.ZoomLevel = ZoomLevel;
                FeatureCanvas.ZoomLevel = ZoomLevel;

                if (ZoomLevelChanged != null)
                    ZoomLevelChanged(this, ZoomLevel - 1, ZoomLevel);

            }

        }

        #endregion

        #region (private) ZoomOutButton_Click(Sender, RoutedEventArgs)

        /// <summary>
        /// Zoom out of the map.
        /// </summary>
        /// <param name="Sender">The sender of the event.</param>
        /// <param name="RoutedEventArgs">The event arguments.</param>
        private void ZoomOutButton_Click(Object Sender, RoutedEventArgs RoutedEventArgs)
        {

            if (ZoomLevel > MinZoomLevel)
            {

                ZoomLevel--;

                TilesCanvas.ZoomLevel   = ZoomLevel;
                HeatmapCanvas.ZoomLevel = ZoomLevel;
                FeatureCanvas.ZoomLevel = ZoomLevel;

                if (ZoomLevelChanged != null)
                    ZoomLevelChanged(this, ZoomLevel + 1, ZoomLevel);

            }

        }

        #endregion


        #region (private) ProcessMouseMove(Sender, MouseEventArgs)

        /// <summary>
        /// The mouse was moved above all canvas.
        /// </summary>
        /// <param name="Sender">The sender of the event.</param>
        /// <param name="MouseEventArgs">The mouse event arguments.</param>
        private void ProcessMouseMove(Object Sender, MouseEventArgs MouseEventArgs)
        {

            Int32 MapSizeAtZoomlevel;
            var MousePosition = MouseEventArgs.GetPosition(this);

            if (LastMousePositionDuringMovementX != MousePosition.X ||
                LastMousePositionDuringMovementY != MousePosition.Y)
            {

                #region Send GeoPositionChanged event

                if (GeoPositionChanged != null)
                {
                    GeoPositionChanged(this, GeoCalculations.Mouse_2_WorldCoordinates(MousePosition.X - DrawingOffsetX,
                                                                                      MousePosition.Y - DrawingOffsetY,
                                                                                      ZoomLevel));
                }

                #endregion

                #region The left mouse button is still pressed => dragging the map!

                if (MouseEventArgs.LeftButton == MouseButtonState.Pressed)
                {

                    MapSizeAtZoomlevel = (Int32)(Math.Pow(2, ZoomLevel) * 256);

                    DrawingOffset_AtMovementStart_X = DrawingOffset_AtMovementStart_X % MapSizeAtZoomlevel;
                    DrawingOffset_AtMovementStart_Y = DrawingOffset_AtMovementStart_Y % MapSizeAtZoomlevel;

                    DrawingOffsetX = (Int32)(Math.Round(DrawingOffset_AtMovementStart_X + MousePosition.X - LastClickPositionX) % MapSizeAtZoomlevel);
                    DrawingOffsetY = (Int32)(Math.Round(DrawingOffset_AtMovementStart_Y + MousePosition.Y - LastClickPositionY) % MapSizeAtZoomlevel);

                    #region Avoid endless vertical scrolling

                    var MapVerticalStart = (Int32)(-MapSizeAtZoomlevel + this.ActualHeight + 1);

                    if (DrawingOffsetY < MapVerticalStart)
                        DrawingOffsetY = MapVerticalStart;

                    if (DrawingOffsetY > 0)
                        DrawingOffsetY = 0;

                    #endregion

                    TilesCanvas.SetDisplayOffset(DrawingOffsetX, DrawingOffsetY);
                    HeatmapCanvas.SetDisplayOffset(DrawingOffsetX, DrawingOffsetY);
                    FeatureCanvas.SetDisplayOffset(DrawingOffsetX, DrawingOffsetY);

                }

                #endregion

                LastMousePositionDuringMovementX = MousePosition.X;
                LastMousePositionDuringMovementY = MousePosition.Y;

            }

        }

        #endregion

        #region (private) ProcessMouseLeftButtonDown(Sender, MouseButtonEventArgs)

        /// <summary>
        /// The mouse was moced above all canvas.
        /// </summary>
        /// <param name="Sender">The sender of the event.</param>
        /// <param name="MouseButtonEventArgs">The mouse button event arguments.</param>
        private void ProcessMouseLeftButtonDown(Object Sender, MouseButtonEventArgs MouseButtonEventArgs)
        {

            // We'll need this for when the Form starts to move
            var MousePosition = MouseButtonEventArgs.GetPosition(this);
            LastClickPositionX = MousePosition.X;
            LastClickPositionY = MousePosition.Y;

            DrawingOffset_AtMovementStart_X = DrawingOffsetX;
            DrawingOffset_AtMovementStart_Y = DrawingOffsetY;

            HeatmapCanvas.ProcessMouseLeftButtonDown(Sender, MouseButtonEventArgs);
            FeatureCanvas.ProcessMouseLeftButtonDown(Sender, MouseButtonEventArgs);

        }

        #endregion


        #region (private) AddMapCanvasContextMenu()

        /// <summary>
        /// Add a context menu to the mapping canvas.
        /// </summary>
        private void AddMapCanvasContextMenu()
        {

            this.ContextMenu = new ContextMenu();

            // Find map providers via reflection
            MapProviders = new AutoDiscovery<IMapProvider>(Autostart:         true,
                                                           IdentificatorFunc: (MapProvider) => MapProvider.Name);

            // Add all map providers to the mapping canvas context menu
            foreach (var _MapProvider in MapProviders.RegisteredNames)
            {

                var _MapProviderMenuItem = new MenuItem()
                {
                    Header             = _MapProvider,
                    HeaderStringFormat = _MapProvider,
                    IsCheckable        = true
                };

                _MapProviderMenuItem.Click += new RoutedEventHandler(ChangeMapProvider);

                this.ContextMenu.Items.Add(_MapProviderMenuItem);

            }

        }

        #endregion

        #region (private) ChangeMapProvider(Sender, RoutedEventArgs)

        /// <summary>
        /// Changes the current map provider.
        /// </summary>
        /// <param name="Sender">The sender of the event.</param>
        /// <param name="RoutedEventArgs">The event arguments.</param>
        private void ChangeMapProvider(Object Sender, RoutedEventArgs RoutedEventArgs)
        {
            ChangeMapProvider((Sender as MenuItem).HeaderStringFormat);
        }

        #endregion

        #region (private) ChangeMapProvider(MapProviderName)

        /// <summary>
        /// Changes the current map provider.
        /// </summary>
        /// <param name="MapProviderName">The well-known name of the map provider.</param>
        private void ChangeMapProvider(String MapProviderName)
        {

            var OldMapProvider = TilesCanvas.MapProvider;

            if (MapProviderName != null && MapProviderName != "")
            {

                foreach (var item in this.ContextMenu.Items)
                {
                    var CurrentMenuItem = item as MenuItem;
                    CurrentMenuItem.IsChecked = (CurrentMenuItem.HeaderStringFormat == MapProviderName);
                }

                TilesCanvas.MapProvider = MapProviderName;

            }

        }

        #endregion


        #region AddFeature

        public Feature AddFeature(String Name, Double Latitude, Double Longitude, Double width, Double height, Color StrokeColor)
        {
            return FeatureCanvas.AddFeature(Name, Latitude, Longitude, width, height, StrokeColor);
        }

        #endregion

    }

}
