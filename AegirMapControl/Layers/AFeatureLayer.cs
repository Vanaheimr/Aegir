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
using System.ComponentModel;

using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;

using de.ahzf.Vanaheimr.Aegir.Controls;

#endregion

namespace de.ahzf.Vanaheimr.Aegir
{

    /// <summary>
    /// The base functionality of all feature layers.
    /// </summary>
    public abstract class AFeatureLayer : Canvas, IFeatureLayer
    {

        #region Data

        protected Int32  ScreenOffsetX;
        protected Int32  ScreenOffsetY;
        protected Int32  DrawingOffset_AtMovementStart_X;
        protected Int32  DrawingOffset_AtMovementStart_Y;

        protected Double LastClickPositionX;
        protected Double LastClickPositionY;
        protected internal UInt32 ZoomLevel;

        protected volatile Boolean IsCurrentlyPainting;

        #endregion

        #region Properties

        #region Id

        /// <summary>
        /// The identification string of this feature layer.
        /// </summary>
        public String Id { get; private set; }

        #endregion

        #region MapControl

        /// <summary>
        /// The hosting map control.
        /// </summary>
        public MapControl MapControl { get; private set; }

        #endregion

        #region ZIndex

        /// <summary>
        /// The z-index of this feature layer.
        /// </summary>
        public Int32 ZIndex { get; private set; }

        #endregion

        #endregion

        #region Events

        #endregion

        #region Constructor(s)

        #region AFeatureLayer()

        /// <summary>
        /// Creates a new feature layer for visualizing map features.
        /// </summary>
        public AFeatureLayer()
        {
            this.SizeChanged += ProcessMapSizeChangedEvent;
        }

        #endregion

        #region AFeatureLayer(Id, ZoomLevel, ScreenOffsetX, ScreenOffsetY, MapControl, ZIndex)

        /// <summary>
        /// Creates a new feature layer for visualizing map features.
        /// </summary>
        /// <param name="Id">The name or identification of this feature layer.</param>
        /// <param name="ZoomLevel">The the zoom level of this feature layer.</param>
        /// <param name="ScreenOffsetX">The x-parameter of the screen offset.</param>
        /// <param name="ScreenOffsetY">The y-parameter of the screen offset.</param>
        /// <param name="MapControl">The hosting map control.</param>
        /// <param name="ZIndex">The z-index of this feature layer.</param>
        public AFeatureLayer(String Id, UInt32 ZoomLevel, Int32 ScreenOffsetX, Int32 ScreenOffsetY, MapControl MapControl, Int32 ZIndex)
            : this()
        {
            this.Id            = Id;
            this.ZoomLevel     = ZoomLevel;
            this.ScreenOffsetX = ScreenOffsetX;
            this.ScreenOffsetY = ScreenOffsetY;
            this.MapControl    = MapControl;
            this.ZIndex        = ZIndex;
        }

        #endregion

        #endregion


        #region (private) ProcessMapSizeChangedEvent(Sender, SizeChangedEventArgs)

        /// <summary>
        /// Whenever the size of the map canvas was changed
        /// this method will be called.
        /// </summary>
        /// <param name="Sender">The sender of the event.</param>
        /// <param name="SizeChangedEventArgs">The event arguments.</param>
        private void ProcessMapSizeChangedEvent(Object Sender, SizeChangedEventArgs SizeChangedEventArgs)
        {
            Redraw();
        }

        #endregion


        #region ProcessMouseLeftButtonDown

        public void ProcessMouseLeftButtonDown(Object Sender, MouseButtonEventArgs MouseButtonEventArgs)
        {

            // We'll need this for when the Form starts to move
            var MousePosition = MouseButtonEventArgs.GetPosition(this);
            LastClickPositionX = MousePosition.X;
            LastClickPositionY = MousePosition.Y;

            DrawingOffset_AtMovementStart_X = ScreenOffsetX;
            DrawingOffset_AtMovementStart_Y = ScreenOffsetY;

            MouseButtonEventArgs.Handled = false;

        }

        #endregion


        #region SetZoomLevel(ZoomLevel)

        /// <summary>
        /// Set the zoom level of this feature layer.
        /// </summary>
        /// <param name="ZoomLevel">The desired zoom level.</param>
        public IFeatureLayer SetZoomLevel(UInt32 ZoomLevel)
        {
            this.ZoomLevel = ZoomLevel;
            Redraw();
            return this;
        }

