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
using System.Windows.Media.Animation;
using System.Windows.Controls;

#endregion

namespace eu.Vanaheimr.Aegir
{

    /// <summary>
    /// Exention methods for working with maps.
    /// </summary>
    public static class AegirWPFExtentionMethods
    {

        #region Animate(this UIElement, Property, From, To, Milliseconds, StartAction = null, FinalAction = null)

        /// <summary>
        /// Animate the change of the given property of the given UIElement.
        /// </summary>
        /// <param name="UIElement">The UIElement to be animated.</param>
        /// <param name="Property">The changing property of the UIElement to be animated.</param>
        /// <param name="From">The start value of the property.</param>
        /// <param name="To">The final value if the property.</param>
        /// <param name="Milliseconds">The duration of the animation in milliseconds.</param>
        /// <param name="StartAction">A delegate to call before the animation starts.</param>
        /// <param name="FinalAction">A delegate to call after the animation has finished.</param>
        public static void Animate(this UIElement         UIElement,
                                        String            Property,
                                        Double            From,
                                        Double            To,
                                        Int32             Milliseconds,
                                        Action<UIElement> StartAction = null,
                                        Action<UIElement> FinalAction = null)

        {

            var Animation = new DoubleAnimation() {
                From      = From,
                To        = To,
                Duration  = new TimeSpan(0, 0, 0, 0, Milliseconds)
            };

            Storyboard.SetTarget(Animation, UIElement);
            Storyboard.SetTargetProperty(Animation, new PropertyPath(Property));

            var storyBoard = new Storyboard();
            storyBoard.Children.Add(Animation);

            if (FinalAction != null)
                storyBoard.Completed += (o, e) =>
                    FinalAction(UIElement);

            if (StartAction != null)
                StartAction(UIElement);

            storyBoard.Begin();

        }

        #endregion


        public static void ForEach<T>(this UIElementCollection Col, Action<T> Action) 
        {

            foreach (var element in Col)
                if (element is T)
                    Action((T) element);

        }

    }

}
