using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows;

namespace GVC_TABLE_PLUGIN.Context
{
    /// <summary>
    /// Contexto global da aplicação Revit
    /// </summary>
    public static class RevitContext
    {
        public static UIControlledApplication Application { get; private set; }
        public static UIApplication UiApp { get; private set; }

        public static UIDocument ActiveUIDoc => UiApp?.ActiveUIDocument;
        public static Document ActiveDoc => ActiveUIDoc?.Document;

        public static bool IsInitialized => Application != null && UiApp != null;

        /// <summary>
        /// Define o contexto a partir de UIApplication (normalmente recebido via comando ou ExternalEvent)
        /// </summary>
        public static void SetUIApplication(UIApplication app)
        {
            UiApp = app;
        }

        /// <summary>
        /// Define o contexto apenas com UIControlledApplication (usado em OnStartup, se necessário)
        /// </summary>
        public static void SetUIControlledApplication(UIControlledApplication app)
        {
            Application = app;
        }

        /// <summary>
        /// Limpa o contexto (usado no OnShutdown)
        /// </summary>
        public static void Clear()
        {
            Application = null;
            UiApp = null;
        }
    }

    /// <summary>
    /// Gerenciador de eventos externos do Revit
    /// </summary>
    public static class RevitEvents
    {
        public static ExternalEvent MainEvent { get; private set; }
        private static bool initialized = false;

        /// <summary>
        /// Inicializa o evento externo se ainda não tiver sido criado
        /// </summary>
        public static void Init()
        {
            if (initialized) return;

            MainEvent = ExternalEvent.Create(new MainCommandHandler());
            initialized = true;
        }

        /// <summary>
        /// Dispara a execução do comando principal
        /// </summary>
        public static void Run()
        {
            Init();
            MainEvent?.Raise();
        }
    }

    /// <summary>
    /// Comando principal que será executado pelo evento externo
    /// </summary>
    public class MainCommandHandler : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            RevitContext.SetUIApplication(app);
            TaskDialog.Show("Executado", "Comando via ExternalEvent executado com sucesso.");
        }

        public string GetName() => "MainCommandHandler";
    }
}
