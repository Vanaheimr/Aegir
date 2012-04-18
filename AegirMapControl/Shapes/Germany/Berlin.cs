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
using de.ahzf.Illias.Commons;
using System.Windows;

#endregion

namespace de.ahzf.Vanaheimr.Aegir
{

    public static partial class GermanyPaths
    {
        public static String Berlin = "M 36 1 L 37 1 37 1 36 3 38 4 39 4 39 4 40 5 40 5 39 6 38 8 38 8 38 9 38 10 39 13 40 12 41 13 43 13 44 15 44 16 45 16 46 17 46 19 48 19 48 20 49 21 49 22 49 22 51 21 51 22 52 22 52 22 51 23 49 26 49 27 50 27 49 29 48 29 48 31 49 31 49 30 50 30 51 29 52 30 53 30 53 30 53 30 53 30 55 31 56 31 56 32 57 33 57 32 56 31 56 31 57 31 58 34 61 34 61 34 62 35 61 35 61 35 60 35 60 37 59 37 59 37 59 39 60 40 59 40 59 41 58 41 55 43 55 43 56 43 56 44 57 44 56 45 55 46 55 45 54 46 53 46 53 47 52 49 52 50 51 50 50 49 50 47 52 46 52 46 51 46 51 44 50 44 50 44 48 45 48 44 47 43 47 43 47 42 44 43 41 43 41 41 41 41 40 41 40 41 38 41 37 41 36 42 36 41 35 38 34 38 34 38 32 39 31 40 31 40 32 43 31 43 31 45 28 44 28 43 26 43 26 42 25 41 24 40 24 39 21 41 20 39 20 39 17 41 17 41 16 40 15 40 15 39 15 38 13 38 11 39 10 39 7 41 7 42 8 42 8 42 7 42 6 42 5 43 5 42 4 42 2 41 2 40 1 40 1 39 0 38 2 38 3 37 3 37 4 36 3 34 3 34 3 31 3 31 3 30 4 29 4 29 7 26 8 25 7 25 6 24 6 23 5 24 3 24 3 23 4 20 4 18 5 18 6 18 6 17 6 16 6 15 6 14 5 15 4 15 4 13 5 13 6 13 7 12 8 12 9 12 10 13 10 13 11 14 11 13 12 13 13 12 12 12 12 12 11 11 11 10 12 10 13 7 14 8 14 7 17 8 17 6 16 6 16 5 18 5 18 3 20 3 20 3 20 4 20 5 21 6 21 7 22 8 25 8 27 7 27 6 27 6 28 6 28 5 28 4 29 5 30 5 30 5 31 6 32 6 32 5 33 4 33 4 33 4 34 4 34 4 35 4 36 3 33 2 34 1 35 1 36 0 z";
    }

    /// <summary>
    /// A feature on an Aegir map.
    /// </summary>
    public class Berlin : AShape
    {

        #region Berlin(StrokeColor, StrokeThickness, FillColor)

        public Berlin(Color StrokeColor, Double StrokeThickness, Color FillColor)
            : base(GermanyPaths.Berlin, 52.675022, 13.093819, 0, 52.338722, 13.768409, StrokeColor, StrokeThickness, FillColor)
        { }

        #endregion

    }

}

