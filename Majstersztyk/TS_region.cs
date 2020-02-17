using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Majstersztyk
{
    public abstract class TS_region
    {
        public bool IsCorrect { get { return IsObjectCorrect(); } }

        public string Name { get; set; }

        public virtual double Area { get; protected set; }
        public virtual double StaticMomX { get; protected set; }
        public virtual double StaticMomY { get; protected set; }
        public virtual double InertiaMomX { get; protected set; }
        public virtual double InertiaMomY { get; protected set; }
        public virtual double DeviationMomXY { get; protected set; }
        public virtual TS_point Centroid { get; protected set; }
        public virtual double CentrInertiaMom_X { get; protected set; }
        public virtual double CentrInertiaMom_Y { get; protected set; }
        public virtual double CentrDeviationMom_XY { get; protected set; }
        public virtual double CentrPrincipleInertiaMom_1 { get; protected set; }
        public virtual double CentrPrincipleInertiaMom_2 { get; protected set; }
        public virtual double AngleOfPrincipleLayout { get; protected set; }
        public virtual string TypeOf {get{return typeOf;}}
		protected string typeOf;
        
        protected abstract double CalcArea();
        protected abstract double CalcSx();
        protected abstract double CalcSy();
        protected abstract double CalcIx();
        protected abstract double CalcIy();
        protected abstract double CalcIxy();

        protected abstract bool IsObjectCorrect();
        //public abstract void Update();

        protected void CalcProperties()
        {
            Area = CalcArea();
            StaticMomX = CalcSx();
            StaticMomY = CalcSy();
            InertiaMomX = CalcIx();
            InertiaMomY = CalcIy();
            DeviationMomXY = CalcIxy();
            Centroid = CalcCentroid();
            CalcCentralProp();
            CalcCentralMainProp();
            //Console.WriteLine(this.ToString());
        }
        
        protected TS_point CalcCentroid()
        {
            return new TS_point(StaticMomY / Area, StaticMomX / Area);
        }
        
        public static bool AreDoublesEqual(double d1, double d2)
		{
        	double diff = Math.Abs(Math.Min(d1,d2)/1000000);
			
			if (Math.Abs(d1-d2) <= diff) return true;
			
			return false;
		}

        protected virtual void CalcCentralProp() {
            CentrInertiaMom_X = InertiaMomX - Math.Pow(Centroid.Y, 2) * Area;
            CentrInertiaMom_Y = InertiaMomY - Math.Pow(Centroid.X, 2) * Area;
            CentrDeviationMom_XY = DeviationMomXY - (Centroid.X * Centroid.Y) * Area;
        }

        private void CalcCentralMainProp() {
            if (AreDoublesEqual(CentrInertiaMom_X, CentrInertiaMom_Y)) {
                AngleOfPrincipleLayout = 0;
            } else {
                double tg2fi0 = -2 * CentrDeviationMom_XY / (CentrInertiaMom_X - CentrInertiaMom_Y);
                AngleOfPrincipleLayout = Math.Atan(tg2fi0) / 2;
            }
            CentrPrincipleInertiaMom_1 = 1d / 2 * (CentrInertiaMom_X + CentrInertiaMom_Y) +
                1d / 2 * Math.Sqrt(Math.Pow(CentrInertiaMom_X - CentrInertiaMom_Y, 2) + 4 * Math.Pow(CentrDeviationMom_XY, 2));
            CentrPrincipleInertiaMom_2 = 1d / 2 * (CentrInertiaMom_X + CentrInertiaMom_Y) -
            1d / 2 * Math.Sqrt(Math.Pow(CentrInertiaMom_X - CentrInertiaMom_Y, 2) + 4 * Math.Pow(CentrDeviationMom_XY, 2));
        }

        public override string ToString() {
            string text = "";
            string format = "{0:e4}";

            text += "Name: " + Name + 
                Environment.NewLine + "Type: " + TypeOf + 
                Environment.NewLine + "Is correct?: " + IsCorrect;
            text += Environment.NewLine + "Area: " + String.Format(format, Area);
            text += Environment.NewLine + "Static moment X: " + String.Format(format, StaticMomX);
            text += Environment.NewLine + "Static moment Y: " + String.Format(format, StaticMomY);
            text += Environment.NewLine + "Inertia moment X: " + String.Format(format, InertiaMomX);
            text += Environment.NewLine + "Inertia moment Y: " + String.Format(format, InertiaMomY);
            text += Environment.NewLine + "Deviation moment XY: " + String.Format(format, DeviationMomXY);
            text += Environment.NewLine + "Centroid: " + Centroid.ToString();
            text += Environment.NewLine + "Central inertia moment X: " + String.Format(format, CentrInertiaMom_X);
            text += Environment.NewLine + "Central inertia moment Y: " + String.Format(format, CentrInertiaMom_Y);
            text += Environment.NewLine + "Central deviation moment XY: " + String.Format(format, CentrDeviationMom_XY);
            text += Environment.NewLine + "Central principle inertia moment X: " + String.Format(format, CentrPrincipleInertiaMom_1);
            text += Environment.NewLine + "Central principle inertia moment Y: " + String.Format(format, CentrPrincipleInertiaMom_2);
            text += Environment.NewLine + "Angle of principle layout: " + String.Format(format, AngleOfPrincipleLayout / Math.PI * 180) + " [deg]";

            return text;
        }
    }
}
