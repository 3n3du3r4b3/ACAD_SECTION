using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Majstersztyk
{
    public class TS_reinforcement:TS_region
    {
        public List<TS_bar> Bars { get; private set; }
        public TS_materials.TS_material Material { get; private set; }

        public override string TypeOf { get { return typeOf; } }
        private new string typeOf = "Reinforcement";

        public TS_reinforcement() {
            Bars = new List<TS_bar>();
            Material = null;
        }

        public TS_reinforcement(List<TS_bar> bars, TS_materials.TS_material material) {
            Bars = bars;
            Material = material;
            CalcProperties();
        }
        
        public TS_reinforcement(List<TS_bar> bars, TS_materials.TS_material material, string Name):this(bars,material){
        	this.Name = Name;
        }

        protected override double CalcArea(){
			double area = 0;
        	foreach (var bar in Bars) {
				area += bar.Area;
        	}
			return area;
        }
        
        protected override double CalcSx(){
			double sx = 0;
			foreach (var bar in Bars) {
				sx += bar.Area * bar.coordinates.Y;
			}
			return sx;
        }
        
        protected override double CalcSy(){
        	double sy = 0;
			foreach (var bar in Bars) {
				sy += bar.Area * bar.coordinates.X;
			}
			return sy;
        }
        
        protected override double CalcIx(){
			double ix = 0;
			foreach (var bar in Bars) {
				double ib = Math.PI * Math.Pow(bar.Diameter, 4) / 64;
				ix += ib + bar.Area * Math.Pow(bar.coordinates.Y, 2);
			}
			return ix;
        }
        
        protected override double CalcIy(){
        	double iy = 0;
			foreach (var bar in Bars) {
				double ib = Math.PI * Math.Pow(bar.Diameter, 4) / 64;
				iy += ib + bar.Area * Math.Pow(bar.coordinates.X, 2);
			}
			return iy;
        }

        protected override double CalcIxy(){
			double ixy = 0;
			foreach (var bar in Bars) {
				ixy += bar.Area * bar.coordinates.X * bar.coordinates.Y;
			}
			return ixy;
        }

        protected override void CalcCentralProp() {
            double centrx = InertiaMomX - Math.Pow(Centroid.Y, 2) * Area;
            double centry = InertiaMomY - Math.Pow(Centroid.X, 2) * Area;
            double centrxy = DeviationMomXY - (Centroid.X * Centroid.Y) * Area;
            
            CentrInertiaMom_X = centrx;
            CentrInertiaMom_Y = centry;
            CentrDeviationMom_XY = centrxy;
        }

        protected override bool IsObjectCorrect(){
        	for (int i = 0; i < Bars.Count; i++) {
        		for (int j = 0; j < Bars.Count; j++) {
        			if (i != j) {
        				double dist = Math.Sqrt(Math.Pow(Bars[i].coordinates.X-Bars[j].coordinates.X,2)+Math.Pow(Bars[i].coordinates.Y-Bars[j].coordinates.Y,2));
        				double minDist = 0.5 * (Bars[i].Diameter + Bars[j].Diameter);
        				if (dist < minDist) {
							return false;
        				}
        			}
        		}
        	}
			return true;
        }
        
        public override string ToString()
		{
			string text = "";
			text += Environment.NewLine + "Material: " + Material.Name 
				+ " Elastic modulus: " + String.Format("{0:E2}", Material.E);
			text += base.ToString();
			
			return text;
		}
        
    }
}
