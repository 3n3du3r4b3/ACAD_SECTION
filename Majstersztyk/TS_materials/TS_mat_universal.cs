/*
 * Created by SharpDevelop.
 * User: TS040198
 * Date: 17/12/2018
 * Time: 11:38
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Majstersztyk.TS_materials
{
	/// <summary>
	/// Description of TS_mat_universal.
	/// </summary>
	public class TS_mat_universal:TS_material
	{
		public TS_mat_universal(double E, string Name)
		{
			this.E = E;
			this.Name = Name;
		}
	}
}
