using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;

namespace revit_api_csharp
{
    [Transaction(TransactionMode.Manual)]
    internal class PlaceLoopElement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //get uidoc
            UIDocument uidoc = commandData.Application.ActiveUIDocument;

            //get document
            Document doc = uidoc.Document;

            //create points
            XYZ p1 = new XYZ(-10, -10, 0);
            XYZ p2 = new XYZ(10, -10, 0);
            XYZ p3 = new XYZ(15, 0, 0);
            XYZ p4 = new XYZ(10, 10, 0);
            XYZ p5 = new XYZ(-10, 10, 0);

            //create lines
            Line l1 = Line.CreateBound(p1, p2);
            Arc l2 = Arc.Create(p2, p4, p3);
            Line l3 = Line.CreateBound(p4, p5);
            Line l4 = Line.CreateBound(p5, p1);

            //create list of curves
            List<Curve> curves = new List<Curve>();
            curves.Add(l1);
            curves.Add(l2);
            curves.Add(l3);
            curves.Add(l4);

            //create offset curve
            CurveLoop crvLoop = CurveLoop.Create(curves);
            double distance = UnitUtils.ConvertToInternalUnits(135, UnitTypeId.Millimeters);
            CurveLoop crvOffset = CurveLoop.CreateViaOffset(crvLoop, distance, new XYZ(0, 0, 1));

            //create curve array
            CurveArray crvArray = new CurveArray();
            foreach (Curve c in curves)
            {
                crvArray.Append(c);
            }

            try
            {
                using (Transaction trans = new Transaction(doc,"Floor Place"))
                {
                    trans.Start();

                    doc.Create.NewFloor(crvArray, false);

                    trans.Commit();

                }
                return Result.Succeeded;
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }

        }
    }
}
