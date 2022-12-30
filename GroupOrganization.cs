using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Xml.Schema;

namespace revit_api_csharp
{
    [Transaction(TransactionMode.Manual)]
    internal class GroupOrganization : IExternalCommand
    {
        public static string NomeAleatorio(int tamanho)
        {   //Generate a random name
            String chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Random random = new Random();
            String result = new string(
                Enumerable.Repeat(chars, tamanho)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get application and document objets
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            View view = doc.ActiveView;

            //Select all project groups
            IList<Group> allGrps = new FilteredElementCollector(doc)
                .OfClass(typeof(Group))
                .Cast<Group>()
                .ToList();

            IList<String> grpName = new List<String>();

            //Rename all project groups to string Name
            foreach (Group grp in allGrps)
            {
                using(Transaction trans = new Transaction(doc,"Update GroupName"))
                {
                    trans.Start();
                    grp.GroupType.Name = NomeAleatorio(11);
                    trans.Commit();
                }
            }

            //Ordenate groups by flat or upright
            IList<Group> grpsHorizontal = new List<Group>();
            IList<Group> grpsVertical = new List<Group>();

            foreach (Group grp in allGrps)
            {
                BoundingBoxXYZ bBox = grp.get_BoundingBox(view);
                
                double distX = bBox.Max.X - bBox.Min.X;
                double distY = bBox.Max.Y - bBox.Min.Y;

                if (distX >= distY)
                {
                    grpsHorizontal.Add(grp);
                }
                else if (distY > distX)
                {
                    grpsVertical.Add(grp);
                }
            }

            //Flat manipulations
            IList<XYZ> hPoints = new List<XYZ>();
            IList<LocationPoint> hLocations = new List<LocationPoint>();

            foreach (Group grp in grpsHorizontal)
            {
                //Take the groups points and groups locations
                LocationPoint hLocation = grp.Location as LocationPoint;
                hPoints.Add(hLocation.Point);
                hLocations.Add(grp.Location as LocationPoint);
            }

            int yTotal = hPoints.Count;
            foreach (XYZ pointY in hPoints)
            {
                IList<XYZ> Var = hPoints.Where(x => x.Y == pointY.Y).ToList();
                if (Var.Count > 1)
                {
                    yTotal -= 1;
                }
            }

            int hCont = 0;
            int nGroup = 1;

            while (hCont < yTotal)
            {
                IList<double> pointsY = new List<double>();
                foreach (XYZ point in hPoints)
                {
                    pointsY.Add(point.Y);
                }
                double yMax = pointsY.Max();

                List<XYZ> linePoints = hPoints.Where(x => x.Y == yMax).ToList();

                foreach (XYZ linepoint in linePoints)
                {
                    hPoints.Remove(linepoint);
                }

                IList<XYZ> linePointsOrdened = linePoints.OrderBy(x => x.X).ToList();
                foreach (XYZ linepoint in linePointsOrdened) 
                {
                    foreach (Group grp in grpsHorizontal)
                    {
                        LocationPoint lPoint = grp.Location as LocationPoint;
                        XYZ refPoint = lPoint.Point;

                        if(linepoint.ToString() == refPoint.ToString())
                        {
                            using (Transaction trans = new Transaction(doc, "Update GroupName (2)"))
                            {
                                string grpNome = grp.Name.ToString();
                                if (grpNome.All(char.IsDigit)){
                                    continue;
                                }
                                else
                                {
                                    trans.Start();
                                    if (nGroup < 10)
                                    {
                                        grp.GroupType.Name = "0" + nGroup.ToString();
                                    }
                                    else if (nGroup >= 10)
                                    {
                                        grp.GroupType.Name = nGroup.ToString();
                                    }
                                    trans.Commit();

                                    nGroup += 1;
                                }

                            }
                            break;
                        }

                    }
                }
                hCont += 1;
            }

            //Upright manipulations
            List<XYZ> vPoints = new List<XYZ>();
            IList<LocationPoint> vLocations = new List<LocationPoint>();

            foreach (Group grp in grpsVertical)
            {
                //take all groups points and groups locations
                LocationPoint vLocation = grp.Location as LocationPoint;
                vPoints.Add(vLocation.Point);
                vLocations.Add(grp.Location as LocationPoint);
            }

            List<string> pointsX = new List<string>();
            foreach (XYZ _p in vPoints)
            {
                pointsX.Add(_p.X.ToString("F"));
            }

            HashSet<string> pXWithoutDuplicates = new HashSet<string>(pointsX); //remove duplicates
            int xTotal = pXWithoutDuplicates.Count; //total upright groups
            
            int vCont = 0;
            while (vCont < xTotal)
            {
                
                IList<double> points = new List<double>();
                foreach (XYZ point in vPoints)
                {
                    points.Add(point.X);
                }
                double xMin = points.Min();

                List<XYZ> linePoints = vPoints
                    .Where(x => x.X.ToString("F") == xMin.ToString("F"))
                    .OrderByDescending(p => p.Y)
                    .ToList();

                foreach(XYZ linepoint in linePoints)
                {
                    vPoints.Remove(linepoint);
                    foreach (Group grp in grpsVertical)
                    {
                        LocationPoint lPoint = grp.Location as LocationPoint;
                        XYZ refPoint = lPoint.Point;

                        if (linepoint.ToString() == refPoint.ToString())
                        {
                            using (Transaction trans = new Transaction(doc, "Update GroupName (2)"))
                            {
                                string grpNome = grp.Name.ToString();
                                if (grpNome.All(char.IsDigit))
                                {
                                    continue;
                                }
                                else
                                {
                                    trans.Start();
                                    if (nGroup < 10)
                                    {
                                        grp.GroupType.Name = "0" + nGroup.ToString();
                                    }
                                    else if (nGroup >= 10)
                                    {
                                        grp.GroupType.Name = nGroup.ToString();
                                    }
                                    trans.Commit();

                                    nGroup += 1;
                                }

                            }
                            break;
                        }

                    }
                    
        }
                    
        vCont += 1;
            }
            TaskDialog.Show("Grupos no Modelo", "Os grupos foram renomeados com sucesso!");

            return Result.Succeeded;

        }
    }
}
