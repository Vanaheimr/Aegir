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
using System.Windows;
using System.Windows.Controls;

#endregion

namespace eu.Vanaheimr.Aegir.Controls
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
