namespace Catsland.CatsEditor.PropertyEditorWidget {
    partial class ColorWidget {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent() {
            this.colorBox = new System.Windows.Forms.Button();
            this.alphaBox = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.alphaBox)).BeginInit();
            this.SuspendLayout();
            // 
            // colorBox
            // 
            this.colorBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.colorBox.BackColor = System.Drawing.Color.Black;
            this.colorBox.Location = new System.Drawing.Point(3, 2);
            this.colorBox.Margin = new System.Windows.Forms.Padding(2, 2, 1, 2);
            this.colorBox.Name = "colorBox";
            this.colorBox.Size = new System.Drawing.Size(250, 23);
            this.colorBox.TabIndex = 0;
            this.colorBox.UseVisualStyleBackColor = false;
            this.colorBox.Click += new System.EventHandler(this.colorBox_Click);
            // 
            // alphaBox
            // 
            this.alphaBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.alphaBox.DecimalPlaces = 3;
            this.alphaBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.alphaBox.Location = new System.Drawing.Point(255, 3);
            this.alphaBox.Margin = new System.Windows.Forms.Padding(1, 2, 2, 2);
            this.alphaBox.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.alphaBox.Name = "alphaBox";
            this.alphaBox.Size = new System.Drawing.Size(52, 21);
            this.alphaBox.TabIndex = 1;
            this.alphaBox.Value = new decimal(new int[] {
            10,
            0,
            0,
            65536});
            this.alphaBox.ValueChanged += new System.EventHandler(this.alphaBox_ValueChanged);
            // 
            // ColorWidget
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.alphaBox);
            this.Controls.Add(this.colorBox);
            this.Name = "ColorWidget";
            this.Size = new System.Drawing.Size(310, 28);
            ((System.ComponentModel.ISupportInitialize)(this.alphaBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button colorBox;
        private System.Windows.Forms.NumericUpDown alphaBox;
    }
}
