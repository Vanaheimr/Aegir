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
using System.Windows.Shapes;
using System.Windows.Media;

using de.ahzf.Vanaheimr.Aegir;

#endregion

namespace de.Vanaheimr.Aegir
{

    public class Feature : Shape, IPosition
    {

        #region Properties

        public String Name      { get; set; }

        public Double Latitude  { get; set; }
        public Double Longitude { get; set; }
        public Double Altitude  { get; set; }

        #region Geometry

        private readonly Geometry Geometry;

        protected override Geometry DefiningGeometry
        {
            get
            {
                return Geometry;
            }
        }

        #endregion

        #endregion

        public Feature()
        {
            this.Geometry = new EllipseGeometry();
        }

        public Feature(Geometry Geometry)
        {
            this.Geometry = Geometry;
        }

    }

}

