/*
 * Copyright (c) 2011-2013 Achim 'ahzf' Friedland <achim@graph-database.org>
 * This file is part of Aegir <http://www.github.com/Vanaheimr/Aegir>
 * 
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 3 of the License, or
 * (at your option) any later version.
 * 
 * You may obtain a copy of the License at
 *   http://www.gnu.org/licenses/gpl.html
 * 
 * This program is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * General Public License for more details.
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