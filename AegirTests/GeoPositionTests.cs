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

using NUnit.Framework;

#endregion

namespace eu.Vanaheimr.Aegir.UnitTests
{

    /// <summary>
    /// Unit tests for the Aegir GeoCoordinate class.
    /// </summary>
    [TestFixture]
    public class GeoCoordinateTests
    {

        #region GeoCoordinateInitializerTest()

        [Test]
        public void GeoCoordinateInitializerTest()
        {

            var Random = new Random();
            var RandomLatitude  = new Latitude ((Random.NextDouble() - 0.5) * 180);
            var RandomLongitude = new Longitude((Random.NextDouble() - 0.5) * 180);

            var GeoCoordinate1 = new GeoCoordinate(RandomLatitude, RandomLongitude);
            Assert.AreEqual(RandomLatitude.Value,  GeoCoordinate1.Latitude.Value,  0.0000001);
            Assert.AreEqual(RandomLongitude.Value, GeoCoordinate1.Longitude.Value, 0.0000001);

        }

        #endregion

        #region GeoCoordinateToStringAndBackParsingTest()

        [Test]
        public void GeoCoordinateToStringAndBackParsingTest()
        {

            var Random = new Random();
            var RandomLatitude  = new Latitude ((Random.NextDouble() - 0.5) * 180);
            var RandomLongitude = new Longitude((Random.NextDouble() - 0.5) * 180);

            var GeoCoordinate1 = new GeoCoordinate(RandomLatitude, RandomLongitude);
            Assert.AreEqual(RandomLatitude.Value,  GeoCoordinate1.Latitude.Value,  0.0000001);
            Assert.AreEqual(RandomLongitude.Value, GeoCoordinate1.Longitude.Value, 0.0000001);

            var GeoCoordinate2 = GeoCoordinate.ParseString(GeoCoordinate1.ToGeoString());
            Assert.AreEqual(RandomLatitude.Value,  GeoCoordinate2.Latitude.Value,  0.0000001);
            Assert.AreEqual(RandomLongitude.Value, GeoCoordinate2.Longitude.Value, 0.0000001);

            var GeoCoordinate3 = GeoCoordinate.ParseString(GeoCoordinate1.ToGeoString(GeoFormat.Decimal));
            Assert.AreEqual(RandomLatitude.Value,  GeoCoordinate3.Latitude.Value,  0.0000001);
            Assert.AreEqual(RandomLongitude.Value, GeoCoordinate3.Longitude.Value, 0.0000001);

            var GeoCoordinate4 = GeoCoordinate.ParseString(GeoCoordinate1.ToGeoString(GeoFormat.Sexagesimal));
            Assert.AreEqual(RandomLatitude.Value,  GeoCoordinate4.Latitude.Value,  0.0000001);
            Assert.AreEqual(RandomLongitude.Value, GeoCoordinate4.Longitude.Value, 0.0000001);

            var GeoCoordinate5 = GeoCoordinate.ParseString(RandomLatitude.ToString() + ", " + RandomLongitude.ToString());
            Assert.AreEqual(RandomLatitude.Value,  GeoCoordinate5.Latitude.Value,  0.0000001);
            Assert.AreEqual(RandomLongitude.Value, GeoCoordinate5.Longitude.Value, 0.0000001);

        }

        #endregion

        #region GeoCoordinateStringParsingTest()

