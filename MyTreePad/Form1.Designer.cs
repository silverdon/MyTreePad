namespace MyTreePad
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeFontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contentFontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.treeViewNotes = new System.Windows.Forms.TreeView();
            this.treeContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expandNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collapseNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expandAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collapseAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addChildNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addSiblingNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textBoxContent = new System.Windows.Forms.TextBox();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.treeContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(800, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.newToolStripMenuItem.Text = "&New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.Menu_New_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openToolStripMenuItem.Text = "&Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.Menu_Open_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.Menu_Save_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveAsToolStripMenuItem.Text = "Save &As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.Menu_SaveAs_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.treeFontToolStripMenuItem,
            this.contentFontToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // treeFontToolStripMenuItem
            // 
            this.treeFontToolStripMenuItem.Name = "treeFontToolStripMenuItem";
            this.treeFontToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.treeFontToolStripMenuItem.Text = "Tree &Font...";
            this.treeFontToolStripMenuItem.Click += new System.EventHandler(this.Menu_View_TreeFont_Click);
            // 
            // contentFontToolStripMenuItem
            // 
            this.contentFontToolStripMenuItem.Name = "contentFontToolStripMenuItem";
            this.contentFontToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.contentFontToolStripMenuItem.Text = "&Content Font...";
            this.contentFontToolStripMenuItem.Click += new System.EventHandler(this.Menu_View_ContentFont_Click);
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 24);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.treeViewNotes);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.textBoxContent);
            this.splitContainer.Size = new System.Drawing.Size(800, 426);
            this.splitContainer.SplitterDistance = 266;
            this.splitContainer.TabIndex = 1;
            // 
            // treeViewNotes
            // 
            this.treeViewNotes.ContextMenuStrip = this.treeContextMenu;
            this.treeViewNotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewNotes.LabelEdit = true;
            this.treeViewNotes.Location = new System.Drawing.Point(0, 0);
            this.treeViewNotes.Name = "treeViewNotes";
            this.treeViewNotes.Size = new System.Drawing.Size(266, 426);
            this.treeViewNotes.TabIndex = 0;
            this.treeViewNotes.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeViewNotes_AfterSelect);
            this.treeViewNotes.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeViewNotes_NodeMouseClick);
            // 
            // treeContextMenu
            // 
            this.treeContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyNodeToolStripMenuItem,
            this.pasteNodeToolStripMenuItem,
            this.expandNodeToolStripMenuItem,
            this.collapseNodeToolStripMenuItem,
            this.expandAllToolStripMenuItem,
            this.collapseAllToolStripMenuItem,
            this.addChildNodeToolStripMenuItem,
            this.addSiblingNodeToolStripMenuItem,
            this.toolStripSeparator1,
            this.deleteNodeToolStripMenuItem});
            this.treeContextMenu.Name = "treeContextMenu";
            this.treeContextMenu.Size = new System.Drawing.Size(200, 220);
            this.treeContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.TreeContextMenu_Opening);
            // 
            // copyNodeToolStripMenuItem
            // 
            this.copyNodeToolStripMenuItem.Name = "copyNodeToolStripMenuItem";
            this.copyNodeToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.copyNodeToolStripMenuItem.Text = "Copy Node";
            this.copyNodeToolStripMenuItem.Click += new System.EventHandler(this.Context_Copy_Click);
            // 
            // pasteNodeToolStripMenuItem
            // 
            this.pasteNodeToolStripMenuItem.Name = "pasteNodeToolStripMenuItem";
            this.pasteNodeToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.pasteNodeToolStripMenuItem.Text = "Paste Node";
            this.pasteNodeToolStripMenuItem.Click += new System.EventHandler(this.Context_Paste_Click);
            // 
            // expandNodeToolStripMenuItem
            // 
            this.expandNodeToolStripMenuItem.Name = "expandNodeToolStripMenuItem";
            this.expandNodeToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.expandNodeToolStripMenuItem.Text = "Expand Node";
            this.expandNodeToolStripMenuItem.Click += new System.EventHandler(this.Context_Expand_Click);
            // 
            // collapseNodeToolStripMenuItem
            // 
            this.collapseNodeToolStripMenuItem.Name = "collapseNodeToolStripMenuItem";
            this.collapseNodeToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.collapseNodeToolStripMenuItem.Text = "Collapse Node";
            this.collapseNodeToolStripMenuItem.Click += new System.EventHandler(this.Context_Collapse_Click);
            // 
            // expandAllToolStripMenuItem
            // 
            this.expandAllToolStripMenuItem.Name = "expandAllToolStripMenuItem";
            this.expandAllToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.expandAllToolStripMenuItem.Text = "Expand All";
            this.expandAllToolStripMenuItem.Click += new System.EventHandler(this.Context_ExpandAll_Click);
            // 
            // collapseAllToolStripMenuItem
            // 
            this.collapseAllToolStripMenuItem.Name = "collapseAllToolStripMenuItem";
            this.collapseAllToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.collapseAllToolStripMenuItem.Text = "Collapse All";
            this.collapseAllToolStripMenuItem.Click += new System.EventHandler(this.Context_CollapseAll_Click);
            // 
            // addChildNodeToolStripMenuItem
            // 
            this.addChildNodeToolStripMenuItem.Name = "addChildNodeToolStripMenuItem";
            this.addChildNodeToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.addChildNodeToolStripMenuItem.Text = "Add Child Node";
            this.addChildNodeToolStripMenuItem.Click += new System.EventHandler(this.Context_AddChild_Click);
            // 
            // addSiblingNodeToolStripMenuItem
            // 
            this.addSiblingNodeToolStripMenuItem.Name = "addSiblingNodeToolStripMenuItem";
            this.addSiblingNodeToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.addSiblingNodeToolStripMenuItem.Text = "Add Sibling Node";
            this.addSiblingNodeToolStripMenuItem.Click += new System.EventHandler(this.Context_AddSibling_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(196, 6);
            // 
            // deleteNodeToolStripMenuItem
            // 
            this.deleteNodeToolStripMenuItem.Name = "deleteNodeToolStripMenuItem";
            this.deleteNodeToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.deleteNodeToolStripMenuItem.Text = "Delete Node";
            this.deleteNodeToolStripMenuItem.Click += new System.EventHandler(this.Context_DeleteNode_Click);
            // 
            // textBoxContent
            // 
            this.textBoxContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxContent.Location = new System.Drawing.Point(0, 0);
            this.textBoxContent.Multiline = true;
            this.textBoxContent.Name = "textBoxContent";
            this.textBoxContent.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxContent.Size = new System.Drawing.Size(530, 426);
            this.textBoxContent.TabIndex = 0;
            this.textBoxContent.TextChanged += new System.EventHandler(this.TextBoxContent_TextChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "Form1";
            this.Text = "MyTreePad";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.treeContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        // These are the declarations for the controls.
        // The logic is in Form1.cs
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem treeFontToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem contentFontToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TreeView treeViewNotes;
        private System.Windows.Forms.TextBox textBoxContent;
        private System.Windows.Forms.ContextMenuStrip treeContextMenu;
        private System.Windows.Forms.ToolStripMenuItem copyNodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteNodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandNodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem collapseNodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem collapseAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addChildNodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addSiblingNodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem deleteNodeToolStripMenuItem;
    }
}