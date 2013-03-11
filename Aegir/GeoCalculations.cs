﻿/*
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

#endregion

namespace eu.Vanaheimr.Aegir
{

    /// <summary>
    /// Usefull calculations for handling Aegir maps.
    /// </summary>
    public static class GeoCalculations
    {

        #region WorldCoordinates_2_Tile(GeoPosition, ZoomLevel)

        /// <summary>
        /// Converts the given world coordinates and zoom level to the X and Y number of map tiles.
        /// </summary>
        /// <param name="GeoCoordinate">The geographical position on the map.</param>
        /// <param name="ZoomLevel">The ZoomLevel.</param>
        /// <returns>The TileX and TileY number as a tuple.</returns>
        public static Tuple<UInt64, UInt64> WorldCoordinates_2_Tile(GeoCoordinate GeoCoordinate, UInt32 ZoomLevel)
        {
            return WorldCoordinates_2_Tile(GeoCoordinate.Latitude, GeoCoordinate.Longitude, ZoomLevel);
        }

        #endregion

        #region WorldCoordinates_2_Tile(Latitude, Longitude, ZoomLevel)

        /// <summary>
        /// Converts the given world coordinates and zoom level to the X and Y number of map tiles.
        /// </summary>
        /// <param name="Latitude">The latitude.</param>
        /// <param name="Longitude">The longitude.</param>
        /// <param name="ZoomLevel">The ZoomLevel.</param>
        /// <returns>The TileX and TileY number as a tuple.</returns>
        public static Tuple<UInt64, UInt64> WorldCoordinates_2_Tile(Latitude Latitude, Longitude Longitude, UInt32 ZoomLevel)
        {

            return new Tuple<UInt64, UInt64>(
                (UInt64) Math.Floor(((Longitude.Value + 180.0) / 360.0) * (1 << (Int32) ZoomLevel)),
                (UInt64) Math.Floor((1.0 - Math.Log (
                                                              Math.Tan(Latitude.Value * Math.PI / 180.0) +
                                                        1.0 / Math.Cos(Latitude.Value * Math.PI / 180.0)
                                                    ) / Math.PI)
                                    / 2.0 * (1 << (Int32) ZoomLevel))
            );

        }

        #endregion

        #region Tile_2_WorldCoordinates(TileX, TileY, ZoomLevel)

        /// <summary>
        /// Converts the given X and Y number and zoom level of map tiles to world coordinates.
        /// </summary>
        /// <param name="TileX">The tile X number.</param>
        /// <param name="TileY">The tile Y number.</param>
        /// <param name="ZoomLevel">The zoom level.</param>
        /// <returns>The latitude and longitude of the first upper level pixel of the given tile.</returns>
        public static GeoCoordinate Tile_2_WorldCoordinates(Double TileX, Double TileY, UInt32 ZoomLevel)
        {

            var n = Math.PI - ((2.0 * Math.PI * TileY) / Math.Pow(2.0, ZoomLevel));

            return new GeoCoordinate(
                new Latitude((TileX / Math.Pow(2.0, ZoomLevel) * 360.0) - 180.0),
                new Longitude(180.0 / Math.PI * Math.Atan(Math.Sinh(n)))
            );

        }

        #endregion

        #region Mouse_2_WorldCoordinates(MouseX, MouseY, ZoomLevel)

        /// <summary>
        /// Converts the given mouse position on the map and zoom level
        /// of the map to world coordinates.
        /// </summary>
        /// <param name="MouseX">The X position of the mouse on the map.</param>
        /// <param name="MouseY">The Y position of the mouse on the map.</param>
        /// <param name="ZoomLevel">The zoom level.</param>
        /// <returns>The latitude and longitude of the first upper level pixel of the given tile.</returns>
        public static GeoCoordinate Mouse_2_WorldCoordinates(Double MouseX, Double MouseY, UInt32 ZoomLevel)
        {

            var MapSize = Math.Pow(2.0, ZoomLevel) * 256;

            var n = Math.PI - ((2.0 * Math.PI * MouseY) / MapSize);

            return new GeoCoordinate(                
                new Latitude((180.0 / Math.PI * Math.Atan(Math.Sinh(n))) % 90),
                new Longitude(((MouseX / MapSize * 360.0) - 180.0) % 90)
            );

        }

        #endregion

        #region WorldCoordinates_2_Screen(GeoPosition, ZoomLevel)

        /// <summary>
        /// Converts the given world coordinates and zoom level to the X and Y number of map tiles.
        /// </summary>
        /// <param name="GeoCoordinate">The geographical position on the map.</param>
        /// <param name="ZoomLevel">The ZoomLevel.</param>
        /// <returns>The TileX and TileY number as a tuple.</returns>
        public static Tuple<UInt64, UInt64> WorldCoordinates_2_Screen(GeoCoordinate GeoCoordinate, UInt32 ZoomLevel)
        {
            return WorldCoordinates_2_Screen(GeoCoordinate.Latitude, GeoCoordinate.Longitude, ZoomLevel);
        }

        #endregion

        #region WorldCoordinates_2_Screen(Latitude, Longitude, ZoomLevel)

        /// <summary>
        /// Converts the given world coordinates and zoom level to the X and Y number of map tiles.
        /// </summary>
        /// <param name="Latitude">The latitude.</param>
        /// <param name="Longitude">The longitude.</param>
        /// <param name="ZoomLevel">The ZoomLevel.</param>
        /// <returns>The TileX and TileY number as a tuple.</returns>
        public static Tuple<UInt64, UInt64> WorldCoordinates_2_Screen(Latitude Latitude, Longitude Longitude, UInt32 ZoomLevel)
        {

            var MapSize = Math.Pow(2.0, ZoomLevel) * 256;

            return new Tuple<UInt64, UInt64>(
                (UInt64) (((Longitude.Value + 180.0) / 360.0) * MapSize),
                (UInt64) (((1.0 - Math.Log(
                                                              Math.Tan(Latitude.Value * Math.PI / 180.0) +
                                                        1.0 / Math.Cos(Latitude.Value * Math.PI / 180.0)
                                                    ) / Math.PI)
                                    / 2.0) * MapSize)
            );

        }

        #endregion

    }

}
