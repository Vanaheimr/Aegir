/*
 * Copyright (c) 2010-2013, Achim 'ahzf' Friedland <achim@graph-database.org>
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
    /// The base functionality of all map layers.
    /// </summary>
    public abstract class AMapLayer : Canvas, IMapLayer
    {

        #region Data

        protected Int64  ScreenOffsetX;
        protected Int64  ScreenOffsetY;
        protected Int64  DrawingOffset_AtMovementStart_X;
        protected Int64  DrawingOffset_AtMovementStart_Y;

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
        public Int32 ZIndex { get; protected set; }

        #endregion

        #endregion

        #region Constructor(s)

        #region AMapLayer()

        /// <summary>
        /// Creates a new feature layer for visualizing map features.
        /// </summary>
        public AMapLayer()
        {
            this.SizeChanged += ProcessMapSizeChangedEvent;
        }

        #endregion

        #region AMapLayer(Id, ZoomLevel, ScreenOffsetX, ScreenOffsetY, MapControl, ZIndex)

        /// <summary>
        /// Creates a new feature layer for visualizing map features.
        /// </summary>
        /// <param name="Id">The name or identification of this feature layer.</param>
        /// <param name="ZoomLevel">The the zoom level of this feature layer.</param>
        /// <param name="ScreenOffsetX">The x-parameter of the screen offset.</param>
        /// <param name="ScreenOffsetY">The y-parameter of the screen offset.</param>
        /// <param name="MapControl">The hosting map control.</param>
        /// <param name="ZIndex">The z-index of this feature layer.</param>
        public AMapLayer(String Id, UInt32 ZoomLevel, Int64 ScreenOffsetX, Int64 ScreenOffsetY, MapControl MapControl, Int32 ZIndex)
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


        #region ZoomTo(ZoomLevel, ScreenOffsetX, ScreenOffsetY)

        /// <summary>
        /// Set the zoom level and screen offset of this map layer.
        /// </summary>
        /// <param name="ZoomLevel">The desired zoom level.</param>
        /// <param name="ScreenOffsetX">The x-parameter of the screen offset.</param>
        /// <param name="ScreenOffsetY">The y-parameter of the screen offset.</param>
        public IMapLayer ZoomTo(UInt32 ZoomLevel, Int64 ScreenOffsetX, Int64 ScreenOffsetY)
        {

            this.ZoomLevel     = ZoomLevel;
            this.ScreenOffsetX = ScreenOffsetX;
            this.ScreenOffsetY = ScreenOffsetY;

            Redraw();

            return this;

        }

        #endregion

        #region SetDisplayOffset(OffsetX, OffsetY)

        /// <summary>
        /// Set the screen offset of this map layer.
        /// </summary>
        /// <param name="ScreenOffsetX">The x-parameter of the screen offset.</param>
        /// <param name="ScreenOffsetY">The y-parameter of the screen offset.</param>
        public IMapLayer SetDisplayOffset(Int64 ScreenOffsetX, Int64 ScreenOffsetY)
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

            if (this.IsVisible && !IsCurrentlyPainting)
            {

                IsCurrentlyPainting = true;

                if (!DesignerProperties.GetIsInDesignMode(this))
                {

                    Feature Feature;
                    Tuple<UInt64, UInt64> XY;

                    foreach (var Child in this.Children)
                    {

                        Feature = Child as Feature;

                        if (Feature != null)
                        {
                            
                            XY = GeoCalculations.WorldCoordinates_2_Screen(Feature.Latitude, Feature.Longitude, ZoomLevel);
                            
                            Canvas.SetLeft(Feature, ScreenOffsetX + (Int64) XY.Item1);
                            Canvas.SetTop (Feature, ScreenOffsetY + (Int64) XY.Item2);

                            //if (Feature.GeoWidth != 0)
                            //    Feature.Width = 

                        }

                    }

                }

                IsCurrentlyPainting = false;

                return true;

            }

            return false;

        }

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
            var FeatureLayer = Object as IMapLayer;
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
        public Int32 CompareTo(IMapLayer FeatureLayer)
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
            var FeatureLayer = Object as IMapLayer;
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
        public Boolean Equals(IMapLayer FeatureLayer)
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
