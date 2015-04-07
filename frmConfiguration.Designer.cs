namespace FolderMonitor
{
    partial class frmConfiguration
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmConfiguration));
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.cmdApply = new System.Windows.Forms.Button();
            this.lvwMonitorItems = new System.Windows.Forms.ListView();
            this.tsEditItems = new System.Windows.Forms.ToolStrip();
            this.tsNewItem = new System.Windows.Forms.ToolStripButton();
            this.tsEdit = new System.Windows.Forms.ToolStripButton();
            this.tsRemove = new System.Windows.Forms.ToolStripButton();
            this.button1 = new System.Windows.Forms.Button();
            this.tsEditItems.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.Location = new System.Drawing.Point(206, 360);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(85, 33);
            this.cmdCancel.TabIndex = 0;
            this.cmdCancel.Text = "&Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOK.Location = new System.Drawing.Point(388, 360);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(85, 33);
            this.cmdOK.TabIndex = 1;
            this.cmdOK.Text = "&OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // cmdApply
            // 
            this.cmdApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdApply.Location = new System.Drawing.Point(297, 360);
            this.cmdApply.Name = "cmdApply";
            this.cmdApply.Size = new System.Drawing.Size(85, 33);
            this.cmdApply.TabIndex = 2;
            this.cmdApply.Text = "&Save";
            this.cmdApply.UseVisualStyleBackColor = true;
            this.cmdApply.Click += new System.EventHandler(this.cmdApply_Click);
            // 
            // lvwMonitorItems
            // 
            this.lvwMonitorItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwMonitorItems.FullRowSelect = true;
            this.lvwMonitorItems.GridLines = true;
            this.lvwMonitorItems.Location = new System.Drawing.Point(12, 115);
            this.lvwMonitorItems.MultiSelect = false;
            this.lvwMonitorItems.Name = "lvwMonitorItems";
            this.lvwMonitorItems.Size = new System.Drawing.Size(461, 238);
            this.lvwMonitorItems.TabIndex = 3;
            this.lvwMonitorItems.UseCompatibleStateImageBehavior = false;
            this.lvwMonitorItems.View = System.Windows.Forms.View.Details;
            this.lvwMonitorItems.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.lvwMonitorItems_ItemCheck);
            this.lvwMonitorItems.SelectedIndexChanged += new System.EventHandler(this.lvwMonitorItems_SelectedIndexChanged);
            // 
            // tsEditItems
            // 
            this.tsEditItems.Dock = System.Windows.Forms.DockStyle.None;
            this.tsEditItems.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsEditItems.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsNewItem,
            this.tsEdit,
            this.tsRemove});
            this.tsEditItems.Location = new System.Drawing.Point(12, 87);
            this.tsEditItems.Name = "tsEditItems";
            this.tsEditItems.Size = new System.Drawing.Size(179, 27);
            this.tsEditItems.TabIndex = 4;
            this.tsEditItems.Text = "toolStrip1";
            // 
            // tsNewItem
            // 
            this.tsNewItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsNewItem.Image = ((System.Drawing.Image)(resources.GetObject("tsNewItem.Image")));
            this.tsNewItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsNewItem.Name = "tsNewItem";
            this.tsNewItem.Size = new System.Drawing.Size(52, 24);
            this.tsNewItem.Text = "New...";
            this.tsNewItem.Click += new System.EventHandler(this.tsNewItem_Click);
            // 
            // tsEdit
            // 
            this.tsEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsEdit.Enabled = false;
            this.tsEdit.Image = ((System.Drawing.Image)(resources.GetObject("tsEdit.Image")));
            this.tsEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsEdit.Name = "tsEdit";
            this.tsEdit.Size = new System.Drawing.Size(48, 24);
            this.tsEdit.Text = "Edit...";
            this.tsEdit.Click += new System.EventHandler(this.tsEdit_Click);
            // 
            // tsRemove
            // 
            this.tsRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsRemove.Enabled = false;
            this.tsRemove.Image = ((System.Drawing.Image)(resources.GetObject("tsRemove.Image")));
            this.tsRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsRemove.Name = "tsRemove";
            this.tsRemove.Size = new System.Drawing.Size(67, 24);
            this.tsRemove.Text = "Remove";
            this.tsRemove.Click += new System.EventHandler(this.tsRemove_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(348, 25);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // frmConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(485, 405);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tsEditItems);
            this.Controls.Add(this.lvwMonitorItems);
            this.Controls.Add(this.cmdApply);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.cmdCancel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmConfiguration";
            this.Text = "Folder Monitor Configuration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmConfiguration_FormClosing);
            this.Load += new System.EventHandler(this.frmConfiguration_Load);
            this.Shown += new System.EventHandler(this.frmConfiguration_Shown);
            this.VisibleChanged += new System.EventHandler(this.frmConfiguration_VisibleChanged);
            this.tsEditItems.ResumeLayout(false);
            this.tsEditItems.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Button cmdApply;
        private System.Windows.Forms.ListView lvwMonitorItems;
        private System.Windows.Forms.ToolStrip tsEditItems;
        private System.Windows.Forms.ToolStripButton tsNewItem;
        private System.Windows.Forms.ToolStripButton tsEdit;
        private System.Windows.Forms.ToolStripButton tsRemove;
        private System.Windows.Forms.Button button1;
    }
}

