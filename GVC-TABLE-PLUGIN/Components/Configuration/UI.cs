using GVC_TABLE_PLUGIN.Components.Configuration;
using System.Data;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

public static class TemplateConfigurationUI
{
    private static Form _cachedForm;

    public static Form GetOrCreateForm(TemplateConfigurationFunctions functions, List<Reference> references)
    {
        if (_cachedForm != null && !_cachedForm.IsDisposed)
            return _cachedForm;

        var form = new Form
        {
            Text = "Template Configuration",
            Width = 1200,
            Height = 700
        };

        var tableView = CreateTableView();
        var referenceTree = CreateReferenceTree(functions, references, tableView);
        var addReferenceButton = CreateAddReferenceButton(functions, references, referenceTree);
        var addSubSelectionButton = CreateAddSubSelectionButton(functions, references, referenceTree);
        var exportButton = CreateExportButton(references);
        var editOutputButton = CreateButton("Editar Output", 550, 620);

        form.Controls.AddRange(new Control[]
        {
            referenceTree, tableView,
            addReferenceButton, addSubSelectionButton,
            exportButton, editOutputButton
        });

        form.FormClosing += (s, e) => { e.Cancel = true; form.Hide(); };
        _cachedForm = form;
        return form;
    }

    private static TreeView CreateReferenceTree(TemplateConfigurationFunctions functions, List<Reference> references, DataGridView tableView)
    {
        var tree = new TreeView
        {
            Left = 10,
            Top = 10,
            Width = 300,
            Height = 600,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom
        };

        tree.AfterSelect += (s, e) =>
        {
            tableView.DataSource = null;

            if (e.Node.Parent == null)
            {
                var reference = references.FirstOrDefault(r => r.Name == e.Node.Text);
                if (reference != null)
                {
                    tableView.DataSource = functions.CollectionToTable(reference.Selection);
                }
            }
            else
            {
                var reference = references.FirstOrDefault(r => r.Name == e.Node.Parent.Text);
                var sub = reference?.SubSelections.ElementAtOrDefault(e.Node.Index);
                if (sub != null)
                {
                    var table = new DataTable();
                    foreach (var col in sub.Columns)
                    {
                        table.Columns.Add(col);
                    }
                    tableView.DataSource = table;
                }
            }
        };

        return tree;
    }

    private static DataGridView CreateTableView()
    {
        return new DataGridView
        {
            Left = 320,
            Top = 10,
            Width = 850,
            Height = 600,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        };
    }

    private static Button CreateAddReferenceButton(TemplateConfigurationFunctions functions, List<Reference> references, TreeView referenceTree)
    {
        var button = CreateButton("Adicionar Referência", 10, 620);

        button.Click += (s, e) =>
        {
            using var inputForm = new Form
            {
                Text = "Nova Referência",
                Width = 400,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen,
                MinimizeBox = false,
                MaximizeBox = false
            };

            var nameLabel = new Label { Left = 10, Top = 20, Text = "Nome da Referência:", AutoSize = true };
            var nameBox = new TextBox { Left = 150, Top = 15, Width = 220 };
            var okButton = new Button { Text = "OK", Left = 280, Width = 90, Top = 60, DialogResult = DialogResult.OK };
            var cancelButton = new Button { Text = "Cancelar", Left = 180, Width = 90, Top = 60, DialogResult = DialogResult.Cancel };

            inputForm.Controls.AddRange(new Control[] { nameLabel, nameBox, okButton, cancelButton });
            inputForm.AcceptButton = okButton;
            inputForm.CancelButton = cancelButton;

            if (inputForm.ShowDialog() != DialogResult.OK) return;

            string referenceName = nameBox.Text.Trim();
            if (string.IsNullOrEmpty(referenceName)) return;

            if (references.Any(r => r.Name == referenceName))
            {
                MessageBox.Show("Essa referência já existe!", "Aviso");
                return;
            }

            var selection = functions.GetSelectionIds();
            var columns = functions.GetPropertyNames(selection);

            var reference = new Reference
            {
                Name = referenceName,
                Selection = selection.ToList(),
                Columns = columns,
                Output = new ReferenceOutputConfig()
            };

            references.Add(reference);
            referenceTree.Nodes.Add(new TreeNode(referenceName));
        };

        return button;
    }

