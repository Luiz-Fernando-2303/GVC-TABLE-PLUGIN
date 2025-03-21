using System;
using System.Windows;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using GVC_TABLE_PLUGIN.Components;

namespace GVC_TABLE_PLUGIN
{

    [Transaction(TransactionMode.Manual)]
    public class MainClass : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            TableInterface interfaceControls = new(commandData);
            interfaceControls.GetPanel("A1234567-89AB-CDEF-0123-456789ABCDEF").Show();
            interfaceControls.GetPanel("C2F1A4D6-7B8C-4A3F-9E12-AB56F0D98324").Show();        

            return Result.Succeeded;
        }
    }

    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    public class RegisterDockablePane : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            CreatePanels(application);
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public static void CreatePanels(UIControlledApplication application)                                     
        {
            var panels = new List<(string guid, string name)>
            {
                ("A1234567-89AB-CDEF-0123-456789ABCDEF", "Configuração do template"),
                ("C2F1A4D6-7B8C-4A3F-9E12-AB56F0D98324", "Visualização do template")
            };

            try
            {
                foreach (var (guid, name) in panels)
                {
                    TableInterfaceRegister.RegisterPanel(
                        application, 
                        name, 
                        guid, 
                        $"<html><h1>{name}</h1><p>{guid}</p></html>"
                    );
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("OnStartup() - ERRO", e.ToString());
            }
        }
    }
}
