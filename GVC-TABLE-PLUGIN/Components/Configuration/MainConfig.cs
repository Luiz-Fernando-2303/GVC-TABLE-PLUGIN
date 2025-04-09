using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using WinForms = System.Windows.Forms;

namespace GVC_TABLE_PLUGIN.Components.Configuration
{
    public partial class TemplateConfiguration
    {
        private static TemplateConfiguration _instance;
        private static WinForms.Form _form;

        private readonly TemplateConfigurationFunctions _functions;
        private readonly List<Reference> _references = new();

        public static void Show(ExternalCommandData commandData)
        {
            if (_instance == null || _form == null || _form.IsDisposed)
            {
                UIDocument uiDoc = commandData.Application.ActiveUIDocument;
                _instance = new TemplateConfiguration(uiDoc);
            }

            if (!_form.Visible)
            {
                _form.Show();
            }
            else
            {
                _form.BringToFront();
            }
        }

        private TemplateConfiguration(UIDocument uiDoc)
        {
            _functions = new TemplateConfigurationFunctions(uiDoc);
            _form = TemplateConfigurationUI.GetOrCreateForm(_functions, _references);
        }
    }
}
