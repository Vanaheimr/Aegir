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

using org.GraphDefined.Vanaheimr.Aegir.Controls;

#endregion

namespace org.GraphDefined.Vanaheimr.Aegir
{

    /// <summary>
    /// The base functionality of all map layers.
    /// </summary>
    public abstract class AMapLayer : Canvas,
                                      IEquatable<AMapLayer>, IComparable<AMapLayer>,
                                      IEquatable<String>,    IComparable<String>
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

            #region Initial Checks

            if (String.IsNullOrEmpty(Id.Trim()))
                throw new ArgumentNullException("Id", "The map layer identification must not be null or empty!");

            if (MapControl == null)
                throw new ArgumentNullException("MapControl", "The MapControl must not be null!");

            if (MapControl.ContainsLayerId(Id))
                throw new ApplicationException("The given 'Id' is already used!");

            #endregion

            this.Id          = Id;
            this.MapControl  = MapControl;
            this.ZIndex      = ZIndex;

        }

        #endregion

        #endregion


        #region (private) ProcessMapSizeChangedEvent(Sender, SizeChangedEventArgs)

        /// <summary>
        /// Called whenever the size of the map layer was changed.
        /// </summary>
        /// <param name="Sender">The sender of the event.</param>
        /// <param name="SizeChangedEventArgs">The event arguments.</param>
        private void ProcessMapSizeChangedEvent(Object Sender, SizeChangedEventArgs SizeChangedEventArgs)
        {
            Redraw();
        }

        #endregion


        public abstract void Move(Double X, Double Y);
        public abstract void Redraw();


        #region IComparable<Identifier/AMapLayer> Members

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

        #region CompareTo(MapLayer)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Identifier">An object to compare with.</param>
        public Int32 CompareTo(AMapLayer MapLayer)
        {

            if ((Object) MapLayer == null)
                throw new ArgumentNullException("The given feature layer must not be null!");

            return this.Id.CompareTo(MapLayer.Id);

        }

        #endregion

        #endregion

        #region IEquatable<Identifier/AMapLayer> Members

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

        #region Equals(MapLayer)

        /// <summary>
        /// Compares two feature layers for equality.
        /// </summary>
        /// <param name="MapLayer">A feature layer to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(AMapLayer MapLayer)
        {

            if ((Object) MapLayer == null)
                return false;

            return this.Id.Equals(MapLayer.Id);

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