        [Test]
        public void GeoCoordinateStringParsingTest()
        {

            var GeoCoordinate1 = GeoCoordinate.ParseString("49.44903° N, 11.07488° E");
            Assert.AreEqual(49.44903,        GeoCoordinate1.Latitude.Value);
            Assert.AreEqual(11.07488,        GeoCoordinate1.Longitude.Value);

            var GeoCoordinate2 = GeoCoordinate.ParseString("49,44903° N, 11,07488° E");
            Assert.AreEqual(49.44903,        GeoCoordinate2.Latitude.Value);
            Assert.AreEqual(11.07488,        GeoCoordinate2.Longitude.Value);

            var GeoCoordinate3 = GeoCoordinate.ParseString("49,44903° N,11.07488° E");
            Assert.AreEqual(49.44903,        GeoCoordinate3.Latitude.Value);
            Assert.AreEqual(11.07488,        GeoCoordinate3.Longitude.Value);

            var GeoCoordinate4 = GeoCoordinate.ParseString("  49.44903 N  , 11.07488 E ");
            Assert.AreEqual(49.44903,        GeoCoordinate4.Latitude.Value);
            Assert.AreEqual(11.07488,        GeoCoordinate4.Longitude.Value);

            var GeoCoordinate5 = GeoCoordinate.ParseString("49,44903 N\t,\t11,07488 E");
            Assert.AreEqual(49.44903,        GeoCoordinate5.Latitude.Value);
            Assert.AreEqual(11.07488,        GeoCoordinate5.Longitude.Value);

            var GeoCoordinate6 = GeoCoordinate.ParseString("-49.44903°, -11.07488° ");
            Assert.AreEqual(-49.44903,       GeoCoordinate6.Latitude.Value);
            Assert.AreEqual(-11.07488,       GeoCoordinate6.Longitude.Value);

            var GeoCoordinate7 = GeoCoordinate.ParseString("49.44903 N, 11.07488 E");
            Assert.AreEqual(49.44903,        GeoCoordinate7.Latitude.Value);
            Assert.AreEqual(11.07488,        GeoCoordinate7.Longitude.Value);

            var GeoCoordinate8 = GeoCoordinate.ParseString("-49.44903°, -11.07488");
            Assert.AreEqual(-49.44903,       GeoCoordinate8.Latitude.Value);
            Assert.AreEqual(-11.07488,       GeoCoordinate8.Longitude.Value);

            var GeoCoordinate9 = GeoCoordinate.ParseString("49.44903 S, 11.07488 W");
            Assert.AreEqual(-49.44903,       GeoCoordinate9.Latitude.Value);
            Assert.AreEqual(-11.07488,       GeoCoordinate9.Longitude.Value);

            var GeoCoordinateX = GeoCoordinate.ParseString("-49.44903 S, -11.07488 W");
            Assert.IsNull(GeoCoordinateX);

        }

        #endregion

        #region GeoCoordinateInitializerTest2()

        [Test]
        public void GeoCoordinateInitializerTest2()
        {

            var GeoCoordinate1 = new GeoCoordinate(new Latitude(-73.9874), new Longitude(0));
            var b = GeoCoordinate1.ToGeoString(GeoFormat.Sexagesimal);
            var c = GeoCoordinate.ParseString("49.44903° N, 11.07488° E");
            var d = c.ToGeoString();

        }

        #endregion

        #region ToGeoHashTest1()

        [Test]
        public void ToGeoHashTest1()
        {

            var geo01       = new GeoCoordinate(new Latitude(57.64911), new Longitude(10.40744));
            var geo01hash   = geo01.ToGeoHash(11);
            var geo01hash32 = geo01.ToGeoHash32(16);
            var geo01hash64 = geo01.ToGeoHash64(32);

            Assert.AreEqual("u4pruydqqvj",          geo01hash.Value);
            Assert.AreEqual(3793206966U,            geo01hash32.Value);
            Assert.AreEqual(16291699867698975045UL, geo01hash64.Value);

            var geo01rev    = geo01hash.ToGeoCoordinate(5);
            var geo01rev32  = geo01hash.ToGeoCoordinate(5);
            var geo01rev64  = geo01hash.ToGeoCoordinate(5);

            Assert.IsTrue(geo01.Equals(geo01rev));
            Assert.IsTrue(geo01 == geo01rev);
            Assert.AreEqual(geo01.Latitude.Value,  geo01rev.Latitude.Value,    0.000001);
            Assert.AreEqual(geo01.Longitude.Value, geo01rev.Longitude.Value,   0.000001);

            Assert.IsTrue(geo01.Equals(geo01rev32));
            Assert.IsTrue(geo01 == geo01rev32);
            Assert.AreEqual(geo01.Latitude.Value,  geo01rev32.Latitude.Value,  0.000001);
            Assert.AreEqual(geo01.Longitude.Value, geo01rev32.Longitude.Value, 0.000001);

            Assert.IsTrue(geo01.Equals(geo01rev64));
            Assert.IsTrue(geo01 == geo01rev64);
            Assert.AreEqual(geo01.Latitude.Value,  geo01rev64.Latitude.Value,  0.000001);
            Assert.AreEqual(geo01.Longitude.Value, geo01rev64.Longitude.Value, 0.000001);

        }

        #endregion

        #region ToGeoHashTest2()

