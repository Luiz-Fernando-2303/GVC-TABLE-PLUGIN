using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows;
using System.Windows.Controls;
using System;
using System.IO;

namespace GVC_TABLE_PLUGIN.Components
{
    public class TableInterface
    {
        private readonly ExternalCommandData commandData;

        public TableInterface(ExternalCommandData commandData_)
        {
            commandData = commandData_;
        }

        public DockablePane GetPanel(string guid)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document document = uiDoc.Document;

            try
            {
                DockablePaneId paneId = TableInterfaceRegister.GetPaneId(guid);
                DockablePane pane = uiApp.GetDockablePane(paneId);

                if (pane == null)
                {
                    MessageBox.Show("O painel não foi encontrado.");
                    return null;
                }

                return pane;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
    }

    public class TableInterfaceRegister
    {
        public static DockablePaneId GetPaneId(string guid)
        {
            return new DockablePaneId(new Guid(guid));
        }

        public static Dictionary<string, string> RegisterPanel(UIControlledApplication application, string name, string guid, string HTML)
        {
            try
            {
                DockablePaneId paneId = GetPaneId(guid);
                DockableWindow panel = new (HTML);

                application.RegisterDockablePane(paneId, name, panel);

                return new Dictionary<string, string>
                {
                    {"guid", guid },
                    {"name", name }
                };

            }
            catch (Exception ex)
            {
                TaskDialog.Show("RegisterPanel() - ERRO", ex.Message);
                return null;
            }
        }

        public static void UnregisterPanel(UIControlledApplication application, string guid)
        {
            DockablePaneId id = GetPaneId(guid);

        }
    }

    public class DockableWindow : UserControl, IDockablePaneProvider
    {
        public DockableWindow(string HTML)
        {
            WebBrowser webBrowser = new WebBrowser();
            webBrowser.NavigateToString($"{HTML}");

            Content = webBrowser;
        }

        public void SetupDockablePane(DockablePaneProviderData data)
        {
            data.FrameworkElement = this;
        }
    }
}
