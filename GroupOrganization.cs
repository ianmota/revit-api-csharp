using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;


namespace revit_api_csharp
{
    public class PickGroup : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem.Category.Id.Equals((int)BuiltInCategory.OST_IOSModelGroups);
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }

    [Transaction(TransactionMode.Manual)]
    internal class GroupOrganization : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get application and document objets
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            Reference pickedObj = null;

            //Pick a group
            Selection sel = uidoc.Selection;
            PickGroup selGroup = new PickGroup();
            pickedObj = sel.PickObject(ObjectType.Element, selGroup, "Please, select a group");


            return Result.Succeeded;

        }
    }
}
