using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

namespace revit_api_csharp
{
    [TransactionAttribute(TransactionMode.ReadOnly)]
    public class GetElementId : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Get UIDocument
            UIDocument uidoc = commandData.Application.ActiveUIDocument;

            //Get Document
            Document doc = uidoc.Document;

            try
            {

            //Pick object
            Reference pickedObject = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);

                //Retrieve element
                ElementId eleId = pickedObject.ElementId;

                Element ele = doc.GetElement(eleId);

                //Get Element Type

                ElementId eTypeId = ele.GetTypeId();
                ElementType eType = doc.GetElement(eTypeId) as ElementType;

            //Display Element Id
            if (pickedObject != null)
            {
                TaskDialog.Show("Element Classification", eleId.ToString() + Environment.NewLine + 
                    "Category: " + ele.Category.Name + Environment.NewLine +
                    "Instance: " + ele.Name + Environment.NewLine +
                    "Symbol: " + eType.Name + Environment.NewLine +
                    "Family: " + eType.FamilyName);
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
