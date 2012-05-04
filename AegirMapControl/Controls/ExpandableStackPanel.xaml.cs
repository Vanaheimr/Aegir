/*
 * Copyright (c) 2010-2012, Achim 'ahzf' Friedland <achim@graph-database.org>
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
using System.Windows.Controls;

#endregion

namespace de.ahzf.Vanaheimr.Aegir.Controls
{

    /// <summary>
    /// Interaction logic for ExpandableStackPanel.xaml
    /// </summary>
    public partial class ExpandableStackPanel : UserControl
    {

        #region Properties

        #region Title

        /// <summary>
        /// The title of the exander panel.
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(String), typeof(ExpandableStackPanel));

        /// <summary>
        /// The title of the exander panel.
        /// </summary>
        public String Title
        {

            get
            {
                return (String) GetValue(TitleProperty);
            }

            set
            {
                SetValue(TitleProperty, value);
            }

        }

        #endregion

        #region IsExpaned

        /// <summary>
        /// Gets or sets whether the internal expander
        /// content window is visible or not.
        /// </summary>
        public Boolean IsExpaned
        {

            get
            {
                return Expander1.IsExpanded;
            }

            set
            {
                Expander1.IsExpanded = value;
            }

        }

        #endregion

        #endregion

        #region ExpandableStackPanel()

        /// <summary>
        /// Create a new expandable stack panel.
        /// </summary>
        public ExpandableStackPanel()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        #endregion


        #region AddUIElement(UIElement)

        /// <summary>
        /// Add the given map layer to this map control.
        /// </summary>
        /// <param name="Layer">A map layer.</param>
        /// <param name="Visibility">The map layer is visible or not at the start of the application.</param>
        public void AddUIElement(UIElement UIElement)
        {

            if (UIElement == null)
                throw new ApplicationException("The parameter 'UIElement' must not be null!");

            InternalStackPanel.Children.Add(UIElement);

        }

        #endregion

    }

}
