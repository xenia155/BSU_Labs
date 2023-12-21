using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Drawing;
using CGALDotNet;
using CGALDotNet.Arrangements;
using CGALDotNet.Polygons;
using CGALDotNet.Hulls;
using CGALDotNet.Triangulations;
using CGALDotNetGeometry.Numerics;
//using Autodesk.AutoCAD.Runtime;
//using Autodesk.AutoCAD.ApplicationServices;
//using Autodesk.AutoCAD.DatabaseServices;
//using Autodesk.AutoCAD.Geometry;
//using Autodesk.AutoCAD.EditorInput;
using Teigha.DatabaseServices;
using Teigha.Runtime;
using Teigha.Geometry;
using Teigha.Colors;
using Teigha.BoundaryRepresentation;
using HostMgd.ApplicationServices;
using HostMgd.EditorInput;
using HostMgd.Windows;
using HostMgd.Windows.ToolPalette;

using Aspose.Gis;
using Aspose.Gis.Geometries;
using Aspose.Gis.Rendering;
using System.Runtime.InteropServices;
using CGALDotNetGeometry.Shapes;
using CGALDotNet.Geometry;

namespace nanoCGALDotNet
{
    public class L_System
    {
        double scale, angle = 90;
        string[] rules, patterns;
        string lsystem;
        double startx = 10.0, starty = 10.0;
        int maxdepth = 5;
        // Конструктор (правила и аксиомы)
        public L_System()
        {
            //Hilbert curve
            rules = new string[2]; patterns = new string[2];
            rules[0] = "L"; rules[1] = "R";
            patterns[0] = "+RF-LFL-FR+";
            patterns[1] = "-LF+RFR+FL-";
            angle = 90; lsystem = "L";
            scale = 300.0; startx = 10.0; starty = 10.0;
            //Koch snowflake
            //rules=new string[1]; patterns=new string[1];
            //rules[0]="F"; patterns[0]="F+F–F+F";
            //angle=60; lsystem="F–F–F";
            //scale=30.0;
            //startx= 200.0 - scale*4;
            //starty=200.0;
        }
        // Головная процедура построения и отрисовки
        public void LSystem()
        {
            string current = lsystem;
            string next = lsystem;
            bool found;
            int depth = 0;

            if (GetParams() == false)
                return;

            while (depth < maxdepth)
            {
                current = next; next = "";
                for (int i = 0; i < current.Length; i++)
                {
                    found = false;
                    for (int j = 0; j < rules.Length; j++)
                    {
                        if (current[i] == rules[j][0])
                        {
                            next = next + patterns[j]; found = true;
                        }
                    }
                    if (!found) next = next + current[i];
                }
                depth++;
            }
            DrawLSystem(next);
        }

