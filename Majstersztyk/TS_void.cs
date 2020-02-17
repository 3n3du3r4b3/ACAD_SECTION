/*
 * Created by SharpDevelop.
 * User: TS040198
 * Date: 12/11/2018
 * Time: 16:31
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace Majstersztyk
{
	/// <summary>
	/// Description of TS_void.
	/// </summary>
	public class TS_void:TS_contour
	{
        public TS_void(List<TS_point> vertices) : base(vertices) { }
        
        public override string TypeOf { get { return typeOf; } }
        private new string typeOf = "Void";

        public override double Area { get { return -base.Area; } }
        public override double StaticMomX { get { return -base.StaticMomX; } }
        public override double StaticMomY { get { return -base.StaticMomY; } }
        public override double InertiaMomX { get { return -base.InertiaMomX; } }
        public override double InertiaMomY { get { return -base.InertiaMomY; } }
        public override double DeviationMomXY { get { return -base.DeviationMomXY; } } /*
        public override double CentrInertiaMom_X { get { return -base.CentrInertiaMom_X; } }
        public override double CentrInertiaMom_Y { get { return -base.CentrInertiaMom_Y; } }
        public override double CentrDeviationMom_XY { get { return -base.CentrDeviationMom_XY; } }
        public override double CentrPrincipleInertiaMom_1 { get { return -base.CentrPrincipleInertiaMom_1; } }
        public override double CentrPrincipleInertiaMom_2 { get { return -base.CentrPrincipleInertiaMom_2; } }*/
    }
}
