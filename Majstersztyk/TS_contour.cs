/*
 * Created by SharpDevelop.
 * User: TS040198
 * Date: 12/11/2018
 * Time: 16:36
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace Majstersztyk
{
	/// <summary>
	/// Description of TS_contour.
	/// </summary>
	public class TS_contour:TS_region
	{
		public List<TS_point> Vertices { get; protected set; }
		public List<TS_side> Sides { get; protected set; }
		public override string TypeOf { get { return typeOf; } }
		private static new string typeOf = "Contour";
		protected TS_contour() {
			Vertices = new List<TS_point>();
		}

		public TS_contour(List<TS_point> vertices) {
			Update(vertices);
		}

		public void Update(List<TS_point> vertices) {
			Vertices = vertices;
			if (CalcArea() < 0)
			{
				Vertices.Reverse();
			}

			Sides = GenerateSides(vertices);
			CalcProperties();
		}

		protected override double CalcArea() {
			double A = 0;
			for (int i = 0; i < Vertices.Count; i++)
			{
				int j = IndexOfNextVertex(i);
				A += ((Vertices[j].X - Vertices[i].X) * (Vertices[j].Y + Vertices[i].Y));
			}
			return 1d / 2 * A;
		}

		protected override double CalcSx() {
			double Sx = 0;
			for (int i = 0; i < Vertices.Count; i++)
			{
				int j = IndexOfNextVertex(i);
				Sx += ((Vertices[j].X - Vertices[i].X) * (Vertices[i].Y * Vertices[i].Y + Vertices[i].Y * Vertices[j].Y + Vertices[j].Y * Vertices[j].Y));
			}
			return 1d / 6 * Sx;
		}

		protected override double CalcSy() {
			double Sy = 0;
			for (int i = 0; i < Vertices.Count; i++)
			{
				int j = IndexOfNextVertex(i);
				Sy += ((Vertices[i].Y - Vertices[j].Y) * (Vertices[i].X * Vertices[i].X + Vertices[i].X * Vertices[j].X + Vertices[j].X * Vertices[j].X));
			}
			return 1d / 6 * Sy;
		}

		protected override double CalcIx() {
			double Ix = 0;
			for (int i = 0; i < Vertices.Count; i++)
			{
				int j = IndexOfNextVertex(i);
				Ix += (Vertices[j].X - Vertices[i].X) *
					(Vertices[i].Y * Vertices[i].Y * Vertices[i].Y +
					 Vertices[i].Y * Vertices[i].Y * Vertices[j].Y +
					 Vertices[i].Y * Vertices[j].Y * Vertices[j].Y +
					 Vertices[j].Y * Vertices[j].Y * Vertices[j].Y);
			}
			return 1d / 12 * Ix;
		}

		protected override double CalcIy() {
			double Iy = 0;
			for (int i = 0; i < Vertices.Count; i++)
			{
				int j = IndexOfNextVertex(i);
				Iy += (Vertices[i].Y - Vertices[j].Y) *
					(Vertices[i].X * Vertices[i].X * Vertices[i].X +
					 Vertices[i].X * Vertices[i].X * Vertices[j].X +
					 Vertices[i].X * Vertices[j].X * Vertices[j].X +
					 Vertices[j].X * Vertices[j].X * Vertices[j].X);
			}
			return 1d / 12 * Iy;
		}

		protected override double CalcIxy() {
			double Ixy = 0;
			for (int i = 0; i < Vertices.Count; i++)
			{
				int j = IndexOfNextVertex(i);
				Ixy += (Vertices[j].X - Vertices[i].X) *
					(Vertices[i].X *
					 (3 * Vertices[i].Y * Vertices[i].Y +
					  Vertices[j].Y * Vertices[j].Y +
					  2 * Vertices[i].Y * Vertices[j].Y) +
					 (Vertices[j].X *
					  (3 * Vertices[j].Y * Vertices[j].Y +
					   Vertices[i].Y * Vertices[i].Y +
					   2 * Vertices[i].Y * Vertices[j].Y)));
			}
			return 1d / 24 * Ixy;
		}
		
		public int IndexOfNextVertex(int currentVertex)
		{
			return (currentVertex + 1) % Vertices.Count;
		}
		
		public int IndexOfNextSide(int currentSide)
		{
			return (currentSide + 1) % Sides.Count;
		}

		protected static List<TS_side> GenerateSides(List<TS_point> points)
		{
			List<TS_side> sides = new List<TS_side>();
			for (int i = 0; i < points.Count; i++)
			{
				int j = (i + 1) % points.Count;
				sides.Add(new TS_side(points[i], points[j]));
			}
			return sides;
		}

		protected override bool IsObjectCorrect()
		{
			for (int i = 0; i < Sides.Count; i++)
			{
				for (int j = 0; j < Sides.Count; j++)
				{
					if (Math.Abs(i - j) > 1)
						if ((Math.Abs(i - j) + 1) != Sides.Count)
							if (Sides[i].IsCrossedWith(Sides[j]))
								return false;
				}
			}
			return true;
		}
		
		public bool IsPointInside(TS_point point){
			if (IsAVertex(point)) return false;
			
			foreach (var side in Sides) {
				if (side.IsContain(point)) return false;
			}
			
			double sum = 0;
			for (int i = 0; i < Vertices.Count; i++) {
				TS_side vector1 = new TS_side(point, Vertices[i]);
				TS_side vector2 = new TS_side(point, Vertices[IndexOfNextSide(i)]);
				double angle = vector1.DirectedAngleToTheVector(vector2);
				sum += angle;
			}
			
			if (IsAlmostZero(sum)) {
				
				return false;
			}
			
			if (TS_point.TS_AreDoublesEqual(sum%(2*Math.PI),0)) {
				return true;
			}
			
			//Angle Summation Test http://erich.realtimerendering.com/ptinpoly/#referrer=blog.vokiel.com
			//The worst algorithm in the world ;)
			
			
			/* THREE CROSSING ALGORITHM http://erich.realtimerendering.com/ptinpoly/#referrer=blog.vokiel.com
			TS_line horLine = new TS_line(0, point.Y);
			List<TS_point> horPoints = new List<TS_point>();
			
			foreach (var side in Sides) {
				TS_point pointX = side.CrossedPoint(horLine);
				if (pointX.X > point.X && !horPoints.Contains(pointX)) {  //  to trzeba INACZEJ!
					horPoints.Add(pointX);
					}
				}
			
			int countH = horPoints.Count;
			if (countH % 2 != 0) return true;
			
			TS_line vertLine = new TS_line(1, 0, -point.X);
			List<TS_point> vertPoints = new List<TS_point>();
			
			foreach (var side in Sides) {
				TS_point pointY = side.CrossedPoint(vertLine);
				if (pointY.Y > point.Y && !vertPoints.Contains(pointY)) {
					vertPoints.Add(pointY);
				}
			}
			
			int countV = vertPoints.Count;
			if (countV % 2 != 0) return true;
			
			TS_line horLineL = new TS_line(0, point.Y);
			List<TS_point> horPointsL = new List<TS_point>();
			
			foreach (var side in Sides) {
				TS_point pointX = side.CrossedPoint(horLineL);
				if (pointX.X > point.X && !horPointsL.Contains(pointX)) {
					horPointsL.Add(pointX);
					}
				}
			
			int countHL = horPointsL.Count;
			if (countHL % 2 != 0) return true;
			
			List<TS_point> vertPointsD = new List<TS_point>();
			
			foreach (var side in Sides) {
				TS_point pointY = side.CrossedPoint(vertLine);
				if (pointY.Y > point.Y && !vertPointsD.Contains(pointY)) {
					vertPointsD.Add(pointY);
				}
			}
			
			int countVD = vertPointsD.Count;
			if (countVD % 2 != 0) return true;
			*/
			return true;
		}
		
		private bool IsAlmostZero(double x)
		{
			double error = Math.Pow(10, -10);
			if (Math.Abs(x) < error)
				return true;
			return false;
		}
		
		public bool IsAVertex(TS_point point){
			foreach (var vertex in Vertices) {
				if (AreDoublesEqual(vertex.X,point.X) && AreDoublesEqual(vertex.Y,point.Y)) {
					return true;
				}
			}
			return false;
		}
		
		public override string ToString()
		{
			return Environment.NewLine + base.ToString();
		}

		public virtual TS_point TopLeftPoint {
			get {
				double Xmin = Vertices[0].X;
				double Ymax = Vertices[0].Y;

				foreach (TS_point thisPoint in Vertices)
				{
					if (thisPoint.X < Xmin)
						Xmin = thisPoint.X;
					if (thisPoint.Y > Ymax)
						Ymax = thisPoint.Y;
				}

				return new TS_point(Xmin, Ymax);
			}
		}

		public virtual TS_point BottomRightPoint {
			get {
				double Xmax = Vertices[0].X;
				double Ymin = Vertices[0].Y;

				foreach (TS_point thisPoint in Vertices)
				{
					if (thisPoint.X > Xmax)
						Xmax = thisPoint.X;
					if (thisPoint.Y < Ymin)
						Ymin = thisPoint.Y;
				}

				return new TS_point(Xmax, Ymin);
			}
		}

	}
}
