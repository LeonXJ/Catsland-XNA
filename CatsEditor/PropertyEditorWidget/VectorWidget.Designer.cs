namespace Catsland.CatsEditor.PropertyEditorWidget {
    partial class VectorWidget {
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
            this.xLabel = new System.Windows.Forms.Label();
            this.xValue = new System.Windows.Forms.NumericUpDown();
            this.yValue = new System.Windows.Forms.NumericUpDown();
            this.yLabel = new System.Windows.Forms.Label();
            this.zLabel = new System.Windows.Forms.Label();
            this.zValue = new System.Windows.Forms.NumericUpDown();
            this.wLabel = new System.Windows.Forms.Label();
            this.wValue = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.xValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.wValue)).BeginInit();
            this.SuspendLayout();
            // 
            // xLabel
            // 
            this.xLabel.AutoSize = true;
            this.xLabel.Location = new System.Drawing.Point(5, 5);
            this.xLabel.Name = "xLabel";
            this.xLabel.Size = new System.Drawing.Size(11, 12);
            this.xLabel.TabIndex = 0;
            this.xLabel.Text = "X";
            // 
            // xValue
            // 
            this.xValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.xValue.DecimalPlaces = 3;
            this.xValue.Location = new System.Drawing.Point(20, 3);
            this.xValue.Margin = new System.Windows.Forms.Padding(1);
            this.xValue.Name = "xValue";
            this.xValue.Size = new System.Drawing.Size(135, 21);
            this.xValue.TabIndex = 1;
            this.xValue.ValueChanged += new System.EventHandler(this.Value_ValueChanged);
            // 
            // yValue
            // 
            this.yValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.yValue.DecimalPlaces = 3;
            this.yValue.Location = new System.Drawing.Point(20, 26);
            this.yValue.Margin = new System.Windows.Forms.Padding(1);
            this.yValue.Name = "yValue";
            this.yValue.Size = new System.Drawing.Size(135, 21);
            this.yValue.TabIndex = 3;
            this.yValue.ValueChanged += new System.EventHandler(this.Value_ValueChanged);
            // 
            // yLabel
            // 
            this.yLabel.AutoSize = true;
            this.yLabel.Location = new System.Drawing.Point(5, 28);
            this.yLabel.Name = "yLabel";
            this.yLabel.Size = new System.Drawing.Size(11, 12);
            this.yLabel.TabIndex = 2;
            this.yLabel.Text = "Y";
            // 
            // zLabel
            // 
            this.zLabel.AutoSize = true;
            this.zLabel.Location = new System.Drawing.Point(5, 50);
            this.zLabel.Name = "zLabel";
            this.zLabel.Size = new System.Drawing.Size(11, 12);
            this.zLabel.TabIndex = 0;
            this.zLabel.Text = "Z";
            // 
            // zValue
            // 
            this.zValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.zValue.DecimalPlaces = 3;
            this.zValue.Location = new System.Drawing.Point(20, 48);
            this.zValue.Margin = new System.Windows.Forms.Padding(1);
            this.zValue.Name = "zValue";
            this.zValue.Size = new System.Drawing.Size(135, 21);
            this.zValue.TabIndex = 1;
            this.zValue.ValueChanged += new System.EventHandler(this.Value_ValueChanged);
            // 
            // wLabel
            // 
            this.wLabel.AutoSize = true;
            this.wLabel.Location = new System.Drawing.Point(5, 73);
            this.wLabel.Name = "wLabel";
            this.wLabel.Size = new System.Drawing.Size(11, 12);
            this.wLabel.TabIndex = 2;
            this.wLabel.Text = "W";
            // 
            // wValue
            // 
            this.wValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.wValue.DecimalPlaces = 3;
            this.wValue.Location = new System.Drawing.Point(20, 71);
            this.wValue.Margin = new System.Windows.Forms.Padding(1);
            this.wValue.Name = "wValue";
            this.wValue.Size = new System.Drawing.Size(135, 21);
            this.wValue.TabIndex = 3;
            this.wValue.ValueChanged += new System.EventHandler(this.Value_ValueChanged);
            // 
            // VectorWidget
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.wValue);
            this.Controls.Add(this.wLabel);
            this.Controls.Add(this.yValue);
            this.Controls.Add(this.zValue);
            this.Controls.Add(this.yLabel);
            this.Controls.Add(this.zLabel);
            this.Controls.Add(this.xValue);
            this.Controls.Add(this.xLabel);
            this.Name = "VectorWidget";
            this.Size = new System.Drawing.Size(158, 93);
            ((System.ComponentModel.ISupportInitialize)(this.xValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.wValue)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label xLabel;
        private System.Windows.Forms.NumericUpDown xValue;
        private System.Windows.Forms.NumericUpDown yValue;
        private System.Windows.Forms.Label yLabel;
        private System.Windows.Forms.Label zLabel;
        private System.Windows.Forms.NumericUpDown zValue;
        private System.Windows.Forms.Label wLabel;
        private System.Windows.Forms.NumericUpDown wValue;
    }
}
