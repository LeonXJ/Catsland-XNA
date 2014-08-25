namespace Catsland.Editor {
    partial class BTTreeEditor {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BTTreeEditor));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btTreeViewer = new Catsland.MapEditorControlLibrary.BTTreeViewer();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.propertyEditor = new System.Windows.Forms.PropertyGrid();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuInsert = new System.Windows.Forms.ToolStripMenuItem();
            this.menuInsertCompositeNode = new System.Windows.Forms.ToolStripMenuItem();
            this.menuInsertConditionNode = new System.Windows.Forms.ToolStripMenuItem();
            this.menuInsertActionNode = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 409);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1036, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 25);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1036, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 50);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btTreeViewer);
            this.splitContainer1.Panel1.Controls.Add(this.splitter1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.propertyEditor);
            this.splitContainer1.Size = new System.Drawing.Size(1036, 359);
            this.splitContainer1.SplitterDistance = 810;
            this.splitContainer1.TabIndex = 2;
            // 
            // btTreeViewer
            // 
            this.btTreeViewer.AutoScroll = true;
            this.btTreeViewer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.btTreeViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btTreeViewer.Location = new System.Drawing.Point(175, 0);
            this.btTreeViewer.Name = "btTreeViewer";
            this.btTreeViewer.Size = new System.Drawing.Size(635, 359);
            this.btTreeViewer.TabIndex = 1;
            this.btTreeViewer.OnBTNodeSelected += new System.EventHandler<Catsland.MapEditorControlLibrary.BTNodeSelectedArgs>(this.btTreeViewer_OnBTNodeSelected);
            this.btTreeViewer.OnBTNodeDeselected += new System.EventHandler<Catsland.MapEditorControlLibrary.BTNodeSelectedArgs>(this.btTreeViewer_OnBTNodeDeselected);
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(0, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(175, 359);
            this.splitter1.TabIndex = 0;
            this.splitter1.TabStop = false;
            // 
            // propertyEditor
            // 
            this.propertyEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyEditor.Location = new System.Drawing.Point(0, 0);
            this.propertyEditor.Name = "propertyEditor";
            this.propertyEditor.Size = new System.Drawing.Size(222, 359);
            this.propertyEditor.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuInsert});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1036, 25);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuInsert
            // 
            this.menuInsert.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuInsertCompositeNode,
            this.menuInsertConditionNode,
            this.menuInsertActionNode});
            this.menuInsert.Name = "menuInsert";
            this.menuInsert.Size = new System.Drawing.Size(53, 21);
            this.menuInsert.Text = "Insert";
            // 
            // menuInsertCompositeNode
            // 
            this.menuInsertCompositeNode.Name = "menuInsertCompositeNode";
            this.menuInsertCompositeNode.Size = new System.Drawing.Size(176, 22);
            this.menuInsertCompositeNode.Text = "Composite Node";
            // 
            // menuInsertConditionNode
            // 
            this.menuInsertConditionNode.Name = "menuInsertConditionNode";
            this.menuInsertConditionNode.Size = new System.Drawing.Size(176, 22);
            this.menuInsertConditionNode.Text = "Condition Node";
            // 
            // menuInsertActionNode
            // 
            this.menuInsertActionNode.Name = "menuInsertActionNode";
            this.menuInsertActionNode.Size = new System.Drawing.Size(176, 22);
            this.menuInsertActionNode.Text = "Action Node";
            // 
            // BTTreeEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1036, 431);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "BTTreeEditor";
            this.Text = "BTTreeEditor";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.PropertyGrid propertyEditor;
        private System.Windows.Forms.Splitter splitter1;
        private MapEditorControlLibrary.BTTreeViewer btTreeViewer;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuInsert;
        private System.Windows.Forms.ToolStripMenuItem menuInsertCompositeNode;
        private System.Windows.Forms.ToolStripMenuItem menuInsertConditionNode;
        private System.Windows.Forms.ToolStripMenuItem menuInsertActionNode;

    }
}