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
    [Transaction(TransactionMode.Manual)]
    internal class DeleteElement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //get uidocument
            UIDocument uidoc = commandData.Application.ActiveUIDocument;

            //get doc
            Document doc = uidoc.Document;

            try
            {
                Reference pickedobject = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);

                if(pickedobject != null)
                {
                    using(Transaction trans = new Transaction(doc, "Delete Element"))
                    {
                        trans.Start();
                        doc.Delete(pickedobject.ElementId);

                        TaskDialog tdialog = new TaskDialog("Delete");
                        tdialog.MainContent = "Are you sure you want to delet this element?";
                        tdialog.CommonButtons = TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.Cancel;

                        if(tdialog.Show() == TaskDialogResult.Ok)
                        {
                            trans.Commit();
                            TaskDialog.Show("Delete", pickedobject.ElementId.ToString() + "deleted");

                        }
                        else
                        {
                            trans.RollBack();
                            TaskDialog.Show("Delete", pickedobject.ElementId.ToString() + "not deleted");
                        }
                    }
                }
            }
            catch
            {

            }



            return Result.Succeeded;
        }
    }
}
