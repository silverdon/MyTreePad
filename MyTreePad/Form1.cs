using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

#nullable disable // Add this line to prevent the null warnings

namespace MyTreePad
{
    public partial class Form1 : Form
    {
        // File handling
        private string currentFilePath = null;
        private const string HJT_HEADER = "<Treepad version 3.0>";
        private const string HJT_NODE_END = "<end node> 5P9i0s8y19Z";
        private const string HJT_NODE_TYPE = "dt=Text";

        // Fonts for UI (added to allow changing sizes)
        private Font treeFont;
        private Font contentFont;

        // In-memory clipboard for a node subtree (not persisted to system clipboard)
        private TreeNode clipboardNode;

        // Track whether there are unsaved changes
        private bool isDirty = false;

        public Form1()
        {
            // This method is defined in Form1.Designer.cs
            InitializeComponent();

            // Set up our new document
            InitializeNewDocument();

            // Initialize font fields with current control fonts
            treeFont = treeViewNotes.Font;
            contentFont = textBoxContent.Font;

            // Wire up events not already wired in designer
            treeViewNotes.AfterLabelEdit += TreeViewNotes_AfterLabelEdit;
            this.FormClosing += Form1_FormClosing;
        }

        /// <summary>
        /// Sets up a blank document with a single root node.
        /// </summary>
        private void InitializeNewDocument()
        {
            treeViewNotes.Nodes.Clear();
            textBoxContent.Clear();
            TreeNode rootNode = new TreeNode("My Document");
            rootNode.Tag = "This is the root node.";
            treeViewNotes.Nodes.Add(rootNode);
            treeViewNotes.SelectedNode = rootNode;
            currentFilePath = null;
            SetDirty(false);
            RefreshTitle();
        }

        #region Dirty / Title helpers

        private void SetDirty(bool value)
        {
            isDirty = value;
            RefreshTitle();
        }

        private void RefreshTitle()
        {
            string fileName = string.IsNullOrEmpty(currentFilePath) ? "New Document" : Path.GetFileName(currentFilePath);
            this.Text = $"MyTreePad - {fileName}" + (isDirty ? " *" : "");
        }

        /// <summary>
        /// If there are unsaved changes prompt the user to save. Returns true to continue the pending action.
        /// </summary>
        private bool PromptSaveIfDirty()
        {
            if (!isDirty) return true;

            var result = MessageBox.Show("The current document has unsaved changes. Do you want to save them?",
                "Save Changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                Menu_Save_Click(this, EventArgs.Empty);
                // If the save did not clear dirty (user cancelled Save As), abort
                return !isDirty;
            }
            else if (result == DialogResult.No)
            {
                return true;
            }
            else // Cancel
            {
                return false;
            }
        }

        #endregion

        #region Event Handlers (Linking UI together)

        /// <summary>
        /// When the user types in the text box, save the text
        /// back to the currently selected node's Tag property.
        /// </summary>
        private void TextBoxContent_TextChanged(object sender, EventArgs e)
        {
            if (treeViewNotes.SelectedNode != null)
            {
                treeViewNotes.SelectedNode.Tag = textBoxContent.Text;
                SetDirty(true);
            }
        }

