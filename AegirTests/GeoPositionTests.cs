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
using System.Text.RegularExpressions;
using System.Globalization;

using NUnit.Framework;

#endregion

namespace de.ahzf.Vanaheimr.Aegir.UnitTests
{

    /// <summary>
    /// Unit tests for the Aegir GeoPosition class.
    /// </summary>
    [TestFixture]
    public class GeoPositionTests
    {

        [Test]
        public void GeoPositionInitializerTest()
        {

            var Random = new Random();
            var RandomLatitude  = (Random.NextDouble() - 0.5) * 180;
            var RandomLongitude = (Random.NextDouble() - 0.5) * 180;

            var GeoPosition1 = new GeoPosition(RandomLatitude, RandomLongitude);
            Assert.AreEqual(RandomLatitude,  GeoPosition1.Latitude);
            Assert.AreEqual(RandomLongitude, GeoPosition1.Longitude);

        }

        [Test]
        public void GeoPositionToStringAndBackParsingTest()
        {

            var Random = new Random();
            var RandomLatitude  = (Random.NextDouble() - 0.5) * 180;
            var RandomLongitude = (Random.NextDouble() - 0.5) * 180;

            var GeoPosition1 = new GeoPosition(RandomLatitude, RandomLongitude);
            Assert.AreEqual(RandomLatitude,  GeoPosition1.Latitude);
            Assert.AreEqual(RandomLongitude, GeoPosition1.Longitude);

            var GeoPosition2 = GeoPosition.ParseString(GeoPosition1.ToGeoString());
            Assert.AreEqual(RandomLatitude,  GeoPosition2.Latitude);
            Assert.AreEqual(RandomLongitude, GeoPosition2.Longitude);

            var GeoPosition3 = GeoPosition.ParseString(GeoPosition1.ToGeoString(GeoFormat.Decimal));
            Assert.AreEqual(RandomLatitude,  GeoPosition3.Latitude);
            Assert.AreEqual(RandomLongitude, GeoPosition3.Longitude);

            var GeoPosition4 = GeoPosition.ParseString(GeoPosition1.ToGeoString(GeoFormat.Sexagesimal));
            Assert.AreEqual(RandomLatitude,  GeoPosition4.Latitude);
            Assert.AreEqual(RandomLongitude, GeoPosition4.Longitude);

            var GeoPosition5 = GeoPosition.ParseString(RandomLatitude.ToString() + ", " + RandomLongitude.ToString());
            Assert.AreEqual(RandomLatitude,  GeoPosition5.Latitude);
            Assert.AreEqual(RandomLongitude, GeoPosition5.Longitude);

        }

        [Test]
        public void GeoPositionStringParsingTest()
        {

            var GeoPosition1 = GeoPosition.ParseString("49.44903° N, 11.07488° E");
            Assert.AreEqual(49.44903,        GeoPosition1.Latitude);
            Assert.AreEqual(11.07488,        GeoPosition1.Longitude);

            var GeoPosition2 = GeoPosition.ParseString("49,44903° N, 11,07488° E");
            Assert.AreEqual(49.44903,        GeoPosition2.Latitude);
            Assert.AreEqual(11.07488,        GeoPosition2.Longitude);

            var GeoPosition3 = GeoPosition.ParseString("49,44903° N,11.07488° E");
            Assert.AreEqual(49.44903,        GeoPosition3.Latitude);
            Assert.AreEqual(11.07488,        GeoPosition3.Longitude);

            var GeoPosition4 = GeoPosition.ParseString("  49.44903 N  , 11.07488 E ");
            Assert.AreEqual(49.44903,        GeoPosition4.Latitude);
            Assert.AreEqual(11.07488,        GeoPosition4.Longitude);

            var GeoPosition5 = GeoPosition.ParseString("49,44903 N\t,\t11,07488 E");
            Assert.AreEqual(49.44903,        GeoPosition5.Latitude);
            Assert.AreEqual(11.07488,        GeoPosition5.Longitude);

            var GeoPosition6 = GeoPosition.ParseString("-49.44903° N, -11.07488° E");
            Assert.AreEqual(-49.44903,        GeoPosition6.Latitude);
            Assert.AreEqual(-11.07488,        GeoPosition6.Longitude);

            var GeoPosition7 = GeoPosition.ParseString("49.44903 N, 11.07488 E");
            Assert.AreEqual(49.44903,        GeoPosition7.Latitude);
            Assert.AreEqual(11.07488,        GeoPosition7.Longitude);

            var GeoPosition8 = GeoPosition.ParseString("-49.44903°, -11.07488");
            Assert.AreEqual(-49.44903,       GeoPosition8.Latitude);
            Assert.AreEqual(-11.07488,       GeoPosition8.Longitude);

            var GeoPosition9 = GeoPosition.ParseString("49.44903 S, 11.07488 W");
            Assert.AreEqual(-49.44903,        GeoPosition9.Latitude);
            Assert.AreEqual(-11.07488,        GeoPosition9.Longitude);

            var GeoPositionX = GeoPosition.ParseString("-49.44903 S, -11.07488 W");
            Assert.IsNull(GeoPositionX);

        }

        [Test]
        public void GeoPositionInitializerTest2()
        {

            var GeoPosition1 = new GeoPosition(-73.9874, 0);
            var b = GeoPosition1.ToGeoString(GeoFormat.Sexagesimal);
            var c = GeoPosition.ParseString("49.44903° N, 11.07488° E");
            var d = c.ToGeoString();

        }

    }

}
