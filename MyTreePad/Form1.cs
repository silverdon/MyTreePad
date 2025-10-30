using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
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

        // Active editing node visual indicator
        private TreeNode activeEditingNode;
        private Color activeNodePrevBackColor;
        private Color activeNodePrevForeColor;

        // --- new search state fields ---
        private string lastSearchText = null;
        private SearchScope lastSearchScope = SearchScope.CurrentNode;
        private TreeNode lastFoundNode = null;
        private int lastFoundIndex = -1;
        private int lastFoundLength = 0;

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

            // Wire focus events to mark active editing node
            textBoxContent.Enter += TextBoxContent_Enter;
            textBoxContent.Leave += TextBoxContent_Leave;
        }

        /// <summary>
        /// Sets up a blank document with a single root node.
        /// </summary>
        private void InitializeNewDocument()
        {
            // Clear any active indicator first
            ClearActiveEditingNode();

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

            // Update active editing marker if text box is focused
            if (textBoxContent.Focused)
            {
                SetActiveEditingNode(e.Node);
            }
            else
            {
                ClearActiveEditingNode();
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

        // When the user focuses the text area, highlight the selected node as active
        private void TextBoxContent_Enter(object sender, EventArgs e)
        {
            SetActiveEditingNode(treeViewNotes.SelectedNode);
        }

        // When the user leaves the text area, clear the active highlight
        private void TextBoxContent_Leave(object sender, EventArgs e)
        {
            ClearActiveEditingNode();
        }

        #endregion

        #region Active editing marker helpers

        private void SetActiveEditingNode(TreeNode node)
        {
            if (activeEditingNode == node) return;

            // Restore previous first
            ClearActiveEditingNode();

            if (node == null) return;

            // Save previous colors (may be Color.Empty)
            activeNodePrevBackColor = node.BackColor;
            activeNodePrevForeColor = node.ForeColor;

            // Apply highlight (visible while tree is not focused)
            node.BackColor = Color.LightYellow;
            node.ForeColor = Color.Black;

            activeEditingNode = node;
        }

        private void ClearActiveEditingNode()
        {
            if (activeEditingNode != null)
            {
                // Restore previous colors (may be Color.Empty so it inherits)
                activeEditingNode.BackColor = activeNodePrevBackColor;
                activeEditingNode.ForeColor = activeNodePrevForeColor;
                activeEditingNode = null;
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
            // Clear any active indicator first
            ClearActiveEditingNode();

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

        #region Search and Replace Logic

        // Add menu handlers to open find/replace dialog (called from Designer)
        private void Menu_Edit_Find_Click(object sender, EventArgs e)
        {
            using (var dlg = new FindReplaceForm(this, showReplace: false))
            {
                dlg.ShowDialog(this);
            }
        }

        private void Menu_Edit_Replace_Click(object sender, EventArgs e)
        {
            using (var dlg = new FindReplaceForm(this, showReplace: true))
            {
                dlg.ShowDialog(this);
            }
        }

        // Public method used by dialog: find next occurrence
        public bool FindNext(string searchText, SearchScope scope, bool matchCase, bool wholeWord, bool useRegex)
        {
            if (string.IsNullOrEmpty(searchText)) return false;

            bool resetStart = !string.Equals(searchText, lastSearchText, StringComparison.Ordinal) || scope != lastSearchScope;
            lastSearchText = searchText;
            lastSearchScope = scope;

            // Build node list according to scope
            var nodes = new List<TreeNode>(GetNodesForScope(scope));

            if (nodes.Count == 0) return false;

            int startNodeIndex = 0;
            int startCharIndex = 0;

            if (!resetStart && lastFoundNode != null)
            {
                int idx = nodes.IndexOf(lastFoundNode);
                if (idx >= 0)
                {
                    startNodeIndex = idx;
                    startCharIndex = lastFoundIndex + Math.Max(1, lastFoundLength);
                }
                else
                {
                    startNodeIndex = Math.Max(0, nodes.IndexOf(treeViewNotes.SelectedNode));
                    startCharIndex = textBoxContent.SelectionStart;
                }
            }
            else
            {
                int selIndex = Math.Max(0, nodes.IndexOf(treeViewNotes.SelectedNode));
                if (selIndex < 0) selIndex = 0;
                startNodeIndex = selIndex;
                startCharIndex = textBoxContent.SelectionStart;
            }

            StringComparison comp = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            // If wholeWord but not regex, build regex pattern and force regex search
            if (wholeWord && !useRegex)
            {
                useRegex = true;
            }

            // Search loop (wrap-around)
            for (int loop = 0; loop < 2; loop++)
            {
                for (int ni = startNodeIndex; ni < nodes.Count; ni++)
                {
                    string content = (nodes[ni].Tag as string) ?? string.Empty;
                    if (useRegex)
                    {
                        string pattern = searchText;
                        if (wholeWord)
                        {
                            pattern = @"\b" + searchText + @"\b";
                        }
                        var options = RegexOptions.Compiled;
                        if (!matchCase) options |= RegexOptions.IgnoreCase;
                        var match = Regex.Match(content, pattern, options);
                        if (match.Success && match.Index >= startCharIndex)
                        {
                            lastFoundNode = nodes[ni];
                            lastFoundIndex = match.Index;
                            lastFoundLength = match.Length;

                            treeViewNotes.SelectedNode = nodes[ni];
                            textBoxContent.Text = content;
                            textBoxContent.Focus();
                            textBoxContent.SelectionStart = lastFoundIndex;
                            textBoxContent.SelectionLength = lastFoundLength;
                            return true;
                        }
                    }
                    else
                    {
                        int searchFrom = (ni == startNodeIndex) ? startCharIndex : 0;
                        int found = content.IndexOf(searchText, searchFrom, comp);
                        if (found >= 0)
                        {
                            lastFoundNode = nodes[ni];
                            lastFoundIndex = found;
                            lastFoundLength = searchText.Length;

                            treeViewNotes.SelectedNode = nodes[ni];
                            textBoxContent.Text = content;
                            textBoxContent.Focus();
                            textBoxContent.SelectionStart = found;
                            textBoxContent.SelectionLength = searchText.Length;
                            return true;
                        }
                    }
                }
                // wrap
                startNodeIndex = 0;
                startCharIndex = 0;
            }

            // not found
            lastFoundNode = null;
            lastFoundIndex = -1;
            lastFoundLength = 0;
            return false;
        }

        // Replace next: find next occurrence and replace the current occurrence
        public bool ReplaceNext(string searchText, string replaceText, SearchScope scope, bool matchCase, bool wholeWord, bool useRegex)
        {
            bool found = FindNext(searchText, scope, matchCase, wholeWord, useRegex);
            if (!found) return false;

            var node = lastFoundNode;
            if (node == null) return false;

            string content = (node.Tag as string) ?? string.Empty;
            int pos = lastFoundIndex;
            if (pos < 0 || pos > content.Length) return false;

            if (useRegex)
            {
                string pattern = searchText;
                if (wholeWord) pattern = @"\b" + searchText + @"\b";
                var options = RegexOptions.Compiled;
                if (!matchCase) options |= RegexOptions.IgnoreCase;
                var rx = new Regex(pattern, options);
                // Replace only the first occurrence at or after pos
                var match = rx.Match(content, pos);
                if (!match.Success) return false;
                string newContent = content.Substring(0, match.Index) + replaceText + content.Substring(match.Index + match.Length);
                node.Tag = newContent;
                if (treeViewNotes.SelectedNode == node)
                {
                    textBoxContent.Text = newContent;
                    textBoxContent.SelectionStart = match.Index;
                    textBoxContent.SelectionLength = replaceText.Length;
                }
                lastFoundIndex = match.Index + replaceText.Length;
                lastFoundLength = replaceText.Length;
            }
            else
            {
                string newContent = content.Substring(0, pos) + replaceText + content.Substring(pos + searchText.Length);
                node.Tag = newContent;
                if (treeViewNotes.SelectedNode == node)
                {
                    textBoxContent.Text = newContent;
                    textBoxContent.SelectionStart = pos;
                    textBoxContent.SelectionLength = replaceText.Length;
                }
                lastFoundIndex = pos + replaceText.Length;
                lastFoundLength = replaceText.Length;
            }

            SetDirty(true);
            return true;
        }

        // Replace all matches in scope, returns total replacements
        public int ReplaceAll(string searchText, string replaceText, SearchScope scope, bool matchCase, bool wholeWord, bool useRegex)
        {
            if (string.IsNullOrEmpty(searchText)) return 0;

            var nodes = new List<TreeNode>(GetNodesForScope(scope));
            int total = 0;

            bool effectiveRegex = useRegex || wholeWord;
            string pattern = searchText;
            Regex rx = null;
            RegexOptions options = RegexOptions.Compiled;
            if (!matchCase) options |= RegexOptions.IgnoreCase;
            if (effectiveRegex)
            {
                if (wholeWord) pattern = @"\b" + searchText + @"\b";
                rx = new Regex(pattern, options);
            }

            foreach (var node in nodes)
            {
                string content = (node.Tag as string) ?? string.Empty;
                if (effectiveRegex)
                {
                    var matches = rx.Matches(content);
                    if (matches.Count > 0)
                    {
                        content = rx.Replace(content, replaceText);
                        node.Tag = content;
                        if (treeViewNotes.SelectedNode == node) textBoxContent.Text = content;
                        total += matches.Count;
                    }
                }
                else
                {
                    int countBefore = CountOccurrences(content, searchText, matchCase);
                    if (countBefore > 0)
                    {
                        content = matchCase ? content.Replace(searchText, replaceText)
                                             : Regex.Replace(content, Regex.Escape(searchText), replaceText, RegexOptions.IgnoreCase);
                        node.Tag = content;
                        if (treeViewNotes.SelectedNode == node) textBoxContent.Text = content;
                        total += countBefore;
                    }
                }
            }

            if (total > 0) SetDirty(true);

            lastFoundNode = null;
            lastFoundIndex = -1;
            lastFoundLength = 0;
            return total;
        }

        // Helper: count occurrences (case-sensitive or insensitive)
        private int CountOccurrences(string text, string pattern, bool caseSensitive)
        {
            if (string.IsNullOrEmpty(pattern)) return 0;
            int count = 0;
            int idx = 0;
            StringComparison comp = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            while ((idx = text.IndexOf(pattern, idx, comp)) >= 0)
            {
                count++;
                idx += pattern.Length;
            }
            return count;
        }

        // Build node enumeration based on scope (corrected implementation)
        private IEnumerable<TreeNode> GetNodesForScope(SearchScope scope)
        {
            if (scope == SearchScope.CurrentNode)
            {
                if (treeViewNotes.SelectedNode != null) yield return treeViewNotes.SelectedNode;
                yield break;
            }
            else if (scope == SearchScope.AllNodes)
            {
                foreach (var n in PreOrder(treeViewNotes.Nodes)) yield return n;
                yield break;
            }
            else // SubNodes
            {
                if (treeViewNotes.SelectedNode != null)
                {
                    yield return treeViewNotes.SelectedNode;
                    foreach (var n in PreOrder(treeViewNotes.SelectedNode.Nodes))
                        yield return n;
                    yield break;
                }
                else
                {
                    foreach (var n in PreOrder(treeViewNotes.Nodes)) yield return n;
                    yield break;
                }
            }
        }

        // Pre-order traversal helper
        private IEnumerable<TreeNode> PreOrder(TreeNodeCollection nodes)
        {
            foreach (TreeNode n in nodes)
            {
                yield return n;
                foreach (var child in PreOrder(n.Nodes))
                    yield return child;
            }
        }

        // Make the SearchScope enum public so it matches the accessibility of Form1's public methods

        public enum SearchScope
        {
            CurrentNode = 0,
            AllNodes = 1,
            SubNodes = 2
        }
        #endregion
    }
}