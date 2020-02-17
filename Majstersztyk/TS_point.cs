/*
 * Created by SharpDevelop.
 * User: TS040198
 * Date: 05/12/2018
 * Time: 12:34
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Majstersztyk
{
	/// <summary>
	/// Description of TS_point.
	/// </summary>
	public class TS_point
	{
		public double X {get; private set;}
		public double Y {get; private set;}
		
		public TS_point(){
			X = Double.NaN;
			Y = Double.NaN;
		}
		
		public TS_point(double X, double Y)
		{
			this.X = X;
			this.Y = Y;
		}
		
		public bool IsReal(){
			if (Double.IsNaN(X) || Double.IsNaN(Y))
				return false;
			return true;
		}
		
		public TS_point Transform(TS_point punkt0){
			return new TS_point(X-punkt0.X, Y-punkt0.Y);
		}
		
		public static bool TS_AreDoublesEqual(double d1, double d2)
		{
			if (Double.IsInfinity(d1) && !Double.IsInfinity(d2)) {
				return false;
			}
			
			if (!Double.IsInfinity(d1) && Double.IsInfinity(d2)) {
				return false;
			}
			
			double diff = Math.Abs(Math.Min(d1, d2) / 1000000);
			
			if (Math.Abs(d1-d2) <= diff) return true;
			
			return false;
		}
		
		public override string ToString()
		{
			string format = "{0:e4}";
			return "[" + string.Format(format,X) + ";" + string.Format(format,Y) + "]";
		}
		
		public bool IsTheSame(TS_point point2){
			if (TS_AreDoublesEqual(X, point2.X) && TS_AreDoublesEqual(Y, point2.Y)) return true;
			return false;
		}
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			TS_point other = obj as TS_point;
				if (other == null)
					return false;
			return TS_AreDoublesEqual(this.X, other.X) && TS_AreDoublesEqual(this.Y, other.Y);
		}
/*
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				hashCode += 1000000007 * X.GetHashCode();
				hashCode += 1000000009 * Y.GetHashCode();
			}
			return hashCode;
		}

		public static bool operator ==(TS_point lhs, TS_point rhs) {
			if (TS_AreDoublesEqual(lhs.X, rhs.X) && TS_AreDoublesEqual(lhs.Y, rhs.Y))
				return true;
			if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
				return false;
			return lhs.Equals(rhs);
		}

		public static bool operator !=(TS_point lhs, TS_point rhs) {
			return !(lhs == rhs);
		}
*/
		#endregion

	}
}
