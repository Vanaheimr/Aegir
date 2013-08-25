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

        public const UInt32 MinZoomLevel =  1;
        public const UInt32 MaxZoomLevel = 23;

        public  Point  ScreenOffset                     { get; private set; }
        public  Point  ScreenOffset_AtMovementStart     { get; private set; }

        private Point  LastMouseClickPosition;
        private Point  LastMousePositionDuringMovement;

        /// <summary>
        /// The stepping when moving the map
        /// via the keyboard arrow keys
        /// </summary>
        private readonly UInt32 KeyboardStepping;

        private readonly Dictionary<String, AMapLayer> MapLayers;

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

        #region MapCenter

        /// <summary>
        /// The geo coordinates of the map center;
        /// </summary>
        public GeoCoordinate MapCenter { get; private set; }

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
        public delegate void MapViewChangedEventHandler(MapControl Sender, Point ScreenOffset, UInt64 Movements);

        /// <summary>
        /// An event getting fired whenever the view
        /// of the map changed.
        /// </summary>
        public event MapViewChangedEventHandler MapViewChanged;

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initialize the Aegir map control.
        /// </summary>
        public MapControl()
        {

            InitializeComponent();

            this.MapLayers                = new Dictionary<String, AMapLayer>();
            this.ZoomOutButton.IsEnabled  = false;
            this.ZoomLevel                = MinZoomLevel;
            this.Focusable                = true;
            this.KeyboardStepping         = 50;
            this.InvertedKeyBoard         = false;

            this.KeyDown                 += new KeyEventHandler(ProcessKeyboardEvents);

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

            if (this.ActualHeight != 0 && this.ActualWidth != 00)
                ZoomIn(this.ActualWidth / 2, this.ActualHeight / 2);

            RoutedEventArgs.Handled = true;

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

            if (this.ActualHeight != 0 && this.ActualWidth != 00)
                ZoomOut(this.ActualWidth / 2, this.ActualHeight / 2);

            RoutedEventArgs.Handled = true;

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

            if (LastMousePositionDuringMovement.X != MousePosition.X ||
                LastMousePositionDuringMovement.Y != MousePosition.Y)
            {

                #region The left mouse button is pressed => drag the map!

                if (MouseEventArgs.LeftButton == MouseButtonState.Pressed)
                {
                    if (Sender == BackgroundLayer &&
                        LastMouseClickPosition.X != 0 &&
                        LastMouseClickPosition.Y != 0)
                    {

                        MapSizeAtZoomLevel = (Int64) (Math.Pow(2, ZoomLevel) * 256);

                        this.ScreenOffset_AtMovementStart = new Point(this.ScreenOffset_AtMovementStart.X % MapSizeAtZoomLevel,
                                                                      this.ScreenOffset_AtMovementStart.Y % MapSizeAtZoomLevel);

                        this.ScreenOffset = new Point((Math.Round(ScreenOffset_AtMovementStart.X + MousePosition.X - LastMouseClickPosition.X) % MapSizeAtZoomLevel),
                                                      (Math.Round(ScreenOffset_AtMovementStart.Y + MousePosition.Y - LastMouseClickPosition.Y) % MapSizeAtZoomLevel));

                        AvoidEndlessVerticalScrolling();

                        MapCenter = GeoCalculations.Mouse2GeoCoordinate(MousePosition.X - this.ScreenOffset.X,
                                                                        MousePosition.Y - this.ScreenOffset.Y,
                                                                        this.ZoomLevel);

                        MapLayers.Values.ForEach(MapLayer => MapLayer.Move(MousePosition.X - LastMousePositionDuringMovement.X,
                                                                           MousePosition.Y - LastMousePositionDuringMovement.Y));

                        MapLayers.Values.ForEach(MapLayer => MapLayer.Redraw());

                        #region Send MapViewChanged events

                        MapMovements++;

                        if (MapViewChanged != null)
                            MapViewChanged(this, ScreenOffset, this.MapMovements);

                        #endregion

                    }
                }

                else
                {
                    LastMouseClickPosition.X = 0;
                    LastMouseClickPosition.Y = 0;
                }

                #endregion

                #region Send current mouse position as geo position

                if (GeoPositionChanged != null)
                {

                    GeoPositionChanged(this, GeoCalculations.Mouse2GeoCoordinate(MousePosition.X - this.ScreenOffset.X,
                                                                                 MousePosition.Y - this.ScreenOffset.Y,
                                                                                 ZoomLevel));

                }

                #endregion

                LastMousePositionDuringMovement = MousePosition;

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

            if (Sender == BackgroundLayer)
            {

                LastMouseClickPosition        = MouseButtonEventArgs.GetPosition(this);
                ScreenOffset_AtMovementStart  = ScreenOffset;

                this.Focus();

            }

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
                        MoveTo(this.ScreenOffset.X - KeyboardStepping, this.ScreenOffset.Y);
                    else
                        MoveTo(this.ScreenOffset.X + KeyboardStepping, this.ScreenOffset.Y);

                    KeyEventArgs.Handled = true;
                    break;

                case Key.Up:

                    if (InvertedKeyBoard)
                        MoveTo(this.ScreenOffset.X, this.ScreenOffset.Y + KeyboardStepping);
                    else
                        MoveTo(this.ScreenOffset.X, this.ScreenOffset.Y - KeyboardStepping);

                    KeyEventArgs.Handled = true;
                    break;

                case Key.Right:

                    if (InvertedKeyBoard)
                        MoveTo(this.ScreenOffset.X + KeyboardStepping, this.ScreenOffset.Y);
                    else
                        MoveTo(this.ScreenOffset.X - KeyboardStepping, this.ScreenOffset.Y);

                    KeyEventArgs.Handled = true;
                    break;

                case Key.Down:

                    if (InvertedKeyBoard)
                        MoveTo(this.ScreenOffset.X, this.ScreenOffset.Y - KeyboardStepping);
                    else
                        MoveTo(this.ScreenOffset.X, this.ScreenOffset.Y + KeyboardStepping);

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
        public void MoveTo(Double ScreenOffsetX, Double ScreenOffsetY)
        {

            var MapSizeAtZoomLevel = (Int64) (Math.Pow(2, this.ZoomLevel) * 256);

            this.ScreenOffset = new Point(ScreenOffsetX % MapSizeAtZoomLevel,
                                          ScreenOffsetY % MapSizeAtZoomLevel);

            AvoidEndlessVerticalScrolling();

//            MapLayers.Values.ForEach(Canvas => Canvas.ZoomTo(this.ZoomLevel, this.ScreenOffsetX, this.ScreenOffsetY));
            MapLayers.Values.ForEach(MapLayer => MapLayer.Redraw());

            #region Send MapViewChanged events

            MapMovements++;

            if (MapViewChanged != null)
                MapViewChanged(this, this.ScreenOffset, this.MapMovements);

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
            var NewOffset    = GeoCalculations.GeoCoordinate2ScreenXY(Latitude, Longitude, ZoomLevel);

            var MapSizeAtZoomLevel = (Int64) (Math.Pow(2, ZoomLevel) * 256);

            this.ScreenOffset = new Point((-((Int64) NewOffset.X) + ForegroundLayer.ActualWidth  / 2) % MapSizeAtZoomLevel,
                                          (-((Int64) NewOffset.Y) + ForegroundLayer.ActualHeight / 2) % MapSizeAtZoomLevel);

            AvoidEndlessVerticalScrolling();

            MapLayers.Values.ForEach(MapLayer => MapLayer.Redraw());

            #region Send MapViewChanged events

            MapMovements++;

            if (MapViewChanged != null)
                MapViewChanged(this, this.ScreenOffset, this.MapMovements);

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

                ZoomTo(GeoCalculations.Mouse2GeoCoordinate(ScreenX - this.ScreenOffset.X,
                                                           ScreenY - this.ScreenOffset.Y,
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

                ZoomTo(GeoCalculations.Mouse2GeoCoordinate(ScreenX - this.ScreenOffset.X,
                                                           ScreenY - this.ScreenOffset.Y,
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

            if (this.ScreenOffset.Y < MapVerticalStart)
                this.ScreenOffset = new Point(this.ScreenOffset.X, MapVerticalStart);

            if (this.ScreenOffset.Y > 0)
                this.ScreenOffset = new Point(this.ScreenOffset.X, 0);

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
            where T : AMapLayer
        {

            // Find the constructor of the map layer
            var _ConstructorInfo = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                                            null,
                                                            new Type[] {
                                                                typeof(String),
                                                                typeof(MapControl),
                                                                typeof(Int32)
                                                            },
                                                            null);

            if (_ConstructorInfo == null)
                throw new ArgumentException("A appropriate constructor for type '" + typeof(T).Name + "' could not be found!");

            // Invoke the constructor of the map layer
            var _MapLayer = _ConstructorInfo.Invoke(new Object[] { Id, this, ZIndex }) as AMapLayer;

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
        public AMapLayer AddLayer(AMapLayer Layer, Visibility Visibility = Visibility.Visible, Boolean AddToLayerPanel = true)
        {

            #region Initial checks

            if (Layer == null)
                throw new ApplicationException("The parameter 'Layer' must not be null!");

            var CurrentLayerAsCanvas = Layer as Canvas;

            if (CurrentLayerAsCanvas == null)
                throw new ApplicationException("The parameter 'Layer' must inherit from Canvas!");

            if (Layer.Id == null)
                throw new ApplicationException("The identification of the 'Layer' must be set!");

            if (Layer.MapControl != this)
                throw new ApplicationException("The MapControl of the 'Layer' is invalid!");

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

        #region ContainsLayer(MapLayer)

        /// <summary>
        /// Checks if the given map is present within the map control.
        /// </summary>
        /// <param name="MapLayer">A map layer.</param>
        public Boolean ContainsLayerId(AMapLayer MapLayer)
        {
            return MapLayers.ContainsKey(MapLayer.Id);
        }

        #endregion

        #region ContainsLayerId(Id)

        /// <summary>
        /// Checks if the given map id is present within the map control.
        /// </summary>
        /// <param name="Id">The identification of the map layer.</param>
        public Boolean ContainsLayerId(String Id)
        {
            return MapLayers.ContainsKey(Id);
        }

        #endregion

    }

}
