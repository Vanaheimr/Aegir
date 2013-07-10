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
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using eu.Vanaheimr.Styx;
using eu.Vanaheimr.Illias.Commons;

#endregion

namespace eu.Vanaheimr.Aegir
{

    public static class Ext
    {

        public static String GeoCoord2Shape(this IEnumerable<GeoCoordinate> _GeoCoordinates, ScreenXY OnScreenUpperLeft, UInt32 _ZoomLevel)
        {

            var OnScreenUpperLeftX = (Int64) OnScreenUpperLeft.X;
            var OnScreenUpperLeftY = (Int64) OnScreenUpperLeft.Y;

            var st = new StringBuilder("M ");

            Action<ScreenXY> aa = XY_ => {
                st.Append(((Int64) XY_.X) - OnScreenUpperLeftX);
                st.Append(" ");
                st.Append(((Int64) XY_.Y) - OnScreenUpperLeftY);
                st.Append(" ");
            };

            _GeoCoordinates.
                Select(GeoCoord => GeoCalculations.WorldCoordinates_2_Screen(GeoCoord, _ZoomLevel)).
                ForEach(XY => { aa(XY); st.Append("L "); },
                        XY => aa(XY));

            st.Append("Z ");

            return st.ToString();

        }

    }


    public class GeoCoord2ShapePipe : FuncPipe<IEnumerable<GeoCoordinate>, String>
    {

        #region Constructor(s)

        public GeoCoord2ShapePipe(ScreenXY OnScreenUpperLeft,
                                  UInt32 ZoomLevel,
                                  IEnumerable<IEnumerable<GeoCoordinate>> IEnumerable = null,
                                  IEnumerator<IEnumerable<GeoCoordinate>> IEnumerator = null)

            : base(el => el.GeoCoord2Shape(OnScreenUpperLeft, ZoomLevel),
                   IEnumerable,
                   IEnumerator)
                           
        { }

        #endregion

    }

}
