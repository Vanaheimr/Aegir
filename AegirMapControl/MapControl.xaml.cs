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

        #region DisplayOffsetChanged

        /// <summary>
        /// An event handler getting fired whenever the
        /// display offset of the map changed.
        /// </summary>
        public delegate void DisplayOffsetChangedEventHandler(MapControl Sender, Int32 X, Int32 Y);

        /// <summary>
        /// An event getting fired whenever the display offset
        /// of the map changed.
        /// </summary>
        public event DisplayOffsetChangedEventHandler DisplayOffsetChanged;

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

            this.ZoomLevel               = MinZoomLevel;
            this.ZoomOutButton.IsEnabled = false;

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
                                                           IdentificatorFunc: (MapProviderClass) => MapProviderClass.Name);

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


        #region (private) ZoomInButton_Click(Sender, RoutedEventArgs)

        /// <summary>
        /// Zoom into the map at the current center of the map.
        /// </summary>
        /// <param name="Sender">The sender of the event.</param>
        /// <param name="RoutedEventArgs">The event arguments.</param>
        private void ZoomInButton_Click(Object Sender, RoutedEventArgs RoutedEventArgs)
        {
            ZoomIn(this.ActualWidth / 2, this.ActualHeight / 2);
        }

        #endregion

        #region (private) ZoomOutButton_Click(Sender, RoutedEventArgs)

        /// <summary>
        /// Zoom out of the map at the current center of the map.
        /// </summary>
        /// <param name="Sender">The sender of the event.</param>
        /// <param name="RoutedEventArgs">The event arguments.</param>
        private void ZoomOutButton_Click(Object Sender, RoutedEventArgs RoutedEventArgs)
        {
            ZoomOut(this.ActualWidth / 2, this.ActualHeight / 2);
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

                    DrawingOffsetX = (Int32) (Math.Round(DrawingOffset_AtMovementStart_X + MousePosition.X - LastClickPositionX) % MapSizeAtZoomlevel);
                    DrawingOffsetY = (Int32) (Math.Round(DrawingOffset_AtMovementStart_Y + MousePosition.Y - LastClickPositionY) % MapSizeAtZoomlevel);

                    #region Avoid endless vertical scrolling

                    var MapVerticalStart = (Int32)(-MapSizeAtZoomlevel + this.ActualHeight + 1);

                    if (DrawingOffsetY < MapVerticalStart)
                        DrawingOffsetY = MapVerticalStart;

                    if (DrawingOffsetY > 0)
                        DrawingOffsetY = 0;

                    #endregion

                    if (DisplayOffsetChanged != null)
                        DisplayOffsetChanged(this, DrawingOffsetX, DrawingOffsetY);

                    TilesCanvas.  SetDisplayOffset(DrawingOffsetX, DrawingOffsetY);
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
        /// The mouse was moved.
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

        #region (private) ProcessMouseLeftDoubleClick(Sender, MouseButtonEventArgs)

        /// <summary>
        /// The left mouse button was double clicked (from: PreviewMouseLeftButtonDown).
        /// </summary>
        /// <param name="Sender">The sender of the event.</param>
        /// <param name="MouseButtonEventArgs">The mouse button event arguments.</param>
        private void ProcessMouseLeftDoubleClick(Object Sender, MouseButtonEventArgs MouseButtonEventArgs)
        {

            if (MouseButtonEventArgs.ClickCount > 1)
            {

                // Zoom in/out at the given mouse position
                var MousePosition = MouseButtonEventArgs.GetPosition(this);
                ZoomIn(MousePosition.X, MousePosition.Y);

                MouseButtonEventArgs.Handled = true;

            }

        }

        #endregion

        #region (private) ProcessMouseWheel(Sender, MouseWheelEventArgs)

        /// <summary>
        /// The mouse wheel was moved.
        /// </summary>
        /// <param name="Sender">The sender of the event.</param>
        /// <param name="MouseWheelEventArgs">The mouse wheel event arguments.</param>
        private void ProcessMouseWheel(Object Sender, MouseWheelEventArgs MouseWheelEventArgs)
        {

            // Zoom in/out at the given mouse position
            var MousePosition = MouseWheelEventArgs.GetPosition(this);

            if (MouseWheelEventArgs.Delta < 0)
                ZoomOut(MousePosition.X, MousePosition.Y);

            else if (MouseWheelEventArgs.Delta > 0)
                ZoomIn (MousePosition.X, MousePosition.Y);

        }

        #endregion


        #region ZoomTo(Latitude, Longitude, ZoomLevel)

        /// <summary>
        /// Zoom into the map onto a given zoom level.
        /// </summary>
        /// <param name="Latitude">The latitude of the zoom center on the map.</param>
        /// <param name="Longitude">The longitude of the zoom center on the map.</param>
        /// <param name="ZoomLevel">The desired zoom level.</param>
        public void ZoomTo(Double Latitude, Double Longitude, UInt32 ZoomLevel)
        {

            #region Initial checks

            if (ZoomLevel < MinZoomLevel || ZoomLevel > MaxZoomLevel)
                throw new ArgumentException("Invalid zoom level!");

            #endregion

        }

        #endregion

        #region ZoomIn(ScreenX, ScreenY)

        /// <summary>
        /// Zoom into the map.
        /// </summary>
        /// <param name="ScreenX">The x-coordinate of the zoom center on the map.</param>
        /// <param name="ScreenY">The y-coordinate of the zoom center on the map.</param>
        public void ZoomIn(Double ScreenX, Double ScreenY)
        {

            if (ZoomLevel < MaxZoomLevel)
            {

                ZoomLevel++;

                var _Kuerzung = (Int32) (Math.Pow(2, ZoomLevel) * 256);

                DrawingOffset_AtMovementStart_X = DrawingOffset_AtMovementStart_X % _Kuerzung;
                DrawingOffset_AtMovementStart_Y = DrawingOffset_AtMovementStart_Y % _Kuerzung;

                DrawingOffsetX = DrawingOffsetX % _Kuerzung;
                DrawingOffsetY = DrawingOffsetY % _Kuerzung;

                TilesCanvas.ZoomLevel   = ZoomLevel;
                HeatmapCanvas.ZoomLevel = ZoomLevel;
                FeatureCanvas.ZoomLevel = ZoomLevel;

                if (ZoomLevelChanged != null)
                    ZoomLevelChanged(this, ZoomLevel - 1, ZoomLevel);

                if (ZoomLevel == MaxZoomLevel)
                    ZoomInButton.IsEnabled = false;

                if (ZoomLevel > MinZoomLevel)
                    ZoomOutButton.IsEnabled = true;

            }

        }

        #endregion

        #region ZoomOut(ScreenX, ScreenY)

        /// <summary>
        /// Zoom out of the map.
        /// </summary>
        /// <param name="ScreenX">The x-coordinate of the zoom center on the map.</param>
        /// <param name="ScreenY">The y-coordinate of the zoom center on the map.</param>
        public void ZoomOut(Double ScreenX, Double ScreenY)
        {

            if (ZoomLevel > MinZoomLevel)
            {

                ZoomLevel--;

                var _Kuerzung = (Int32) (Math.Pow(2, ZoomLevel) * 256);

                DrawingOffset_AtMovementStart_X = DrawingOffset_AtMovementStart_X % _Kuerzung;
                DrawingOffset_AtMovementStart_Y = DrawingOffset_AtMovementStart_Y % _Kuerzung;

                DrawingOffsetX = DrawingOffsetX % _Kuerzung;
                DrawingOffsetY = DrawingOffsetY % _Kuerzung;

                TilesCanvas.ZoomLevel   = ZoomLevel;
                HeatmapCanvas.ZoomLevel = ZoomLevel;
                FeatureCanvas.ZoomLevel = ZoomLevel;

                if (ZoomLevelChanged != null)
                    ZoomLevelChanged(this, ZoomLevel + 1, ZoomLevel);

                if (ZoomLevel == MinZoomLevel)
                    ZoomOutButton.IsEnabled = false;

                if (ZoomLevel < MaxZoomLevel)
                    ZoomInButton.IsEnabled = true;

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

            var OldMapProvider = TilesCanvas.MapProvider;

            if (MapProviderName != null && MapProviderName != "")
            {

                foreach (var Item in this.ContextMenu.Items)
                {
                    
                    var MenuItem = Item as MenuItem;

                    if (MenuItem != null)
                        MenuItem.IsChecked = (MenuItem.HeaderStringFormat == MapProviderName);

                }

                TilesCanvas.MapProvider = MapProviderName;

            }

        }

        #endregion


        #region AddFeature(Name, Latitude, Longitude, width, height, Color)

        public Feature AddFeature(String Name, Double Latitude, Double Longitude, Double Width, Double Height, Color Color)
        {
            return FeatureCanvas.AddFeature(Name, Latitude, Longitude, Width, Height, Color);
        }

        #endregion


    }

}
