using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GVC_TABLE_PLUGIN.Components
{
    internal class RevitSelector
    {
        private readonly ExternalCommandData commandData;
        
        public RevitSelector(ExternalCommandData commandData_)
        {
            commandData = commandData_;
        }

        public ICollection<ElementId> GetSelectionIds()
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Selection selection = uiDoc.Selection;

            ICollection<ElementId> selectedIds = selection.GetElementIds();
            return selectedIds;
        }


    }
}
