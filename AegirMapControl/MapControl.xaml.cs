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

        #endregion

        #region Properties

        #region MapProvider

        /// <summary>
        /// The map tiles provider for this map.
        /// </summary>
        public String MapProvider
        {
            
            get
            {
                return MapCanvas1.MapProvider;
            }

            set
            {
                if (value != null && value != "")
                    MapCanvas1.MapProvider = value;
            }

        }

        #endregion

        #endregion

        #region Events

        #region GeoPositionChanged

        /// <summary>
        /// An event getting fired whenever the position of the mouse
        /// on the map changes.
        /// </summary>
        public event MappingCanvas.GeoPositionChangedEventHandler GeoPositionChanged
        {

            add
            {
                this.MapCanvas1.GeoPositionChanged += value;
            }

            remove
            {
                this.MapCanvas1.GeoPositionChanged -= value;
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
            ChangeMapProvider(MapCanvas1.MapProvider);
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
            MapCanvas1.ZoomIn();
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
            MapCanvas1.ZoomOut();
        }

        #endregion


        #region (private) AllCanvas_MouseMove(Sender, MouseEventArgs)

        /// <summary>
        /// The mouse was moved above all canvas.
        /// </summary>
        /// <param name="Sender">The sender of the event.</param>
        /// <param name="MouseEventArgs">The mouse event arguments.</param>
        private void AllCanvas_MouseMove(Object Sender, MouseEventArgs MouseEventArgs)
        {
            MapCanvas1.AllCanvas_MouseMove(Sender, MouseEventArgs);
        }

        #endregion

        #region (private) MapCanvas_MouseLeftButtonDown

        /// <summary>
        /// The mouse was moced above all canvas.
        /// </summary>
        /// <param name="Sender">The sender of the event.</param>
        /// <param name="MouseButtonEventArgs">The mouse button event arguments.</param>
        private void MapCanvas_MouseLeftButtonDown(Object Sender, MouseButtonEventArgs MouseButtonEventArgs)
        {
            MapCanvas1.canvas1_MouseLeftButtonDown(Sender, MouseButtonEventArgs);
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

            if (MapProviderName != null && MapProviderName != "")
            {

                MapCanvas1.MapProvider = MapProviderName;

                foreach (var item in this.ContextMenu.Items)
                {
                    var CurrentMenuItem = item as MenuItem;
                    CurrentMenuItem.IsChecked = (CurrentMenuItem.HeaderStringFormat == MapProviderName);
                }

            }

        }

        #endregion


        #region (private) MappingCanvas_SizeChanged(Sender, SizeChangedEventArgs)

        /// <summary>
        /// The size of the mapping canvas has changed.
        /// </summary>
        /// <param name="Sender">The sender of the event.</param>
        /// <param name="SizeChangedEventArgs">The size changed event arguments.</param>
        private void MappingCanvas_SizeChanged(Object Sender, SizeChangedEventArgs SizeChangedEventArgs)
        {
            MapCanvas1.SizeChangedEvent(Sender, SizeChangedEventArgs);
        }

        #endregion

    }

}
