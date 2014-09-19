using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias.Geometry;
using org.GraphDefined.Vanaheimr.Illias.Geometry.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.GraphDefined.Vanaheimr.Aegir
{

    public class TriCircle
    {

        public GeoTriangle      Triangle;
        public GeoCircle        Circle;
        public Boolean          Deleted;
        public List<TriCircle>  Neighbors;

        public TriCircle(GeoTriangle Triangle, GeoCircle Circle)
        {
            this.Triangle   = Triangle;
            this.Circle     = Circle;
            this.Neighbors  = new List<TriCircle>();
        }

        public TriCircle(GeoTriangle Triangle)
        {

            this.Triangle   = Triangle;
            this.Circle     = new GeoCircle(Triangle.P1,
                                            Triangle.P2,
                                            Triangle.P3);
            this.Neighbors  = new List<TriCircle>();

        }

    }

    public static class MathsExt
    {

        #region DelaunayTriangulation(this VoronoiFeatures)

        /// <summary>
        /// Calculate a delaunay triangulation for the given enumeration of pixels.
        /// </summary>
        /// <typeparam name="T">The type of the pixels.</typeparam>
        /// <param name="VoronoiFeatures">An enumeration of pixels of type T.</param>
        /// <returns>An enumeration of triangles of type T.</returns>
        public static IEnumerable<TriCircle> DelaunayTriangulation(this IEnumerable<VoronoiFeature> VoronoiFeatures)
        {

            #region Initial Checks

            var _VoronoiFeatures  = VoronoiFeatures.ToList();
            var _NumberOfFeatures = _VoronoiFeatures.Count;

            //if (_NumberOfFeatures < 3)
            //    throw new ArgumentException("Need at least three pixels for triangulation!");

            var TriCircles = new List<TriCircle>();

            #endregion

            #region Set up the first triangle

            var j = 0;
            VoronoiFeature[] tri1;
            TriCircle triangle = null;

            do
            {
                tri1     = _VoronoiFeatures.Skip(j++).Take(3).ToArray();
                triangle = new TriCircle(new GeoTriangle(tri1[0].GeoCoordinate, tri1[1].GeoCoordinate, tri1[2].GeoCoordinate));
            }
            while (triangle.Circle.Radius == 0);

            TriCircles.Add(triangle);
            _VoronoiFeatures = _VoronoiFeatures.Where(vf => !tri1.Contains(vf)).ToList();

            #endregion

            #region Include each point one at a time into the existing mesh

            for (int i = 0; i < _VoronoiFeatures.Count; i++)
            {

                var Found = false;

                #region The new voronoi feature lies inside an existing triangle

                var TriCircleList = TriCircles.Where(TriCircle => TriCircle.Deleted == false).
                                               OrderBy(TriCircle => TriCircle.Circle.Radius).ToArray();

                foreach (var TriCircle in TriCircleList)
                {

                    if (TriCircle.Triangle.Contains(_VoronoiFeatures[i].GeoCoordinate))
                    {

                        var Edges = new List<GeoLine>();

                        Edges.Add(new GeoLine(TriCircle.Triangle.P1, TriCircle.Triangle.P2));
                        Edges.Add(new GeoLine(TriCircle.Triangle.P2, TriCircle.Triangle.P3));
                        Edges.Add(new GeoLine(TriCircle.Triangle.P3, TriCircle.Triangle.P1));

                        // Add three new triangles for the current point
                        foreach (var _Edge in Edges)
                            TriCircles.Add(new TriCircle(new GeoTriangle(_Edge.P1, _Edge.P2, _VoronoiFeatures[i].GeoCoordinate)));

                        TriCircle.Deleted = true;
                        Found = true;
                        break;

                    }

                }

                #endregion

                #region An external feature

                if (!Found)
                {

                    var PointList1 = (TriCircles.Select(TriCircle => new
                    {
                        point = TriCircle.Triangle.P1,
                        distance = TriCircle.Triangle.P1.DistanceTo(_VoronoiFeatures[i].GeoCoordinate)
                    }).

                                     Union(
                                     TriCircles.Select(TriCircle => new
                                     {
                                         point = TriCircle.Triangle.P2,
                                         distance = TriCircle.Triangle.P2.DistanceTo(_VoronoiFeatures[i].GeoCoordinate)
                                     })).

                                     Union(
                                     TriCircles.Select(TriCircle => new
                                     {
                                         point = TriCircle.Triangle.P3,
                                         distance = TriCircle.Triangle.P3.DistanceTo(_VoronoiFeatures[i].GeoCoordinate)
                                     }))).

                                     OrderBy(agg => agg.distance).ToArray();

                    var PointList2 = PointList1.
                                     Where(v =>
                                     {

                                         var newLine = new GeoLine(_VoronoiFeatures[i].GeoCoordinate, v.point);

                                         foreach (var edges in TriCircles.Select(v2 => v2.Triangle.Borders))
                                         {
                                             foreach (var edge in edges)
                                             {
                                                 if (edge.IntersectsWith(newLine, false, true))
                                                 {
                                                     return false;
                                                 }
                                             }
                                         }

                                         return true;

                                     }).

                                     ToArray();

                    TriCircles.Add(new TriCircle(new GeoTriangle(PointList2[0].point, PointList2[1].point, _VoronoiFeatures[i].GeoCoordinate)));

                }

                #endregion

            }

            #endregion

            return TriCircles.Where(tc => tc.Deleted == false);//.Select(tc => tc.Triangle);

        }

        #endregion

    }
}
