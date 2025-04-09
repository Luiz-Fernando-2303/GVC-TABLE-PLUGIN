using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Data;

namespace GVC_TABLE_PLUGIN.Components.Configuration
{
    public class TemplateConfigurationFunctions
    {
        private readonly UIDocument _uiDoc;
        private readonly Document _doc;

        public TemplateConfigurationFunctions(UIDocument uiDoc)
        {
            _uiDoc = uiDoc;
            _doc = uiDoc.Document;
        }

        public ICollection<ElementId> GetSelectionIds() => _uiDoc.Selection.GetElementIds();

        public List<string> GetPropertyNames(ICollection<ElementId> selection) =>
            selection.SelectMany(id => _doc.GetElement(id)?.GetOrderedParameters())
                     .Where(p => p?.Definition != null)
                     .Select(p => p.Definition.Name)
                     .Distinct()
                     .ToList();

        public DataTable CollectionToTable(ICollection<ElementId> ids)
        {
            var propertyNames = GetPropertyNames(ids);
            var table = new DataTable();

            foreach (var name in propertyNames)
                table.Columns.Add(name, typeof(string));

            foreach (var id in ids)
            {
                var element = _doc.GetElement(id);
                var row = table.NewRow();

                foreach (var property in element.GetOrderedParameters())
                {
                    if (property.Definition != null)
                        row[property.Definition.Name] = property.AsValueString();
                }

                table.Rows.Add(row);
            }

            return table;
        }
    }
}
