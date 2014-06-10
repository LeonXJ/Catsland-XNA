namespace Catsland.CatsEditor.PropertyEditorWidget {
    partial class TextureWidget {
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
            this.textureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.textureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // textureBox
            // 
            this.textureBox.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.textureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.textureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textureBox.Location = new System.Drawing.Point(0, 0);
            this.textureBox.Name = "textureBox";
            this.textureBox.Size = new System.Drawing.Size(168, 162);
            this.textureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.textureBox.TabIndex = 0;
            this.textureBox.TabStop = false;
            this.textureBox.Click += new System.EventHandler(this.textureBox_Click);
            // 
            // TextureWidget
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textureBox);
            this.Name = "TextureWidget";
            this.Size = new System.Drawing.Size(168, 162);
            ((System.ComponentModel.ISupportInitialize)(this.textureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox textureBox;
    }
}
