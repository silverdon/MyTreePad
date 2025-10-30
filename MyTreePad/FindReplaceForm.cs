using System;
using System.Windows.Forms;
using System.Drawing;

namespace MyTreePad
{
    internal enum SearchScope
    {
        CurrentNode = 0,
        AllNodes = 1,
        SubNodes = 2
    }

    public partial class FindReplaceForm : Form
    {
        private readonly Form1 mainForm;

        private TextBox? txtFind;
        private TextBox? txtReplace;
        private ComboBox? cmbScope;
        private CheckBox? chkMatchCase;
        private CheckBox? chkWholeWord;
        private CheckBox? chkUseRegex;
        private Button? btnFindNext;
        private Button? btnReplace;
        private Button? btnReplaceAll;
        private Button? btnClose;

        private bool showReplaceControls;

        public FindReplaceForm(Form1 owner, bool showReplace = true)
        {
            mainForm = owner ?? throw new ArgumentNullException(nameof(owner));
            showReplaceControls = showReplace;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Find / Replace";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.ClientSize = new Size(460, 230);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Label lblFind = new Label() { Text = "Find:", Left = 10, Top = 10, Width = 50 };
            txtFind = new TextBox() { Left = 70, Top = 8, Width = 370 };

            Label lblReplace = new Label() { Text = "Replace:", Left = 10, Top = 40, Width = 50 };
            txtReplace = new TextBox() { Left = 70, Top = 38, Width = 370 };

            Label lblScope = new Label() { Text = "Scope:", Left = 10, Top = 72, Width = 50 };
            cmbScope = new ComboBox()
            {
                Left = 70,
                Top = 70,
                Width = 220,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Items = { "Current Node", "All Nodes", "Current Node + Sub-nodes" }
            };
            cmbScope.SelectedIndex = 0;

            chkMatchCase = new CheckBox() { Text = "Match case", Left = 300, Top = 70, Width = 100 };
            chkWholeWord = new CheckBox() { Text = "Whole word", Left = 300, Top = 92, Width = 100 };
            chkUseRegex = new CheckBox() { Text = "Use regex", Left = 300, Top = 114, Width = 100 };

            btnFindNext = new Button() { Text = "Find Next", Left = 10, Top = 150, Width = 95 };
            btnReplace = new Button() { Text = "Replace", Left = 115, Top = 150, Width = 95 };
            btnReplaceAll = new Button() { Text = "Replace All", Left = 220, Top = 150, Width = 95 };
            btnClose = new Button() { Text = "Close", Left = 385, Top = 150, Width = 55 };

            btnFindNext.Click += BtnFindNext_Click;
            btnReplace.Click += BtnReplace_Click;
            btnReplaceAll.Click += BtnReplaceAll_Click;
            btnClose.Click += (s, e) => this.Close();

            // Hide replace controls if opened as Find-only
            txtReplace.Visible = showReplaceControls;
            lblReplace.Visible = showReplaceControls;
            btnReplace.Visible = showReplaceControls;
            btnReplaceAll.Visible = showReplaceControls;

            this.Controls.AddRange(new Control[] {
                lblFind, txtFind, lblReplace, txtReplace, lblScope, cmbScope, chkMatchCase, chkWholeWord, chkUseRegex,
                btnFindNext, btnReplace, btnReplaceAll, btnClose
            });

            // Allow Enter to trigger Find Next
            this.AcceptButton = btnFindNext;
        }

        private void BtnFindNext_Click(object? sender, EventArgs e)
        {
            if (txtFind == null)
            {
                MessageBox.Show("Find textbox is not initialized.", "Find", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string search = txtFind.Text;
            if (string.IsNullOrEmpty(search))
            {
                MessageBox.Show("Enter text to find.", "Find", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var scope = (Form1.SearchScope)cmbScope!.SelectedIndex;
            bool found = mainForm.FindNext(search, scope, chkMatchCase!.Checked, chkWholeWord!.Checked, chkUseRegex!.Checked);
            if (!found) MessageBox.Show($"\"{search}\" not found in the selected scope.", "Find", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnReplace_Click(object? sender, EventArgs e)
        {
            if (txtFind == null || txtReplace == null)
            {
                MessageBox.Show("Find or Replace textbox is not initialized.", "Replace", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string search = txtFind.Text;
            string replace = txtReplace.Text;
            if (string.IsNullOrEmpty(search))
            {
                MessageBox.Show("Enter text to find.", "Replace", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var scope = (Form1.SearchScope)cmbScope!.SelectedIndex;
            bool replaced = mainForm.ReplaceNext(search, replace, scope, chkMatchCase!.Checked, chkWholeWord!.Checked, chkUseRegex!.Checked);
            if (!replaced) MessageBox.Show($"\"{search}\" not found to replace.", "Replace", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnReplaceAll_Click(object? sender, EventArgs e)
        {
            if (txtFind == null || txtReplace == null)
            {
                MessageBox.Show("Find or Replace textbox is not initialized.", "Replace All", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string search = txtFind.Text;
            string replace = txtReplace.Text;
            if (string.IsNullOrEmpty(search))
            {
                MessageBox.Show("Enter text to find.", "Replace All", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var scope = (Form1.SearchScope)cmbScope!.SelectedIndex;
            int count = mainForm.ReplaceAll(search, replace, scope, chkMatchCase!.Checked, chkWholeWord!.Checked, chkUseRegex!.Checked);
            MessageBox.Show($"Replaced {count} occurrence(s).", "Replace All", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}