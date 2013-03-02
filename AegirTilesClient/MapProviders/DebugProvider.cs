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

/*
#region Usings

using System;
using System.IO;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Imaging;

#endregion

namespace de.Vanaheimr.Aegir.Tiles
{

    /// <summary>
    /// A Debug Map Provider
    /// </summary>
    public class DebugProvider : AMapProvider
    {

        #region Data

        /// <summary>
        /// The well-known name for this map provider.
        /// </summary>
        public static String Name = "Debug";

        #endregion

        #region Constructor(s)

        #region DebugProvider()

        /// <summary>
        /// Creates a new Debug Map Provider.
        /// </summary>
        public DebugProvider()
            : base(Name:               Name,
                   Description:        "For debugging.",
                   InfoUri:            "",
                   Copyright:          "CC",
                   IsMemoryCacheable:  true,
                   MemoryCacheEnabled: true,
                   UriPattern:         "<none>",
                   Hosts:              new String[1] { "<none>" })
        { }

        #endregion

        #endregion


        #region GetTile(Zoom, X, Y)

        /// <summary>
        /// Return the tile for the given zoom, x and y parameters.
        /// </summary>
        /// <param name="Zoom">The zoom level.</param>
        /// <param name="X">The tile x-value.</param>
        /// <param name="Y">The tile y-value.</param>
        /// <returns>A stream containing the tile.</returns>
        public override Stream GetTile(UInt32 Zoom, UInt32 X, UInt32 Y)
        {

            var _Bitmap   = new Bitmap(256, 256);
            var _Graphics = Graphics.FromImage(_Bitmap);

            _Graphics.FillEllipse(new SolidBrush(Color.FromArgb(245, 245, 245)), 0, 0, 255, 255);
            _Graphics.DrawEllipse(new Pen(Color.FromArgb(250, 250, 250)), 0, 0, 255, 255);

            var _StringFormat = new StringFormat();
            _StringFormat.Alignment     = StringAlignment.Center;
            _StringFormat.LineAlignment = StringAlignment.Center;
            _Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

            _Graphics.DrawString(String.Format("Zoom {0}\nX={1} Y={2}", Zoom, X, Y),
                                 new Font("Arial", 12),
                                 Brushes.Black,
                                 new RectangleF(0, 0, 256, 256),
                                 _StringFormat);
            
            var _MemoryStream = new MemoryStream();
            _Bitmap.Save(_MemoryStream, ImageFormat.Png);
            _Bitmap.Dispose();

            return _MemoryStream;

        }

        #endregion

    }

}
*/