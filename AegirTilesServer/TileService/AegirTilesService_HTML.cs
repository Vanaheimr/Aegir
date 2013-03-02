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
using System.Text;
using System.Reflection;

using de.ahzf.Vanaheimr.Hermod.HTTP;

#endregion

namespace de.ahzf.Vanaheimr.Aegir.Tiles
{

    /// <summary>
    /// HTML content representation.
    /// </summary>
    public class AegirTileService_HTML : AAegirTileService
    {

        #region Constructor(s)

        #region AegirTileService_HTML()

        /// <summary>
        /// Creates a new tile service for HTML content.
        /// </summary>
        public AegirTileService_HTML()
            : base(HTTPContentType.HTML_UTF8)
        { }

        #endregion

        #region AegirTileService_HTML(IHTTPConnection)

        /// <summary>
        /// Creates a new tile service for HTML content.
        /// </summary>
        /// <param name="IHTTPConnection">The http connection for this request.</param>
        public AegirTileService_HTML(IHTTPConnection IHTTPConnection)
            : base(IHTTPConnection, HTTPContentType.HTML_UTF8)
        {
            this.CallingAssembly = Assembly.GetExecutingAssembly();
        }

        #endregion

        #endregion


        #region (private) HTMLBuilder(myHeadline, myFunc)

        /// <summary>
        /// A little HTML genereator...
        /// </summary>
        /// <param name="myHeadline"></param>
        /// <param name="AddHTMLAction"></param>
        /// <returns></returns>
        private String HTMLBuilder(String myHeadline, Action<StringBuilder> AddHTMLAction = null)
        {

            var _StringBuilder = new StringBuilder();

            _StringBuilder.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            _StringBuilder.AppendLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.1//EN\" \"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\">");
            _StringBuilder.AppendLine("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
            _StringBuilder.AppendLine("<head>");
            _StringBuilder.AppendLine("<title>Gera</title>");
            _StringBuilder.AppendLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"/resources/style.css\" />");
            _StringBuilder.AppendLine("</head>");
            _StringBuilder.AppendLine("<body>");
            _StringBuilder.Append("<h1>").Append(myHeadline).AppendLine("</h1>");
            _StringBuilder.AppendLine("<table>");
            _StringBuilder.AppendLine("<tr>");
            _StringBuilder.AppendLine("<td style=\"width: 100px\">&nbsp;</td>");
            _StringBuilder.AppendLine("<td>");

            if (AddHTMLAction != null)
                AddHTMLAction(_StringBuilder);

            _StringBuilder.AppendLine("</td>");
            _StringBuilder.AppendLine("</tr>");
            _StringBuilder.AppendLine("</table>");
            _StringBuilder.AppendLine("</body>");
            _StringBuilder.AppendLine("</html>").AppendLine();

            return _StringBuilder.ToString();

        }

        #endregion


        #region GetLandingpage()

        /// <summary>
        /// The HTML landing page.
        /// </summary>
        public HTTPResponse GetLandingpage()
        {

            var _RequestHeader = IHTTPConnection.RequestHeader;
            var _Content       = Encoding.UTF8.GetBytes(HTMLBuilder("Hello world!",
                    str => str.AppendLine("<a href=\"/Accounts\">List AccountIds</a><br />").
                               AppendLine("<script type=\"text/javascript\" src=\"resources/jQuery/jquery-1.6.1.min.js\"></script>").
                               AppendLine("<script type=\"text/javascript\">").
                               AppendLine("  function CreateNewAccount() {").
                               AppendLine("    jQuery.ajax({").
                               AppendLine("        type:     \"put\",").
                               AppendLine("        url:      \"/Account/\" + $(\"#NewAccountForm input[name=NewAccountId]\").val(),").
                               AppendLine("        dataType: \"json\",").
                               AppendLine("        async:    false,").
                               AppendLine("        success:  function(msg) { $(\"#infoarea\").html(\" <a href=\\\"/Account/\" + msg.AccountId + \" \\\">\" + msg.AccountId + \"</a>\");},").
                               AppendLine("        error:    function(msg) { alert(\"error: \" + msg); },").
                               AppendLine("        });").
                               AppendLine("  }").
                               AppendLine("</script>").
                               AppendLine("<form action=\"/Accounts\" method=\"post\">").
                               AppendLine(  "<input id=\"Senden\" type=\"submit\" name=\"senden\" value=\"Create random account\" />").
                               AppendLine("</form><br />").
                               AppendLine("<form id=\"NewAccountForm\" action=\"javascript:CreateNewAccount()\">").
                               AppendLine(  "<input id=\"NewAccountId\" type=\"text\" name=\"NewAccountId\" />").
                               AppendLine(  "<input id=\"Senden\" type=\"submit\" name=\"senden\" value=\"Create new account\" />").
                               AppendLine("</form><br />").
                               AppendLine("<div id=\"infoarea\"></div>")
                ));

            return new HTTPResponseBuilder()
            {
                HTTPStatusCode = HTTPStatusCode.OK,
                CacheControl   = "no-cache",
                Content        = _Content,
                ContentType    = HTTPContentType.HTML_UTF8
            };

        }

        #endregion




        public HTTPResponse GetTiles(String Mapprovider, String Zoom, String X, String Y)
        {
            throw new NotImplementedException();
        }

        
    }

}

