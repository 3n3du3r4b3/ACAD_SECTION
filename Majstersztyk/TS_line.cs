/*
 * Created by SharpDevelop.
 * User: TS040198
 * Date: 05/12/2018
 * Time: 14:25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Majstersztyk
{
	/// <summary>
	/// Description of TS_line.
	/// </summary>
	public class TS_line
	{
		public double GenA {get; private set;}
		public double GenB {get; private set;}
		public double GenC {get; private set;}
		// dla postaci prostej Ax + By + C = 0
		
		public double SlopeA {get; private set;}
		public double InterceptB {get; private set;}
		// dla postaci prostej y = ax + b
		
		private TS_point point1 {get;set;}
		private	TS_point point2 {get;set;}
		
		public TS_line(TS_side division)
		{
			point1 = division.StartPoint;
			point2 = division.EndPoint;
			Factors();
		}
		
		public TS_line(TS_point pointA, TS_point pointB)
		{
			point1 = pointA;
			point2 = pointB;
			Factors();
		}
		
		public TS_line(double SlopeA, double InterceptB){
			this.SlopeA = SlopeA;
			this.InterceptB = InterceptB;
			GenB = -1;
			GenA = SlopeA;
			GenC = InterceptB;
		}
		
		public TS_line(double A, double B, double C){
			this.GenA = A;
			this.GenB = B;
			this.GenC = C;
			SlopeA = GenA / (-GenB);
			InterceptB = GenC / (-GenB);
		}
		
		private double SlopeFactorA()
		{
			if (TS_point.TS_AreDoublesEqual(point1.X, point2.X)) return Double.NaN;
			return (point2.Y - point1.Y)/(point2.X - point1.X);
		}
		
		private double InterceptFactorB()
		{
			SlopeA = SlopeFactorA();
			if (Double.IsNaN(SlopeA)) return Double.NaN;
			return point2.Y - (SlopeA * point2.X);
		}
		
		private void Factors()
		{
			if (TS_point.TS_AreDoublesEqual(point1.X, point2.X)) {
				SlopeA = Double.PositiveInfinity;
				InterceptB = Double.NaN;
				GenA = 1;
				GenB = 0;
				GenC = -point1.X;
			}
			else 
			{
				SlopeA = (point2.Y - point1.Y)/(point2.X - point1.X);
				InterceptB = point2.Y - (SlopeA * point2.X);
				GenA = SlopeA;
				GenB = -1;
				GenC = InterceptB;
			}
		}
		
		private double GetValueOf_Yaxis(double x){
			return (-GenA * x - GenC) / GenB;
		}
		
		private double GetValueOf_Xaxis(double y){
			return (-GenB * y - GenC) / GenA;
		}
		
		public TS_point Intersection(TS_line line2)
		{
			double a1 = SlopeA;
			double b1 = InterceptB;
			double a2 = line2.SlopeA;
			double b2 = line2.InterceptB;
			
			if (TS_point.TS_AreDoublesEqual(a1, a2)) return new TS_point(Double.NaN, Double.NaN);
			/*
			double x = (b1-b2)/(a2-a1);
			double y = (a2*b1 - b2*a1)/(a2-a1);
			return new TS_point(x,y);
			*/
			
			double A1 = GenA;
			double A2 = line2.GenA;
			double B1 = GenB;
			double B2 = line2.GenB;
			double C1= GenC;
			double C2 = line2.GenC;
			
			double D1 = (B2-B1)/(A1-A2);
			double D2 = (C2-C1)/(A1-A2);
			
			double y = -(A1 * D2 + C1) / (A1 * D1 + B1);
			double x = D1 * y + D2;
			
			return new TS_point(x,y);
		}

        public bool IsCrossedWith(TS_line line2)
        {
            TS_point crossPoint = Intersection(line2);
            if (Double.IsNaN(crossPoint.X) || Double.IsNaN(crossPoint.Y))
            {
                return false;
            }
            return true;
        }

        public bool IsContain(TS_point point)
        {
            if (TS_point.TS_AreDoublesEqual(GenA * point.X + GenB * point.Y + GenC, 0))
            {
                return true;
            }
            return false;
        }
		
		public override string ToString()
		{
			string format = "{0:0.###}";
			string A = String.Format(format, GenA);
			string B = String.Format(format, GenB);
			string C = String.Format(format, GenC);
			string a = String.Format(format, SlopeA);
			string b = String.Format(format, InterceptB);
			
			return A + "x+" + B + "y+" + C + "=0";
		}
		
		public double AngleToOX(){
			if (TS_point.TS_AreDoublesEqual(GenB,0.0)) {
				return Math.PI /2;
			}
			return Math.Atan(SlopeA);
		}
		
	}
}