    private static Button CreateAddSubSelectionButton(TemplateConfigurationFunctions functions, List<Reference> references, TreeView referenceTree)
    {
        var button = CreateButton("Adicionar SubSeleção", 170, 620);

        button.Click += (s, e) =>
        {
            var selectedNode = referenceTree.SelectedNode;
            if (selectedNode == null || selectedNode.Parent != null) return;

            var reference = references.FirstOrDefault(r => r.Name == selectedNode.Text);
            if (reference == null) return;

            var dataTable = functions.CollectionToTable(reference.Selection);
            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                MessageBox.Show("Não há dados disponíveis para filtrar.", "Erro");
                return;
            }

            using var inputForm = new Form
            {
                Text = "Nova SubSeleção",
                Width = 420,
                Height = 280,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen,
                MinimizeBox = false,
                MaximizeBox = false
            };

            var nameLabel = new Label { Left = 10, Top = 20, Text = "Nome da SubSeleção:", AutoSize = true };
            var nameBox = new TextBox { Left = 160, Top = 15, Width = 230 };

            var columnLabel = new Label { Left = 10, Top = 60, Text = "Coluna:", AutoSize = true };
            var columnBox = new ComboBox
            {
                Left = 160,
                Top = 55,
                Width = 230,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            columnBox.Items.AddRange(dataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray());

            var operatorLabel = new Label { Left = 10, Top = 100, Text = "Operador:", AutoSize = true };
            var operatorBox = new ComboBox
            {
                Left = 160,
                Top = 95,
                Width = 230,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            operatorBox.Items.AddRange(new[] { "=", "!=", "<", "<=", ">", ">=" });

            var valueLabel = new Label { Left = 10, Top = 140, Text = "Valor:", AutoSize = true };
            var valueBox = new TextBox { Left = 160, Top = 135, Width = 230 };

            var okButton = new Button { Text = "OK", Left = 300, Top = 200, Width = 90, DialogResult = DialogResult.OK };
            var cancelButton = new Button { Text = "Cancelar", Left = 200, Top = 200, Width = 90, DialogResult = DialogResult.Cancel };

            inputForm.Controls.AddRange(new Control[]
            {
                nameLabel, nameBox,
                columnLabel, columnBox,
                operatorLabel, operatorBox,
                valueLabel, valueBox,
                okButton, cancelButton
            });

            inputForm.AcceptButton = okButton;
            inputForm.CancelButton = cancelButton;

            if (inputForm.ShowDialog() != DialogResult.OK) return;

            string name = nameBox.Text.Trim();
            string column = columnBox.SelectedItem?.ToString();
            string op = operatorBox.SelectedItem?.ToString();
            string value = valueBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(column) || string.IsNullOrWhiteSpace(op) || string.IsNullOrWhiteSpace(value))
            {
                MessageBox.Show("Todos os campos devem ser preenchidos.", "Erro");
                return;
            }

            var filteredTable = dataTable.Clone();

            foreach (var row in dataTable.AsEnumerable())
            {
                var cellValue = row[column]?.ToString();
                if (string.IsNullOrWhiteSpace(cellValue)) continue;

                bool match = false;

                if (double.TryParse(cellValue, out var cellNum) && double.TryParse(value, out var valNum))
                {
                    match = op switch
                    {
                        "=" => cellNum == valNum,
                        "!=" => cellNum != valNum,
                        "<" => cellNum < valNum,
                        "<=" => cellNum <= valNum,
                        ">" => cellNum > valNum,
                        ">=" => cellNum >= valNum,
                        _ => false
                    };
                }
                else
                {
                    match = op switch
                    {
                        "=" => cellValue == value,
                        "!=" => cellValue != value,
                        _ => false
                    };
                }

                if (match) filteredTable.ImportRow(row);
            }

            var subSelection = new SubSelection
            {
                Name = name,
                ComparedColumn = column,
                ComparedValue = value,
                Condition = $"{column} {op} '{value}'",
                Columns = filteredTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList(),
                Selection = reference.Selection,
                Output = new ReferenceOutputConfig()
            };

            reference.SubSelections.Add(subSelection);
            selectedNode.Nodes.Add(new TreeNode(name));
            selectedNode.Expand();
        };

        return button;
    }



    private static Button CreateExportButton(List<Reference> references)
    {
        var button = CreateButton("Exportar Template JSON", 360, 620);

        button.Click += (s, e) =>
        {
            using var saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                Title = "Exportar Template JSON"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string json = JsonSerializer.Serialize(references, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(saveFileDialog.FileName, json);
                MessageBox.Show("Template exportado com sucesso!", "Exportação Concluída");
            }
        };

        return button;
    }

    private static Button CreateButton(string text, int left, int top)
    {
        return new Button
        {
            Text = text,
            Left = left,
            Top = top,
            Width = 150,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left
        };
    }
}