        #endregion

        #region SetDisplayOffset(OffsetX, OffsetY)

        /// <summary>
        /// Sets this feature layer to the given screen offset.
        /// </summary>
        /// <param name="ScreenOffsetX">The x-parameter of the screen offset.</param>
        /// <param name="ScreenOffsetY">The y-parameter of the screen offset.</param>
        public IFeatureLayer SetDisplayOffset(Int32 ScreenOffsetX, Int32 ScreenOffsetY)
        {
            this.ScreenOffsetX = ScreenOffsetX;
            this.ScreenOffsetY = ScreenOffsetY;
            Redraw();
            return this;
        }

        #endregion

        #region (virtual) Redraw()

        /// <summary>
        /// Redraws this feature layer.
        /// </summary>
        /// <returns>True if the map was repainted; false otherwise.</returns>
        public virtual Boolean Redraw()
        {

            if (this.Visibility == Visibility.Visible && !IsCurrentlyPainting)
            {

                IsCurrentlyPainting = true;

                if (!DesignerProperties.GetIsInDesignMode(this))
                {

                    Feature Feature;
                    Tuple<UInt32, UInt32> XY;

                    foreach (var Child in this.Children)
                    {

                        Feature = Child as Feature;

                        if (Feature != null)
                        {
                            XY = GeoCalculations.WorldCoordinates_2_Screen(Feature.Latitude, Feature.Longitude, (Int32) ZoomLevel);
                            Canvas.SetLeft(Feature, ScreenOffsetX + XY.Item1);
                            Canvas.SetTop(Feature, ScreenOffsetY + XY.Item2);
                        }

                    }

                }

                IsCurrentlyPainting = false;

                return true;

            }

            return false;

        }

        #endregion


        #region AddFeature

        public abstract Feature AddFeature(String Name, Double Latitude, Double Longitude, Double Width, Double Height, Color Color);

        #endregion


        #region IComparable<Identifier> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException("The given object must not be null!");

            // Check if the given object is a feature layer identifier.
            var FeatureLayer = Object as IFeatureLayer;
            if ((Object) FeatureLayer == null)
                throw new ArgumentException("The given object is not an IFeatureLayer!");

            return this.Id.CompareTo(FeatureLayer.Id);

        }

        #endregion

        #region CompareTo(Identifier)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Identifier">An object to compare with.</param>
        public Int32 CompareTo(String Identifier)
        {

            if ((Object) Identifier == null)
                throw new ArgumentNullException("The given feature layer identifier must not be null!");

            return this.Id.CompareTo(Identifier);

        }

        #endregion

        #region CompareTo(FeatureLayer)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Identifier">An object to compare with.</param>
        public Int32 CompareTo(IFeatureLayer FeatureLayer)
        {

            if ((Object) FeatureLayer == null)
                throw new ArgumentNullException("The given feature layer must not be null!");

            return this.Id.CompareTo(FeatureLayer.Id);

        }

        #endregion

        #endregion

        #region IEquatable<Identifier> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public new Boolean Equals(Object Object)
        {

            if (Object == null)
                return false;

            // Check if the given object is a feature layer.
            var FeatureLayer = Object as IFeatureLayer;
            if ((Object)FeatureLayer == null)
                return false;

            return this.Id.Equals(FeatureLayer.Id);

        }

        #endregion

        #region Equals(Identifier)

        /// <summary>
        /// Compares two feature layer identifiers for equality.
        /// </summary>
        /// <param name="Identifier">An identifier to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(String Identifier)
        {

            if ((Object) Identifier == null || Identifier == "")
                return false;

            return this.Id.Equals(Identifier);

        }

        #endregion

        #region Equals(FeatureLayer)

        /// <summary>
        /// Compares two feature layers for equality.
        /// </summary>
        /// <param name="FeatureLayer">A feature layer to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(IFeatureLayer FeatureLayer)
        {

            if ((Object) FeatureLayer == null)
                return false;

            return this.Id.Equals(FeatureLayer.Id);

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Returns the hash code for this feature layer.
        /// </summary>
        public new Int32 GetHashCode()
        {
            return Id.GetHashCode();
        }

        #endregion

    }

}
