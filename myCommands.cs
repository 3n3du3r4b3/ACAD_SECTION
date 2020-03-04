// (C) Copyright 2019 by  
//
using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Majstersztyk;
using System.Collections.Generic;

// This line is not mandatory, but improves loading performances
[assembly: CommandClass(typeof(acad_geometry_data.MyCommands))]

namespace acad_geometry_data
{
    
    // This class is instantiated by AutoCAD for each document when
    // a command is called by the user the first time in the context
    // of a given document. In other words, non static data in this class
    // is implicitly per-document!
    public class MyCommands
    {
        TS_section section;
        /*
        // The CommandMethod attribute can be applied to any public  member 
        // function of any public class.
        // The function should take no arguments and return nothing.
        // If the method is an intance member then the enclosing class is 
        // intantiated for each document. If the member is a static member then
        // the enclosing class is NOT intantiated.
        //
        // NOTE: CommandMethod has overloads where you can provide helpid and
        // context menu.

        // Modal Command with localized name
        [CommandMethod("MyGroup", "MyCommand", "MyCommandLocal", CommandFlags.Modal)]
        public void MyCommand() // This method can have any name
        {
            // Put your command code here
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed;
            if (doc != null)
            {
                ed = doc.Editor;
                ed.WriteMessage("Hello, this is your first command.");

            }
        }

        // Modal Command with pickfirst selection
        [CommandMethod("MyGroup", "MyPickFirst", "MyPickFirstLocal", CommandFlags.Modal | CommandFlags.UsePickSet)]
        public void MyPickFirst() // This method can have any name
        {
            PromptSelectionResult result = Application.DocumentManager.MdiActiveDocument.Editor.GetSelection();
            if (result.Status == PromptStatus.OK)
            {
                // There are selected entities
                // Put your command using pickfirst set code here
            }
            else
            {
                // There are no selected entities
                // Put your command code here
            }
        }

        // Application Session Command with localized name
        [CommandMethod("MyGroup", "MySessionCmd", "MySessionCmdLocal", CommandFlags.Modal | CommandFlags.Session)]
        public void MySessionCmd() // This method can have any name
        {
            // Put your command code here
        }

        // LispFunction is similar to CommandMethod but it creates a lisp 
        // callable function. Many return types are supported not just string
        // or integer.
        [LispFunction("MyLispFunction", "MyLispFunctionLocal")]
        public int MyLispFunction(ResultBuffer args) // This method can have any name
        {
            // Put your command code here

            // Return a value to the AutoCAD Lisp Interpreter
            return 1;
        }
        */

        [CommandMethod("TS_SECTION-PROP")]
        public void SectionProperties()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Transaction tr = ed.Document.TransactionManager.StartTransaction();
            Database database = ed.Document.Database;

            section = new TS_section();
            List<TS_part> parts = new List<TS_part>();
            List<TS_reinforcement> reo = new List<TS_reinforcement>();

            int partsNo = HowMany(ed, "How many parts do you want to have? ");

            if (partsNo == -999)
            {
                tr.Dispose();
                return;
            }

            for (int i = 1; i <= partsNo; i++)
            {
                TS_contour contour;
                List<TS_void> voids = new List<TS_void>();
                TS_part part;

                string name = GetString(ed, "\nEnter name of the part " + i + " (optional - Esc to skip): ");
                if (name == null)
                {
                    name = "PART no. " + i;
                }

                string name_mat = GetString(ed, "\nEnter name of the material of " + name + " (optional - Esc to skip): ");
                if (name_mat == null)
                {
                    name_mat = "MATERIAL of PART no. " + i;
                }

                double moduleE = GetValue(ed, "\nEnter value of elastic modulus of " + name_mat + " (obligatory): ");
                if (moduleE == -999)
                {
                    tr.Dispose();
                    return;
                }

                Majstersztyk.TS_materials.TS_mat_universal material = new Majstersztyk.TS_materials.TS_mat_universal(moduleE, name_mat);

                Polyline pline = GetPolyline(ed, tr, "\nSelect contour of part " + i + " " + name + ": ");
                if (pline == null)
                {
                    tr.Dispose();
                    return;
                }
                else
                {
                    contour = new TS_contour(GetVertices(pline));
                }

                List<Polyline> voidy = GetPolylines(ed, tr, "\nSelect voids of part " + i + " " + name + " (optional - Esc to skip) : ");
                if (voidy != null)
                {
                    foreach (Polyline dziura in voidy)
                    {
                        voids.Add(new TS_void(GetVertices(dziura)));
                    }
                }

                part = new TS_part(material, contour, voids);
                part.Name = name;
                parts.Add(part);
            }

