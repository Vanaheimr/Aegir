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
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace de.ahzf.Vanaheimr.Aegir
{

    /// <summary>
    /// http://www.geojson.org/geojson-spec.html
    /// </summary>
    public class GeoJSON
    {

        // 2.1.1. Positions
        // A position is the fundamental geometry construct. The "coordinates" member
        // of a geometry object is composed of one position (in the case of a Point
        // geometry), an array of positions (LineString or MultiPoint geometries), an
        // array of arrays of positions (Polygons, MultiLineStrings), or a
        // multidimensional array of positions (MultiPolygon).
        // 
        // A position is represented by an array of numbers. There must be at least
        // two elements, and may be more. The order of elements must follow x, y, z
        // order (easting, northing, altitude for coordinates in a projected coordinate
        // reference system, or longitude, latitude, altitude for coordinates in a
        // geographic coordinate reference system). Any number of additional elements
        // are allowed -- interpretation and meaning of additional elements is beyond
        // the scope of this specification.


        // Point
        //
        // Point coordinates are in x, y order (easting, northing for projected
        // coordinates, longitude, latitude for geographic coordinates):
        //
        // {
        //    "type":        "Point",
        //    "coordinates": [100.0, 0.0]
        // }


        // LineString
        //
        // Coordinates of LineString are an array of positions (see 2.1.1. Positions):
        //
        // {
        //    "type":        "LineString",
        //    "coordinates": [ [100.0, 0.0], [101.0, 1.0] ]
        // }


//Polygon

//Coordinates of a Polygon are an array of LinearRing coordinate arrays. The first element in the array represents the exterior ring. Any subsequent elements represent interior rings (or holes).

//No holes:

//{ "type": "Polygon",
//  "coordinates": [
//    [ [100.0, 0.0], [101.0, 0.0], [101.0, 1.0], [100.0, 1.0], [100.0, 0.0] ]
//    ]
// }
//With holes:

//{ "type": "Polygon",
//  "coordinates": [
//    [ [100.0, 0.0], [101.0, 0.0], [101.0, 1.0], [100.0, 1.0], [100.0, 0.0] ],
//    [ [100.2, 0.2], [100.8, 0.2], [100.8, 0.8], [100.2, 0.8], [100.2, 0.2] ]
//    ]
// }
//MultiPoint

//Coordinates of a MultiPoint are an array of positions:

//{ "type": "MultiPoint",
//  "coordinates": [ [100.0, 0.0], [101.0, 1.0] ]
//  }
//MultiLineString

//Coordinates of a MultiLineString are an array of LineString coordinate arrays:

//{ "type": "MultiLineString",
//  "coordinates": [
//      [ [100.0, 0.0], [101.0, 1.0] ],
//      [ [102.0, 2.0], [103.0, 3.0] ]
//    ]
//  }
//MultiPolygon

//Coordinates of a MultiPolygon are an array of Polygon coordinate arrays:

//{ "type": "MultiPolygon",
//  "coordinates": [
//    [[[102.0, 2.0], [103.0, 2.0], [103.0, 3.0], [102.0, 3.0], [102.0, 2.0]]],
//    [[[100.0, 0.0], [101.0, 0.0], [101.0, 1.0], [100.0, 1.0], [100.0, 0.0]],
//     [[100.2, 0.2], [100.8, 0.2], [100.8, 0.8], [100.2, 0.8], [100.2, 0.2]]]
//    ]
//  }
//GeometryCollection

//Each element in the geometries array of a GeometryCollection is one of the geometry objects described above:

//{ "type": "GeometryCollection",
//  "geometries": [
//    { "type": "Point",
//      "coordinates": [100.0, 0.0]
//      },
//    { "type": "LineString",
//      "coordinates": [ [101.0, 0.0], [102.0, 1.0] ]
//      }
//  ]
//}










//        2.2. Feature Objects

//A GeoJSON object with the type "Feature" is a feature object.

//A feature object must have a member with the name "geometry". The value of the geometry member is a geometry object as defined above or a JSON null value.
//A feature object must have a member with the name "properties". The value of the properties member is an object (any JSON object or a JSON null value).
//If a feature has a commonly used identifier, that identifier should be included as a member of the feature object with the name "id".


  // { "type":     "FeatureCollection",
  //   "features": [

  //      {
  //          "type":       "Feature",
  //          "geometry":   {
  //                            "type":        "Point",
  //                            "coordinates": [102.0, 0.5]
  //                        },
  //          "properties": {
  //                            "prop0":       "value0"
  //                        }
  //      },


  //  { "type": "Feature",
  //    "geometry": {
  //      "type": "LineString",
  //      "coordinates": [
  //        [102.0, 0.0], [103.0, 1.0], [104.0, 0.0], [105.0, 1.0]
  //        ]
  //      },
  //    "properties": {
  //      "prop0": "value0",
  //      "prop1": 0.0
  //      }
  //    },


  //  { "type": "Feature",
  //     "geometry": {
  //       "type": "Polygon",
  //       "coordinates": [
  //         [ [100.0, 0.0], [101.0, 0.0], [101.0, 1.0],
  //           [100.0, 1.0], [100.0, 0.0] ]
  //         ]
  //     },
  //     "properties": {
  //       "prop0": "value0",
  //       "prop1": {"this": "that"}
  //       }
  //     }
  //   ]
  // }


    }
}
