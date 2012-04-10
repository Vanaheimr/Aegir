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

#endregion

namespace de.Vanaheimr.Aegir.Controls
{

    /// <summary>
    /// Interaction logic for MapControl.xaml
    /// </summary>
    public partial class MapControl : UserControl
    {

        #region MapControl()

        /// <summary>
        /// Initialize the MapControl component.
        /// </summary>
        public MapControl()
        {
            InitializeComponent();
        }

        #endregion



        private void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {
            MapCanvas1.ZoomIn();
        }

        private void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            MapCanvas1.ZoomOut();
        }

        private void MappingCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MapCanvas1.SizeChangedEvent(sender, e);
        }



        #region canvas1_MouseLeftButtonDown

        private void canvas1_MouseLeftButtonDown(object sender, MouseButtonEventArgs eventArgs)
        {
            MapCanvas1.canvas1_MouseLeftButtonDown(sender, eventArgs);
        }

        #endregion

        #region canvas1_MouseMove

        private void AllCanvas_MouseMove(Object sender, MouseEventArgs eventArgs)
        {
            MapCanvas1.AllCanvas_MouseMove(sender, eventArgs);
        }

        #endregion


        
    }

}
