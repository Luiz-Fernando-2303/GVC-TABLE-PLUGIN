using Autodesk.Revit.DB;

namespace GVC_TABLE_PLUGIN.Components.Configuration
{
    public class ReferenceOutputConfig
    {
        public string OutputType { get; set; } = "Tabela HTML";
        public string PrimaryColumn { get; set; }
        public List<string> SubItemColumns { get; set; } = new();
        public int StartIndex { get; set; } = 0;
        public int EndIndex { get; set; } = 10;
        public string IdField { get; set; } = "";
    }

    public class SubSelection
    {
        public string Name { get; set; }
        public string ComparedColumn { get; set; }
        public string ComparedValue { get; set; }
        public string Condition { get; set; }
        public ICollection<ElementId> Selection { get; set; }
        public List<string> Columns { get; set; } = new();
        public ReferenceOutputConfig Output { get; set; } = new();
    }

    public class Reference
    {
        public string Name { get; set; }
        public ICollection<ElementId> Selection { get; set; }
        public List<string> Columns { get; set; } = new();
        public ReferenceOutputConfig Output { get; set; } = new();
        public List<SubSelection> SubSelections { get; set; } = new();
    }
}
