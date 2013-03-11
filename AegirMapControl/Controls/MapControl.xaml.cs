/*
 * Copyright (c) 2010-2013, Achim 'ahzf' Friedland <achim@graph-database.org>
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
using System.Reflection;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

using eu.Vanaheimr.Illias.Commons;

#endregion

namespace eu.Vanaheimr.Aegir.Controls
{

    /// <summary>
    /// Interaction logic for MapControl.xaml
    /// </summary>
    public partial class MapControl : UserControl
    {

        #region Data

        public const UInt32 MinZoomLevel = 1;
        public const UInt32 MaxZoomLevel = 23;

        private Int64  ScreenOffsetX;
        private Int64  ScreenOffsetY;
        private Int64  ScreenOffset_AtMovementStart_X;
        private Int64  ScreenOffset_AtMovementStart_Y;

        private Double LastClickPositionX;
        private Double LastClickPositionY;

        private Double LastMousePositionDuringMovementX;
        private Double LastMousePositionDuringMovementY;

        /// <summary>
        /// The stepping when moving the map
        /// via the keyboard arrow keys
        /// </summary>
        private readonly UInt32 KeyboardStepping;

        private readonly Dictionary<String, IMapLayer> MapLayers;

        private readonly StackPanel LayerPanel;

        #endregion

        #region Properties

        #region ZoomLevel

        /// <summary>
        /// The zoom level of the map.
        /// </summary>
        public UInt32 ZoomLevel { get; private set; }

        #endregion

        #region MapMoves

        /// <summary>
        /// The number of movements of this map.
        /// </summary>
        public UInt64 MapMovements { get; private set; }

        #endregion

        #region InvertedKeyBoard

        /// <summary>
        /// Invert the keyboard commands for moving the map.
        /// </summary>
        public Boolean InvertedKeyBoard { get; set; }

        #endregion

        #endregion

        #region Events

        #region GeoPositionChanged

        /// <summary>
        /// An event handler getting fired whenever the position
        /// of the mouse on the map changed.
        /// </summary>
        public delegate void GeoPositionChangedEventHandler(MapControl Sender, GeoCoordinate GeoPosition);

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

        #region MapViewChanged

        /// <summary>
        /// An event handler getting fired whenever the
        /// view of the map changed.
        /// </summary>
        public delegate void MapViewChangedEventHandler(MapControl Sender, Int64 DisplayOffsetX, Int64 DisplayOffsetY, UInt64 Movements);

        /// <summary>
        /// An event getting fired whenever the view
        /// of the map changed.
        /// </summary>
        public event MapViewChangedEventHandler MapViewChanged;

        #endregion

        #endregion

        #region MapControl()

        /// <summary>
        /// Initialize the Aegir map control.
        /// </summary>
        public MapControl()
        {

            #region Initialize the map control

            InitializeComponent();

            this.MapLayers                = new Dictionary<String, IMapLayer>();
            this.ZoomOutButton.IsEnabled  = false;
            this.ZoomLevel                = MinZoomLevel;
            this.Focusable                = true;
            this.KeyboardStepping         = 50;
            this.InvertedKeyBoard         = false;

            this.KeyDown                 += new KeyEventHandler(ProcessKeyboardEvents);

            #endregion

        }

        #endregion


        // Event processing

        #region ZoomInButton_Click(Sender, RoutedEventArgs)

        /// <summary>
        /// The ZoomIn-button was pressed, so zoom into the map
        /// at the current center of the map.
        /// </summary>
        /// <param name="Sender">The sender of the event.</param>
        /// <param name="RoutedEventArgs">The event arguments.</param>
        public void ZoomInButton_Click(Object Sender, RoutedEventArgs RoutedEventArgs)
        {
            ZoomIn(this.ActualWidth / 2, this.ActualHeight / 2);
        }

        #endregion

        #region ZoomOutButton_Click(Sender, RoutedEventArgs)

        /// <summary>
        /// The ZoomOut-button was pressed, so zoom out of the map
        /// at the current center of the map.
        /// </summary>
        /// <param name="Sender">The sender of the event.</param>
        /// <param name="RoutedEventArgs">The event arguments.</param>
        public void ZoomOutButton_Click(Object Sender, RoutedEventArgs RoutedEventArgs)
        {
            ZoomOut(this.ActualWidth / 2, this.ActualHeight / 2);
        }

        #endregion

        #region ProcessMouseMove(Sender, MouseEventArgs)

        /// <summary>
        /// The mouse was moved above all canvas.
        /// </summary>
        /// <param name="Sender">The sender of the event.</param>
        /// <param name="MouseEventArgs">The mouse event arguments.</param>
        public void ProcessMouseMove(Object Sender, MouseEventArgs MouseEventArgs)
        {

            Int64 MapSizeAtZoomLevel;
            var MousePosition = MouseEventArgs.GetPosition(this);

            if (LastMousePositionDuringMovementX != MousePosition.X ||
                LastMousePositionDuringMovementY != MousePosition.Y)
            {

                #region Send GeoPositionChanged event

                if (GeoPositionChanged != null)
                {
                    GeoPositionChanged(this, GeoCalculations.Mouse_2_WorldCoordinates(MousePosition.X - this.ScreenOffsetX,
                                                                                      MousePosition.Y - this.ScreenOffsetY,
                                                                                      ZoomLevel));
                }

                #endregion


                var NewMapCenter = GeoCalculations.Mouse_2_WorldCoordinates(MousePosition.X - this.ScreenOffsetX,
                                                                            MousePosition.Y - this.ScreenOffsetY,
                                                                            this.ZoomLevel);

                var NewOffset = GeoCalculations.WorldCoordinates_2_Screen(NewMapCenter.Latitude, NewMapCenter.Longitude, this.ZoomLevel);


                #region The left mouse button is still pressed => dragging the map!

                if (MouseEventArgs.LeftButton == MouseButtonState.Pressed)
                {

                    MapSizeAtZoomLevel = (Int64) (Math.Pow(2, ZoomLevel) * 256);

                    this.ScreenOffset_AtMovementStart_X = this.ScreenOffset_AtMovementStart_X % MapSizeAtZoomLevel;
                    this.ScreenOffset_AtMovementStart_Y = this.ScreenOffset_AtMovementStart_Y % MapSizeAtZoomLevel;

                    this.ScreenOffsetX = (Int32) (Math.Round(ScreenOffset_AtMovementStart_X + MousePosition.X - LastClickPositionX) % MapSizeAtZoomLevel);
                    this.ScreenOffsetY = (Int32) (Math.Round(ScreenOffset_AtMovementStart_Y + MousePosition.Y - LastClickPositionY) % MapSizeAtZoomLevel);

                    AvoidEndlessVerticalScrolling();

                    MapLayers.Values.ForEach(Canvas => Canvas.SetDisplayOffset(ScreenOffsetX, ScreenOffsetY));

                    #region Send MapViewChanged events

                    MapMovements++;

                    if (MapViewChanged != null)
                        MapViewChanged(this, this.ScreenOffsetX, this.ScreenOffsetY, this.MapMovements);

                    #endregion

                }

                #endregion

                LastMousePositionDuringMovementX = MousePosition.X;
                LastMousePositionDuringMovementY = MousePosition.Y;

            }

        }

        #endregion

        #region ProcessMouseLeftButtonDown(Sender, MouseButtonEventArgs)

        /// <summary>
        /// The mouse was moved.
        /// </summary>
        /// <param name="Sender">The sender of the event.</param>
        /// <param name="MouseButtonEventArgs">The mouse button event arguments.</param>
        public void ProcessMouseLeftButtonDown(Object Sender, MouseButtonEventArgs MouseButtonEventArgs)
        {

            // We'll need this for when the Form starts to move
            var MousePosition = MouseButtonEventArgs.GetPosition(this);
            LastClickPositionX = MousePosition.X;
            LastClickPositionY = MousePosition.Y;

            ScreenOffset_AtMovementStart_X = ScreenOffsetX;
            ScreenOffset_AtMovementStart_Y = ScreenOffsetY;

            this.Focus();

        }

        #endregion

        #region ProcessMouseLeftDoubleClick(Sender, MouseButtonEventArgs)

        /// <summary>
        /// The left mouse button was double clicked (from: PreviewMouseLeftButtonDown),
        /// so zoom in/out at the given mouse position.
        /// </summary>
        /// <param name="Sender">The sender of the event.</param>
        /// <param name="MouseButtonEventArgs">The mouse button event arguments.</param>
        public void ProcessMouseLeftDoubleClick(Object Sender, MouseButtonEventArgs MouseButtonEventArgs)
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

        #region ProcessMouseWheel(Sender, MouseWheelEventArgs)

        /// <summary>
        /// The mouse wheel was moved so zoom in/out at
        /// the current center of the map.
        /// </summary>
        /// <param name="Sender">The sender of the event.</param>
        /// <param name="MouseWheelEventArgs">The mouse wheel event arguments.</param>
        public void ProcessMouseWheel(Object Sender, MouseWheelEventArgs MouseWheelEventArgs)
        {

            // Zoom in/out at the current center of the map.
            var MousePosition = MouseWheelEventArgs.GetPosition(this);

            if (MouseWheelEventArgs.Delta < 0)
                ZoomOut(this.ActualWidth / 2, this.ActualHeight / 2);

            else if (MouseWheelEventArgs.Delta > 0)
                ZoomIn (this.ActualWidth / 2, this.ActualHeight / 2);

        }

        #endregion

        #region ProcessKeyboardEvents(Sender, KeyEventArgs)

        /// <summary>
        /// Keys on the keyboard had been pressed.
        /// </summary>
        /// <param name="Sender">The sender of the event.</param>
        /// <param name="KeyEventArgs">The key arguments.</param>
        public void ProcessKeyboardEvents(Object Sender, KeyEventArgs KeyEventArgs)
        {

            switch (KeyEventArgs.Key)
            {

                #region Process arrow keys for moving the map

                case Key.Left:

                    if (InvertedKeyBoard)
                        MoveTo(this.ScreenOffsetX - KeyboardStepping, this.ScreenOffsetY);
                    else
                        MoveTo(this.ScreenOffsetX + KeyboardStepping, this.ScreenOffsetY);

                    KeyEventArgs.Handled = true;
                    break;

                case Key.Up:

                    if (InvertedKeyBoard)
                        MoveTo(this.ScreenOffsetX, this.ScreenOffsetY + KeyboardStepping);
                    else
                        MoveTo(this.ScreenOffsetX, this.ScreenOffsetY - KeyboardStepping);

                    KeyEventArgs.Handled = true;
                    break;

                case Key.Right:

                    if (InvertedKeyBoard)
                        MoveTo(this.ScreenOffsetX + KeyboardStepping, this.ScreenOffsetY);
                    else
                        MoveTo(this.ScreenOffsetX - KeyboardStepping, this.ScreenOffsetY);

                    KeyEventArgs.Handled = true;
                    break;

                case Key.Down:

                    if (InvertedKeyBoard)
                        MoveTo(this.ScreenOffsetX, this.ScreenOffsetY - KeyboardStepping);
                    else
                        MoveTo(this.ScreenOffsetX, this.ScreenOffsetY + KeyboardStepping);

                    KeyEventArgs.Handled = true;
                    break;

                #endregion

                #region Process + and - keys for zooming the map

                case Key.Add:
                case Key.OemPlus:

                    ZoomInButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

                    KeyEventArgs.Handled = true;
                    break;

                case Key.Subtract:
                case Key.OemMinus:

                    ZoomOutButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    
                    KeyEventArgs.Handled = true;
                    break;

                #endregion

                #region Process other keys...

                case Key.I:

                    InvertedKeyBoard = !InvertedKeyBoard;

                    KeyEventArgs.Handled = true;
                    break;

                case Key.M:

                    LayerPanel1.IsExpaned = !LayerPanel1.IsExpaned;

                    KeyEventArgs.Handled = true;
                    break;

                #endregion

            }

        }

        #endregion


        // Map view

        #region MoveTo(GeoPosition)

        /// <summary>
        /// Move to given position on the map.
        /// </summary>
        /// <param name="GeoPosition">The geographical coordinates to move to.</param>
        public void MoveTo(GeoCoordinate GeoPosition)
        {
            ZoomTo(GeoPosition.Latitude, GeoPosition.Longitude, this.ZoomLevel);
        }

        #endregion

        #region ZoomTo(GeoPosition, ZoomLevel)

        /// <summary>
        /// Zoom into the map onto the given coordinates and zoom level.
        /// </summary>
        /// <param name="GeoPosition">The geographical coordinates to move to.</param>
        /// <param name="ZoomLevel">The desired zoom level.</param>
        public void ZoomTo(GeoCoordinate GeoPosition, UInt32 ZoomLevel)
        {
            ZoomTo(GeoPosition.Latitude, GeoPosition.Longitude, ZoomLevel);
        }

        #endregion

        #region MoveTo(ScreenOffsetX, ScreenOffsetY)

        /// <summary>
        /// Move to given position on the map.
        /// </summary>
        /// <param name="ScreenOffsetX">The new x parameter of the screen offset.</param>
        /// <param name="ScreenOffsetY">The new y parameter of the screen offset.</param>
        public void MoveTo(Int64 ScreenOffsetX, Int64 ScreenOffsetY)
        {

            var MapSizeAtZoomLevel = (Int64) (Math.Pow(2, this.ZoomLevel) * 256);

            this.ScreenOffsetX = ScreenOffsetX % MapSizeAtZoomLevel;
            this.ScreenOffsetY = ScreenOffsetY % MapSizeAtZoomLevel;

            AvoidEndlessVerticalScrolling();

            MapLayers.Values.ForEach(Canvas => Canvas.ZoomTo(this.ZoomLevel, this.ScreenOffsetX, this.ScreenOffsetY));

            #region Send MapViewChanged events

            MapMovements++;

            if (MapViewChanged != null)
                MapViewChanged(this, this.ScreenOffsetX, this.ScreenOffsetY, this.MapMovements);

            #endregion

        }

        #endregion

        #region MoveTo(Latitude, Longitude)

        /// <summary>
        /// Move to given position on the map.
        /// </summary>
        /// <param name="Latitude">The latitude of the zoom center on the map.</param>
        /// <param name="Longitude">The longitude of the zoom center on the map.</param>
        public void MoveTo(Latitude Latitude, Longitude Longitude)
        {
            ZoomTo(Latitude, Longitude, this.ZoomLevel);
        }

        #endregion

        #region ZoomTo(Latitude, Longitude, ZoomLevel)

        /// <summary>
        /// Zoom into the map onto the given coordinates and zoom level.
        /// </summary>
        /// <param name="Latitude">The latitude of the zoom center on the map.</param>
        /// <param name="Longitude">The longitude of the zoom center on the map.</param>
        /// <param name="ZoomLevel">The desired zoom level.</param>
        public void ZoomTo(Latitude Latitude, Longitude Longitude, UInt32 ZoomLevel)
        {

            #region Initial checks

            if (Latitude.Value > 90  || Latitude.Value < -90)
                throw new ArgumentException("Invalid Latitude!");

            if (Longitude.Value > 90 || Longitude.Value < -90)
                throw new ArgumentException("Invalid Longitude!");

            if (ZoomLevel < MinZoomLevel || ZoomLevel > MaxZoomLevel)
                throw new ArgumentException("Invalid zoom level!");

            #endregion

            var OldZoomLevel = this.ZoomLevel;
            this.ZoomLevel   = ZoomLevel;

            var NewOffset = GeoCalculations.WorldCoordinates_2_Screen(Latitude, Longitude, this.ZoomLevel);

            var MapSizeAtZoomLevel = (Int64) (Math.Pow(2, ZoomLevel) * 256);

            this.ScreenOffsetX = (Int64) (-((Int64) NewOffset.Item1) + ForegroundLayer.ActualWidth  / 2);
            this.ScreenOffsetY = (Int64) (-((Int64) NewOffset.Item2) + ForegroundLayer.ActualHeight / 2);

            this.ScreenOffsetX = this.ScreenOffsetX % MapSizeAtZoomLevel;
            this.ScreenOffsetY = this.ScreenOffsetY % MapSizeAtZoomLevel;

            AvoidEndlessVerticalScrolling();

            MapLayers.Values.ForEach(Canvas => Canvas.ZoomTo(this.ZoomLevel, ScreenOffsetX, ScreenOffsetY));

            #region Send MapViewChanged events

            MapMovements++;

            if (MapViewChanged != null)
                MapViewChanged(this, this.ScreenOffsetX, this.ScreenOffsetY, this.MapMovements);

            #endregion

            #region Send ZoomLevelChanged event

            if (this.ZoomLevel != OldZoomLevel && ZoomLevelChanged != null)
                ZoomLevelChanged(this, OldZoomLevel, this.ZoomLevel);

            #endregion

            #region Activate/Deactivate zoom buttons

            if (this.ZoomLevel == MinZoomLevel)
                ZoomOutButton.IsEnabled = false;

            if (this.ZoomLevel  > MinZoomLevel)
                ZoomOutButton.IsEnabled = true;

            if (this.ZoomLevel  < MaxZoomLevel)
                ZoomInButton.IsEnabled  = true;

            if (this.ZoomLevel == MaxZoomLevel)
                ZoomInButton.IsEnabled  = false;

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

            if (this.ZoomLevel < MaxZoomLevel)
            {

                ZoomTo(GeoCalculations.Mouse_2_WorldCoordinates(ScreenX - this.ScreenOffsetX,
                                                                ScreenY - this.ScreenOffsetY,
                                                                this.ZoomLevel),
                       this.ZoomLevel + 1);

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

            if (this.ZoomLevel > MinZoomLevel)
            {

                ZoomTo(GeoCalculations.Mouse_2_WorldCoordinates(ScreenX - this.ScreenOffsetX,
                                                                ScreenY - this.ScreenOffsetY,
                                                                this.ZoomLevel),
                       this.ZoomLevel - 1);

            }

        }

        #endregion


        #region (private) AvoidEndlessVerticalScrolling()

        /// <summary>
        /// Avoid an endless vertical scrolling of the map.
        /// </summary>
        private void AvoidEndlessVerticalScrolling()
        {

            var MapVerticalStart = (Int32) (Math.Pow(2, ZoomLevel) * -256 + this.ActualHeight + 1);

            if (this.ScreenOffsetY < MapVerticalStart)
                this.ScreenOffsetY = MapVerticalStart;

            if (this.ScreenOffsetY > 0)
                this.ScreenOffsetY = 0;

        }

        #endregion


        // Map layer management

        #region AddLayer<T>(Id, ZIndex, Visibility = Visibility.Visible, AddToLayerPanel = true)

        /// <summary>
        /// Create a new map layer of the given type and add it to the map control.
        /// </summary>
        /// <typeparam name="T">The class of the map layer to create.</typeparam>
        /// <param name="Id">The identification of the map layer.</param>
        /// <param name="ZIndex">The z-index of the map layer.</param>
        /// <param name="Visibility">The map layer is visible or not at the start of the application.</param>
        /// <param name="AddToLayerPanel">Wether to add this map layer to the layer panel or not.</param>
        public T AddLayer<T>(String Id, Int32 ZIndex, Visibility Visibility = Visibility.Visible, Boolean AddToLayerPanel = true)
            where T : class, IMapLayer
        {

            // Find the constructor of the map layer
            var _ConstructorInfo = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                                            null,
                                                            new Type[] {
                                                                typeof(String),
                                                                typeof(UInt32),
                                                                typeof(Int64),
                                                                typeof(Int64),
                                                                typeof(MapControl),
                                                                typeof(Int32)
                                                            },
                                                            null);

            if (_ConstructorInfo == null)
                throw new ArgumentException("A appropriate constructor for type '" + typeof(T).Name + "' could not be found!");

            // Invoke the constructor of the map layer
            var _MapLayer = _ConstructorInfo.Invoke(new Object[] { Id, ZoomLevel, ScreenOffsetX, ScreenOffsetY, this, ZIndex }) as IMapLayer;

            if (_MapLayer != null)
                AddLayer(_MapLayer, Visibility, AddToLayerPanel);

            return _MapLayer as T;

        }

        #endregion

        #region AddLayer(Layer, Visibility = Visibility.Visible, AddToLayerPanel = true)

        /// <summary>
        /// Add the given map layer to this map control.
        /// </summary>
        /// <param name="Layer">A map layer.</param>
        /// <param name="Visibility">The map layer is visible or not at the start of the application.</param>
        /// <param name="AddToLayerPanel">Wether to add this map layer to the layer panel or not.</param>
        public IMapLayer AddLayer(IMapLayer Layer, Visibility Visibility = Visibility.Visible, Boolean AddToLayerPanel = true)
        {

            #region Initial checks

            if (Layer == null)
                throw new ApplicationException("The parameter 'Layer' must not be null!");

            var CurrentLayerAsCanvas = Layer as Canvas;

            if (CurrentLayerAsCanvas == null)
                throw new ApplicationException("The parameter 'Layer' must inherit from Canvas!");

            if (Layer.Id == null)
                throw new ApplicationException("The identification of the 'Layer' must be set!");

            if (Layer.MapControl == null)
                throw new ApplicationException("The MapControl of the 'Layer' must be set!");

            #endregion

            #region Add the given feature layer

            LayerGrid.Children.Add(CurrentLayerAsCanvas);
            MapLayers.Add(Layer.Id, Layer);

            CurrentLayerAsCanvas.SetValue(Canvas.ZIndexProperty, Layer.ZIndex);

            #endregion

            #region Add a checkbox entry to the feature layer panel

            if (AddToLayerPanel)
                LayerPanel1.AddLayer(Layer, Visibility);

            #endregion

            return Layer;

        }

        #endregion

    }

}