            int reoNo = HowMany(ed, "How many reinforcement groups do you want to have? ");

            if (reoNo == -999)
            {
                tr.Dispose();
                return;
            }

            for (int i = 1; i <= reoNo; i++)
            {
                List<TS_bar> bars = new List<TS_bar>();
                TS_reinforcement reoGroup;

                string name = GetString(ed, "\nEnter name of the reinforcement group no. " + i + " (optional - Esc to skip): ");
                if (name == null)
                {
                    name = "REO GROUP no. " + i;
                }

                string name_mat = GetString(ed, "\nEnter name of the material of " + name + " (optional - Esc to skip): ");
                if (name_mat == null)
                {
                    name_mat = "MATERIAL of REO GROUP no. " + i;
                }

                double moduleE = GetValue(ed, "\nEnter value of elastic modulus of " + name_mat + " (obligatory): ");
                if (moduleE == -999)
                {
                    tr.Dispose();
                    return;
                }

                Majstersztyk.TS_materials.TS_mat_universal material = new Majstersztyk.TS_materials.TS_mat_universal(moduleE, name_mat);

                List<Circle> bary = GetCircles(ed, tr, "\nSelect bars (circles only) for reinforcement group no. " + i + " " + name + ": ");
                if (bary != null)
                {
                    foreach (Circle bar in bary)
                    {
                        bars.Add(new TS_bar(new TS_point(bar.Center.X, bar.Center.Y), bar.Diameter));
                    }
                }

                reoGroup = new TS_reinforcement(bars, material, name);

                if (reoGroup.IsCorrect != true)
                {
                    ed.WriteMessage("\nReinforcement seems to be not correct... Continuing anyway...");
                }

                reo.Add(reoGroup);

            }

            section.Update(parts, reo);

            ed.WriteMessage(section.ToString());

            try
            {
                Circle circle = new Circle(new Point3d(section.Centroid.X, section.Centroid.Y, 0), Vector3d.ZAxis, Math.Sqrt(section.Area) / 8);
                Xline line1 = new Xline();
                line1.BasePoint = circle.Center;
                line1.UnitDir =
                    new Vector3d(Math.Cos(section.AngleOfPrincipleLayout),
                    Math.Sin(section.AngleOfPrincipleLayout), 0);
                Xline line2 = new Xline();
                line2.BasePoint = circle.Center;
                line2.UnitDir =
                    new Vector3d(-Math.Sin(section.AngleOfPrincipleLayout),
                    Math.Cos(section.AngleOfPrincipleLayout), 0);

                BlockTableRecord blockTableRecord = (BlockTableRecord)tr.GetObject(database.CurrentSpaceId, OpenMode.ForWrite);

                blockTableRecord.AppendEntity(line1);
                blockTableRecord.AppendEntity(line2);
                blockTableRecord.AppendEntity(circle);
                tr.AddNewlyCreatedDBObject(line1, true);
                tr.AddNewlyCreatedDBObject(line2, true);
                tr.AddNewlyCreatedDBObject(circle, true);

            }
            catch (System.Exception ex)
            {
                ed.WriteMessage(ex.Message);
            }
            /*
            SectionSaved sectionSaved = new SectionSaved();
            sectionSaved.Section = section;
            sectionSaved.AddVertexAt(0, new Point2d(0, 0), 0, 0, 0);
            sectionSaved.AddVertexAt(1, new Point2d(0, 1000), 0, 0, 0);

            SectionSaved2 sectionSaved2 = new SectionSaved2();
            sectionSaved2.Section = section;
            BlockTableRecord blockTableRecord2;

            try
            {
                blockTableRecord2 = (BlockTableRecord)tr.GetObject(database.CurrentSpaceId, OpenMode.ForWrite);


                blockTableRecord2.AppendEntity(sectionSaved);
                tr.AddNewlyCreatedDBObject(sectionSaved, true);
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage(ex.Message);
            }*/

            ed.WriteMessage("\nConstant torsion: " + section.ConstantTorsion);

            tr.Commit();
            tr.Dispose();
        }


        [CommandMethod("TS_SECTION-STRESSES")]
        public void SectionStresses()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Transaction tr = ed.Document.TransactionManager.StartTransaction();
            /*
            ObjectId objId = GetSection(ed, "");

            DBObject sec = tr.GetObject(objId, OpenMode.ForRead);

            if (sec != null)
            {
                section = (sec as SectionSaved).Section;
            }
            */
            if (section == null)
            {
                ed.WriteMessage("\nUse function \"TS-SECTION-PROP\" before using this function");
                return;
            }

