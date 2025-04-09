using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using WinForms = System.Windows.Forms;

namespace GVC_TABLE_PLUGIN.Components
{
    public static class TableInterface
    {
        public static DockablePane GetPanel(string guid)
        {
            try
            {
                var uiApp = Context.RevitContext.UiApp;

                if (uiApp == null)
                {
                    WinForms.MessageBox.Show("Contexto do Revit não está disponível.");
                    return null;
                }

                DockablePaneId paneId = TableInterfaceRegister.GetPaneId(guid);
                return uiApp.GetDockablePane(paneId);
            }
            catch (Exception ex)
            {
                WinForms.MessageBox.Show($"Erro ao obter painel: {ex.Message}");
                return null;
            }
        }
    }

    public static class TableInterfaceRegister
    {
        private static readonly Dictionary<string, DockableWindow> RegisteredWindows = new();

        public static DockablePaneId GetPaneId(string guid)
        {
            return new DockablePaneId(new Guid(guid));
        }

        public static void RegisterPanel(UIControlledApplication application, string name, string guid, object content)
        {
            try
            {
                DockablePaneId paneId = GetPaneId(guid);

                var panel = new DockableWindow(content);
                RegisteredWindows[guid] = panel;
                application.RegisterDockablePane(paneId, name, panel);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("RegisterPanel() - ERRO", ex.Message);
            }
        }

        public static DockableWindow GetRegisteredWindow(string guid)
        {
            RegisteredWindows.TryGetValue(guid, out var window);
            return window;
        }
    }

    public class DockableWindow : UserControl, IDockablePaneProvider
    {
        private readonly ContentControl contentContainer;

        public DockableWindow(object content)
        {
            contentContainer = new ContentControl();
            this.Content = contentContainer;
            SetInternalContent(content);
        }

        public void SetupDockablePane(DockablePaneProviderData data)
        {
            data.FrameworkElement = this;
            data.InitialState = new DockablePaneState
            {
                DockPosition = DockPosition.Right
            };
        }

        private void SetInternalContent(object newContent)
        {
            if (newContent is string htmlString)
            {
                var browser = new System.Windows.Forms.WebBrowser();
                browser.DocumentText = htmlString;

                var host = new WindowsFormsHost
                {
                    Child = browser
                };

                contentContainer.Content = host;
            }
            else if (newContent is System.Windows.Forms.Form form)
            {
                form.TopLevel = false;
                form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                form.Show();

                var host = new WindowsFormsHost
                {
                    Child = form
                };

                contentContainer.Content = host;
            }
            else
            {
                throw new ArgumentException("Invalid content type. Only HTML strings and WinForms.Forms are supported.");
            }
        }
    }
}
