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
        public static void UpdatePanel(string guid, string html, WinForms.Form form)
        {
            var dockableWindow = TableInterfaceRegister.GetRegisteredWindow(guid);

            if (dockableWindow == null)
            {
                WinForms.MessageBox.Show("Painel não encontrado ou não registrado.");
                return;
            }

            UIElement newContent = null;

            if (!string.IsNullOrEmpty(html))
            {
                var browser = new WinForms.WebBrowser();
                browser.DocumentText = html;
                newContent = new WindowsFormsHost { Child = browser };
            }
            else if (form != null)
            {
                newContent = new WindowsFormsHost { Child = form };
            }

            dockableWindow.SetInternalContent(newContent);
        }

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

        public static void RegisterPanel(UIControlledApplication application, string name, string guid, string html, WinForms.Form form)
        {
            try
            {
                DockablePaneId paneId = GetPaneId(guid);

                var panel = new DockableWindow();
                RegisteredWindows[guid] = panel;

                application.RegisterDockablePane(paneId, name, panel);
                TableInterface.UpdatePanel(guid, html, form);
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

    public class DockableWindow : System.Windows.Controls.UserControl, IDockablePaneProvider
    {
        private readonly ContentControl contentContainer;

        public DockableWindow()
        {
            contentContainer = new ContentControl();
            this.Content = contentContainer;
        }

        public void SetupDockablePane(DockablePaneProviderData data)
        {
            data.FrameworkElement = this;
        }

        public void SetInternalContent(UIElement newContent)
        {
            contentContainer.Content = newContent;
        }
    }
}
