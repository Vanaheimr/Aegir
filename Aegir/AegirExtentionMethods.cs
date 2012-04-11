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

#endregion

namespace de.ahzf.Vanaheimr.Aegir
{

    /// <summary>
    /// Exention methods for working with maps.
    /// </summary>
    public static class AegirExtentionMethods
    {

        #region ToGeoString(this GeoCoordinateTuple)

        /// <summary>
        /// Converts the given geo coordinate of type Tuple&lt;Longitude, Latitude&gt;
        /// into a user-friendly string representaion.
        /// </summary>
        /// <param name="GeoCoordinateTuple">A tuple of types &lt;Longitude, Latitude&gt;.</param>
        /// <returns>A user-friendly string representation of the given geo coordinate.</returns>
        public static String ToGeoString(this Tuple<Double, Double> GeoCoordinateTuple)
        {

            var SorN = (GeoCoordinateTuple.Item2 < 0) ? "S" : "N";
            var WorO = (GeoCoordinateTuple.Item1 < 0) ? "W" : "O";

            return String.Format("{0}° {1} {2}° {3}", Math.Abs(GeoCoordinateTuple.Item2), SorN, Math.Abs(GeoCoordinateTuple.Item1), WorO);

        }

        #endregion

    }

}
