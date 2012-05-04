﻿/*
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
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using de.ahzf.Styx;
using de.ahzf.Illias.Commons;

#endregion

namespace de.ahzf.Vanaheimr.Aegir
{

    /// <summary>
    /// A class holding all neccesary 
    /// </summary>
    public class ShapeInfo
    {

        #region Properties

        /// <summary>
        /// The description of this shape.
        /// </summary>
        public String Description      { get; private set; }

        /// <summary>
        /// The minimal latitude of this shape.
        /// </summary>
        public Double Latitude_Start   { get; private set; }

        /// <summary>
        /// The minimal longitude of this shape.
        /// </summary>
        public Double Longitude_Start  { get; private set; }

        /// <summary>
        /// The maximal latitude of this shape.
        /// </summary>
        public Double Latitude_End     { get; private set; }

        /// <summary>
        /// The maximal longitude of this shape.
        /// </summary>
        public Double Longitude_End    { get; private set; }

        /// <summary>
        /// A dictionary 
        /// </summary>
        public IDictionary<UInt32, String> PathAtZoomLevel { get; private set; }

        #endregion


        public ShapeInfo(String Description,
                         Double Latitude_Start,
                         Double Longitude_Start,
                         Double Latitude_End,
                         Double Longitude_End,
                         IDictionary<UInt32, String> PathAtZoomLevel)
        {

            #region Initial checks

            if (Latitude_Start > Latitude_End)
                throw new ArgumentException("");

            if (Longitude_Start > Longitude_End)
                throw new ArgumentException("");

            if (PathAtZoomLevel == null || PathAtZoomLevel.Count == 0)
                throw new ArgumentException("");

            #endregion

            this.Description     = Description;
            this.Latitude_Start  = Latitude_Start;
            this.Longitude_Start = Longitude_Start;
            this.Latitude_End    = Latitude_End;
            this.Longitude_End   = Longitude_End;
            this.PathAtZoomLevel = PathAtZoomLevel;

        }


    }


    public static class Polyfiles
    {

        //public Polyfile2Shape()
        //{
        //}

        public static ShapeInfo Polyfile2ShapeInfo(IEnumerable<String> InputData, UInt32 min_resolution, UInt32 max_resolution)
        {

            #region Init

            var Integer      = 1U;
            var ShapeNumber  = 1U;
            var IsFirstLine  = true;
            var Description  = String.Empty;

            var min_lat      = Double.MaxValue;
            var min_lng      = Double.MaxValue;
            var max_lat      = Double.MinValue;
            var max_lng      = Double.MinValue;

            var Shapes       = new Dictionary<UInt32, Tuple<List<GeoCoordinate>, Dictionary<UInt32, Tuple<List<Tuple<UInt64, UInt64>>, StringBuilder>>>>();

            GeoCoordinate GeoCoordinate = null;

            #endregion

            foreach (var Line in InputData)
            {

                #region Process first line

                if (IsFirstLine)
                {
                    Description = Line;
                    IsFirstLine = false;
                }

                #endregion

                #region A single Integer on a line indicates the start of a new shape

                else if (UInt32.TryParse(Line, out Integer))
                {
                    ShapeNumber = Integer;
                    Shapes.Add(ShapeNumber, new Tuple<List<GeoCoordinate>, Dictionary<UInt32, Tuple<List<Tuple<UInt64, UInt64>>, StringBuilder>>>(
                                                new List<GeoCoordinate>(),
                                                new Dictionary<UInt32, Tuple<List<Tuple<UInt64, UInt64>>, StringBuilder>>()));
                }

                #endregion

                #region "END" indicates the end of a shape

                else if (Line == "END")
                    continue;

                #endregion

                #region The rest of the file are "Longitude Latitude" encoded geo coordinates

                else if (GeoCoordinate.TryParseString(Line, out GeoCoordinate))
                {

                    // Polyfiles store "Longitude Latitude"!!!
                    Shapes[ShapeNumber].Item1.Add(new GeoCoordinate(GeoCoordinate.Longitude, GeoCoordinate.Latitude));

                    if (min_lat > GeoCoordinate.Longitude)
                        min_lat = GeoCoordinate.Longitude;

                    if (min_lng > GeoCoordinate.Latitude)
                        min_lng = GeoCoordinate.Latitude;

                    if (max_lat < GeoCoordinate.Longitude)
                        max_lat = GeoCoordinate.Longitude;

                    if (max_lng < GeoCoordinate.Latitude)
                        max_lng = GeoCoordinate.Latitude;

                }

                #endregion

                #region Unknown data found...

                else
                    throw new Exception("Unknown data found!");

                #endregion

            }


            //var Output1 = new StreamWriter("PolyfileReader/" + Filename.Name.Replace(".poly", ".data"));
            //var Array = new StringBuilder();

            //var Output2 = new StreamWriter("PolyfileReader/" + "ghjk".Replace(".poly", ".geo"));
            //var Language = new StringBuilder();

            //Shapes.ForEach((shape) =>
            //{
            //    Array.AppendLine(shape.Key.ToString());
            //    shape.Value.Item1.ForEach(c =>
            //    {
            //        Array.AppendLine("\t\t\t{ " + c.Latitude.ToString("00.000000").Replace(",", ".") + ", " + c.Longitude.ToString("00.000000").Replace(",", ".") + " },");
            //    });
            //});

            //Output1.WriteLine(Array.ToString());
            //Output1.Flush();
            //Output1.Close();


            var diff_lat = Math.Abs(min_lat - max_lat);
            var diff_lng = Math.Abs(min_lng - max_lng);

            //Output2.WriteLine("From:       " + max_lat.ToString("00.000000").Replace(",", ".")  + ", " +  min_lng.ToString("00.000000").Replace(",", "."));
            //Output2.WriteLine("To:         " + min_lat.ToString("00.000000").Replace(",", ".")  + ", " +  max_lng.ToString("00.000000").Replace(",", "."));
            //Output2.WriteLine("Diff:       " + diff_lat.ToString("00.000000").Replace(",", ".") + ", " + diff_lng.ToString("00.000000").Replace(",", "."));
            //Output2.WriteLine("Resolution: " + min_resolution + " -> " + max_resolution);

            Shapes.ForEach((shape) =>
            {

                for (var resolution = min_resolution; resolution <= max_resolution; resolution++)
                {

                    shape.Value.Item2.Add(resolution, new Tuple<List<Tuple<UInt64, UInt64>>, StringBuilder>(new List<Tuple<UInt64, UInt64>>(), new StringBuilder()));

                    shape.Value.Item1.ForEach(coor =>
                    {
                        var XY = GeoCalculations.WorldCoordinates_2_Screen(coor, resolution);
                        shape.Value.Item2[resolution].Item1.Add(XY);
                    });

                }

            });

            var min_x = 0UL;
            var min_y = 0UL;

            for (var resolution = min_resolution; resolution <= max_resolution; resolution++)
            {

                min_x = UInt64.MaxValue;
                min_y = UInt64.MaxValue;

                Shapes.ForEach((shape) =>
                {
                    shape.Value.Item2[resolution].Item1.ForEach(XY =>
                    {
                        if (XY.Item1 < min_x) min_x = XY.Item1;
                        if (XY.Item2 < min_y) min_y = XY.Item2;
                    });
                });

                Shapes.ForEach((shape) =>
                {

                    var Char = "M ";

                    shape.Value.Item2[resolution].Item1.ForEach(XY =>
                    {
                        shape.Value.Item2[resolution].Item2.Append(Char + (XY.Item1 - min_x) + " " + (XY.Item2 - min_y) + " ");
                        if (Char == "L ") Char = "";
                        if (Char == "M ") Char = "L ";
                    });

                    shape.Value.Item2[resolution].Item2.Append("Z ");

                });

            }


            var ShapeLanguage = String.Empty;

            var ShapeDic = new Dictionary<UInt32, String>();

            for (var resolution = min_resolution; resolution <= max_resolution; resolution++)
            {
                ShapeLanguage = "\"";
                Shapes.ForEach((shape) => ShapeLanguage += shape.Value.Item2[resolution].Item2.ToString().Trim() + " ");
                ShapeDic.Add(resolution, ShapeLanguage.TrimEnd() + "\",");
            }

            return new ShapeInfo(Description, max_lat, max_lng, min_lat, min_lng, ShapeDic);

        }

    }

}