        // Процедура отрисовки черепашьей графики  
        void DrawLSystem(string ls)
        {
            Document doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = doc.Database;
            Editor ed = doc.Editor;
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                using (DocumentLock DocLock = doc.LockDocument())
                {
                    using (BlockTable acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable)
                    {
                        using (BlockTableRecord acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord)
                        {
                            Point3dCollection pts = new Point3dCollection();
                            ////
                            double dx = scale / Math.Pow(2, maxdepth), dy = 0.0;
                            double rx, ry, x = startx, y = starty;
                            double save_x = 0, save_y = 0;
                            for (int i = 0; i < ls.Length; i++)
                            {
                                if (ls[i] == 'F')
                                {
                                    Line proj_line = new Line(new Teigha.Geometry.Point3d(x, y, 0), new Teigha.Geometry.Point3d(x + dx, y + dy, 0));
                                    proj_line.ColorIndex = 5;
                                    acBlkTblRec.AppendEntity(proj_line);
                                    acTrans.AddNewlyCreatedDBObject(proj_line, true);
                                    x += dx; y += dy;
                                }
                                if (ls[i] == '+')
                                {
                                    rx = dx; ry = dy;
                                    double radian = Math.PI * angle / 180.0;
                                    dx = rx * Math.Cos(radian) - ry * Math.Sin(radian);
                                    dy = rx * Math.Sin(radian) + ry * Math.Cos(radian);
                                }
                                if (ls[i] == '-')
                                {
                                    rx = dx; ry = dy;
                                    double radian = Math.PI * angle / 180.0;
                                    dx = rx * Math.Cos(-radian) - ry * Math.Sin(-radian);
                                    dy = rx * Math.Sin(-radian) + ry * Math.Cos(-radian);
                                }
                            }
                            /////
                        }
                    }
                }
                acTrans.Commit();
            }
        }
        // Процедура запроса параметров
        //тут было static
        public bool GetParams()
        {

            Document doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            Teigha.Geometry.Point3d position = Teigha.Geometry.Point3d.Origin;
            PromptPointOptions ppo =
              new PromptPointOptions(
                "\nSelect base point of tree: "
              );


            PromptPointResult ppr =
              ed.GetPoint(ppo);


            if (ppr.Status != PromptStatus.OK)
                return false;
            position = ppr.Value;
            startx = position.X;
            starty = position.Y;
            PromptIntegerOptions pdo =
              new PromptIntegerOptions(
                "\nEnter maxdepth <5>: "
              );
            pdo.AllowNone = true;
            PromptIntegerResult pdr =
              ed.GetInteger(pdo);
            if (pdr.Status != PromptStatus.None &&
                pdr.Status != PromptStatus.OK)
                return false;
            if (pdr.Status == PromptStatus.OK)
                maxdepth = pdr.Value;
            else
                maxdepth = 5;
            if (maxdepth > 10)
                maxdepth = 10;
            return true;
        }

    }

    public class Fractal_plant
    {
        double scale, angle = 25;
        string[] rules, patterns;
        string lsystem;
        double startx = 10.0, starty = 10.0;
        int maxdepth = 5;
        // Конструктор (правила и аксиомы)
        public Fractal_plant()
        {
            //Fractal plant
            rules = new string[2]; patterns = new string[2];
            rules[0] = "X"; rules[1] = "F";
            patterns[0] = "F-[[X]+X]+F[+FX]-X";
            patterns[1] = "FF";
            angle = 25; lsystem = "X";
            scale = 300.0; startx = 10.0; starty = 10.0;
        }
        // Головная процедура построения и отрисовки
        public void System()
        {
            string current = lsystem;
            string next = lsystem;
            bool found;
            int depth = 0;

            if (GetParams() == false)
                return;

            while (depth < maxdepth)
            {
                current = next; next = "";
                for (int i = 0; i < current.Length; i++)
                {
                    found = false;
                    for (int j = 0; j < rules.Length; j++)
                    {
                        if (current[i] == rules[j][0])
                        {
                            next = next + patterns[j]; found = true;
                        }
                    }
                    if (!found) next = next + current[i];
                }
                depth++;
            }
            DrawLSystem(next);
        }

        // Процедура отрисовки черепашьей графики  
        void DrawLSystem(string ls)
        {
            Document doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = doc.Database;
            Editor ed = doc.Editor;
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                using (DocumentLock DocLock = doc.LockDocument())
                {
                    using (BlockTable acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable)
                    {
                        using (BlockTableRecord acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord)
                        {
                            Point3dCollection pts = new Point3dCollection();
                            ////
                            double dx = scale / Math.Pow(2, maxdepth), dy = 0.0;
                            double rx, ry, x = startx, y = starty;
                            Stack<double> save_dx = new Stack<double>();
                            Stack<double> save_dy = new Stack<double>();
                            Stack<double> save_x = new Stack<double>();
                            Stack<double> save_y = new Stack<double>();
                            for (int i = 0; i < ls.Length; i++)
                            {
                                if (ls[i] == 'F')
                                {
                                    Line proj_line = new Line(new Teigha.Geometry.Point3d(x, y, 0), new Teigha.Geometry.Point3d(x + dx, y + dy, 0));
                                    proj_line.ColorIndex = 4;
                                    acBlkTblRec.AppendEntity(proj_line);
                                    acTrans.AddNewlyCreatedDBObject(proj_line, true);
                                    x += dx; y += dy;
                                }
                                if (ls[i] == '+')
                                {
                                    rx = dx; ry = dy;
                                    double radian = Math.PI * angle / 180.0;
                                    dx = rx * Math.Cos(radian) - ry * Math.Sin(radian);
                                    dy = rx * Math.Sin(radian) + ry * Math.Cos(radian);
                                }
                                if (ls[i] == '-')
                                {
                                    rx = dx; ry = dy;
                                    double radian = Math.PI * angle / 180.0;
                                    dx = rx * Math.Cos(-radian) - ry * Math.Sin(-radian);
                                    dy = rx * Math.Sin(-radian) + ry * Math.Cos(-radian);
                                }
                                if (ls[i] == '[')
                                {
                                    save_x.Push(x); save_y.Push(y);
                                    save_dx.Push(dx); save_dy.Push(dy);
                                }
                                if (ls[i] == ']')
                                {
                                    x = save_x.Pop(); y = save_y.Pop();
                                    dx = save_dx.Pop(); dy = save_dy.Pop();
                                }
                            }
                            /////
                        }
                    }
                }
                acTrans.Commit();
            }
        }
        // Процедура запроса параметров
        //тут было static
        public bool GetParams()
        {

            Document doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            Teigha.Geometry.Point3d position = Teigha.Geometry.Point3d.Origin;
            PromptPointOptions ppo =
              new PromptPointOptions(
                "\nSelect base point of tree: "
              );


            PromptPointResult ppr =
              ed.GetPoint(ppo);


            if (ppr.Status != PromptStatus.OK)
                return false;
            position = ppr.Value;
            startx = position.X;
            starty = position.Y;
            PromptIntegerOptions pdo =
              new PromptIntegerOptions(
                "\nEnter maxdepth <5>: "
              );
            pdo.AllowNone = true;
            PromptIntegerResult pdr =
              ed.GetInteger(pdo);
            if (pdr.Status != PromptStatus.None &&
                pdr.Status != PromptStatus.OK)
                return false;
            if (pdr.Status == PromptStatus.OK)
                maxdepth = pdr.Value;
            else
                maxdepth = 5;
            if (maxdepth > 10)
                maxdepth = 10;
            return true;
        }

    }

    public class Dragon_curve
    {
        double scale, angle = 90;
        string[] rules, patterns;
        string system;
        double startx = 10.0, starty = 10.0;
        int maxdepth = 5;
        // Конструктор (правила и аксиомы)
        public Dragon_curve()
        {
            //Fractal plant
            rules = new string[2]; patterns = new string[2];
            rules[0] = "X"; rules[1] = "Y";
            patterns[0] = "X+YF+";
            patterns[1] = "-FX-Y";
            angle = 90; system = "FX";
            scale = 300.0; startx = 10.0; starty = 10.0;
        }
        // Головная процедура построения и отрисовки
        public void System()
        {
            string current = system;
            string next = system;
            bool found;
            int depth = 0;

            if (GetParams() == false)
                return;

            while (depth < maxdepth)
            {
                current = next; next = "";
                for (int i = 0; i < current.Length; i++)
                {
                    found = false;
                    for (int j = 0; j < rules.Length; j++)
                    {
                        if (current[i] == rules[j][0])
                        {
                            next = next + patterns[j]; found = true;
                        }
                    }
                    if (!found) next = next + current[i];
                }
                depth++;
            }
            DrawLSystem(next);
        }

        // Процедура отрисовки черепашьей графики  
        void DrawLSystem(string ls)
        {
            Document doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = doc.Database;
            Editor ed = doc.Editor;
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                using (DocumentLock DocLock = doc.LockDocument())
                {
                    using (BlockTable acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable)
                    {
                        using (BlockTableRecord acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord)
                        {
                            Point3dCollection pts = new Point3dCollection();
                            ////
                            double dx = scale / Math.Pow(2, maxdepth), dy = 0.0;
                            double rx, ry, x = startx, y = starty;
                            for (int i = 0; i < ls.Length; i++)
                            {
                                if (ls[i] == 'F')
                                {
                                    Line proj_line = new Line(new Teigha.Geometry.Point3d(x, y, 0), new Teigha.Geometry.Point3d(x + dx, y + dy, 0));
                                    proj_line.ColorIndex = 4;
                                    acBlkTblRec.AppendEntity(proj_line);
                                    acTrans.AddNewlyCreatedDBObject(proj_line, true);
                                    x += dx; y += dy;
                                }
                                if (ls[i] == '+')
                                {
                                    rx = dx; ry = dy;
                                    double radian = Math.PI * angle / 180.0;
                                    dx = rx * Math.Cos(radian) - ry * Math.Sin(radian);
                                    dy = rx * Math.Sin(radian) + ry * Math.Cos(radian);
                                }
                                if (ls[i] == '-')
                                {
                                    rx = dx; ry = dy;
                                    double radian = Math.PI * angle / 180.0;
                                    dx = rx * Math.Cos(-radian) - ry * Math.Sin(-radian);
                                    dy = rx * Math.Sin(-radian) + ry * Math.Cos(-radian);
                                }
                            }
                            /////
                        }
                    }
                }
                acTrans.Commit();
            }
        }
        // Процедура запроса параметров
        //тут было static
        public bool GetParams()
        {

            Document doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            Teigha.Geometry.Point3d position = Teigha.Geometry.Point3d.Origin;
            PromptPointOptions ppo =
              new PromptPointOptions(
                "\nSelect base point of tree: "
              );


            PromptPointResult ppr =
              ed.GetPoint(ppo);


            if (ppr.Status != PromptStatus.OK)
                return false;
            position = ppr.Value;
            startx = position.X;
            starty = position.Y;
            PromptIntegerOptions pdo =
              new PromptIntegerOptions(
                "\nEnter maxdepth <5>: "
              );
            pdo.AllowNone = true;
            PromptIntegerResult pdr =
              ed.GetInteger(pdo);
            if (pdr.Status != PromptStatus.None &&
                pdr.Status != PromptStatus.OK)
                return false;
            if (pdr.Status == PromptStatus.OK)
                maxdepth = pdr.Value;
            else
                maxdepth = 5;
            if (maxdepth > 10)
                maxdepth = 10;
            return true;
        }

    }

    public class Serpinskij_triangle
    {
        double scale, angle = 60;
        string[] rules, patterns;
        string system;
        double startx = 10.0, starty = 10.0;
        int maxdepth = 5;
        // Конструктор (правила и аксиомы)
        public Serpinskij_triangle()
        {
            //Fractal plant
            rules = new string[2]; patterns = new string[2];
            rules[0] = "A"; rules[1] = "B";
            patterns[0] = "B-A-B";
            patterns[1] = "A+B+A";
            angle = 60; system = "A";
            scale = 300.0; startx = 10.0; starty = 10.0;
        }
        // Головная процедура построения и отрисовки
        public void System()
        {
            string current = system;
            string next = system;
            bool found;
            int depth = 0;

            if (GetParams() == false)
                return;

            while (depth < maxdepth)
            {
                current = next; next = "";
                for (int i = 0; i < current.Length; i++)
                {
                    found = false;
                    for (int j = 0; j < rules.Length; j++)
                    {
                        if (current[i] == rules[j][0])
                        {
                            next = next + patterns[j]; found = true;
                        }
                    }
                    if (!found) next = next + current[i];
                }
                depth++;
            }
            DrawLSystem(next);
        }

        // Процедура отрисовки черепашьей графики  
        void DrawLSystem(string ls)
        {
            Document doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = doc.Database;
            Editor ed = doc.Editor;
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                using (DocumentLock DocLock = doc.LockDocument())
                {
                    using (BlockTable acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable)
                    {
                        using (BlockTableRecord acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord)
                        {
                            Point3dCollection pts = new Point3dCollection();
                            ////
                            double dx = scale / Math.Pow(2, maxdepth), dy = 0.0;
                            double rx, ry, x = startx, y = starty;
                            for (int i = 0; i < ls.Length; i++)
                            {
                                if (ls[i] == 'A' || ls[i] == 'B')
                                {
                                    Line proj_line = new Line(new Teigha.Geometry.Point3d(x, y, 0), new Teigha.Geometry.Point3d(x + dx, y + dy, 0));
                                    proj_line.ColorIndex = 4;
                                    acBlkTblRec.AppendEntity(proj_line);
                                    acTrans.AddNewlyCreatedDBObject(proj_line, true);
                                    x += dx; y += dy;
                                }
                                if (ls[i] == '+')
                                {
                                    rx = dx; ry = dy;
                                    double radian = Math.PI * angle / 180.0;
                                    dx = rx * Math.Cos(radian) - ry * Math.Sin(radian);
                                    dy = rx * Math.Sin(radian) + ry * Math.Cos(radian);
                                }
                                if (ls[i] == '-')
                                {
                                    rx = dx; ry = dy;
                                    double radian = Math.PI * angle / 180.0;
                                    dx = rx * Math.Cos(-radian) - ry * Math.Sin(-radian);
                                    dy = rx * Math.Sin(-radian) + ry * Math.Cos(-radian);
                                }
                            }
                            /////
                        }
                    }
                }
                acTrans.Commit();
            }
        }
        // Процедура запроса параметров
        //тут было static
        public bool GetParams()
        {

            Document doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            Teigha.Geometry.Point3d position = Teigha.Geometry.Point3d.Origin;
            PromptPointOptions ppo =
              new PromptPointOptions(
                "\nSelect base point of tree: "
              );


            PromptPointResult ppr =
              ed.GetPoint(ppo);


            if (ppr.Status != PromptStatus.OK)
                return false;
            position = ppr.Value;
            startx = position.X;
            starty = position.Y;
            PromptIntegerOptions pdo =
              new PromptIntegerOptions(
                "\nEnter maxdepth <5>: "
              );
            pdo.AllowNone = true;
            PromptIntegerResult pdr =
              ed.GetInteger(pdo);
            if (pdr.Status != PromptStatus.None &&
                pdr.Status != PromptStatus.OK)
                return false;
            if (pdr.Status == PromptStatus.OK)
                maxdepth = pdr.Value;
            else
                maxdepth = 5;
            if (maxdepth > 10)
                maxdepth = 10;
            return true;
        }

    }

    public class Koh_curve
    {
        double scale, angle = 90;
        string[] rules, patterns;
        string system;
        double startx = 10.0, starty = 10.0;
        int maxdepth = 5;
        // Конструктор (правила и аксиомы)
        public Koh_curve()
        {
            //Fractal plant
            rules = new string[1]; patterns = new string[1];
            rules[0] = "F";
            patterns[0] = "F+F-F-F+F";
            angle = 90; system = "F";
            scale = 300.0; startx = 10.0; starty = 10.0;
        }
        // Головная процедура построения и отрисовки
        public void System()
        {
            string current = system;
            string next = system;
            bool found;
            int depth = 0;

            if (GetParams() == false)
                return;

            while (depth < maxdepth)
            {
                current = next; next = "";
                for (int i = 0; i < current.Length; i++)
                {
                    found = false;
                    for (int j = 0; j < rules.Length; j++)
                    {
                        if (current[i] == rules[j][0])
                        {
                            next = next + patterns[j]; found = true;
                        }
                    }
                    if (!found) next = next + current[i];
                }
                depth++;
            }
            DrawLSystem(next);
        }

        // Процедура отрисовки черепашьей графики  
        void DrawLSystem(string ls)
        {
            Document doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = doc.Database;
            Editor ed = doc.Editor;
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                using (DocumentLock DocLock = doc.LockDocument())
                {
                    using (BlockTable acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable)
                    {
                        using (BlockTableRecord acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord)
                        {
                            Point3dCollection pts = new Point3dCollection();
                            ////
                            double dx = scale / Math.Pow(2, maxdepth), dy = 0.0;
                            double rx, ry, x = startx, y = starty;
                            for (int i = 0; i < ls.Length; i++)
                            {
                                if (ls[i] == 'F')
                                {
                                    Line proj_line = new Line(new Teigha.Geometry.Point3d(x, y, 0), new Teigha.Geometry.Point3d(x + dx, y + dy, 0));
                                    proj_line.ColorIndex = 4;
                                    acBlkTblRec.AppendEntity(proj_line);
                                    acTrans.AddNewlyCreatedDBObject(proj_line, true);
                                    x += dx; y += dy;
                                }
                                if (ls[i] == '+')
                                {
                                    rx = dx; ry = dy;
                                    double radian = Math.PI * angle / 180.0;
                                    dx = rx * Math.Cos(radian) - ry * Math.Sin(radian);
                                    dy = rx * Math.Sin(radian) + ry * Math.Cos(radian);
                                }
                                if (ls[i] == '-')
                                {
                                    rx = dx; ry = dy;
                                    double radian = Math.PI * angle / 180.0;
                                    dx = rx * Math.Cos(-radian) - ry * Math.Sin(-radian);
                                    dy = rx * Math.Sin(-radian) + ry * Math.Cos(-radian);
                                }
                            }
                            /////
                        }
                    }
                }
                acTrans.Commit();
            }
        }
        // Процедура запроса параметров
        //тут было static
        public bool GetParams()
        {

            Document doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            Teigha.Geometry.Point3d position = Teigha.Geometry.Point3d.Origin;
            PromptPointOptions ppo =
              new PromptPointOptions(
                "\nSelect base point of tree: "
              );


            PromptPointResult ppr =
              ed.GetPoint(ppo);


            if (ppr.Status != PromptStatus.OK)
                return false;
            position = ppr.Value;
            startx = position.X;
            starty = position.Y;
            PromptIntegerOptions pdo =
              new PromptIntegerOptions(
                "\nEnter maxdepth <5>: "
              );
            pdo.AllowNone = true;
            PromptIntegerResult pdr =
              ed.GetInteger(pdo);
            if (pdr.Status != PromptStatus.None &&
                pdr.Status != PromptStatus.OK)
                return false;
            if (pdr.Status == PromptStatus.OK)
                maxdepth = pdr.Value;
            else
                maxdepth = 5;
            if (maxdepth > 10)
                maxdepth = 10;
            return true;
        }

    }



    public class CGALACAD
    {
    }
    public class Commands : IExtensionApplication
    {
        struct MMTria
        {
            public int v1;
            public int v2;
            public int v3;
        }
        // функция инициализации (выполняется при загрузке плагина)
        public void Initialize()
        {
            MessageBox.Show("Плагин для вызова функций библиотеки CGAL");
        }

        // функция, выполняемая при выгрузке плагина
        public void Terminate()
        {
            MessageBox.Show("Выгрузка плагина для вызова функций библиотеки CGAL!");
        }

        public static Func<double[], double[], double> L2Norm_Squared_Double = (x, y) =>
        {
            double dist = Math.Sqrt(Math.Pow(x[0] - y[0], 2) + Math.Pow(x[1] - y[1], 2));
            return dist;
        };

        private double GetAreaDMD(Polyline oPoly)
        {
            int nPoints;
            double nArea;
            nPoints = oPoly.NumberOfVertices - 1; // -1 weil Nullbasiert
            nArea = 0.00;

            for (int i = 0; i < nPoints; i++)
            {
                nArea -= (oPoly.GetPoint2dAt(i).X * oPoly.GetPoint2dAt(i + 1).Y);
                nArea += (oPoly.GetPoint2dAt(i).Y * oPoly.GetPoint2dAt(i + 1).X);
            } // for ...
            nArea -= (oPoly.GetPoint2dAt(nPoints).X * oPoly.GetPoint2dAt(0).Y);
            nArea += (oPoly.GetPoint2dAt(nPoints).Y * oPoly.GetPoint2dAt(0).X);

            return nArea / 2;
        } // GetAreaDMD()

        [CommandMethod("CGAL_Simplify", CommandFlags.Session)]
        public void CGALSimplify()
        {
            // UI to select polyline for simplification
            //Database acCurDb = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            Document doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = doc.Database;
            Editor ed = doc.Editor;
            // Ask the user to select 2d polyline
            // for simplification
            PromptEntityOptions opt =
              new PromptEntityOptions(
                "\nSelect entity: "
              );
            PromptEntityResult res =
              ed.GetEntity(opt);
            if (res.Status != PromptStatus.OK)
                return;
            DBObject obj;
            Transaction tr1 = doc.TransactionManager.StartTransaction();
            using (tr1)
            {
                obj = tr1.GetObject(res.ObjectId, OpenMode.ForRead);
                // Let trunsform 
            }



            CGALDotNetGeometry.Numerics.Point2d[] points = null;
            points = new CGALDotNetGeometry.Numerics.Point2d[1];
            Polyline pl = obj as Polyline;
            if (pl == null)
            {
                MessageBox.Show("Wrong type!");
                return;
            }
            int vn = pl.NumberOfVertices;
            points = new CGALDotNetGeometry.Numerics.Point2d[vn];
            for (int ii = 0; ii < vn; ii++)
            {
                Teigha.Geometry.Point3d v3d = pl.GetPoint3dAt(ii);
                points[ii] = new CGALDotNetGeometry.Numerics.Point2d(v3d.X, v3d.Y);
            }


            Simpl form1 = new Simpl();
            if (form1.ShowDialog() == DialogResult.Cancel)
                return;

            int loc_thred = Convert.ToInt16(form1.textBoxStep.Text);

            // start simplify
            var polygon = new Polygon2<EIK>(points);
            //    var polygon = PolygonFactory<EIK>.KochStar(30, 3);
            var param = PolygonSimplificationParams.Default;
            param.threshold = loc_thred;
            param.stop = POLYGON_SIMP_STOP_FUNC.BELOW_THRESHOLD;
            param.cost = POLYGON_SIMP_COST_FUNC.SCALED_SQ_DIST;

            try
            {
                polygon.Simplify(param);
            }
            catch (Teigha.Runtime.Exception ex)
            {
                ed.WriteMessage(
                  "\nIncorrect input params",
                  ex
                );
                return;
            }



            var simplifiedPolygonPoints = new CGALDotNetGeometry.Numerics.Point2d[polygon.Count];
            polygon.GetPoints(simplifiedPolygonPoints, simplifiedPolygonPoints.Length);

            // Let save results 
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                using (DocumentLock DocLock = doc.LockDocument())
                {
                    using (BlockTable acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable)
                    {
                        using (BlockTableRecord acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord)
                        {

                            // Create simplified polygon
                            Point3dCollection pts = new Point3dCollection();
                            for (int i = 0; i < simplifiedPolygonPoints.Length; i++)
                            {
                                Teigha.Geometry.Point3d ppp = new Teigha.Geometry.Point3d(
                                                            simplifiedPolygonPoints[i].x,
                                                            simplifiedPolygonPoints[i].y, 0);
                                if ((ppp.X != 0) && (ppp.Y != 0))
                                    pts.Add(ppp);
                            }
                            Polyline3d pline = new Polyline3d(Poly3dType.SimplePoly, pts, false);
                            pline.Closed = true;
                            acBlkTblRec.AppendEntity((Entity)pline);
                            acTrans.AddNewlyCreatedDBObject(pline, true);
                        }
                    }
                }
                acTrans.Commit();
            }

            // Let save results
        }

        [CommandMethod("CGAL_PolygonOffset", CommandFlags.Session)]
        public void CGALPolygonOffset()
        {
            // UI to select polyline for simplification
            Document doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = doc.Database;
            Editor ed = doc.Editor;
            // Ask the user to select 2d polyline
            // for simplification
            PromptEntityOptions opt =
              new PromptEntityOptions(
                "\nSelect entity: "
              );
            PromptEntityResult res =
              ed.GetEntity(opt);
            if (res.Status != PromptStatus.OK)
                return;
            DBObject obj;
            Transaction tr1 = doc.TransactionManager.StartTransaction();
            using (tr1)
            {
                obj = tr1.GetObject(res.ObjectId, OpenMode.ForWrite);
                // Let trunsform 

                CGALDotNetGeometry.Numerics.Point2d[] points = null;
                points = new CGALDotNetGeometry.Numerics.Point2d[1];
                Polyline pl = obj as Polyline;
                if (pl == null)
                {
                    MessageBox.Show("Wrong type!");
                    return;
                }

                bool pl_invert = false;
                if (GetAreaDMD(pl) < 0)
                {
                    MessageBox.Show("GetAreaDMD: " + GetAreaDMD(pl).ToString());
                    pl_invert = true;
                }

                int vn = pl.NumberOfVertices;
                points = new CGALDotNetGeometry.Numerics.Point2d[vn];
                if (pl_invert)
                {
                    for (int ii = 0; ii < vn; ii++)
                    {
                        Teigha.Geometry.Point3d v3d = pl.GetPoint3dAt(ii);
                        points[ii] = new CGALDotNetGeometry.Numerics.Point2d(v3d.X, v3d.Y);
                    }
                }
                else
                {
                    for (int ii = 0; ii < vn; ii++)
                    {
                        Teigha.Geometry.Point3d v3d = pl.GetPoint3dAt(vn - 1 - ii);
                        points[ii] = new CGALDotNetGeometry.Numerics.Point2d(v3d.X, v3d.Y);
                    }
                }


                Simpl form1 = new Simpl();
                if (form1.ShowDialog() == DialogResult.Cancel)
                    return;

                double off_set = Convert.ToDouble(form1.textBoxStep.Text);

                // start polygon offset
                var polygon = new Polygon2<EIK>(points);

                var instance = PolygonOffset2<EIK>.Instance;
                var exterior = new List<Polygon2<EIK>>();


                try
                {
                    instance.CreateExteriorOffset(polygon, off_set, exterior);
                }
                catch (Teigha.Runtime.Exception ex)
                {
                    ed.WriteMessage(
                      "\nIncorrect input params",
                      ex
                    );
                    return;
                }



                var simplifiedPolygonPoints = new CGALDotNetGeometry.Numerics.Point2d[exterior[1].Count];
                exterior[1].GetPoints(simplifiedPolygonPoints, simplifiedPolygonPoints.Length);

                // Let save results 
                if (true) //using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    using (DocumentLock DocLock = doc.LockDocument())
                    {
                        using (BlockTable acBlkTbl = tr1.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable)
                        {
                            using (BlockTableRecord acBlkTblRec = tr1.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord)
                            {

                                // Create simplified polygon
                                Point3dCollection pts = new Point3dCollection();
                                for (int i = 0; i < simplifiedPolygonPoints.Length; i++)
                                {
                                    Teigha.Geometry.Point3d ppp = new Teigha.Geometry.Point3d(
                                                                simplifiedPolygonPoints[i].x,
                                                                simplifiedPolygonPoints[i].y, 0);
                                    pts.Add(ppp);
                                }
                                Polyline3d pline = new Polyline3d(Poly3dType.SimplePoly, pts, false);
                                pline.Closed = true;
                                acBlkTblRec.AppendEntity((Entity)pline);
                                tr1.AddNewlyCreatedDBObject(pline, true);
                            }
                        }
                    }
                    tr1.Commit();
                }
            }
            // Let save results
        }

        /*[CommandMethod("CGAL_PrintResubt", CommandFlags.Session)]
        public void CGALPrintResult()
        {
            using (VectorLayer layer = VectorLayer.Open("C:\\Users\\User\\Documents\\MMVarmTempCsvLocation\\NewShapeFile_out.shp", Drivers.Shapefile))
            {
                using (var map = new Map(800, 400))
                {
                    map.Add(layer);
                    map.Render("C:\\Users\\User\\Documents\\MMVarmTempCsvLocation\\land_out.png", Renderers.Png);
                }
            }

            FormResult form1 = new FormResult();
            form1.pictureBox1.Image = System.Drawing.Image.FromFile(@"C:\\Users\\User\\Documents\\MMVarmTempCsvLocation\\land_out.png");
            form1.ShowDialog();
        }*/

        [CommandMethod("CGAL_PolygonVisibility", CommandFlags.Session)]
        public void CGALPolygonVisibility()
        {
            // UI to select polyline for simplification
            Document doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = doc.Database;
            Editor ed = doc.Editor;
            // Ask the user to select 2d polyline
            // for simplification
            PromptEntityOptions opt =
              new PromptEntityOptions(
                "\nSelect polyline: "
              );
            PromptEntityResult res =
              ed.GetEntity(opt);
            if (res.Status != PromptStatus.OK)
                return;
            DBObject obj;
            ObjectIdCollection ids = new ObjectIdCollection();

            Transaction tr1 = doc.TransactionManager.StartTransaction();
            using (tr1)
            {
                obj = tr1.GetObject(res.ObjectId, OpenMode.ForWrite);
                // Let trunsform 

                CGALDotNetGeometry.Numerics.Point2d[] points = null;
                points = new CGALDotNetGeometry.Numerics.Point2d[1];
                Polyline pl = obj as Polyline;
                if (pl == null)
                {
                    MessageBox.Show("Wrong type!");
                    tr1.Commit();
                    return;
                }

                // Let ask point =======================================================
                PromptPointOptions ppo = new PromptPointOptions("\n\tSet visibility point: ");
                PromptPointResult ppr = ed.GetPoint(ppo);
                if (ppr.Status != PromptStatus.OK)
                {
                    MessageBox.Show("Wrong point set!");
                    tr1.Commit();
                    return;
                }

                CGALDotNetGeometry.Numerics.Point2d point_set = new CGALDotNetGeometry.Numerics.Point2d(ppr.Value.X, ppr.Value.Y);

                // End ask point =======================================================

                bool pl_invert = false;
                if (GetAreaDMD(pl) < 0)
                {
                    MessageBox.Show("GetAreaDMD: " + GetAreaDMD(pl).ToString());
                    pl_invert = true;
                }

                CGALDotNetGeometry.Numerics.Point2d point = new CGALDotNetGeometry.Numerics.Point2d(0, 0);
                int vn = pl.NumberOfVertices;
                points = new CGALDotNetGeometry.Numerics.Point2d[vn];
                if (pl_invert)
                {
                    for (int ii = 0; ii < vn; ii++)
                    {
                        Teigha.Geometry.Point3d v3d = pl.GetPoint3dAt(ii);
                        points[ii] = new CGALDotNetGeometry.Numerics.Point2d(v3d.X, v3d.Y);
                        point.x += v3d.X;
                        point.y += v3d.Y;
                    }
                }
                else
                {
                    for (int ii = 0; ii < vn; ii++)
                    {
                        Teigha.Geometry.Point3d v3d = pl.GetPoint3dAt(vn - 1 - ii);
                        points[ii] = new CGALDotNetGeometry.Numerics.Point2d(v3d.X, v3d.Y);
                        point.x += v3d.X;
                        point.y += v3d.Y;
                    }
                }
                point.x /= vn;
                point.y /= vn;


                Simpl form1 = new Simpl();
                if (form1.ShowDialog() == DialogResult.Cancel)
                    return;

                double off_set = Convert.ToDouble(form1.textBoxStep.Text);

                // start polygon offset
                var polygon = new Polygon2<EIK>(points);

                var instance = PolygonOffset2<EIK>.Instance;
                var exterior = new List<Polygon2<EIK>>();
                var instanceVisibility = PolygonVisibility<EIK>.Instance;
                var region = new Polygon2<EIK>(points);
                //PolygonWithHoles2<EIK> region;

                try
                {
                    //instance.CreateExteriorOffset(polygon, off_set, exterior);
                    instanceVisibility = PolygonVisibility<EIK>.Instance;
                }
                catch (Teigha.Runtime.Exception ex)
                {
                    ed.WriteMessage(
                      "\nIncorrect input params",
                      ex
                    );
                    return;
                }

                if (instanceVisibility.ComputeVisibility(point_set, polygon, out region))
                {
                    //The op was successful
                    region.Print();
                    MessageBox.Show("region.Print()! ");
                }


                var simplifiedPolygonPoints = new CGALDotNetGeometry.Numerics.Point2d[1];
                //exterior[1].GetPoints(simplifiedPolygonPoints, simplifiedPolygonPoints.Length);

                simplifiedPolygonPoints = new CGALDotNetGeometry.Numerics.Point2d[region.Count];
                region.GetPoints(simplifiedPolygonPoints, simplifiedPolygonPoints.Length);

                //ObjectIdCollection ids = new ObjectIdCollection();

                // ASPOSE object

                Polygon curvePolygon = new Polygon();
                LinearRing aspose_line_str = new LinearRing();


                // Let save results 
                if (true) //using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    using (DocumentLock DocLock = doc.LockDocument())
                    {
                        using (BlockTable acBlkTbl = tr1.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable)
                        {
                            using (BlockTableRecord acBlkTblRec = tr1.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord)
                            {

                                // Create simplified polygon
                                Point3dCollection pts = new Point3dCollection();
                                for (int i = 0; i < simplifiedPolygonPoints.Length; i++)
                                {
                                    Teigha.Geometry.Point3d ppp = new Teigha.Geometry.Point3d(
                                                                simplifiedPolygonPoints[i].x,
                                                                simplifiedPolygonPoints[i].y, 0);
                                    pts.Add(ppp);

                                    aspose_line_str.AddPoint(simplifiedPolygonPoints[i].x,
                                        simplifiedPolygonPoints[i].y);
                                }
                                aspose_line_str.AddPoint(simplifiedPolygonPoints[0].x,
                                    simplifiedPolygonPoints[0].y);
                                Polyline3d pline = new Polyline3d(Poly3dType.SimplePoly, pts, false);
                                pline.Closed = true;
                                //ids.Add(acBlkTblRec.AppendEntity((Entity)pline));
                                //tr1.AddNewlyCreatedDBObject(pline, true);


                                Polyline pl2d = new Polyline(simplifiedPolygonPoints.Length);
                                pl2d.Normal = new Teigha.Geometry.Vector3d(0, 0, 1);
                                for (int i = 0; i < simplifiedPolygonPoints.Length; i++)
                                {
                                    Teigha.Geometry.Point2d tmp_pnt2 = new Teigha.Geometry.Point2d(simplifiedPolygonPoints[i].x, simplifiedPolygonPoints[i].y);
                                    pl2d.AddVertexAt(i, tmp_pnt2, 0.0, 0.0, 0.0);
                                }
                                pl2d.Closed = true;
                                ids.Add(acBlkTblRec.AppendEntity((Entity)pl2d));
                                tr1.AddNewlyCreatedDBObject(pl2d, true);


                                //////Hatch new_hat = new Hatch();
                                //////acBlkTblRec.AppendEntity(new_hat);
                                //////tr1.AddNewlyCreatedDBObject(new_hat, true);
                                //////// Add the hatch loops and complete the hatch
                                //////new_hat.ColorIndex = 5;
                                //////new_hat.Associative = true;
                                //////new_hat.AppendLoop(HatchLoopTypes.Default, ids);
                                //////new_hat.EvaluateHatch(true);

                            }
                        }
                    }
                    tr1.Commit();
                }
                // save aspose
                using (VectorLayer layer = VectorLayer.Create("C:\\Users\\User\\Documents\\MMVarmTempCsvLocation\\NewShapeFile_out.shp", Drivers.Shapefile))
                {
                    Feature firstFeature = layer.ConstructFeature();
                    curvePolygon.ExteriorRing = aspose_line_str;
                    firstFeature.Geometry = curvePolygon;
                    layer.Add(firstFeature);
                    ////using (var map = new Map(800, 400))
                    ////{
                    ////    map.Add(layer);
                    ////    map.Render("C:\\Users\\User\\Documents\\MMVarmTempCsvLocation\\land_out.svg", Renderers.Svg);
                    ////}
                }
            }
            // Let save results
            tr1 = doc.TransactionManager.StartTransaction();
            using (BlockTable acBlkTbl = tr1.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable)
            {
                using (BlockTableRecord acBlkTblRec = tr1.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord)
                {
                    Hatch new_hat = new Hatch();
                    acBlkTblRec.AppendEntity(new_hat);
                    tr1.AddNewlyCreatedDBObject(new_hat, true);
                    // Add the hatch loops and complete the hatch
                    new_hat.ColorIndex = 5;
                    new_hat.SetHatchPattern(HatchPatternType.PreDefined, "SOLID");
                    new_hat.Associative = true;
                    new_hat.AppendLoop(HatchLoopTypes.Default, ids);
                    new_hat.EvaluateHatch(true);
                }
            }
            tr1.Commit();

        }


        [CommandMethod("CGAL_ConvexHull", CommandFlags.Session)]
        public void CGALConvexHall()
        {
            // UI to select polyline for simplification
            Document doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = HostApplicationServices.WorkingDatabase;
            Editor ed = doc.Editor;
            // Ask the user to select 2d polyline
            // for simplification
            PromptEntityOptions opt =
              new PromptEntityOptions(
                "\nSelect entity: "
              );

            PromptEntityResult res = ed.GetEntity(opt);
            if (res.Status != PromptStatus.OK)
                return;

            MessageBox.Show("After GetEntity!");

            DBObject obj;
            Polyline pl;
            Transaction tr1 = acCurDb.TransactionManager.StartTransaction();
            using (tr1)
            {
                obj = tr1.GetObject(res.ObjectId, OpenMode.ForRead);
                MessageBox.Show("After GetEntity1!");
                // Let trunsform 
                MessageBox.Show("After GetObject2! " + obj.ObjectId.ToString());
                pl = obj as Polyline;

                //MessageBox.Show("After GetObject3! " + obj.ObjectId.ToString());



                CGALDotNetGeometry.Numerics.Point2d[] points = null;
                points = new CGALDotNetGeometry.Numerics.Point2d[1];

                MessageBox.Show("After new CGALDotNetGeometry!");
                //Polyline pl = obj as Polyline;
                if (pl == null)
                {
                    MessageBox.Show("Wrong type!");
                    return;
                }

                MessageBox.Show("pl.NumberOfVertices " + pl.NumberOfVertices.ToString());

                int vn = pl.NumberOfVertices;
                points = new CGALDotNetGeometry.Numerics.Point2d[vn];
                for (int ii = 0; ii < vn; ii++)
                {
                    Teigha.Geometry.Point3d v3d = pl.GetPoint3dAt(ii);
                    points[ii] = new CGALDotNetGeometry.Numerics.Point2d(v3d.X, v3d.Y);
                }

                // start polygon offset
                var polygon = new Polygon2<EIK>();
                var convhull = new ConvexHull2<EIK>();
                try
                {
                    polygon = convhull.CreateHull(points, vn);
                }
                catch (Teigha.Runtime.Exception ex)
                {
                    ed.WriteMessage(
                      "\nIncorrect input params",
                      ex
                    );
                    return;
                }
                MessageBox.Show("after CreateHull " + pl.NumberOfVertices.ToString());


                var simplifiedPolygonPoints = new CGALDotNetGeometry.Numerics.Point2d[polygon.Count];
                polygon.GetPoints(simplifiedPolygonPoints, simplifiedPolygonPoints.Length);

                // Let save results 
                if (true)
                {
                    if (true)
                    {
                        MessageBox.Show("After Transaction!");
                        using (BlockTable acBlkTbl = tr1.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable)
                        {
                            MessageBox.Show("After acBlkTbl!");
                            using (BlockTableRecord acBlkTblRec = tr1.GetObject(acCurDb.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord)
                            {
                                MessageBox.Show("After acBlkTblRec!");
                                // Create simplified polygon
                                Point3dCollection pts = new Point3dCollection();
                                for (int i = 0; i < simplifiedPolygonPoints.Length; i++)
                                {
                                    Teigha.Geometry.Point3d ppp = new Teigha.Geometry.Point3d(
                                                                simplifiedPolygonPoints[i].x,
                                                                simplifiedPolygonPoints[i].y, 0);
                                    pts.Add(ppp);
                                }
                                Polyline3d pline = new Polyline3d(Poly3dType.SimplePoly, pts, false);
                                pline.Closed = true;
                                acBlkTblRec.AppendEntity((Entity)pline);
                                tr1.AddNewlyCreatedDBObject(pline, true);
                            }
                        }
                    }
                    tr1.Commit();
                }
            }
            // Let save results
        } // CGALConvexHall
          //  CGALConvexHall End



        //////PromptSelectionOptions pso = new PromptSelectionOptions();

        //////pso.MessageForAdding = "Select profiles:";

        //////pso.AllowDuplicates = false;

        //////PromptSelectionResult psr = ed.GetSelection(pso);

        //////if (psr.Status == PromptStatus.Error) return;
        //////if (psr.Status == PromptStatus.Cancel) return;


        //////SelectionSet ss = psr.Value;

        //////ProfiData pd = new ProfiData();
        //////pd.layerTextName = layerDriftNames;
        //////AssLayers(pd);

        //////int npts = ss.Count;
        //////ObjectId[] idarray = ss.GetObjectIds();

        [CommandMethod("CGAL_TriangulationDelaunay", CommandFlags.Session)]
        public void CGALTriangulation2()
        {
            // UI to select polyline for simplification
            Document doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = HostApplicationServices.WorkingDatabase;
            Editor ed = doc.Editor;

            PromptSelectionOptions pso = new PromptSelectionOptions();

            pso.MessageForAdding = "Select profiles:";

            pso.AllowDuplicates = false;

            PromptSelectionResult psr = ed.GetSelection(pso);

            if (psr.Status == PromptStatus.Error) return;
            if (psr.Status == PromptStatus.Cancel) return;


            SelectionSet ss = psr.Value;


            int npts = ss.Count;
            ObjectId[] idarray = ss.GetObjectIds();

            DBObject obj;
            Transaction tr1 = doc.TransactionManager.StartTransaction();
            using (tr1)
            {
                int vn = npts;
                CGALDotNetGeometry.Numerics.Point2d[] points = new CGALDotNetGeometry.Numerics.Point2d[vn];

                int ii = 0;
                foreach (ObjectId blkId in idarray)
                {
                    DBObject obj_sl = tr1.GetObject(blkId, OpenMode.ForRead);
                    DBPoint poi = obj_sl as DBPoint;
                    if (poi != null)
                    {
                        points[ii] = new CGALDotNetGeometry.Numerics.Point2d(poi.Position.X, poi.Position.Y);
                        ii++;
                    }
                }


                //Create the triangulation.
                MessageBox.Show("Create the triangulation!");
                //var tri = new Triangulation2<EIK>(points);
                var tri = new DelaunayTriangulation2<EIK>(points);

                //Get the triangles a shapes.
                var triangles = new CGALDotNetGeometry.Shapes.Triangle2d[tri.TriangleCount];
                tri.GetTriangles(triangles, triangles.Length);
                MessageBox.Show("triangles.Length " + triangles.Length.ToString());


                // Let save results 
                if (true)
                {
                    if (true)
                    {
                        using (BlockTable acBlkTbl = tr1.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable)



                        {
                            using (BlockTableRecord acBlkTblRec = tr1.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord)
                            {

                                for (int i = 0; i < triangles.Length; i++) // triangles.Length
                                {
                                    MessageBox.Show("triangles[i].A " + triangles[i].A.ToString());
                                    Point3dCollection pts = new Point3dCollection();
                                    Teigha.Geometry.Point3d ppp = new Teigha.Geometry.Point3d(
                                                                triangles[i].A.x,
                                                                triangles[i].A.y, 0);
                                    pts.Add(ppp);
                                    ppp = new Teigha.Geometry.Point3d(
                                                                triangles[i].B.x,
                                                                triangles[i].B.y, 0);
                                    pts.Add(ppp);
                                    ppp = new Teigha.Geometry.Point3d(
                                                                triangles[i].C.x,
                                                                triangles[i].C.y, 0);
                                    pts.Add(ppp);
                                    Polyline3d pline = new Polyline3d(Poly3dType.SimplePoly, pts, false);
                                    pline.Closed = true;
                                    acBlkTblRec.AppendEntity((Entity)pline);
                                    tr1.AddNewlyCreatedDBObject(pline, true);

                                }
                            }
                        }
                    }
                }
                tr1.Commit();
            }
        } // CGALConvexHall

        [CommandMethod("CGAL_Voronoi", CommandFlags.Session)]
        public void CGALVoronoi()
        {
            // UI to select polyline for simplification
            Document doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = HostApplicationServices.WorkingDatabase;
            Editor ed = doc.Editor;

            PromptSelectionOptions pso = new PromptSelectionOptions();

            pso.MessageForAdding = "Select profiles:";

            pso.AllowDuplicates = false;

            PromptSelectionResult psr = ed.GetSelection(pso);

            if (psr.Status == PromptStatus.Error) return;
            if (psr.Status == PromptStatus.Cancel) return;


            SelectionSet ss = psr.Value;


            int npts = ss.Count;
            ObjectId[] idarray = ss.GetObjectIds();

            DBObject obj;
            Transaction tr1 = doc.TransactionManager.StartTransaction();
            using (tr1)
            {
                int vn = npts;
                CGALDotNetGeometry.Numerics.Point2d[] points = new CGALDotNetGeometry.Numerics.Point2d[vn];

                int ii = 0;
                foreach (ObjectId blkId in idarray)
                {
                    DBObject obj_sl = tr1.GetObject(blkId, OpenMode.ForRead);
                    DBPoint poi = obj_sl as DBPoint;
                    if (poi != null)
                    {
                        points[ii] = new CGALDotNetGeometry.Numerics.Point2d(poi.Position.X, poi.Position.Y);
                        ii++;
                    }
                }


                //Create the triangulation.
                MessageBox.Show("Create the triangulation!");
                //var tri = new Triangulation2<EIK>(points);
                var tri = new DelaunayTriangulation2<EIK>(points);

                //Get Voronoi count
                int numSegments = 0;
                int numRays = 0;
                tri.GetVoronoCount(out numSegments, out numRays);
                MessageBox.Show("numSegments " + numSegments.ToString() + " numRays " + numRays.ToString());
                var segments = tri.GetVoronoiSegments();
                var rays = tri.GetVoronoiRays();
                //CGALDotNetGeometry.Shapes.Segment2d sgm;

                //Get the triangles a shapes.
                //var triangles = new CGALDotNetGeometry.Shapes.Triangle2d[tri.TriangleCount];
                //tri.GetTriangles(triangles, triangles.Length);

                // Let save results 
                double tmp = 0;
                int tmp_count = 0;
                if (true)
                {
                    if (true)
                    {
                        using (BlockTable acBlkTbl = tr1.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable)
                        {
                            using (BlockTableRecord acBlkTblRec = tr1.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord)
                            {

                                for (int i = 0; i < numSegments; i++) // triangles.Length
                                {
                                    Point3dCollection pts = new Point3dCollection();
                                    Teigha.Geometry.Point3d ppp = new Teigha.Geometry.Point3d(
                                                                segments[i].A.x,
                                                                segments[i].A.y, 0);
                                    pts.Add(ppp);
                                    ppp = new Teigha.Geometry.Point3d(
                                                                segments[i].B.x,
                                                                segments[i].B.y, 0);
                                    pts.Add(ppp);

                                    tmp += segments[i].Length;
                                    tmp_count++;

                                    Polyline3d pline = new Polyline3d(Poly3dType.SimplePoly, pts, false);
                                    pline.Closed = true;
                                    acBlkTblRec.AppendEntity((Entity)pline);
                                    tr1.AddNewlyCreatedDBObject(pline, true);

                                }
                                tmp /= tmp_count;
                                tmp *= 10;
                                for (int i = 0; i < numRays; i++) // triangles.Length
                                {
                                    Point3dCollection pts = new Point3dCollection();
                                    Teigha.Geometry.Point3d ppp = new Teigha.Geometry.Point3d(
                                                                rays[i].Position.x,
                                                                rays[i].Position.y, 0);
                                    pts.Add(ppp);
                                    ppp = new Teigha.Geometry.Point3d(
                                                                rays[i].Position.x + tmp * rays[i].Direction.x,
                                                                segments[i].B.y + tmp * rays[i].Direction.y, 0);
                                    pts.Add(ppp);

                                    tmp += segments[i].Length;
                                    tmp_count++;

                                    Polyline3d pline = new Polyline3d(Poly3dType.SimplePoly, pts, false);
                                    pline.Closed = true;
                                    acBlkTblRec.AppendEntity((Entity)pline);
                                    tr1.AddNewlyCreatedDBObject(pline, true);
                                }
                            }
                        }
                    }
                }
                tr1.Commit();
            }
        } // CGALConvexHall

        /////////////////////////////////////////////////////////////////////////////////////////////

        [CommandMethod("CGAL_SweepLine", CommandFlags.Session)]
        public void CGALSweepLine()
        {
            // UI to select polyline for simplification
            Document doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = HostApplicationServices.WorkingDatabase;
            Editor ed = doc.Editor;

            PromptSelectionOptions pso = new PromptSelectionOptions();

            pso.MessageForAdding = "Select lines:";

            pso.AllowDuplicates = false;

            PromptSelectionResult psr = ed.GetSelection(pso);

            if (psr.Status == PromptStatus.Error) return;
            if (psr.Status == PromptStatus.Cancel) return;


            SelectionSet ss = psr.Value;


            int npts = ss.Count;
            ObjectId[] idarray = ss.GetObjectIds();

            DBObject obj;
            int lin_number = 0;

            Transaction tr1 = doc.TransactionManager.StartTransaction();
            using (tr1)
            {
                int vn = npts;

                int ii = 0;
                foreach (ObjectId blkId in idarray)
                {
                    DBObject obj_sl = tr1.GetObject(blkId, OpenMode.ForRead);
                    Line lin = obj_sl as Line;
                    if (lin != null)
                    {
                        lin_number++;
                    }

                    DBPoint poi = obj_sl as DBPoint;
                    if (poi != null)
                    {
                    }
                }

                if (lin_number == 0)
                {
                    MessageBox.Show("Nubber of lin:" + lin_number.ToString());
                    tr1.Commit();
                    return;
                }

                var segments = new CGALDotNetGeometry.Shapes.Segment2d[lin_number];
                ii = 0;
                foreach (ObjectId blkId in idarray)
                {
                    DBObject obj_sl = tr1.GetObject(blkId, OpenMode.ForWrite);
                    Line lin = obj_sl as Line;
                    if (lin != null)
                    {
                        segments[ii] = new CGALDotNetGeometry.Shapes.Segment2d(new CGALDotNetGeometry.Numerics.Point2d(lin.StartPoint.X, lin.StartPoint.Y),
                                                                               new CGALDotNetGeometry.Numerics.Point2d(lin.EndPoint.X, lin.EndPoint.Y));
                        ii++;
                        lin.UpgradeOpen();
                        lin.Erase();
                    }
                }

                //Get a instance to the sweep line
                var instance = SweepLine<EEK>.Instance;

                var doIntects = instance.DoIntersect(segments, segments.Length);

                //Find all the unique segments
                var subCurves = instance.ComputeSubcurves(segments, segments.Length);

                //Find all the intection points
                var points = instance.ComputeIntersectionPoints(segments, segments.Length);

                // Let save results 
                if (true)
                {
                    if (true)
                    {
                        using (BlockTable acBlkTbl = tr1.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable)
                        {
                            using (BlockTableRecord acBlkTblRec = tr1.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord)
                            {
                                double len = 0;
                                for (int i = 0; i < subCurves.Length; i++) // triangles.Length
                                {
                                    Point3dCollection pts = new Point3dCollection();
                                    Teigha.Geometry.Point3d ppp = new Teigha.Geometry.Point3d(
                                                                subCurves[i].A.x,
                                                                subCurves[i].A.y, 0);
                                    pts.Add(ppp);
                                    ppp = new Teigha.Geometry.Point3d(
                                                                subCurves[i].B.x,
                                                                subCurves[i].B.y, 0);
                                    pts.Add(ppp);

                                    Polyline3d pline = new Polyline3d(Poly3dType.SimplePoly, pts, false);
                                    pline.Closed = true;
                                    pline.ColorIndex = i % 6;
                                    acBlkTblRec.AppendEntity((Entity)pline);
                                    tr1.AddNewlyCreatedDBObject(pline, true);
                                    len = subCurves[i].Length / 20;
                                }

                                HashSet<Teigha.Geometry.Point3d> dots = new HashSet<Teigha.Geometry.Point3d>();
                                for (int i = 0; i < subCurves.Length; i++)
                                {
                                    Teigha.Geometry.Point3d point1 = new Teigha.Geometry.Point3d(subCurves[i].A.x, subCurves[i].A.y, 0);
                                    Teigha.Geometry.Point3d point2 = new Teigha.Geometry.Point3d(subCurves[i].B.x, subCurves[i].B.y, 0);
                                    if (dots.Contains(point1) == false)
                                        dots.Add(point1);
                                    if (dots.Contains(point2) == false)
                                        dots.Add(point2);
                                }
                                int count = 1;
                                double min_y = 0, max_x = 0;
                                foreach (Teigha.Geometry.Point3d item in dots)
                                {
                                    MText PK_text1 = new MText();
                                    if (item.X > max_x)
                                    {
                                        max_x = item.X;
                                    }
                                    if (item.Y < min_y || min_y == 0)
                                    {
                                        min_y = item.Y;
                                    }
                                    PK_text1.Location = item;
                                    PK_text1.Contents = count.ToString();
                                    count++;
                                    PK_text1.TextHeight = len;
                                    acBlkTblRec.AppendEntity(PK_text1);
                                    tr1.AddNewlyCreatedDBObject(PK_text1, true);
                                }

                                Teigha.Geometry.Point3d pos = new Teigha.Geometry.Point3d(max_x, min_y, 0);
                                Table table = new Table();
                                table.Position = pos;
                                table.SetSize(count, 3);
                                count = 1;
                                table.SetRowHeight(len * 1.2);
                                table.SetColumnWidth(len * 12);
                                table.SetTextHeight(0, 0, len);
                                table.SetTextHeight(0, 1, len);
                                table.SetTextHeight(0, 2, len);
                                table.SetValue(0, 0, "номер");
                                table.SetValue(0, 1, "x");
                                table.SetValue(0, 2, "y");
                                foreach (Teigha.Geometry.Point3d item in dots)
                                {
                                    table.SetValue(count, 0, count);
                                    table.SetValue(count, 1, item.X);
                                    table.SetValue(count, 2, item.Y);
                                    table.SetTextHeight(count, 0, len);
                                    table.SetTextHeight(count, 1, len);
                                    table.SetTextHeight(count, 2, len);
                                    count++;
                                }
                                acBlkTblRec.AppendEntity((Entity)table);
                                tr1.AddNewlyCreatedDBObject(table, true);
                            }
                        }
                    }
                }
                tr1.Commit();
            }
        } // CGALConvexHall


        /////////////////////////////////////////////////////////////////////////////////////////////
        [CommandMethod("CGAL_Triangulation", CommandFlags.Session)]
        public void CGALTriangulation()
        {
            // UI to select polyline for simplification
            Document doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = HostApplicationServices.WorkingDatabase;
            Editor ed = doc.Editor;
            // Ask the user to select 2d polyline
            // for simplification
            PromptEntityOptions opt =
              new PromptEntityOptions(
                "\nSelect entity: "
              );
            PromptEntityResult res =
              ed.GetEntity(opt);
            if (res.Status != PromptStatus.OK)
                return;
            DBObject obj;
            Transaction tr1 = doc.TransactionManager.StartTransaction();
            using (tr1)
            {
                obj = tr1.GetObject(res.ObjectId, OpenMode.ForRead);
                // Let trunsform 

                CGALDotNetGeometry.Numerics.Point2d[] points = null;
                points = new CGALDotNetGeometry.Numerics.Point2d[1];
                Polyline pl = obj as Polyline;
                if (pl == null)
                {
                    MessageBox.Show("Wrong type!");
                    return;
                }
                int vn = pl.NumberOfVertices;
                MessageBox.Show("pl.NumberOfVertices" + vn.ToString());
                points = new CGALDotNetGeometry.Numerics.Point2d[vn];
                for (int ii = 0; ii < vn; ii++)
                {
                    Teigha.Geometry.Point3d v3d = pl.GetPoint3dAt(ii);
                    points[ii] = new CGALDotNetGeometry.Numerics.Point2d(v3d.X, v3d.Y);
                }


                //Create the triangulation.
                MessageBox.Show("Create the triangulation!");
                //var tri = new Triangulation2<EIK>(points);
                var tri = new DelaunayTriangulation2<EIK>(points);

                //Get the triangles a shapes.
                var triangles = new CGALDotNetGeometry.Shapes.Triangle2d[tri.TriangleCount];
                tri.GetTriangles(triangles, triangles.Length);
                MessageBox.Show("triangles.Length " + triangles.Length.ToString());


                // Let save results 
                if (true) //using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    if (true) //using (DocumentLock DocLock = doc.LockDocument())
                    {
                        using (BlockTable acBlkTbl = tr1.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable)
                        {
                            using (BlockTableRecord acBlkTblRec = tr1.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord)
                            {

                                for (int i = 0; i < triangles.Length; i++) // triangles.Length
                                {
                                    MessageBox.Show("triangles[i].A " + triangles[i].A.ToString());
                                    Point3dCollection pts = new Point3dCollection();
                                    Teigha.Geometry.Point3d ppp = new Teigha.Geometry.Point3d(
                                                                triangles[i].A.x,
                                                                triangles[i].A.y, 0);
                                    pts.Add(ppp);
                                    ppp = new Teigha.Geometry.Point3d(
                                                                triangles[i].B.x,
                                                                triangles[i].B.y, 0);
                                    pts.Add(ppp);
                                    ppp = new Teigha.Geometry.Point3d(
                                                                triangles[i].C.x,
                                                                triangles[i].C.y, 0);
                                    pts.Add(ppp);
                                    Polyline3d pline = new Polyline3d(Poly3dType.SimplePoly, pts, false);
                                    pline.Closed = true;
                                    acBlkTblRec.AppendEntity((Entity)pline);
                                    tr1.AddNewlyCreatedDBObject(pline, true);

                                }
                            }
                        }
                    }
                }
                tr1.Commit();
            }
        } // CGALConvexHall



        /////////////////////////////////////////////////////////////////////////////////////////////
        [CommandMethod("CGAL_BUILDSPHERE", CommandFlags.Session)]
        public void CGALBuildsphere()
        {
            // UI to select polyline for simplification
            Document doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = HostApplicationServices.WorkingDatabase;
            Editor ed = doc.Editor;

            PromptSelectionOptions pso = new PromptSelectionOptions();

            pso.MessageForAdding = "Select profiles:";

            pso.AllowDuplicates = false;

            PromptSelectionResult psr = ed.GetSelection(pso);

            if (psr.Status == PromptStatus.Error) return;
            if (psr.Status == PromptStatus.Cancel) return;


            SelectionSet ss = psr.Value;


            int npts = ss.Count;
            ObjectId[] idarray = ss.GetObjectIds();

            DBObject obj;
            Transaction tr1 = doc.TransactionManager.StartTransaction();
            using (tr1)
            {
                int vn = npts;
                CGALDotNetGeometry.Numerics.Point3d[] points = new CGALDotNetGeometry.Numerics.Point3d[vn];

                int ii = 0;
                foreach (ObjectId blkId in idarray)
                {
                    DBObject obj_sl = tr1.GetObject(blkId, OpenMode.ForRead);
                    DBPoint poi = obj_sl as DBPoint;
                    if (poi != null)
                    {
                        points[ii] = new CGALDotNetGeometry.Numerics.Point3d(poi.Position.X, poi.Position.Y, poi.Position.Z);
                        ii++;
                    }
                }




                // Let save results 
                double min_x = 0, min_y = 0, center_x = 0, center_y = 0;
                for (int iii = 0; iii < vn; iii++)
                {
                    if (min_x == 0 || min_x > points[iii].x)
                    {
                        min_x = points[iii].x;
                    }
                    if (min_y == 0 || min_y > points[iii].y)
                    {
                        min_y = points[iii].y;
                    }
                    center_x += points[iii].x;
                    center_y += points[iii].y;
                }

                center_x /= vn;
                center_y /= vn;

                // Точки на проективной сфере
                CGALDotNetGeometry.Numerics.Point3d[] ch_points = new CGALDotNetGeometry.Numerics.Point3d[vn];
                Teigha.Geometry.Point3d tmp_ori_point =
                                new Teigha.Geometry.Point3d(center_x, center_y, 0);
                // Let save results 
                if (true) //using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    if (true) //using (DocumentLock DocLock = doc.LockDocument())
                    {
                        using (BlockTable acBlkTbl = tr1.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable)
                        {
                            using (BlockTableRecord acBlkTblRec = tr1.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord)
                            {

                                Teigha.Geometry.Point3d ppp = new Teigha.Geometry.Point3d(
                                                                center_x,
                                                                center_y, 0.5);
                                Solid3d sol = new Solid3d();
                                sol.CreateSphere(0.5);
                                sol.TransformBy(Matrix3d.Displacement(ppp - new Teigha.Geometry.Point3d(0, 0, 0)));
                                acBlkTblRec.AppendEntity(sol);
                                tr1.AddNewlyCreatedDBObject(sol, true); ;




                                // Цикл проекции
                                for (int i = 0; i < vn; i++)
                                {
                                    // Вычисление точки
                                    Teigha.Geometry.Point3d tmp_shp_point = new Teigha.Geometry.Point3d(points[i].x, points[i].y, 1);
                                    Teigha.Geometry.Vector3d tmp_vec = tmp_shp_point - tmp_ori_point;
                                    double tmp_eps = 1.0 / (tmp_vec.Length * tmp_vec.Length);
                                    tmp_shp_point = tmp_ori_point + tmp_vec * tmp_eps;
                                    ch_points[i] = new CGALDotNetGeometry.Numerics.Point3d(tmp_shp_point.X, tmp_shp_point.Y, tmp_shp_point.Z);
                                    // Создание сферы
                                    Solid3d tmp_sol = new Solid3d();
                                    tmp_sol.CreateSphere(0.01);
                                    tmp_sol.TransformBy(Matrix3d.Displacement(tmp_shp_point - new Teigha.Geometry.Point3d(0, 0, 0)));
                                    tmp_sol.ColorIndex = 3;
                                    acBlkTblRec.AppendEntity(tmp_sol);
                                    tr1.AddNewlyCreatedDBObject(tmp_sol, true);
                                    // Создание отрезков
                                    Line proj_line = new Line(new Teigha.Geometry.Point3d(points[i].x,
                                                              points[i].y, 1), tmp_ori_point);
                                    proj_line.ColorIndex = 5;
                                    acBlkTblRec.AppendEntity(proj_line);
                                    tr1.AddNewlyCreatedDBObject(proj_line, true);
                                }

                            }
                        }
                    }
                }

                // построение проекций


                var hull = ConvexHull3<EIK>.Instance;
                var Polyhedron = hull.CreateHullAsPolyhedron(ch_points, ch_points.Length);
                Point3dCollection vertices = new Point3dCollection();
                Int32Collection faces = new Int32Collection();
                List<EntityColor> tmpVertexColorArray = new List<EntityColor>();
                List<int> faces_indexes = new List<int>();

                int f_count = Polyhedron.FaceCount;
                for (int kk = 0; kk < f_count; kk++)
                {
                    CGALDotNet.Polyhedra.MeshFace3 face = new CGALDotNet.Polyhedra.MeshFace3();
                    Polyhedron.GetFace(kk, out face);
                    CGALDotNet.Polyhedra.MeshHalfedge3 t_he = Polyhedron.GetHalfedge(face.Halfedge);
                    int start_ind = vertices.Count;
                    Teigha.Geometry.Vector3d on_face = new Teigha.Geometry.Vector3d(0, 0, 0);
                    foreach (var e in t_he.EnumerateVertices(Polyhedron))
                    {
                        Teigha.Geometry.Point3d t3d = new Teigha.Geometry.Point3d(e.Point.x, e.Point.y, e.Point.z);
                        vertices.Add(t3d);
                        on_face += t3d - new Teigha.Geometry.Point3d(0, 0, 0);
                    }
                    on_face /= vertices.Count - start_ind;
                    //Check visibility
                    CGALDotNetGeometry.Numerics.Point3d ray_ori =
                          new CGALDotNetGeometry.Numerics.Point3d(tmp_ori_point.X, tmp_ori_point.Y, 0);
                    CGALDotNetGeometry.Numerics.Point3d ray_des =
                          new CGALDotNetGeometry.Numerics.Point3d(on_face.X, on_face.Y, on_face.Z);
                    CGALDotNetGeometry.Numerics.Point3d ray_vec = ray_des - ray_ori;
                    CGALDotNetGeometry.Shapes.Ray3d tets_ray = new CGALDotNetGeometry.Shapes.Ray3d(ray_ori, ray_vec);
                    CGALDotNet.Polyhedra.MeshFace3 ch_face;
                    EntityColor tmp_ec = new EntityColor(100, 0, 100);
                    bool color = false;
                    if (Polyhedron.LocateFace(tets_ray, out ch_face))
                    {
                        if (ch_face.Index == face.Index)
                        {
                            tmp_ec = new EntityColor(0, 110, 0);
                            color = true;
                        }


                    }
                    foreach (var e in t_he.EnumerateVertices(Polyhedron))
                    {
                        tmpVertexColorArray.Add(tmp_ec);
                    }

                    faces.Add(vertices.Count - start_ind);
                    foreach (var e in t_he.EnumerateVertices(Polyhedron))
                    {
                        faces.Add(start_ind);
                        start_ind++;
                    }
                    if (!color)
                    {
                        faces_indexes.Add(kk);
                    }
                }
                HashSet<Teigha.Geometry.Point2d> dots = new HashSet<Teigha.Geometry.Point2d>();
                for (int kk = 0; kk < faces_indexes.Count; kk++)
                {
                    CGALDotNet.Polyhedra.MeshFace3 face = new CGALDotNet.Polyhedra.MeshFace3();
                    Polyhedron.GetFace(faces_indexes[kk], out face);
                    CGALDotNet.Polyhedra.MeshHalfedge3 t_he = Polyhedron.GetHalfedge(face.Halfedge);
                    foreach (var e in t_he.EnumerateVertices(Polyhedron))
                    {
                        Teigha.Geometry.Point3d t3d = new Teigha.Geometry.Point3d(e.Point.x, e.Point.y, e.Point.z);
                        Teigha.Geometry.Vector3d tmp_vec = t3d - tmp_ori_point;
                        double tmp_eps = 1.0 / (tmp_vec.Length * tmp_vec.Length);
                        t3d = tmp_ori_point + tmp_vec * tmp_eps;
                        Teigha.Geometry.Point2d pp = new Teigha.Geometry.Point2d(t3d.X, t3d.Y);
                        dots.Add(pp);
                    }
                }


                using (BlockTable acBlkTbl = tr1.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable)
                {
                    using (BlockTableRecord acBlkTblRec = tr1.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord)
                    {
                        using (SubDMesh sdm = new SubDMesh())
                        {

                            sdm.SetDatabaseDefaults();
                            sdm.SetSubDMesh(vertices, faces, 0);

                            acBlkTblRec.AppendEntity(sdm);
                            tr1.AddNewlyCreatedDBObject(sdm, true);
                            sdm.VertexColorArray = tmpVertexColorArray.ToArray();
                        }
                    }
                }


                // Let save results 

                CGALDotNetGeometry.Numerics.Point2d[] points2 = new CGALDotNetGeometry.Numerics.Point2d[dots.Count];
                int co = 0;
                foreach (Teigha.Geometry.Point2d item in dots)
                {
                    points2[co] = new CGALDotNetGeometry.Numerics.Point2d(item.X, item.Y);
                    co++;
                }
                var triag = new DelaunayTriangulation2<EIK>(points2);
                MessageBox.Show("tri.Length " + triag.TriangleCount.ToString());
                var triangles = new CGALDotNetGeometry.Shapes.Triangle2d[triag.TriangleCount];
                triag.GetTriangles(triangles, triangles.Length);
                MessageBox.Show("triangles.Length " + triangles.Length.ToString());
                using (BlockTable acBlkTbl = tr1.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable)
                {
                    using (BlockTableRecord acBlkTblRec = tr1.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord)
                    {

                        for (int i = 0; i < triangles.Length; i++) // triangles.Length
                        {
                            MessageBox.Show("triangles[i].A " + triangles[i].A.ToString());
                            Point3dCollection pts = new Point3dCollection();
                            Teigha.Geometry.Point3d ppp = new Teigha.Geometry.Point3d(
                                                        triangles[i].A.x,
                                                        triangles[i].A.y, 1);
                            pts.Add(ppp);
                            ppp = new Teigha.Geometry.Point3d(
                                                        triangles[i].B.x,
                                                        triangles[i].B.y, 1);
                            pts.Add(ppp);
                            ppp = new Teigha.Geometry.Point3d(
                                                        triangles[i].C.x,
                                                        triangles[i].C.y, 1);
                            pts.Add(ppp);
                            Polyline3d pline = new Polyline3d(Poly3dType.SimplePoly, pts, false);
                            pline.ColorIndex = 6;
                            pline.Closed = true;
                            acBlkTblRec.AppendEntity((Entity)pline);
                            tr1.AddNewlyCreatedDBObject(pline, true);

                        }
                    }

                }
                tr1.Commit();
            }



        }


        [CommandMethod("CGAL_ConvexHull3D", CommandFlags.Session)]
        public void CGALConvexHull3D()
        {
            // Получение указателей на активный документ(чертеж), БД объектов, редактор геометрии
            Document doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = HostApplicationServices.WorkingDatabase;
            Editor ed = doc.Editor;

            // Создание случайного набора из 10 3D точек
            var points = CGALDotNetGeometry.Numerics.Point3d.RandomPoints(0, 25, new Box3f(-25, 25));

            // Создание случайного набора из 10 3D точек
            var hull = ConvexHull3<EIK>.Instance;
            var Polyhedron = hull.CreateHullAsPolyhedron(points, points.Length);
            Point3dCollection vertices = new Point3dCollection();
            Int32Collection faces = new Int32Collection();

            int f_count = Polyhedron.FaceCount;
            for (int ii = 0; ii < f_count; ii++)
            {
                CGALDotNet.Polyhedra.MeshFace3 face = new CGALDotNet.Polyhedra.MeshFace3();
                Polyhedron.GetFace(ii, out face);
                CGALDotNet.Polyhedra.MeshHalfedge3 t_he = Polyhedron.GetHalfedge(face.Halfedge);
                int start_ind = vertices.Count;
                foreach (var e in t_he.EnumerateVertices(Polyhedron))
                {
                    Teigha.Geometry.Point3d t3d = new Teigha.Geometry.Point3d(e.Point.x, e.Point.y, e.Point.z);
                    vertices.Add(t3d);
                }
                faces.Add(vertices.Count - start_ind);
                foreach (var e in t_he.EnumerateVertices(Polyhedron))
                {
                    faces.Add(start_ind);
                    start_ind++;
                }
            }

            Transaction tr = doc.TransactionManager.StartTransaction();

            using (BlockTable acBlkTbl = tr.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable)
            {
                using (BlockTableRecord acBlkTblRec = tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord)
                {
                    using (SubDMesh sdm = new SubDMesh())
                    {
                        sdm.SetDatabaseDefaults();
                        sdm.SetSubDMesh(vertices, faces, 0);
                        acBlkTblRec.AppendEntity(sdm);
                        tr.AddNewlyCreatedDBObject(sdm, true);
                    }
                }
            }
            tr.Commit();

        }

        [CommandMethod("CGAL_SweepLineARRANGMENT", CommandFlags.Session)]
        public void CGALSweepLineArrangment()
        {
            // UI to select polyline for simplification
            Document doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = HostApplicationServices.WorkingDatabase;
            Editor ed = doc.Editor;

            PromptSelectionOptions pso = new PromptSelectionOptions();

            pso.MessageForAdding = "Select lines:";

            pso.AllowDuplicates = false;

            PromptSelectionResult psr = ed.GetSelection(pso);

            if (psr.Status == PromptStatus.Error) return;
            if (psr.Status == PromptStatus.Cancel) return;


            SelectionSet ss = psr.Value;


            int npts = ss.Count;
            ObjectId[] idarray = ss.GetObjectIds();

            DBObject obj;
            int lin_number = 0;

            Transaction tr1 = doc.TransactionManager.StartTransaction();
            using (tr1)
            {
                int vn = npts;

                int ii = 0;
                foreach (ObjectId blkId in idarray)
                {
                    DBObject obj_sl = tr1.GetObject(blkId, OpenMode.ForRead);
                    Line lin = obj_sl as Line;
                    if (lin != null)
                    {
                        lin_number++;
                    }

                    DBPoint poi = obj_sl as DBPoint;
                    if (poi != null)
                    {
                    }
                }

                if (lin_number == 0)
                {
                    MessageBox.Show("Nubber of lin:" + lin_number.ToString());
                    tr1.Commit();
                    return;
                }

                var segments = new CGALDotNetGeometry.Shapes.Segment2d[lin_number];
                ii = 0;
                foreach (ObjectId blkId in idarray)
                {
                    DBObject obj_sl = tr1.GetObject(blkId, OpenMode.ForWrite);
                    Line lin = obj_sl as Line;
                    if (lin != null)
                    {
                        segments[ii] = new CGALDotNetGeometry.Shapes.Segment2d(new CGALDotNetGeometry.Numerics.Point2d(lin.StartPoint.X, lin.StartPoint.Y),
                                                                               new CGALDotNetGeometry.Numerics.Point2d(lin.EndPoint.X, lin.EndPoint.Y));
                        ii++;
                        lin.UpgradeOpen();
                        lin.Erase();
                    }
                }

                //Get a instance to the sweep line
                var instance = SweepLine<EEK>.Instance;

                var doIntects = instance.DoIntersect(segments, segments.Length);

                //Find all the unique segments
                var subCurves = instance.ComputeSubcurves(segments, segments.Length);

                //Find all the intection points
                var points = instance.ComputeIntersectionPoints(segments, segments.Length);

                // Let save results 
                if (true)
                {
                    if (true)
                    {
                        using (BlockTable acBlkTbl = tr1.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable)
                        {
                            using (BlockTableRecord acBlkTblRec = tr1.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord)
                            {
                                double len = 0;
                                for (int i = 0; i < subCurves.Length; i++) // triangles.Length
                                {
                                    Point3dCollection pts = new Point3dCollection();
                                    Teigha.Geometry.Point3d ppp = new Teigha.Geometry.Point3d(
                                                                subCurves[i].A.x,
                                                                subCurves[i].A.y, 0);
                                    pts.Add(ppp);
                                    ppp = new Teigha.Geometry.Point3d(
                                                                subCurves[i].B.x,
                                                                subCurves[i].B.y, 0);
                                    pts.Add(ppp);

                                    Polyline3d pline = new Polyline3d(Poly3dType.SimplePoly, pts, false);
                                    pline.Closed = true;
                                    pline.ColorIndex = i % 6;
                                    acBlkTblRec.AppendEntity((Entity)pline);
                                    tr1.AddNewlyCreatedDBObject(pline, true);
                                    len = subCurves[i].Length / 20;
                                }

                                HashSet<Teigha.Geometry.Point3d> dots = new HashSet<Teigha.Geometry.Point3d>();
                                for (int i = 0; i < subCurves.Length; i++)
                                {
                                    Teigha.Geometry.Point3d point1 = new Teigha.Geometry.Point3d(subCurves[i].A.x, subCurves[i].A.y, 0);
                                    Teigha.Geometry.Point3d point2 = new Teigha.Geometry.Point3d(subCurves[i].B.x, subCurves[i].B.y, 0);
                                    if (dots.Contains(point1) == false)
                                        dots.Add(point1);
                                    if (dots.Contains(point2) == false)
                                        dots.Add(point2);
                                }
                                int count = 1;
                                double min_y = 0, max_x = 0;
                                foreach (Teigha.Geometry.Point3d item in dots)
                                {
                                    MText PK_text1 = new MText();
                                    if (item.X > max_x)
                                    {
                                        max_x = item.X;
                                    }
                                    if (item.Y < min_y || min_y == 0)
                                    {
                                        min_y = item.Y;
                                    }
                                    PK_text1.Location = item;
                                    PK_text1.Contents = count.ToString();
                                    count++;
                                    PK_text1.TextHeight = len;
                                    acBlkTblRec.AppendEntity(PK_text1);
                                    tr1.AddNewlyCreatedDBObject(PK_text1, true);
                                }

                                Teigha.Geometry.Point3d pos = new Teigha.Geometry.Point3d(max_x, min_y, 0);
                                Table table = new Table();
                                table.Position = pos;
                                table.SetSize(count, 3);
                                count = 1;
                                table.SetRowHeight(len * 1.2);
                                table.SetColumnWidth(len * 12);
                                table.SetTextHeight(0, 0, len);
                                table.SetTextHeight(0, 1, len);
                                table.SetTextHeight(0, 2, len);
                                table.SetValue(0, 0, "номер");
                                table.SetValue(0, 1, "x");
                                table.SetValue(0, 2, "y");
                                foreach (Teigha.Geometry.Point3d item in dots)
                                {
                                    table.SetValue(count, 0, count);
                                    table.SetValue(count, 1, item.X);
                                    table.SetValue(count, 2, item.Y);
                                    table.SetTextHeight(count, 0, len);
                                    table.SetTextHeight(count, 1, len);
                                    table.SetTextHeight(count, 2, len);
                                    count++;
                                }
                                acBlkTblRec.AppendEntity((Entity)table);
                                tr1.AddNewlyCreatedDBObject(table, true);


                                //Get a instance to the sweep line
                                //Create the arrangment object.
                                var arr = new Arrangement2<EEK>();

                                arr.InsertSegments(subCurves, subCurves.Length, false);
                                MessageBox.Show("arr.FaceCount:" + arr.FaceCount);
                                var vertices = new ArrVertex2[arr.VertexCount];
                                arr.GetVertices(vertices, arr.VertexCount);
                                int max_count = 0;
                                for (int kk = 0; kk < arr.VertexCount; kk++)
                                {
                                    if (max_count < vertices[kk].Degree)
                                        max_count = vertices[kk].Degree;
                                }
                                MessageBox.Show("Max vrtex degry:" + max_count.ToString());

                            }
                        }
                    }
                }
                tr1.Commit();
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////
        [CommandMethod("CGAL_BUILDSPHEREFARPOINT", CommandFlags.Session)]
        public void CGALBuildsphereFarPoint()
        {
            // UI to select polyline for simplification
            Document doc = HostMgd.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = HostApplicationServices.WorkingDatabase;
            Editor ed = doc.Editor;

            PromptSelectionOptions pso = new PromptSelectionOptions();

            pso.MessageForAdding = "Select profiles:";

            pso.AllowDuplicates = false;

            PromptSelectionResult psr = ed.GetSelection(pso);

            if (psr.Status == PromptStatus.Error) return;
            if (psr.Status == PromptStatus.Cancel) return;


            SelectionSet ss = psr.Value;


            int npts = ss.Count;
            ObjectId[] idarray = ss.GetObjectIds();

            DBObject obj;
            Transaction tr1 = doc.TransactionManager.StartTransaction();
            using (tr1)
            {
                int vn = npts;
                CGALDotNetGeometry.Numerics.Point3d[] points = new CGALDotNetGeometry.Numerics.Point3d[vn];


                int ii = 0;
                foreach (ObjectId blkId in idarray)
                {
                    DBObject obj_sl = tr1.GetObject(blkId, OpenMode.ForRead);
                    DBPoint poi = obj_sl as DBPoint;
                    if (poi != null)
                    {
                        points[ii] = new CGALDotNetGeometry.Numerics.Point3d(poi.Position.X, poi.Position.Y, poi.Position.Z);
                        ii++;
                    }
                }





                // Let save results 
                double min_x = 0, min_y = 0, center_x = 0, center_y = 0;
                for (int iii = 0; iii < vn; iii++)
                {
                    if (min_x == 0 || min_x > points[iii].x)
                    {
                        min_x = points[iii].x;
                    }
                    if (min_y == 0 || min_y > points[iii].y)
                    {
                        min_y = points[iii].y;
                    }
                    center_x += points[iii].x;
                    center_y += points[iii].y;
                }

                center_x /= vn;
                center_y /= vn;

                // Точки на проективной сфере
                CGALDotNetGeometry.Numerics.Point3d[] ch_points = new CGALDotNetGeometry.Numerics.Point3d[vn];
                Teigha.Geometry.Point3d tmp_ori_point =
                                new Teigha.Geometry.Point3d(center_x, center_y, 0);
                // Let save results 
                if (true) //using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    if (true) //using (DocumentLock DocLock = doc.LockDocument())
                    {
                        using (BlockTable acBlkTbl = tr1.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable)
                        {
                            using (BlockTableRecord acBlkTblRec = tr1.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord)
                            {

                                Teigha.Geometry.Point3d ppp = new Teigha.Geometry.Point3d(
                                                                center_x,
                                                                center_y, 0.5);
                                Solid3d sol = new Solid3d();
                                sol.CreateSphere(0.5);
                                sol.TransformBy(Matrix3d.Displacement(ppp - new Teigha.Geometry.Point3d(0, 0, 0)));
                                acBlkTblRec.AppendEntity(sol);
                                tr1.AddNewlyCreatedDBObject(sol, true); ;




                                // Цикл проекции
                                for (int i = 0; i < vn; i++)
                                {
                                    // Вычисление точки
                                    Teigha.Geometry.Point3d tmp_shp_point = new Teigha.Geometry.Point3d(points[i].x, points[i].y, 1);
                                    Teigha.Geometry.Vector3d tmp_vec = tmp_shp_point - tmp_ori_point;
                                    double tmp_eps = 1.0 / (tmp_vec.Length * tmp_vec.Length);
                                    tmp_shp_point = tmp_ori_point + tmp_vec * tmp_eps;
                                    ch_points[i] = new CGALDotNetGeometry.Numerics.Point3d(tmp_shp_point.X, tmp_shp_point.Y, tmp_shp_point.Z);
                                    // Создание сферы
                                    Solid3d tmp_sol = new Solid3d();
                                    tmp_sol.CreateSphere(0.01);
                                    tmp_sol.TransformBy(Matrix3d.Displacement(tmp_shp_point - new Teigha.Geometry.Point3d(0, 0, 0)));
                                    tmp_sol.ColorIndex = 3;
                                    acBlkTblRec.AppendEntity(tmp_sol);
                                    tr1.AddNewlyCreatedDBObject(tmp_sol, true);
                                    // Создание отрезков
                                    Line proj_line = new Line(new Teigha.Geometry.Point3d(points[i].x,
                                                              points[i].y, 1), tmp_ori_point);
                                    proj_line.ColorIndex = 5;
                                    acBlkTblRec.AppendEntity(proj_line);
                                    tr1.AddNewlyCreatedDBObject(proj_line, true);
                                }

                            }
                        }
                    }
                }

                // построение проекций


                var hull = ConvexHull3<EIK>.Instance;
                var Polyhedron = hull.CreateHullAsPolyhedron(ch_points, ch_points.Length);
                Point3dCollection vertices = new Point3dCollection();
                Int32Collection faces = new Int32Collection();
                List<EntityColor> tmpVertexColorArray = new List<EntityColor>();


                int f_count = Polyhedron.FaceCount;
                List<int> face_fl = new List<int>();

                for (int kk = 0; kk < f_count; kk++)
                {
                    CGALDotNet.Polyhedra.MeshFace3 face = new CGALDotNet.Polyhedra.MeshFace3();
                    Polyhedron.GetFace(kk, out face);
                    CGALDotNet.Polyhedra.MeshHalfedge3 t_he = Polyhedron.GetHalfedge(face.Halfedge);
                    int start_ind = vertices.Count;
                    Teigha.Geometry.Vector3d on_face = new Teigha.Geometry.Vector3d(0, 0, 0);
                    foreach (var e in t_he.EnumerateVertices(Polyhedron))
                    {
                        Teigha.Geometry.Point3d t3d = new Teigha.Geometry.Point3d(e.Point.x, e.Point.y, e.Point.z);
                        vertices.Add(t3d);
                        on_face += t3d - new Teigha.Geometry.Point3d(0, 0, 0);
                    }
                    on_face /= vertices.Count - start_ind;
                    //Check visibility
                    CGALDotNetGeometry.Numerics.Point3d ray_ori =
                          new CGALDotNetGeometry.Numerics.Point3d(tmp_ori_point.X, tmp_ori_point.Y, 0);
                    CGALDotNetGeometry.Numerics.Point3d ray_des =
                          new CGALDotNetGeometry.Numerics.Point3d(on_face.X, on_face.Y, on_face.Z);
                    CGALDotNetGeometry.Numerics.Point3d ray_vec = ray_des - ray_ori;
                    CGALDotNetGeometry.Shapes.Ray3d tets_ray = new CGALDotNetGeometry.Shapes.Ray3d(ray_ori, ray_vec);
                    CGALDotNet.Polyhedra.MeshFace3 ch_face;
                    EntityColor tmp_ec = new EntityColor(100, 0, 100);
                    bool Color = false;
                    if (Polyhedron.LocateFace(tets_ray, out ch_face))
                    {
                        if (ch_face.Index == face.Index)
                        {
                            tmp_ec = new EntityColor(0, 110, 0);
                            Color = true;
                        }

                    }
                    foreach (var e in t_he.EnumerateVertices(Polyhedron))
                    {
                        tmpVertexColorArray.Add(tmp_ec);
                    }

                    faces.Add(vertices.Count - start_ind);
                    foreach (var e in t_he.EnumerateVertices(Polyhedron))
                    {
                        faces.Add(start_ind);
                        start_ind++;
                    }

                    if (Color)
                    {
                        face_fl.Add(1);
                    }
                    else
                    {
                        face_fl.Add(0);
                    }
                }


                using (BlockTable acBlkTbl = tr1.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable)
                {
                    using (BlockTableRecord acBlkTblRec = tr1.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord)
                    {


                        for (int kk = 0; kk < f_count; kk++)
                        {
                            if (face_fl[kk] == 0)
                                continue;

                            List<Teigha.Geometry.Point2d> tmp_vertices = new List<Teigha.Geometry.Point2d>();
                            CGALDotNet.Polyhedra.MeshFace3 face = new CGALDotNet.Polyhedra.MeshFace3();
                            Polyhedron.GetFace(kk, out face);
                            CGALDotNet.Polyhedra.MeshHalfedge3 t_he = Polyhedron.GetHalfedge(face.Halfedge);
                            foreach (var e in t_he.EnumerateVertices(Polyhedron))
                            {
                                Teigha.Geometry.Point3d t3d = new Teigha.Geometry.Point3d(e.Point.x, e.Point.y, e.Point.z);
                                Teigha.Geometry.Vector3d tmp_vec = t3d - tmp_ori_point;
                                double tmp_eps = 1.0 / (tmp_vec.Length * tmp_vec.Length);
                                t3d = tmp_ori_point + tmp_vec * tmp_eps;
                                Teigha.Geometry.Point2d pp = new Teigha.Geometry.Point2d(t3d.X, t3d.Y);
                                tmp_vertices.Add(pp);
                            }
                            Teigha.Geometry.Point2d p0 = new Teigha.Geometry.Point2d(tmp_vertices[0].X, tmp_vertices[0].Y);
                            Teigha.Geometry.Point2d p1 = new Teigha.Geometry.Point2d(tmp_vertices[1].X, tmp_vertices[1].Y);
                            Teigha.Geometry.Point2d p2 = new Teigha.Geometry.Point2d(tmp_vertices[2].X, tmp_vertices[2].Y);


                            Teigha.Geometry.CircularArc2d tc = new CircularArc2d(p0, p1, p2);
                            Teigha.DatabaseServices.Circle circle = new Teigha.DatabaseServices.Circle(new Teigha.Geometry.Point3d(tc.Center.X, tc.Center.Y, 0), new Teigha.Geometry.Vector3d(0, 0, 1), 0.1);
                            circle.ColorIndex = 1;
                            acBlkTblRec.AppendEntity(circle);
                            tr1.AddNewlyCreatedDBObject(circle, true);
                        }

                        /////////////////////////
                        List<MMTria> face_conn = new List<MMTria>();


                        List<Teigha.Geometry.Point2d> face_coord = new List<Teigha.Geometry.Point2d>();
                        for (int kk = 0; kk < f_count; kk++)
                        {
                            MMTria tmp = new MMTria();
                            tmp.v1 = -1;
                            tmp.v2 = -1;
                            tmp.v3 = -1;
                            face_conn.Add(tmp);

                            face_coord.Add(new Teigha.Geometry.Point2d());
                        }


                        for (int kk = 0; kk < f_count; kk++)
                        {
                            if (face_fl[kk] == 0) continue;
                            CGALDotNet.Polyhedra.MeshFace3 face = new CGALDotNet.Polyhedra.MeshFace3();
                            Polyhedron.GetFace(kk, out face);
                            CGALDotNet.Polyhedra.MeshHalfedge3 t_he = Polyhedron.GetHalfedge(face.Halfedge);
                            Point3dCollection tmp_vertices = new Point3dCollection();
                            foreach (var e in t_he.EnumerateVertices(Polyhedron))
                            {
                                Teigha.Geometry.Point3d t3d = new Teigha.Geometry.Point3d(e.Point.x, e.Point.y, e.Point.z);
                                Teigha.Geometry.Vector3d tmp_vec = t3d - tmp_ori_point;
                                double tmp_eps = 1.0 / (tmp_vec.Length * tmp_vec.Length);
                                t3d = tmp_ori_point + tmp_vec * tmp_eps;
                                tmp_vertices.Add(t3d);
                            }
                            Teigha.Geometry.Point2d p0 = new Teigha.Geometry.Point2d(tmp_vertices[0].X, tmp_vertices[0].Y);
                            Teigha.Geometry.Point2d p1 = new Teigha.Geometry.Point2d(tmp_vertices[1].X, tmp_vertices[1].Y);
                            Teigha.Geometry.Point2d p2 = new Teigha.Geometry.Point2d(tmp_vertices[2].X, tmp_vertices[2].Y);

                            Teigha.Geometry.Point3d p03 = new Teigha.Geometry.Point3d(tmp_vertices[0].X, tmp_vertices[0].Y, 0);
                            Teigha.Geometry.Point3d p13 = new Teigha.Geometry.Point3d(tmp_vertices[1].X, tmp_vertices[1].Y, 0);
                            Teigha.Geometry.Point3d p23 = new Teigha.Geometry.Point3d(tmp_vertices[2].X, tmp_vertices[2].Y, 0);

                            Point3dCollection pts = new Point3dCollection();
                            pts.Add(p03);
                            pts.Add(p13);
                            pts.Add(p23);
                            Polyline3d pline = new Polyline3d(Poly3dType.SimplePoly, pts, false);
                            pline.ColorIndex = 6;
                            pline.Closed = true;
                            acBlkTblRec.AppendEntity((Entity)pline);
                            tr1.AddNewlyCreatedDBObject(pline, true);

                            Teigha.Geometry.CircularArc2d tc = new CircularArc2d(p0, p1, p2);
                            face_coord[kk] = tc.Center;
                            Teigha.DatabaseServices.Circle circle = new Teigha.DatabaseServices.Circle(
                              new Teigha.Geometry.Point3d(tc.Center.X, tc.Center.Y, 0), new Teigha.Geometry.Vector3d(0, 0, 1), 0.1);
                            circle.ColorIndex = 1;
                            acBlkTblRec.AppendEntity(circle);
                            tr1.AddNewlyCreatedDBObject(circle, true);
                            foreach (var e in t_he.EnumerateHalfedges(Polyhedron))
                            {
                                MMTria tmp = new MMTria();
                                tmp.v1 = -1;
                                tmp.v2 = -1;
                                tmp.v3 = -1;
                                CGALDotNet.Polyhedra.MeshHalfedge3 m_he = Polyhedron.GetHalfedge(e.Opposite);
                                if (face_fl[m_he.Face] == 1)
                                {
                                    if (face_conn[kk].v1 == -1) tmp.v1 = m_he.Face;
                                    else
                                      if (face_conn[kk].v2 == -1) tmp.v2 = m_he.Face;
                                    else
                                        if (face_conn[kk].v3 == -1) tmp.v3 = m_he.Face;
                                    face_conn[kk] = tmp;
                                }
                            }
                        }

                        for (int kk = 0; kk < f_count; kk++)
                        {
                            if (face_fl[kk] == 0) continue;
                            CGALDotNet.Polyhedra.MeshFace3 face = new CGALDotNet.Polyhedra.MeshFace3();
                            Polyhedron.GetFace(kk, out face);
                            CGALDotNet.Polyhedra.MeshHalfedge3 t_he = Polyhedron.GetHalfedge(face.Halfedge);
                            foreach (var e in t_he.EnumerateHalfedges(Polyhedron))
                            {
                                CGALDotNet.Polyhedra.MeshHalfedge3 m_he = Polyhedron.GetHalfedge(e.Opposite);
                                if (face_fl[m_he.Face] == 0)
                                {
                                    Teigha.Geometry.Point3d p0 = new Teigha.Geometry.Point3d(m_he.SourcePoint(Polyhedron).x, m_he.SourcePoint(Polyhedron).y, m_he.SourcePoint(Polyhedron).z);
                                    Teigha.Geometry.Vector3d tmp_vec = p0 - tmp_ori_point;
                                    double tmp_eps = 1.0 / (tmp_vec.Length * tmp_vec.Length);
                                    p0 = tmp_ori_point + tmp_vec * tmp_eps;
                                    Teigha.Geometry.Point3d p1 = new Teigha.Geometry.Point3d(m_he.TargetPoint(Polyhedron).x, m_he.TargetPoint(Polyhedron).y, m_he.SourcePoint(Polyhedron).z);

                                    Teigha.Geometry.Vector3d tmp_vec2 = p1 - tmp_ori_point;
                                    double tmp_eps2 = 1.0 / (tmp_vec2.Length * tmp_vec2.Length);
                                    p1 = tmp_ori_point + tmp_vec2 * tmp_eps2;

                                    Teigha.Geometry.Vector3d vec = p1 - p0;
                                    Teigha.Geometry.Vector3d vec_orto = new Teigha.Geometry.Vector3d(vec.Y, -vec.X, 0);
                                    vec = new Teigha.Geometry.Point3d(face_coord[kk].X, face_coord[kk].Y, 0) - (p0 + (p1 - p0) * 0.5);
                                    if (vec.DotProduct(vec_orto) < 0) vec_orto *= -1;
                                    Line proj_line = new Line(new Teigha.Geometry.Point3d(face_coord[kk].X, face_coord[kk].Y, 0),
                                                     new Teigha.Geometry.Point3d(face_coord[kk].X, face_coord[kk].Y, 0) + vec_orto * 100);
                                    proj_line.ColorIndex = 1;
                                    acBlkTblRec.AppendEntity(proj_line);
                                    tr1.AddNewlyCreatedDBObject(proj_line, true);
                                }
                            }
                        }


                        for (int kk = 0; kk < f_count; kk++)
                        {
                            if (face_fl[kk] == 0) continue;

                            if (face_conn[kk].v1 > -1)
                            { //Add edge

                                Teigha.Geometry.Point3d p1 = new Teigha.Geometry.Point3d(face_coord[face_conn[kk].v1].X, face_coord[face_conn[kk].v1].Y, 0);
                                Teigha.Geometry.Point3d p2 = new Teigha.Geometry.Point3d(face_coord[kk].X, face_coord[kk].Y, 0);
                                Line proj_line = new Line(p1, p2);
                                proj_line.ColorIndex = 1;
                                acBlkTblRec.AppendEntity(proj_line);
                                tr1.AddNewlyCreatedDBObject(proj_line, true);
                            }
                            if (face_conn[kk].v2 > -1)
                            {
                                //Add edge
                                Teigha.Geometry.Point3d p1 = new Teigha.Geometry.Point3d(face_coord[face_conn[kk].v2].X, face_coord[face_conn[kk].v2].Y, 0);
                                Teigha.Geometry.Point3d p2 = new Teigha.Geometry.Point3d(face_coord[kk].X, face_coord[kk].Y, 0);
                                Line proj_line = new Line(p1, p2);
                                proj_line.ColorIndex = 1;
                                acBlkTblRec.AppendEntity(proj_line);
                                tr1.AddNewlyCreatedDBObject(proj_line, true);
                            }
                            if (face_conn[kk].v3 > -1)
                            {
                                //Add edge
                                Teigha.Geometry.Point3d p1 = new Teigha.Geometry.Point3d(face_coord[face_conn[kk].v3].X, face_coord[face_conn[kk].v3].Y, 0);
                                Teigha.Geometry.Point3d p2 = new Teigha.Geometry.Point3d(face_coord[kk].X, face_coord[kk].Y, 0);
                                Line proj_line = new Line(p1, p2);
                                proj_line.ColorIndex = 1;
                                acBlkTblRec.AppendEntity(proj_line);
                                tr1.AddNewlyCreatedDBObject(proj_line, true);
                            }
                        }

                        /////////////////////////
                        using (SubDMesh sdm = new SubDMesh())
                        {

                            sdm.SetDatabaseDefaults();
                            sdm.SetSubDMesh(vertices, faces, 0);

                            acBlkTblRec.AppendEntity(sdm);
                            tr1.AddNewlyCreatedDBObject(sdm, true);
                            sdm.VertexColorArray = tmpVertexColorArray.ToArray();
                        }
                    }


                    // Let save results 

                    tr1.Commit();
                }



            }
        }

        /// 22.11
        [CommandMethod("LS_Hilbert", CommandFlags.Session)]
        public void LS_Hilbert()
        {
            L_System mf = new L_System();
            mf.LSystem();
        }

        [CommandMethod("Fractal_plant", CommandFlags.Session)]
        public void Fractal_plant()
        {
            Fractal_plant mf = new Fractal_plant();
            mf.System();
        }

        [CommandMethod("Dragon_Curve", CommandFlags.Session)]
        public void Dragon_curve()
        {
            Dragon_curve mf = new Dragon_curve();
            mf.System();
        }

        [CommandMethod("Serpinskij_triangle", CommandFlags.Session)]
        public void Serpinskij_triangle()
        {
            Serpinskij_triangle mf = new Serpinskij_triangle();
            mf.System();
        }

        [CommandMethod("Koh_curve", CommandFlags.Session)]
        public void Koh_curve()
        {
            Koh_curve mf = new Koh_curve();
            mf.System();
        }
    }
}