            double Fx, My, Mz;
            double? Fxx, Myy, Mzz;

            Fxx = GetForce(ed, "\nAxial Force [kN]: ");
            if (Fxx == null)
                return;
            else
                Fx = (double)Fxx;

            Myy = GetForce(ed, "\nBending moment Y-dir [kN * UNIT]: ");
            if (Myy == null)
                return;
            else
                My = (double)Myy;

            Mzz = GetForce(ed, "\nBending moment Z-dir [kN * UNIT]: ");
            if (Mzz == null)
                return;
            else
                Mz = (double)Mzz;

            List<List<StressPoint>> brick = section.GetStresses(Fx, My, Mz);

            Database database = ed.Document.Database;
            BlockTableRecord blockTableRecord = (BlockTableRecord)tr.GetObject(database.CurrentSpaceId, OpenMode.ForWrite);

            for (int i = 0; i < brick.Count; i++)
            {
                Polyline3d pline = new Polyline3d();

                if (brick[i].Count > 1)
                {
                    blockTableRecord.AppendEntity(pline);
                    tr.AddNewlyCreatedDBObject(pline, true);
                }

                for (int j = 0; j < brick[i].Count; j++)
                {
                    Line line = new Line(new Point3d(brick[i][j].PointGlobalCS.X, brick[i][j].PointGlobalCS.Y, 0), new Point3d(brick[i][j].PointGlobalCS.X, brick[i][j].PointGlobalCS.Y, brick[i][j].Stress));
                    blockTableRecord.AppendEntity(line);
                    tr.AddNewlyCreatedDBObject(line, true);
                    if (brick[i].Count > 1)
                    {
                        pline.AppendVertex(new PolylineVertex3d(new Point3d(brick[i][j].PointGlobalCS.X, brick[i][j].PointGlobalCS.Y, brick[i][j].Stress)));
                        if (j == brick[i].Count - 1)
                        {
                            pline.AppendVertex(new PolylineVertex3d(new Point3d(brick[i][0].PointGlobalCS.X, brick[i][0].PointGlobalCS.Y, brick[i][0].Stress)));
                        }
                    }
                }
            }

