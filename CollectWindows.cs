using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;


namespace revit_api_csharp
{
    [TransactionAttribute(TransactionMode.ReadOnly)]
    public class CollectWindows : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //get UIDocument
            UIDocument uidoc = commandData.Application.ActiveUIDocument;

            //get Document
            Document doc = uidoc.Document;

            //create filtered element colector
            FilteredElementCollector collector = new FilteredElementCollector(doc);

            //create element filter
            ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Windows);

            //apply filter
            IList<Element> windows = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();

            //show informations

            TaskDialog.Show("Windows", String.Format("{0} windows counted!", windows.Count));

            return Result.Succeeded;
        }
    }
}
