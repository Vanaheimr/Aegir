﻿/*
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
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;

using de.ahzf.Illias.Commons;
using de.ahzf.Vanaheimr.Aegir.Tiles;

#endregion

namespace de.ahzf.Vanaheimr.Aegir.Controls
{

    /// <summary>
    /// Interaction logic for MapControl.xaml
    /// </summary>
    public partial class MapControl : UserControl
    {

        #region Data

        protected UInt64 MapMoves;

        private StackPanel LayerPanel;

        private Int64 ScreenOffsetX;
        private Int64 ScreenOffsetY;
        private Int64 ScreenOffset_AtMovementStart_X;
        private Int64 ScreenOffset_AtMovementStart_Y;

        private Double LastClickPositionX;
        private Double LastClickPositionY;

        private Double LastMousePositionDuringMovementX;
        private Double LastMousePositionDuringMovementY;

        public const UInt32 MinZoomLevel = 1;
        public const UInt32 MaxZoomLevel = 23;

        private readonly Dictionary<String, ILayer> MapLayers;

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

            //set
            //{
            //    _ZoomLevel = value;
            //    MapLayers.Values.ForEach(Canvas => Canvas.SetZoomLevel(value));
            //}

        }

        #endregion

        //#region MapProvider

        ///// <summary>
        ///// The map tiles provider for this map.
        ///// </summary>
        //public String MapProvider
        //{
            
        //    //get
        //    //{
        //    //    return TilesCanvas.MapProvider;
        //    //}

        //    set
        //    {
        //        if (value != null && value != "")
        //            TilesCanvas.MapProvider = value;
        //    }

        //}

        //#endregion

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

        #region DisplayOffsetChanged

        /// <summary>
        /// An event handler getting fired whenever the
        /// display offset of the map changed.
        /// </summary>
        public delegate void DisplayOffsetChangedEventHandler(MapControl Sender, Int64 X, Int64 Y);

        /// <summary>
        /// An event getting fired whenever the display offset
        /// of the map changed.
        /// </summary>
        public event DisplayOffsetChangedEventHandler DisplayOffsetChanged;

        #endregion

        //#region MapProviderChanged

        ///// <summary>
        ///// An event getting fired whenever the map provider
        ///// of the map changed.
        ///// </summary>
        //public event TilesLayer.MapProviderChangedEventHandler MapProviderChanged
        //{

        //    add
        //    {
        //        this.TilesCanvas.MapProviderChanged += value;
        //    }

        //    remove
        //    {
        //        this.TilesCanvas.MapProviderChanged -= value;
        //    }

        //}

        //#endregion

        #region MapMoved

        /// <summary>
        /// An event handler getting fired whenever the
        /// map provider of the map changed.
        /// </summary>
        public delegate void MapMovedEventHandler(MapControl Sender, UInt64 Movements);

        /// <summary>
        /// An event getting fired whenever the map provider
        /// of the map changed.
        /// </summary>
        public event MapMovedEventHandler MapMoved;

        #endregion

        #endregion

        #region MapControl()

        /// <summary>
        /// Initialize the MapControl component.
        /// </summary>
        public MapControl()
        {

            InitializeComponent();

            this.MapLayers               = new Dictionary<String, ILayer>();
            this.ZoomOutButton.IsEnabled = false;
            this._ZoomLevel              = MinZoomLevel;

            #region Create and add the feature layer panel

            this.LayerPanel = new StackPanel() {
                Background = new SolidColorBrush(Colors.Gray),
                Opacity    = 0.75
            };

            ForegroundLayer.Children.Add(LayerPanel);
            Canvas.SetRight(LayerPanel, 10);
            Canvas.SetTop  (LayerPanel, 10);

            #endregion

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


        #region ProcessMouseMove(Sender, MouseEventArgs)

        /// <summary>
        /// The mouse was moved above all canvas.
        /// </summary>
        /// <param name="Sender">The sender of the event.</param>
        /// <param name="MouseEventArgs">The mouse event arguments.</param>
        public void ProcessMouseMove(Object Sender, MouseEventArgs MouseEventArgs)
        {

            Int32 MapSizeAtZoomlevel;
            var MousePosition = MouseEventArgs.GetPosition(this);

            if (LastMousePositionDuringMovementX != MousePosition.X ||
                LastMousePositionDuringMovementY != MousePosition.Y)
            {

                #region Send GeoPositionChanged event

                if (GeoPositionChanged != null)
                {
                    GeoPositionChanged(this, GeoCalculations.Mouse_2_WorldCoordinates(MousePosition.X - ScreenOffsetX,
                                                                                      MousePosition.Y - ScreenOffsetY,
                                                                                      ZoomLevel));
                }

                #endregion


                var NewMapCenter = GeoCalculations.Mouse_2_WorldCoordinates(MousePosition.X - ScreenOffsetX,
                                                                            MousePosition.Y - ScreenOffsetY,
                                                                            _ZoomLevel);

                var NewOffset = GeoCalculations.WorldCoordinates_2_Screen(NewMapCenter.Latitude, NewMapCenter.Longitude, _ZoomLevel);


                #region The left mouse button is still pressed => dragging the map!

                if (MouseEventArgs.LeftButton == MouseButtonState.Pressed)
                {

                    MapSizeAtZoomlevel = (Int32)(Math.Pow(2, ZoomLevel) * 256);

                    ScreenOffset_AtMovementStart_X = ScreenOffset_AtMovementStart_X % MapSizeAtZoomlevel;
                    ScreenOffset_AtMovementStart_Y = ScreenOffset_AtMovementStart_Y % MapSizeAtZoomlevel;

                    ScreenOffsetX = (Int32) (Math.Round(ScreenOffset_AtMovementStart_X + MousePosition.X - LastClickPositionX) % MapSizeAtZoomlevel);
                    ScreenOffsetY = (Int32) (Math.Round(ScreenOffset_AtMovementStart_Y + MousePosition.Y - LastClickPositionY) % MapSizeAtZoomlevel);

                    #region Avoid endless vertical scrolling

                    var MapVerticalStart = (Int32)(-MapSizeAtZoomlevel + this.ActualHeight + 1);

                    if (ScreenOffsetY < MapVerticalStart)
                        ScreenOffsetY = MapVerticalStart;

                    if (ScreenOffsetY > 0)
                        ScreenOffsetY = 0;

                    #endregion

                    if (DisplayOffsetChanged != null)
                        DisplayOffsetChanged(this, ScreenOffsetX, ScreenOffsetY);

                    MapMoves++;

                    if (MapMoved != null)
                        MapMoved(this, MapMoves);

                    //TilesCanvas.SetDisplayOffset(ScreenOffsetX, ScreenOffsetY);
                    MapLayers.Values.ForEach(Canvas => Canvas.SetDisplayOffset(ScreenOffsetX, ScreenOffsetY));

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

            //MapLayers.Values.ForEach(Canvas => Canvas.ProcessMouseLeftButtonDown(Sender, MouseButtonEventArgs));

        }

        #endregion

        #region ProcessMouseLeftDoubleClick(Sender, MouseButtonEventArgs)

        /// <summary>
        /// The left mouse button was double clicked (from: PreviewMouseLeftButtonDown).
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
        /// The mouse wheel was moved.
        /// </summary>
        /// <param name="Sender">The sender of the event.</param>
        /// <param name="MouseWheelEventArgs">The mouse wheel event arguments.</param>
        public void ProcessMouseWheel(Object Sender, MouseWheelEventArgs MouseWheelEventArgs)
        {

            // Zoom in/out at the given mouse position
            var MousePosition = MouseWheelEventArgs.GetPosition(this);

            if (MouseWheelEventArgs.Delta < 0)
                ZoomOut(MousePosition.X, MousePosition.Y);

            else if (MouseWheelEventArgs.Delta > 0)
                ZoomIn (MousePosition.X, MousePosition.Y);

        }

        #endregion


        #region MoveTo(GeoPosition, ZoomLevel)

        /// <summary>
        /// Move to given position on the map.
        /// </summary>
        /// <param name="GeoPosition">The geographical coordinates to move to.</param>
        public void MoveTo(GeoCoordinate GeoPosition)
        {
            ZoomTo(GeoPosition.Latitude, GeoPosition.Longitude, _ZoomLevel);
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

        #region MoveTo(Latitude, Longitude, ZoomLevel)

        /// <summary>
        /// Move to given position on the map.
        /// </summary>
        /// <param name="Latitude">The latitude of the zoom center on the map.</param>
        /// <param name="Longitude">The longitude of the zoom center on the map.</param>
        public void MoveTo(Double Latitude, Double Longitude)
        {
            ZoomTo(Latitude, Longitude, _ZoomLevel);
        }

        #endregion

        #region ZoomTo(Latitude, Longitude, ZoomLevel)

        /// <summary>
        /// Zoom into the map onto the given coordinates and zoom level.
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

            var OldZoomLevel = _ZoomLevel;
            _ZoomLevel = ZoomLevel;

            var NewOffset = GeoCalculations.WorldCoordinates_2_Screen(Latitude, Longitude, _ZoomLevel);

            var MapSizeAtZoomLevel = (Int64) (Math.Pow(2, ZoomLevel) * 256);

            ScreenOffsetX = (Int64) (-((Int64) NewOffset.Item1) + ForegroundLayer.ActualWidth  / 2);
            ScreenOffsetY = (Int64) (-((Int64) NewOffset.Item2) + ForegroundLayer.ActualHeight / 2);

            ScreenOffsetX = ScreenOffsetX % MapSizeAtZoomLevel;
            ScreenOffsetY = ScreenOffsetY % MapSizeAtZoomLevel;

            if (DisplayOffsetChanged != null)
                DisplayOffsetChanged(this, ScreenOffsetX, ScreenOffsetY);

            MapLayers.Values.ForEach(Canvas => Canvas.ZoomTo(_ZoomLevel, ScreenOffsetX, ScreenOffsetY));

            if (ZoomLevelChanged != null)
                ZoomLevelChanged(this, OldZoomLevel, _ZoomLevel);

            if (_ZoomLevel == MaxZoomLevel)
                ZoomInButton.IsEnabled = false;

            if (_ZoomLevel > MinZoomLevel)
                ZoomOutButton.IsEnabled = true;

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

                ZoomTo(GeoCalculations.Mouse_2_WorldCoordinates(ScreenX - ScreenOffsetX,
                                                                ScreenY - ScreenOffsetY,
                                                                _ZoomLevel), _ZoomLevel + 1);

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

                ZoomTo(GeoCalculations.Mouse_2_WorldCoordinates(ScreenX - ScreenOffsetX,
                                                                ScreenY - ScreenOffsetY,
                                                                _ZoomLevel), _ZoomLevel - 1);

            }

        }

        #endregion


        #region AddLayers

        #region AddLayer<T>(Id, ZIndex, AddToPanel = true)

        /// <summary>
        /// Create a new map layer of the given type and add it to the map control.
        /// </summary>
        /// <typeparam name="T">The class of the map layer to create.</typeparam>
        /// <param name="Id">The identification of the map layer.</param>
        /// <param name="ZIndex">The z-index of the map layer.</param>
        /// <param name="AddToPanel">Wether to add this map layer to the layer panel or not.</param>
        public T AddLayer<T>(String Id, Int32 ZIndex, Boolean AddToPanel = true)
            where T : class, ILayer
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
            var _Layer = _ConstructorInfo.Invoke(new Object[] { Id, ZoomLevel, ScreenOffsetX, ScreenOffsetY, this, ZIndex }) as ILayer;

            if (_Layer != null)
                AddLayer(_Layer, AddToPanel);

            return _Layer as T;

        }

        #endregion

        #region AddLayer(Layer, AddToPanel = true)

        /// <summary>
        /// Add the given map layer to this map control.
        /// </summary>
        /// <param name="Layer">A map layer.</param>
        /// <param name="AddToPanel">Wether to add this map layer to the layer panel or not.</param>
        public ILayer AddLayer(ILayer Layer, Boolean AddToPanel = true)
        {

            #region Initial checks

            if (Layer == null)
                throw new ApplicationException("The parameter 'FeatureLayer' must not be null!");

            var FeatureCanvas = Layer as Canvas;

            if (FeatureCanvas == null)
                throw new ApplicationException("The parameter 'FeatureLayer' must inherit from Canvas!");

            if (Layer.Id == null)
                throw new ApplicationException("The identification of the 'FeatureLayer' must be set!");

            if (Layer.MapControl == null)
                throw new ApplicationException("The MapControl of the 'FeatureLayer' must be set!");

            #endregion

            #region Add the given feature layer

            LayerGrid.Children.Add(FeatureCanvas);
            MapLayers.Add(Layer.Id, Layer);

            FeatureCanvas.SetValue(Canvas.ZIndexProperty, Layer.ZIndex);

            #endregion

            #region Add a checkbox entry to the feature layer panel

            if (AddToPanel)
            {

                var Checkbox = new CheckBox();
                Checkbox.Content   = Layer.Id;
                Checkbox.IsChecked = true;

                Checkbox.MouseEnter += (o, e) =>
                    {
                        var _CheckBox = o as CheckBox;
                        _CheckBox.Foreground = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
                        _CheckBox.Background = new SolidColorBrush(Colors.White);
                    };

                Checkbox.MouseLeave += (o, e) =>
                    {
                        var _CheckBox = o as CheckBox;
                        _CheckBox.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                        _CheckBox.Background = new SolidColorBrush(Colors.Gray);
                    };

                Checkbox.Click += (o, e) =>
                    {

                        var _CheckBox = o as CheckBox;

                        if (_CheckBox.IsChecked.Value)
                        {
                            FeatureCanvas.Visibility = Visibility.Visible;
                            Layer.Redraw();
                        }
                        else
                            FeatureCanvas.Visibility = Visibility.Hidden;

                    };

                LayerPanel.Children.Add(Checkbox);
            
            }

            #endregion

            return Layer;

        }

        #endregion

        #endregion

    }

}
