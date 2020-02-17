/*
 * Created by SharpDevelop.
 * User: TS040198
 * Date: 12/11/2018
 * Time: 14:59
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Majstersztyk.TS_materials
{
	/// <summary>
	/// Description of TS_material.
	/// </summary>
	public abstract class TS_material
	{
		public double E;
		public string Name;
		
		public override string ToString()
		{
			return Name + " E = " + String.Format("{0:e3}", E);
		}
	}
	
	
}
