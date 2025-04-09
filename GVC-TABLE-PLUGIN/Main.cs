using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using GVC_TABLE_PLUGIN.Components;
using GVC_TABLE_PLUGIN.Components.Configuration;

namespace GVC_TABLE_PLUGIN
{
    public class PanelConfigList : List<(string guid, string name, object content)>;

    [Transaction(TransactionMode.Manual)]
    public class OpenConfigPanel : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Context.RevitContext.SetUIApplication(commandData.Application);

            TemplateConfiguration.Show(commandData);
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
                (panelGuid, "Configuração do template", "<!DOCTYPE html><html><body><h1>Configuração do template</h1></body></html>")
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
                foreach (var (guid, name, content) in panels) TableInterfaceRegister.RegisterPanel(application, name, guid, content);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("OnStartup() - ERRO", e.ToString());
            }
        }
    }
}
