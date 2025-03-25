using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Data;
using System.Windows.Forms;

namespace GVC_TABLE_PLUGIN.Components
{
    public class TemplateConfiguration
    {
        private readonly TemplateConfigurationFunctions functions;
        private readonly Dictionary<string, DataTable> referenceTables = new();

        private System.Windows.Forms.Form form;
        private System.Windows.Forms.Panel tablePanel;
        private ListBox referenceList;
        private Button addButton;

        public TemplateConfiguration()
        {
            Context.RevitEvents.Init();
            functions = new TemplateConfigurationFunctions(Context.RevitContext.ActiveUIDoc);
            CreateInterface();
        }

        private void CreateInterface()
        {
            form = new System.Windows.Forms.Form
            {
                Text = "Template Configuration",
                Width = 800,
                Height = 600
            };

            referenceList = new ListBox
            {
                Top = 10,
                Left = 10,
                Width = 200,
                Height = 500,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom
            };
            referenceList.SelectedIndexChanged += OnReferenceSelected;

            addButton = new Button
            {
                Text = "Adicionar Referência",
                Top = 520,
                Left = 10,
                Width = 200,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            addButton.Click += OnAddReferenceClicked;

            tablePanel = new System.Windows.Forms.Panel
            {
                Top = 10,
                Left = 220,
                Width = 550,
                Height = 530,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BorderStyle = BorderStyle.FixedSingle
            };

            form.Controls.Add(referenceList);
            form.Controls.Add(addButton);
            form.Controls.Add(tablePanel);
        }

        public System.Windows.Forms.Form GetInterface()
        {
            return form;
        }

        private void OnAddReferenceClicked(object sender, EventArgs e)
        {
            using (System.Windows.Forms.Form inputForm = new())
            {
                inputForm.Text = "Nova Referência";
                inputForm.Width = 400;
                inputForm.Height = 150;
                inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                inputForm.StartPosition = FormStartPosition.CenterScreen;
                inputForm.MinimizeBox = false;
                inputForm.MaximizeBox = false;

                Label nameLabel = new Label { Left = 10, Top = 20, Text = "Nome da Referência:", AutoSize = true };
                System.Windows.Forms.TextBox nameBox = new System.Windows.Forms.TextBox { Left = 150, Top = 15, Width = 220 };

                Button okButton = new Button { Text = "OK", Left = 280, Width = 90, Top = 60, DialogResult = DialogResult.OK };
                Button cancelButton = new Button { Text = "Cancelar", Left = 180, Width = 90, Top = 60, DialogResult = DialogResult.Cancel };

                inputForm.Controls.Add(nameLabel);
                inputForm.Controls.Add(nameBox);
                inputForm.Controls.Add(okButton);
                inputForm.Controls.Add(cancelButton);

                inputForm.AcceptButton = okButton;
                inputForm.CancelButton = cancelButton;

                if (inputForm.ShowDialog() == DialogResult.OK)
                {
                    string referenceName = nameBox.Text.Trim();

                    if (string.IsNullOrWhiteSpace(referenceName))
                    {
                        MessageBox.Show("O nome da referência é obrigatório!", "Erro");
                        return;
                    }

                    if (referenceTables.ContainsKey(referenceName))
                    {
                        MessageBox.Show("Essa referência já existe!", "Aviso");
                        return;
                    }

                    DataTable table = functions.SelectionTable();

                    referenceTables[referenceName] = table;
                    referenceList.Items.Add(referenceName);
                }
            }
        }

        private void OnReferenceSelected(object sender, EventArgs e)
        {
            if (referenceList.SelectedIndex == -1) return;

            string selectedReference = referenceList.SelectedItem.ToString();

            if (referenceTables.TryGetValue(selectedReference, out DataTable table))
            {
                tablePanel.Controls.Clear();

                DataGridView dataGridView = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    DataSource = table,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
                };

                tablePanel.Controls.Add(dataGridView);
            }
        }
    }

    public class TemplateConfigurationFunctions
    {
        private readonly UIDocument uiDoc;
        private readonly Document doc;

        public TemplateConfigurationFunctions(UIDocument uiDoc)
        {
            this.uiDoc = uiDoc;
            doc = uiDoc.Document;
        }

        public ICollection<ElementId> GetSelectionIds()
        {
            return uiDoc.Selection.GetElementIds();
        }

        public List<string> GetPropertyNames(ICollection<ElementId> selection)
        {
            var propertyNames = new List<string>();

            foreach (ElementId id in selection)
            {
                Element element = doc.GetElement(id);
                foreach (Parameter property in element.GetOrderedParameters())
                {
                    if (property.Definition == null) continue;
                    if (!propertyNames.Contains(property.Definition.Name))
                    {
                        propertyNames.Add(property.Definition.Name);
                    }
                }
            }

            return propertyNames;
        }

        public DataTable SelectionTable()
        {
            ICollection<ElementId> selectedIds = GetSelectionIds();
            List<string> propertyNames = GetPropertyNames(selectedIds);

            DataTable table = new DataTable();
            foreach (string propertyName in propertyNames)
            {
                table.Columns.Add(propertyName, typeof(string));
            }

            foreach (ElementId id in selectedIds)
            {
                Element element = doc.GetElement(id);
                DataRow row = table.NewRow();
                foreach (Parameter property in element.GetOrderedParameters())
                {
                    if (property.Definition == null) continue;
                    row[property.Definition.Name] = property.AsValueString();
                }
                table.Rows.Add(row);
            }

            return table;
        }

        public DataTable CollectionToTable(ICollection<ElementId> ids)
        {
            List<string> propertyNames = GetPropertyNames(ids);

            DataTable table = new DataTable();
            foreach (string propertyName in propertyNames)
            {
                table.Columns.Add(propertyName, typeof(string));
            }

            foreach (ElementId id in ids)
            {
                Element element = doc.GetElement(id);
                DataRow row = table.NewRow();
                foreach (Parameter property in element.GetOrderedParameters())
                {
                    if (property.Definition == null) continue;
                    row[property.Definition.Name] = property.AsValueString();
                }
                table.Rows.Add(row);
            }

            return table;
        }
    }
}
