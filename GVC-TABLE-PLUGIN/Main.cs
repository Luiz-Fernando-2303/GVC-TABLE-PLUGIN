using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using GVC_TABLE_PLUGIN.Components;
using System.Windows.Forms;

namespace GVC_TABLE_PLUGIN
{
    public class PanelConfigList : List<(string guid, string name, string HTML, System.Windows.Forms.Form form)>;

    [Transaction(TransactionMode.Manual)]
    public class OpenConfigPanel : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Context.RevitContext.SetUIApplication(commandData.Application);

            //TemplateConfiguration confiTemplate = new();
            //TableInterface.UpdatePanel("A1234567-89AB-CDEF-0123-456789ABCDEF", null, confiTemplate.GetInterface());

            TableInterface.GetPanel("A1234567-89AB-CDEF-0123-456789ABCDEF").Show();

            return Result.Succeeded;
        }
    }

    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    public class RegisterDockablePane : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            Context.RevitContext.SetUIControlledApplication(application);

            string panelGuid = "A1234567-89AB-CDEF-0123-456789ABCDEF";

            PanelConfigList panels = new()
            {
                (panelGuid, "Configuração do template", null, null)
            };
            CreatePanels(application, panels);

            Context.RevitEvents.Init();
            Context.RevitEvents.Run();

            return Result.Succeeded;
        }


        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public static void CreatePanels(UIControlledApplication application, PanelConfigList panels)
        {
            try
            {
                foreach (var (guid, name, HTML, form) in panels) TableInterfaceRegister.RegisterPanel(application, name, guid, HTML, form);
            }
            catch (Exception e)
            {
                MessageBox.Show("OnStartup() - ERRO", e.ToString());
            }
        }
    }
}
