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
    internal class PlaceFamily : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //get uidoc
            UIDocument uidoc = commandData.Application.ActiveUIDocument;

            //get doc
            Document doc = uidoc.Document;

            //get filtered
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            FamilySymbol symbol = collector.OfClass(typeof(FamilySymbol))
                .WhereElementIsElementType()
                .Cast<FamilySymbol>()
                .First(x => x.Name == "1525 x 762mm");

             /*
              other way to filter collector
            FamilySymbol symbol = null;
            foreach (Element ele in symbols)
            {
                if()
                {
                    symbol = ele as FamilySymbol;
                    break;
                }
            }

                */

            try
            {
                using(Transaction trans = new Transaction(doc, "Place Family"))
                {
                    trans.Start();

                    if (!symbol.IsActive)
                    {
                        symbol.Activate();
                    }

                    doc.Create.NewFamilyInstance(new XYZ(0, 0, 0), symbol, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);

                    trans.Commit();
                }
                
                return Result.Succeeded;
            }
            catch(Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
            
        }
    }
}