            tr.Commit();
            tr.Dispose();

        }

        private int HowMany(Editor ed, string message)
        {
            int parts = -1;
            do
            {
                PromptStringOptions ask = new PromptStringOptions(message);
                PromptResult answer = ed.GetString(ask);

                if (answer.Status == PromptStatus.OK)
                {
                    Int32.TryParse(answer.StringResult, out parts);
                }
                else if (answer.Status == PromptStatus.Error)
                {
                    return -1;
                }
                else if (answer.Status == PromptStatus.Cancel)
                {
                    return -999;
                }

            } while (parts == -1); ;

            return parts;
        }

        private string GetString(Editor ed, string massage)
        {
            PromptStringOptions ask = new PromptStringOptions(massage);
            PromptResult answer = ed.GetString(ask);
            if (answer.Status == PromptStatus.OK)
            {
                return answer.StringResult;
            }
            else return null;
        }

        private double GetValue(Editor ed, string message)
        {
            double value = 0;
            do
            {
                PromptStringOptions ask = new PromptStringOptions(message);
                PromptResult answer = ed.GetString(ask);

                if (answer.Status == PromptStatus.OK)
                {
                    Double.TryParse(answer.StringResult, out value);
                }
                else if (answer.Status == PromptStatus.Error)
                {
                    return 0;
                }
                else if (answer.Status == PromptStatus.Cancel)
                {
                    return -999;
                }

            } while (value == 0); ;

            return value;
        }

        private Polyline GetPolyline(Editor ed, Transaction tr, string message)
        {
            TypedValue[] type = new TypedValue[1];
            type.SetValue(new TypedValue((int)DxfCode.Start, "LWPOLYLINE"), 0);
            SelectionFilter filter = new SelectionFilter(type);
            PromptSelectionOptions options = new PromptSelectionOptions();
            options.SingleOnly = true;
            options.MessageForAdding = message;
            PromptSelectionResult result;

            Polyline pline = null;
            do
            {
                result = ed.GetSelection(options, filter);

                if (result.Status == PromptStatus.Cancel) return null;

            } while (result.Status != PromptStatus.OK); // && result.Value.Count<1);

            try
            {
                Entity ent = (Entity)tr.GetObject(result.Value.GetObjectIds()[0], OpenMode.ForRead);
                pline = ent as Polyline;

                if (!pline.Closed)
                {
                    ed.WriteMessage("\nNOTE: Not closed polyline is treated as closed. ");

                }

                TS_contour contour = new TS_contour(GetVertices(pline));

                if (!contour.IsCorrect)
                {
                    ed.WriteMessage("\nWARNING: Contour seems to be not correct... Continuing anyway...");
                }
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage(ex.Message);
            }

            return pline;
        }

        private List<Polyline> GetPolylines(Editor ed, Transaction tr, string message)
        {
            TypedValue[] type = new TypedValue[1];
            type.SetValue(new TypedValue((int)DxfCode.Start, "LWPOLYLINE"), 0);
            SelectionFilter filter = new SelectionFilter(type);
            PromptSelectionOptions options = new PromptSelectionOptions();
            options.MessageForAdding = message;
            PromptSelectionResult result;
            List<Polyline> entities = new List<Polyline>();
            do
            {
                result = ed.GetSelection(options, filter);

                if (result.Status == PromptStatus.Cancel) return null;

            } while (result.Status != PromptStatus.OK); // && result.Value.Count<1);

            try
            {
                entities = new List<Polyline>();

                for (int i = 0; i < result.Value.Count; i++)
                {
                    Polyline pline = (Entity)tr.GetObject(result.Value.GetObjectIds()[i], OpenMode.ForRead) as Polyline;
                    entities.Add(pline);

                    if (!pline.Closed)
                    {
                        ed.WriteMessage("\nNOTE: Not closed polyline is treated as closed. ");
                    }

                    TS_contour contour = new TS_contour(GetVertices(pline));

                    if (!contour.IsCorrect)
                    {
                        ed.WriteMessage("\nWARNING: Contour seems to be not correct... Continuing anyway...");
                    }
                }
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage(ex.Message);
            }

            return entities;
        }

        private List<Circle> GetCircles(Editor ed, Transaction tr, string message)
        {
            TypedValue[] type = new TypedValue[1];
            type.SetValue(new TypedValue((int)DxfCode.Start, "CIRCLE"), 0);
            SelectionFilter filter = new SelectionFilter(type);
            PromptSelectionOptions options = new PromptSelectionOptions();
            options.MessageForAdding = message;
            PromptSelectionResult result;
            List<Circle> entities = new List<Circle>();
            do
            {
                result = ed.GetSelection(options, filter);

                if (result.Status == PromptStatus.Cancel) return null;

            } while (result.Status != PromptStatus.OK); // && result.Value.Count<1);

            try
            {
                entities = new List<Circle>();

                for (int i = 0; i < result.Value.Count; i++)
                {
                    Circle bar = (Entity)tr.GetObject(result.Value.GetObjectIds()[i], OpenMode.ForRead) as Circle;
                    entities.Add(bar);
                }
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage(ex.Message);
            }

            return entities;
        }

        private List<TS_point> GetVertices(Polyline pline)
        {
            List<TS_point> vertices1 = new List<TS_point>();

            for (int j = 0; j < pline.NumberOfVertices; j++)
            {
                vertices1.Add(new TS_point(pline.GetPoint2dAt(j).X, pline.GetPoint2dAt(j).Y));
            }
            return vertices1;
        }

        private double? GetForce(Editor ed, string message)
        {

            PromptDoubleOptions ask = new PromptDoubleOptions(message);
            ask.AllowNegative = true;
            ask.AllowNone = false;

            PromptDoubleResult answer = ed.GetDouble(ask);

            if (answer.Status == PromptStatus.OK)
            {
                return answer.Value;
            }
            else if (answer.Status == PromptStatus.Error)
            {
                return null;
            }
            else if (answer.Status == PromptStatus.Cancel)
            {
                return null;
            }
            return null;
        }

        private ObjectId GetSection(Editor ed, string message)
        {
            PromptEntityOptions ask = new PromptEntityOptions("\nSelect any object belonging to the section: ");
            ask.AllowNone = false;
            ask.RemoveAllowedClass(typeof(Line));

            PromptEntityResult result = ed.GetEntity(ask);

            if (result.Status == PromptStatus.OK)
            {
                return result.ObjectId;
            }
            return new ObjectId();
        }
    }

    internal class SectionSaved : Polyline
    {
        public SectionSaved():base()
        { }
        public TS_section Section { get; set; }
    }

    internal class SectionSaved2: BlockTableRecord
    {
        public SectionSaved2() : base() { }

        public TS_section Section { get; set; }

    }
}

