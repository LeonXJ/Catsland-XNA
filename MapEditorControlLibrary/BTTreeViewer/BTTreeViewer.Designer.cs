namespace Catsland.MapEditorControlLibrary {
    partial class BTTreeViewer {
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
            this.SuspendLayout();
            // 
            // BTTreeViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.DoubleBuffered = true;
            this.Name = "BTTreeViewer";
            this.Size = new System.Drawing.Size(649, 298);
            this.Scroll += new System.Windows.Forms.ScrollEventHandler(this.BTTreeViewer_Scroll);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BTTreeViewer_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.BTTreeViewer_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BTTreeViewer_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
