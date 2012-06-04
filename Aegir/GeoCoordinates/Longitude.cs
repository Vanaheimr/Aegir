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

#endregion

namespace de.ahzf.Vanaheimr.Aegir
{

    /// <summary>
    /// A Longitude.
    /// </summary>
    public struct Longitude : IEquatable<Longitude>,
                              IComparable<Longitude>,
                              IComparable

    {

        #region Properties

        private Double _Value;

        /// <summary>
        /// Returns the value of the longitude.
        /// </summary>
        public Double Value
        {
            get
            {
                return _Value;
            }
        }

        #endregion

        #region Constructor(s)

        #region Longitude(Value)

        /// <summary>
        /// Create a new longitude.
        /// </summary>
        /// <param name="Value">The value of the longitude.</param>
        public Longitude(Double Value)
        {
            _Value = Value;
        }

        #endregion

        #endregion


        #region Operator overloading

        #region Operator == (Longitude1, Longitude2)

        /// <summary>
        /// Compares two longitudes for equality.
        /// </summary>
        /// <param name="Longitude1">A longitude.</param>
        /// <param name="Longitude2">Another longitude.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (Longitude Longitude1, Longitude Longitude2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(Longitude1, Longitude2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) Longitude1 == null) || ((Object) Longitude2 == null))
                return false;

            return Longitude1.Equals(Longitude2);

        }

        #endregion

        #region Operator != (Longitude1, Longitude2)

        /// <summary>
        /// Compares two vertices for inequality.
        /// </summary>
        /// <param name="Longitude1">A longitude.</param>
        /// <param name="Longitude2">Another longitude.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (Longitude Longitude1, Longitude Longitude2)
        {
            return !(Longitude1 == Longitude2);
        }

        #endregion

        #region Operator <  (Longitude1, Longitude2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Longitude1">A longitude.</param>
        /// <param name="Longitude2">Another longitude.</param>
        /// <returns>true|false</returns>
        public static Boolean operator  < (Longitude Longitude1, Longitude Longitude2)
        {

            if ((Object) Longitude1 == null)
                throw new ArgumentNullException("The given Longitude1 must not be null!");

            if ((Object) Longitude2 == null)
                throw new ArgumentNullException("The given Longitude2 must not be null!");

            return Longitude1.CompareTo(Longitude2) < 0;

        }

        #endregion

        #region Operator <= (Longitude1, Longitude2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Longitude1">A longitude.</param>
        /// <param name="Longitude2">Another longitude.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Longitude Longitude1, Longitude Longitude2)
        {
            return !(Longitude1 > Longitude2);
        }

        #endregion

        #region Operator >  (Longitude1, Longitude2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Longitude1">A longitude.</param>
        /// <param name="Longitude2">Another longitude.</param>
        /// <returns>true|false</returns>
        public static Boolean operator  > (Longitude Longitude1, Longitude Longitude2)
        {

            if ((Object) Longitude1 == null)
                throw new ArgumentNullException("The given Longitude1 must not be null!");

            if ((Object) Longitude2 == null)
                throw new ArgumentNullException("The given Longitude2 must not be null!");

            return Longitude1.CompareTo(Longitude2) > 0;

        }

        #endregion

        #region Operator >= (Longitude1, Longitude2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Longitude1">A longitude.</param>
        /// <param name="Longitude2">Another longitude.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Longitude Longitude1, Longitude Longitude2)
        {
            return !(Longitude1 < Longitude2);
        }

        #endregion

        #endregion

        #region IComparable Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException("The given Object must not be null!");

            return CompareTo((Longitude) Object);

        }

        #endregion

        #region CompareTo(Longitude)

        /// <summary>
        /// Compares two longitudes.
        /// </summary>
        /// <param name="Longitude">Another longitude.</param>
        public Int32 CompareTo(Longitude Longitude)
        {
            return this.Value.CompareTo(Longitude.Value);
        }

        #endregion

        #endregion

        #region IEquatable Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object == null)
                return false;
            
            try
            {
                return this.Equals((Longitude) Object);
            }
            catch (InvalidCastException)
            {
                return false;
            }

        }

        #endregion

        #region Equals(Longitude)

        /// <summary>
        /// Compares two longitudes for equality.
        /// </summary>
        /// <param name="Longitude">Another longitude.</param>
        /// <returns>True if both are equal; False otherwise.</returns>
        public Boolean Equals(Longitude Longitude)
        {
            return this.Value.Equals(Longitude.Value);
        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the hashcode of this object.
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        #endregion

        #region ToString()

        /// <summary>
        /// Returns a string representation of the given object.
        /// </summary>
        public override String ToString()
        {
            return this.Value.ToString();
        }

        #endregion

    }

}