        /// <summary>
        /// When the user clicks a new node in the tree, load
        /// its content (from the Tag) into the text box.
        /// </summary>
        private void TreeViewNotes_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null && e.Node.Tag != null)
            {
                textBoxContent.Text = e.Node.Tag.ToString();
            }
            else
            {
                textBoxContent.Text = string.Empty;
            }
        }

        // Ensure right-click selects the node before showing context menu
        private void TreeViewNotes_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node != null)
            {
                treeViewNotes.SelectedNode = e.Node;
            }
        }

        // Mark dirty after label edit
        private void TreeViewNotes_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Label != null)
            {
                SetDirty(true);
            }
        }

        #endregion

        #region Menu Bar Logic (File > New, Open, Save)

        private void Menu_New_Click(object sender, EventArgs e)
        {
            if (!PromptSaveIfDirty()) return;
            InitializeNewDocument();
        }

        private void Menu_Open_Click(object sender, EventArgs e)
        {
            if (!PromptSaveIfDirty()) return;

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "HJT Files (*.hjt)|*.hjt|All Files (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        LoadFromFile(ofd.FileName);
                        currentFilePath = ofd.FileName;
                        SetDirty(false);
                        RefreshTitle();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error opening file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void Menu_Save_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                Menu_SaveAs_Click(sender, e); // Go to Save As if no path
            }
            else
            {
                try
                {
                    SaveToFile(currentFilePath);
                    SetDirty(false);
                    RefreshTitle();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Menu_SaveAs_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "HJT Files (*.hjt)|*.hjt|All Files (*.*)|*.*";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        SaveToFile(sfd.FileName);
                        currentFilePath = sfd.FileName;
                        SetDirty(false);
                        RefreshTitle();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // Intercept form closing to prompt to save if needed
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!PromptSaveIfDirty())
            {
                e.Cancel = true;
            }
        }

        #endregion

        #region View -> Font Change Handlers (added)

        private void Menu_View_TreeFont_Click(object sender, EventArgs e)
        {
            using (FontDialog fd = new FontDialog())
            {
                fd.Font = treeFont;
                fd.ShowColor = false;
                if (fd.ShowDialog() == DialogResult.OK)
                {
                    treeFont = fd.Font;
                    ApplyTreeFont();
                }
            }
        }

        private void Menu_View_ContentFont_Click(object sender, EventArgs e)
        {
            using (FontDialog fd = new FontDialog())
            {
                fd.Font = contentFont;
                fd.ShowColor = false;
                if (fd.ShowDialog() == DialogResult.OK)
                {
                    contentFont = fd.Font;
                    ApplyContentFont();
                }
            }
        }

        private void ApplyTreeFont()
        {
            if (treeViewNotes != null && treeFont != null)
            {
                treeViewNotes.Font = treeFont;
                // TreeView may need a layout refresh when font changes
                treeViewNotes.Refresh();
            }
        }

        private void ApplyContentFont()
        {
            if (textBoxContent != null && contentFont != null)
            {
                textBoxContent.Font = contentFont;
                textBoxContent.Refresh();
            }
        }

        #endregion

        #region Context Menu Logic (Right-click) - expanded with expand/collapse

        private void Context_AddChild_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeViewNotes.SelectedNode;
            if (selectedNode == null)
            {
                // If no node is selected, add to the root
                if (treeViewNotes.Nodes.Count == 0) InitializeNewDocument();
                selectedNode = treeViewNotes.Nodes[0];
            }

            TreeNode newNode = selectedNode.Nodes.Add("New Node");
            newNode.Tag = "";
            selectedNode.Expand();
            treeViewNotes.SelectedNode = newNode;
            newNode.BeginEdit(); // Allow user to rename
            SetDirty(true);
        }

        private void Context_AddSibling_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeViewNotes.SelectedNode;
            if (selectedNode == null)
            {
                Context_AddChild_Click(sender, e); // Default to adding a child
                return;
            }

            TreeNode parent = selectedNode.Parent;
            TreeNodeCollection collection = (parent == null) ? treeViewNotes.Nodes : parent.Nodes;

            TreeNode newNode = new TreeNode("New Node");
            newNode.Tag = "";
            collection.Insert(selectedNode.Index + 1, newNode);
            treeViewNotes.SelectedNode = newNode;
            newNode.BeginEdit();
            SetDirty(true);
        }

        private void Context_DeleteNode_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeViewNotes.SelectedNode;
            if (selectedNode != null)
            {
                if (MessageBox.Show($"Are you sure you want to delete '{selectedNode.Text}' and all its children?",
                    "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    selectedNode.Remove();
                    textBoxContent.Clear();
                    SetDirty(true);
                }
            }
        }

        // Copy selected node (including subtree) into in-memory clipboard
        private void Context_Copy_Click(object sender, EventArgs e)
        {
            TreeNode sel = treeViewNotes.SelectedNode;
            if (sel != null)
            {
                clipboardNode = (TreeNode)sel.Clone();
            }
        }

        // Paste clipboard subtree as child of selected node (or as root if no selection)
        private void Context_Paste_Click(object sender, EventArgs e)
        {
            if (clipboardNode == null) return;

            TreeNode target = treeViewNotes.SelectedNode;
            TreeNode toInsert = (TreeNode)clipboardNode.Clone();

            if (target == null)
            {
                // Paste as new root node
                treeViewNotes.Nodes.Add(toInsert);
                treeViewNotes.SelectedNode = toInsert;
            }
            else
            {
                // Paste as child of the selected node
                target.Nodes.Add(toInsert);
                target.Expand();
                treeViewNotes.SelectedNode = toInsert;
            }

            SetDirty(true);
        }

        // Expand selected node (single-level)
        private void Context_Expand_Click(object sender, EventArgs e)
        {
            var sel = treeViewNotes.SelectedNode;
            if (sel != null)
            {
                sel.Expand();
            }
        }

        // Collapse selected node (single-level)
        private void Context_Collapse_Click(object sender, EventArgs e)
        {
            var sel = treeViewNotes.SelectedNode;
            if (sel != null)
            {
                sel.Collapse();
            }
        }

        // Expand all children of selected node or whole tree if no selection
        private void Context_ExpandAll_Click(object sender, EventArgs e)
        {
            var sel = treeViewNotes.SelectedNode;
            if (sel != null)
            {
                sel.ExpandAll();
                sel.EnsureVisible();
            }
            else
            {
                treeViewNotes.ExpandAll();
            }
        }

        // Collapse all children of selected node or whole tree if no selection
        private void Context_CollapseAll_Click(object sender, EventArgs e)
        {
            var sel = treeViewNotes.SelectedNode;
            if (sel != null)
            {
                CollapseRecursive(sel);
            }
            else
            {
                foreach (TreeNode n in treeViewNotes.Nodes)
                {
                    CollapseRecursive(n);
                }
            }
        }

        private void CollapseRecursive(TreeNode node)
        {
            foreach (TreeNode child in node.Nodes)
            {
                CollapseRecursive(child);
            }
            node.Collapse();
        }

        // Enable/disable context menu items based on state
        private void TreeContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool hasSelection = treeViewNotes.SelectedNode != null;
            copyNodeToolStripMenuItem.Enabled = hasSelection;
            pasteNodeToolStripMenuItem.Enabled = clipboardNode != null;

            expandNodeToolStripMenuItem.Enabled = hasSelection && !treeViewNotes.SelectedNode.IsExpanded;
            collapseNodeToolStripMenuItem.Enabled = hasSelection && treeViewNotes.SelectedNode.IsExpanded;

            expandAllToolStripMenuItem.Enabled = treeViewNotes.Nodes.Count > 0;
            collapseAllToolStripMenuItem.Enabled = AnyNodeExpanded(treeViewNotes.Nodes);
        }

        private bool AnyNodeExpanded(TreeNodeCollection nodes)
        {
            foreach (TreeNode n in nodes)
            {
                if (n.IsExpanded) return true;
                if (AnyNodeExpanded(n.Nodes)) return true;
            }
            return false;
        }

        #endregion

        #region File I/O Logic (Parsing and Saving HJT Format)

        /// <summary>
        /// Saves the current TreeView structure to the HJT file format.
        /// </summary>
        private void SaveToFile(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.Default))
            {
                writer.WriteLine(HJT_HEADER);
                foreach (TreeNode node in treeViewNotes.Nodes)
                {
                    RecursiveSaveNode(node, writer);
                }
            }

            // After successful write mark as saved
            SetDirty(false);
        }

        /// <summary>
        /// A recursive helper function to save a node and all its children.
        /// </summary>
        private void RecursiveSaveNode(TreeNode node, StreamWriter writer)
        {
            string title = node.Text;
            int level = node.Level;
            string content = (node.Tag as string) ?? string.Empty;

            // Write the node block based on the provided file format
            writer.WriteLine(HJT_NODE_TYPE);
            writer.WriteLine("<node>");
            writer.WriteLine(title);
            writer.WriteLine(level);

            if (!string.IsNullOrEmpty(content))
            {
                writer.Write(content);
                if (!content.EndsWith("\r\n") && !content.EndsWith("\n"))
                {
                    writer.WriteLine();
                }
            }

            writer.WriteLine(HJT_NODE_END);

            // Recurse for all children
            foreach (TreeNode child in node.Nodes)
            {
                RecursiveSaveNode(child, writer);
            }
        }

        /// <summary>
        /// Loads an HJT file into the TreeView.
        /// </summary>
        private void LoadFromFile(string filePath)
        {
            treeViewNotes.Nodes.Clear();
            textBoxContent.Clear();

            using (StreamReader reader = new StreamReader(filePath, Encoding.Default))
            {
                string header = reader.ReadLine();
                if (header != HJT_HEADER)
                {
                    throw new Exception("Not a valid HJT file.");
                }

                Dictionary<int, TreeNode> levelMap = new Dictionary<int, TreeNode>();

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    if (line == HJT_NODE_TYPE)
                    {
                        ParseNodeBlock(reader, levelMap);
                    }
                }
            }
            treeViewNotes.ExpandAll();
            if (treeViewNotes.Nodes.Count > 0)
            {
                treeViewNotes.SelectedNode = treeViewNotes.Nodes[0];
            }

            SetDirty(false);
        }

        /// <summary>
        /// A helper function to parse a single <node> block from the file.
        /// </summary>
        private void ParseNodeBlock(StreamReader reader, Dictionary<int, TreeNode> levelMap)
        {
            string nodeTag = reader.ReadLine(); // Should be "<node>"
            if (nodeTag != "<node>") return;

            string title = reader.ReadLine();
            int level = int.Parse(reader.ReadLine());

            StringBuilder content = new StringBuilder();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith(HJT_NODE_END))
                {
                    break;
                }
                content.AppendLine(line);
            }

            TreeNode newNode = new TreeNode(title);
            newNode.Tag = content.ToString().TrimEnd(new char[] { '\r', '\n' });

            if (level == 0)
            {
                treeViewNotes.Nodes.Add(newNode);
            }
            else
            {
                TreeNode parent = levelMap[level - 1];
                parent.Nodes.Add(newNode);
            }

            levelMap[level] = newNode;
        }

        #endregion
    }
}