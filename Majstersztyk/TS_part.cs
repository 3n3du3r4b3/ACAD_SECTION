/*
 * Created by SharpDevelop.
 * User: TS040198
 * Date: 07/12/2018
 * Time: 11:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace Majstersztyk
{
    /// <summary>
    /// Description of TS_ordinate.
    /// </summary>s
    /// 

    public class TS_part:TS_contour
    {
		public List<TS_void> Voids { get; private set; }
		public TS_contour Contour { get; private set; }
        public TS_materials.TS_material Material { get; private set; }
		public List<TS_reinforcement> BelongingReinforcement { get; private set; }

        public override string TypeOf { get { return typeOf; } }
        private new string typeOf = "Part";

        public TS_part(TS_materials.TS_material material, TS_contour contour, List<TS_void> voids):base()
        {
            BelongingReinforcement = new List<TS_reinforcement>();
			Material = material;
            Voids = voids;
            Contour = contour;
            CalcProperties();
        }
        
        protected override double CalcArea()
        {
            double area = Contour.Area;
            foreach (var myVoid in Voids) {
				area += myVoid.Area;
            }
			return area;
        }

        protected override double CalcSx(){
			double sx = Contour.StaticMomX;
			foreach (var myVoid in Voids) {
				sx += myVoid.StaticMomX;
            }
			return sx;
        }

        protected override double CalcSy(){
        	double sy = Contour.StaticMomY;
			foreach (var myVoid in Voids) {
				sy += myVoid.StaticMomY;
            }
			return sy;
        }

        protected override double CalcIx(){
			double ix = Contour.InertiaMomX;
			foreach (var myVoid in Voids) {
				ix += myVoid.InertiaMomX;
            }
			return ix;        	
        }

        protected override double CalcIy(){
			double iy = Contour.InertiaMomY;
			foreach (var myVoid in Voids) {
				iy += myVoid.InertiaMomY;
            }
			return iy;   
        }

        protected override  double CalcIxy(){
 			double ixy = Contour.DeviationMomXY;
			foreach (var myVoid in Voids) {
				ixy += myVoid.DeviationMomXY;
            }
			return ixy; 
        }
        
        protected override bool IsObjectCorrect()
        {
            if (!Contour.IsCorrect)
                return false;

            foreach (var Void in Voids)
            {
                if (!Void.IsCorrect)
                    return false;
            }

            foreach (var Void in Voids) {
                foreach (var vert in Void.Vertices) {
                    if (!Contour.IsPointInside(vert))
                        return false;
                }
            }

            for (int i = 0; i < Voids.Count; i++) {
                for (int j = 0; j < Voids.Count; j++) {
                    if (i != j) {
                        foreach (var vert in Voids[i].Vertices) {
                            if (Voids[j].IsPointInside(vert)) 
                            	return false;
                        }
                    }
                }
            }

            return true;
        }
        
		public override string ToString()
		{
			string text = Environment.NewLine + "";
			text += Environment.NewLine + "GENERAL PARAMETERS FOR PART WITHOUT CONTAINED REINFORCEMENT:";
			text += Environment.NewLine + Environment.NewLine + "Material: " + Material.Name 
				+ " Elastic modulus: " + String.Format("{0:e2}", Material.E) + Environment.NewLine;
			text += base.ToString();
			
			text += Environment.NewLine + Environment.NewLine + "DETAIL PARAMETERS FOR PART'S MEMBERS:";
			
			text +=  Environment.NewLine + Contour.ToString() + Environment.NewLine;
			
			foreach (var Reo in BelongingReinforcement) {
				text += Environment.NewLine + "Contained reinforcement: ";
				text += Reo;
			}
			
			foreach (var tenvoid in Voids) {
				text += Environment.NewLine + tenvoid.ToString();
			}
			return text;
		}
		
		public TS_reinforcement TakeBelongingReinforcement(TS_reinforcement barGroup){
			//BelongingReinforcement = new List<TS_reinforcement>();
			List<TS_bar> matchinBars = new List<TS_bar>();
			List<TS_bar> notMachingBars = new List<TS_bar>();
				
			foreach (var bar in barGroup.Bars) {	
				if (Contour.IsPointInside(bar.coordinates)) {  //SPR CZY PRĘT JEST W OBRYSIE
						bool IsInVoid = false;
						
						foreach (var _void in Voids) {
							IsInVoid =_void.IsPointInside(bar.coordinates);
							if (IsInVoid) break;
						}
						
						if (IsInVoid) {
							notMachingBars.Add(bar);
						}else{
							matchinBars.Add(bar);
						}
				} else {
							notMachingBars.Add(bar);
				}
				}
			BelongingReinforcement.Add(new TS_reinforcement(matchinBars, barGroup.Material, barGroup.Name));
			return new TS_reinforcement(notMachingBars, barGroup.Material, barGroup.Name);
			}

    }
}