        [Test]
        public void ToGeoHashTest2()
        {

            var geo01       = new GeoCoordinate(new Latitude(38.897), new Longitude(-77.036));
            var geo01hash   = geo01.ToGeoHash(20);
            var geo01hash32 = geo01.ToGeoHash32(16);
            var geo01hash64 = geo01.ToGeoHash64(32);

            Assert.AreEqual("dqcjr0bp7n74cjbuqqub", geo01hash.Value);
            Assert.AreEqual(2590713666U,            geo01hash32.Value);
            Assert.AreEqual(11127030471626460554UL, geo01hash64.Value);

            var geo01rev    = geo01hash.ToGeoCoordinate(3);
            var geo01rev32  = geo01hash.ToGeoCoordinate(3);
            var geo01rev64  = geo01hash.ToGeoCoordinate(3);

            Assert.IsTrue(geo01.Equals(geo01rev));
            Assert.IsTrue(geo01 == geo01rev);
            Assert.AreEqual(geo01.Latitude.Value,  geo01rev.Latitude.Value,    0.000001);
            Assert.AreEqual(geo01.Longitude.Value, geo01rev.Longitude.Value,   0.000001);

            Assert.IsTrue(geo01.Equals(geo01rev32));
            Assert.IsTrue(geo01 == geo01rev32);
            Assert.AreEqual(geo01.Latitude.Value,  geo01rev32.Latitude.Value,  0.000001);
            Assert.AreEqual(geo01.Longitude.Value, geo01rev32.Longitude.Value, 0.000001);

            Assert.IsTrue(geo01.Equals(geo01rev64));
            Assert.IsTrue(geo01 == geo01rev64);
            Assert.AreEqual(geo01.Latitude.Value,  geo01rev64.Latitude.Value,  0.000001);
            Assert.AreEqual(geo01.Longitude.Value, geo01rev64.Longitude.Value, 0.000001);

        }

        #endregion

        #region ToGeoHashTest3()

        [Test]
        public void ToGeoHashTest3()
        {

            var geo01       = new GeoCoordinate(new Latitude(-38.897), new Longitude(-77.036));
            var geo01hash   = geo01.ToGeoHash(20);
            var geo01hash32 = geo01.ToGeoHash32(16);
            var geo01hash64 = geo01.ToGeoHash64(32);

            Assert.AreEqual("6314xp00e1ej140gw3hz", geo01hash.Value);
            Assert.AreEqual(817991144U,             geo01hash32.Value);
            Assert.AreEqual(3513245211907368736UL,  geo01hash64.Value);

            var geo01rev    = geo01hash.ToGeoCoordinate(3);
            var geo01rev32  = geo01hash.ToGeoCoordinate(3);
            var geo01rev64  = geo01hash.ToGeoCoordinate(3);

            Assert.IsTrue(geo01.Equals(geo01rev));
            Assert.IsTrue(geo01 == geo01rev);
            Assert.AreEqual(geo01.Latitude.Value,  geo01rev.Latitude.Value,    0.000001);
            Assert.AreEqual(geo01.Longitude.Value, geo01rev.Longitude.Value,   0.000001);

            Assert.IsTrue(geo01.Equals(geo01rev32));
            Assert.IsTrue(geo01 == geo01rev32);
            Assert.AreEqual(geo01.Latitude.Value,  geo01rev32.Latitude.Value,  0.000001);
            Assert.AreEqual(geo01.Longitude.Value, geo01rev32.Longitude.Value, 0.000001);

            Assert.IsTrue(geo01.Equals(geo01rev64));
            Assert.IsTrue(geo01 == geo01rev64);
            Assert.AreEqual(geo01.Latitude.Value,  geo01rev64.Latitude.Value,  0.000001);
            Assert.AreEqual(geo01.Longitude.Value, geo01rev64.Longitude.Value, 0.000001);

        }

        #endregion

        #region DistanceTest()

        [Test]
        public void DistanceTest()
        {

            var Seattle = new GeoCoordinate(new Latitude(47.621800), new Longitude(-122.350326));
            var Olympia = new GeoCoordinate(new Latitude(47.041917), new Longitude(-122.893766));

            var Distance = Seattle.DistanceTo(Olympia);
            Assert.AreEqual(76.3866157995487, Distance, 0.0001);

        }

        #endregion

        #region MidPointTest1()

        [Test]
        public void MidPointTest1()
        {

            var LA    = new GeoCoordinate(new Latitude(34.122222), new Longitude(118.411111));
            var NYC   = new GeoCoordinate(new Latitude(40.669722), new Longitude( 73.943889));

            var midpt = LA.MidPoint(NYC);
            Assert.AreEqual(39.547079, midpt.Latitude.Value,  0.0001);
            Assert.AreEqual(97.201534, midpt.Longitude.Value, 0.0001);

        }

        #endregion

        #region MidPointTest2()

        [Test]
        public void MidPointTest2()
        {

            var geo01 = new GeoCoordinate(new Latitude(35), new Longitude( 45));
            var geo02 = new GeoCoordinate(new Latitude(35), new Longitude(135));

            var midpt = geo01.MidPoint(geo02);
            Assert.AreEqual(44.719114, midpt.Latitude.Value, 0.0001);
            Assert.AreEqual(90.000000, midpt.Longitude.Value, 0.0001);

        }

        #endregion

    }

}
