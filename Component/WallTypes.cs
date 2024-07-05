using System;

namespace Component
{
    public class WallTypes
    {
        public class CurtainArcWall
        {
            public Point Center { get; set; }
            public Double Radius { get; set; }

            public Double StartAngle { get; set; }
            public Double EndAngle { get; set; }

            public Point Xaxis { get; set; }
            public Point Yaxis { get; set; }

        }

        public class ArcWall
        {
            public Point StartPoint { get; set; }
            public Point PointOnArc { get; set; }
            public Point EndPoint { get; set; }

        }
    }
}
