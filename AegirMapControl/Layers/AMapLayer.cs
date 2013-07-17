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
using System.ComponentModel;

using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;

using eu.Vanaheimr.Aegir.Controls;

#endregion

namespace eu.Vanaheimr.Aegir
{

    /// <summary>
    /// The base functionality of all map layers.
    /// </summary>
    public abstract class AMapLayer : Canvas
    {

        #region Data

        protected          Double   LastClickPositionX;
        protected          Double   LastClickPositionY;

        protected volatile Boolean  IsCurrentlyPainting;

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

        #region AMapLayer(Id, MapControl, ZIndex)

        /// <summary>
        /// Creates a new feature layer for visualizing map features.
        /// </summary>
        /// <param name="Id">The name or identification of this feature layer.</param>
        /// <param name="MapControl">The hosting map control.</param>
        /// <param name="ZIndex">The z-index of this feature layer.</param>
        public AMapLayer(String Id, MapControl MapControl, Int32 ZIndex)
            : this()
        {

            this.Id          = Id;
            this.MapControl  = MapControl;
            this.ZIndex      = ZIndex;

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


        //#region ZoomTo(ZoomLevel, ScreenOffsetX, ScreenOffsetY)

        ///// <summary>
        ///// Set the zoom level and screen offset of this map layer.
        ///// </summary>
        ///// <param name="ZoomLevel">The desired zoom level.</param>
        ///// <param name="ScreenOffsetX">The x-parameter of the screen offset.</param>
        ///// <param name="ScreenOffsetY">The y-parameter of the screen offset.</param>
        //public AMapLayer ZoomTo(UInt32 ZoomLevel, Int64 ScreenOffsetX, Int64 ScreenOffsetY)
        //{

        //    //this.ZoomLevel      = ZoomLevel;
        //    //this.ScreenOffsetX  = ScreenOffsetX;
        //    //this.ScreenOffsetY  = ScreenOffsetY;

        //    Redraw();

        //    return this;

        //}

        //#endregion

        //#region SetDisplayOffset(OffsetX, OffsetY)

        ///// <summary>
        ///// Set the screen offset of this map layer.
        ///// </summary>
        ///// <param name="ScreenOffsetX">The x-parameter of the screen offset.</param>
        ///// <param name="ScreenOffsetY">The y-parameter of the screen offset.</param>
        //public AMapLayer SetDisplayOffset(Int64 ScreenOffsetX, Int64 ScreenOffsetY)
        //{

        //    this.ScreenOffsetX  = ScreenOffsetX;
        //    this.ScreenOffsetY  = ScreenOffsetY;

        //    Redraw();

        //    return this;

        //}

        //#endregion

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
                    ScreenXY ScreenXY;

                    foreach (var Child in this.Children)
                    {

                        Feature = Child as Feature;

                        if (Feature != null)
                        {

                            ScreenXY = GeoCalculations.WorldCoordinates_2_Screen(Feature.Latitude, Feature.Longitude, this.MapControl.ZoomLevel);

                            Canvas.SetLeft(Feature, this.MapControl.ScreenOffsetX + ScreenXY.X);
                            Canvas.SetTop (Feature, this.MapControl.ScreenOffsetY + ScreenXY.Y);

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
            var FeatureLayer = Object as AMapLayer;
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
        public Int32 CompareTo(AMapLayer FeatureLayer)
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
            var FeatureLayer = Object as AMapLayer;
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
        public Boolean Equals(AMapLayer FeatureLayer)
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
