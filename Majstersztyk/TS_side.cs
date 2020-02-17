/*
 * Created by SharpDevelop.
 * User: TS040198
 * Date: 05/12/2018
 * Time: 12:44
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Majstersztyk
{
	/// <summary>
	/// Description of TS_side.
	/// </summary>
	public class TS_side
	{
		public TS_line line {get; private set;}
		
		public TS_point StartPoint {get; private set;}
		public TS_point EndPoint {get; private set;}
		
		public TS_side(TS_point StartPoint, TS_point EndPoint)
		{
			this.StartPoint = StartPoint;
			this.EndPoint = EndPoint;
			line = new TS_line(this);
		}		
		
		public double Length()
		{
			return Math.Sqrt(Math.Pow((EndPoint.X-StartPoint.X),2) + Math.Pow((EndPoint.Y-StartPoint.Y),2));
		}
		
		public bool IsCrossedWith(TS_side side2){
            TS_point crossPoint = line.Intersection(side2.line);
            if (IsContain(crossPoint) && side2.IsContain(crossPoint))
            {
                return true;
            }
            return false;
		}
		
        public bool IsCrossedWith(TS_line line2)
        {
            if (line.IsCrossedWith(line2))
            {
                TS_point crossPoint = line.Intersection(line2);
                if (IsContain(crossPoint))
                {
                    return true;
                }
            }
            return false;
        }
        
        public TS_point CrossedPoint(TS_side side2){
			TS_point point = line.Intersection(side2.line);
			
			if (IsContain(point))
				return point;
			
			return new TS_point();
        }
        
        public TS_point CrossedPoint(TS_line line2){
			TS_point point = line.Intersection(line2);
			
			if (IsContain(point))
				return point;
			
			return new TS_point();
        }

        public bool IsContain(TS_point point)
        {
            double x = point.X;
            double y = point.Y;
            if (point.IsReal() && line.IsContain(point))
            {
                double minX1 = Math.Min(StartPoint.X, EndPoint.X);
                double maxX1 = Math.Max(StartPoint.X, EndPoint.X);
                double minY1 = Math.Min(StartPoint.Y, EndPoint.Y);
                double maxY1 = Math.Max(StartPoint.Y, EndPoint.Y);

                if (x >= minX1 && x <= maxX1 && y >= minY1 && y <= maxY1)
                {
                    return true;
                }
                
        		if (TS_point.TS_AreDoublesEqual(x,StartPoint.X) && TS_point.TS_AreDoublesEqual(y,StartPoint.Y)) {
					return true;
        		}
        	
        		if (TS_point.TS_AreDoublesEqual(x,EndPoint.X) && TS_point.TS_AreDoublesEqual(y,EndPoint.Y)) {
					return true;
        		}
				return false;
                
                /*
				if (IsGreaterOrEqual(x, minX1) && IsLesserOrEqual(x, maxX1) && IsGreaterOrEqual(y, minY1) && IsLesserOrEqual(y, maxY1)) {
					return true;
				}
				return false;*/
            }
            return false;
        }
        
        private bool IsGreaterOrEqual(double x, double xToCompare){
			double dist = Math.Max(Math.Abs(x), Math.Abs(xToCompare)) / 1000000;
			if (x >= xToCompare-dist) return true;
			return false;
        }
        
        private bool IsLesserOrEqual(double x, double xToCompare){
			double dist = Math.Max(Math.Abs(x), Math.Abs(xToCompare)) / 1000000;
			if (x <= xToCompare+dist) return true;
			return false;
        }
        
        public double DirectedAngleToOX(){
			double r = Math.Sqrt(Math.Pow(EndPoint.X - StartPoint.X, 2) + Math.Pow(EndPoint.Y - StartPoint.Y, 2));
			double cos = (EndPoint.X - StartPoint.X) / r;
			if (TS_point.TS_AreDoublesEqual(0, cos)) {
				double sin = (EndPoint.Y - StartPoint.Y) / r;
				if (sin > 0) return Math.PI / 2;
				return Math.PI * 3 / 2;
			}
			if (cos > 0) return Math.Atan(line.SlopeA);
			return (Math.Atan(line.SlopeA) + Math.PI * 3) % (2 * Math.PI);
        }
        
        public double DirectedAngleToTheVector(TS_side vector2){
			//kierunek dodatni zgodny ze wskazówkami zegara
			double angle1 = DirectedAngleToOX();
			double angle2 = vector2.DirectedAngleToOX();
			double deltaA = angle1 - angle2;
			while (deltaA > Math.PI) deltaA -= 2 * Math.PI;
			while (deltaA < -Math.PI) deltaA += 2 * Math.PI;
			return deltaA;
        }

		public override string ToString()
		{
			return StartPoint.ToString() + " \t-->\t" + EndPoint.ToString();
		}
        
	}
}
