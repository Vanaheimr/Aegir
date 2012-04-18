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
        public static String Saarland = "M 64 2 L 64 2 65 3 65 4 67 4 67 6 67 6 68 5 69 6 69 6 69 7 72 7 72 6 72 5 73 5 73 6 75 6 75 7 75 8 75 8 76 8 76 9 78 9 78 11 81 11 81 10 81 10 82 10 83 9 83 8 84 8 84 9 83 10 83 10 84 11 84 12 84 15 85 15 85 15 87 15 87 16 87 17 87 18 87 17 86 17 86 17 85 19 85 19 85 20 85 21 85 22 86 22 86 22 86 23 87 23 87 24 86 25 86 25 85 26 85 26 85 26 82 27 82 27 82 28 82 28 82 29 82 29 83 30 84 30 85 32 86 32 86 33 86 33 86 34 86 34 85 35 85 35 87 35 86 36 88 36 89 37 92 37 91 38 93 38 93 38 92 39 93 39 95 38 96 38 96 39 95 39 95 39 96 40 96 41 95 40 95 41 96 42 96 42 96 43 95 43 95 44 94 44 94 45 94 45 94 46 94 45 95 45 95 45 95 45 95 46 94 46 94 47 94 48 94 49 94 50 93 51 92 51 92 51 90 51 90 51 90 52 90 53 90 53 89 53 89 54 89 55 88 55 88 54 87 54 88 54 87 54 87 55 86 55 85 57 86 57 85 58 86 59 87 59 86 59 86 60 86 61 87 62 88 63 89 64 89 64 90 64 90 65 90 66 90 66 91 67 91 67 92 67 92 70 90 70 90 69 89 69 89 69 87 71 87 71 87 71 86 73 86 73 86 74 84 73 85 73 84 72 84 73 84 72 83 72 83 73 82 72 81 72 81 71 81 72 80 72 78 73 78 73 77 74 77 74 77 73 76 72 76 72 75 72 74 72 74 72 74 73 73 73 73 72 72 72 72 72 72 72 71 72 70 70 69 71 68 71 68 70 66 72 65 70 65 68 64 66 63 64 60 63 60 63 60 63 59 62 54 61 54 60 54 60 53 59 53 59 52 59 52 59 52 59 51 60 51 60 50 60 50 61 49 61 49 60 48 60 47 59 46 59 45 60 45 60 45 60 44 60 45 66 44 67 40 67 40 67 39 67 38 67 35 67 34 66 33 63 34 60 31 60 29 59 29 55 27 54 28 51 27 51 27 50 27 50 26 49 26 49 25 49 25 48 24 48 23 47 20 43 19 41 20 41 20 40 20 40 21 41 22 40 22 40 22 40 23 40 22 39 22 39 22 39 22 39 21 38 21 37 21 37 21 36 19 36 19 36 19 36 18 34 16 30 16 30 16 30 12 27 6 24 4 25 1 26 1 26 1 26 0 26 0 26 1 25 1 24 1 24 1 21 1 20 2 20 1 17 0 17 0 17 0 16 1 16 1 15 1 14 3 14 3 15 5 15 5 15 6 14 6 14 8 15 9 16 10 15 10 15 10 15 11 15 11 15 11 16 12 16 12 17 12 16 13 17 14 16 14 16 14 17 15 17 15 16 16 16 17 16 18 16 19 15 19 15 20 15 20 16 20 17 20 17 21 18 23 18 24 18 23 17 23 17 24 17 24 16 25 15 26 15 26 15 27 15 27 15 27 14 28 14 28 15 28 15 29 14 30 14 30 16 31 16 31 15 32 14 32 13 32 13 34 13 34 12 35 12 35 13 36 13 37 12 37 11 38 11 41 8 42 7 42 8 43 8 43 8 44 9 44 8 45 8 45 7 47 5 47 5 48 6 49 4 51 4 52 4 53 4 53 3 53 2 53 2 54 1 54 2 55 3 55 2 57 1 57 2 58 2 58 1 58 2 58 2 58 3 58 3 59 3 59 2 60 2 60 2 61 1 62 0 62 1 z";
    }

    /// <summary>
    /// A feature on an Aegir map.
    /// </summary>
    public class Saarland : AShape
    {

        #region Saarland(StrokeColor, StrokeThickness, FillColor)

        public Saarland(Color StrokeColor, Double StrokeThickness, Color FillColor)
            : base(GermanyPaths.Saarland, 49.646819, 06.359445, 0, 49.122921, 07.411569, StrokeColor, StrokeThickness, FillColor)
        { }

        #endregion

    }

}

