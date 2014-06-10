namespace MapEditorControlLibrary
{
	partial class AnimationClipViewer
	{
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		/// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region 组件设计器生成的代码

		/// <summary>
		/// 设计器支持所需的方法 - 不要
		/// 使用代码编辑器修改此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
			this.clipName = new System.Windows.Forms.TextBox();
			this.clipMode = new System.Windows.Forms.ComboBox();
			this.deleteBtn = new System.Windows.Forms.Button();
			this.minIndex = new System.Windows.Forms.NumericUpDown();
			this.maxIndex = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.minIndex)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.maxIndex)).BeginInit();
			this.SuspendLayout();
			// 
			// clipName
			// 
			this.clipName.Location = new System.Drawing.Point(3, 4);
			this.clipName.Name = "clipName";
			this.clipName.Size = new System.Drawing.Size(100, 21);
			this.clipName.TabIndex = 0;
			this.clipName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.clipName_KeyPress);
			// 
			// clipMode
			// 
			this.clipMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.clipMode.FormattingEnabled = true;
			this.clipMode.Items.AddRange(new object[] {
            "CLAMP",
            "LOOP",
            "PINGPONG",
            "STOP"});
			this.clipMode.Location = new System.Drawing.Point(196, 4);
			this.clipMode.Name = "clipMode";
			this.clipMode.Size = new System.Drawing.Size(85, 20);
			this.clipMode.TabIndex = 2;
			this.clipMode.SelectedIndexChanged += new System.EventHandler(this.clipMode_SelectedIndexChanged);
			// 
			// deleteBtn
			// 
			this.deleteBtn.Location = new System.Drawing.Point(287, 4);
			this.deleteBtn.Name = "deleteBtn";
			this.deleteBtn.Size = new System.Drawing.Size(23, 21);
			this.deleteBtn.TabIndex = 3;
			this.deleteBtn.Text = "x";
			this.deleteBtn.UseVisualStyleBackColor = true;
			this.deleteBtn.Click += new System.EventHandler(this.deleteBtn_Click);
			// 
			// minIndex
			// 
			this.minIndex.Location = new System.Drawing.Point(109, 3);
			this.minIndex.Name = "minIndex";
			this.minIndex.Size = new System.Drawing.Size(38, 21);
			this.minIndex.TabIndex = 4;
			this.minIndex.ValueChanged += new System.EventHandler(this.minIndex_ValueChanged);
			// 
			// maxIndex
			// 
			this.maxIndex.Location = new System.Drawing.Point(152, 3);
			this.maxIndex.Name = "maxIndex";
			this.maxIndex.Size = new System.Drawing.Size(38, 21);
			this.maxIndex.TabIndex = 4;
			this.maxIndex.ValueChanged += new System.EventHandler(this.maxIndex_ValueChanged);
			// 
			// AnimationClipViewer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this.maxIndex);
			this.Controls.Add(this.minIndex);
			this.Controls.Add(this.deleteBtn);
			this.Controls.Add(this.clipMode);
			this.Controls.Add(this.clipName);
			this.Name = "AnimationClipViewer";
			this.Size = new System.Drawing.Size(313, 28);
			this.Load += new System.EventHandler(this.AnimationClipViewer_Load);
			((System.ComponentModel.ISupportInitialize)(this.minIndex)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.maxIndex)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox clipName;
		private System.Windows.Forms.ComboBox clipMode;
		private System.Windows.Forms.Button deleteBtn;
		private System.Windows.Forms.NumericUpDown minIndex;
		private System.Windows.Forms.NumericUpDown maxIndex;
	}
}
