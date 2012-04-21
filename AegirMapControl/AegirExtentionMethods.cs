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
using System.Windows.Media.Animation;

#endregion

namespace de.ahzf.Vanaheimr.Aegir
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

    }

}
