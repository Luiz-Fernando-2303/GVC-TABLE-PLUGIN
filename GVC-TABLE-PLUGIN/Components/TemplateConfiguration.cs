using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace GVC_TABLE_PLUGIN.Components
{
    internal class TemplateConfiguration
    {
        private readonly TemplateConfigurationFunctions functions;
        public TemplateConfiguration(ExternalCommandData commandData_, List<Object> configList)
        {
            functions = new TemplateConfigurationFunctions(commandData_);
        }
    }

    public class TemplateConfigurationFunctions
    {
        private readonly ExternalCommandData commandData;
        public TemplateConfigurationFunctions(ExternalCommandData commandData)
        {
            this.commandData = commandData;
        }

        public ICollection<ElementId> GetSelectionIds()
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Selection selection = uiDoc.Selection;

            ICollection<ElementId> selectedIds = selection.GetElementIds();
            return selectedIds;
        }
    }

    public class DataReferenceTable
    {
        public List<string> properties = new List<string> { "Area", "Material" };
        public List<ElementId> Ids = new List<ElementId>();
        public DataReferenceTable(List<string> properties)
        {

        }
    }

}
