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
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

#endregion

namespace de.ahzf.Vanaheimr.Aegir.Controls
{

    /// <summary>
    /// An expandable stack panel for management of
    /// the mapping layers.
    /// </summary>
    public class LayerPanel : ExpandableStackPanel
    {

        #region AddLayer(Layer, Visibility = Visibility.Visible)

        /// <summary>
        /// Add the given map layer to this expandable stack panel.
        /// </summary>
        /// <param name="Layer">A map layer.</param>
        /// <param name="Visibility">The map layer is visible or not at the start of the application.</param>
        public void AddLayer(IMapLayer Layer, Visibility Visibility = Visibility.Visible)
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
            
            var Checkbox = new CheckBox();
            Checkbox.Content   = Layer.Id;
            Checkbox.IsChecked = Visibility == Visibility.Visible;
            Layer.Visibility   = Visibility;

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
                        CurrentLayerAsCanvas.Visibility = Visibility.Visible;
                        Layer.Redraw();
                    }
                    else
                        CurrentLayerAsCanvas.Visibility = Visibility.Hidden;

                };

            base.AddUIElement(Checkbox);

        }

        #endregion

    }

}
