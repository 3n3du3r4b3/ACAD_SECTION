using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Majstersztyk
{
    public class TS_bar
    {
        public TS_point coordinates { get; set; }
        public double Diameter { get; set; }
		public double Area { get { return CalcArea(); }}
		//public TS_materials.TS_steel_rf SteelClass { get; set; }

        public TS_bar(TS_point coordinates, double diameter)//, TS_materials.TS_steel_rf steel)
        {
            this.coordinates = coordinates;
            Diameter = diameter;
			//SteelClass = steel;
        }

        private double CalcArea()
        {
            return Math.Pow(Diameter / 2, 2) * Math.PI;
        }
    }
}
