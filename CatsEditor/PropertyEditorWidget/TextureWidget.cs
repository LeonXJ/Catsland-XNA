using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Catsland.Core;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Catsland.CatsEditor.PropertyEditorWidget {
    public partial class TextureWidget : UserControl {
        public TextureWidget() {
            InitializeComponent();
        }

        public void SetObserve(CatTexture _texture) {
            m_texture = _texture;
            // test
            if (_texture.value == null) {
                textureBox.Image = new Bitmap(1, 1);
                return;
            }

            // copy from texture2D to bitmap
            int srcWidth = _texture.value.Width;
            int srcHeight = _texture.value.Height;
            int resWidth = textureBox.Width;
            int resHeight = textureBox.Height;

            Bitmap bitmap = new Bitmap(resWidth, resHeight);
            Byte4[] data = new Byte4[srcWidth * srcHeight];
            _texture.value.GetData<Byte4>(data);

            float jStep = (float)srcWidth / textureBox.Width;
            float iStep = (float)srcHeight / textureBox.Height;

            for (int i = 0; i < resHeight; ++i) {
                for(int j = 0; j < resWidth; ++j){
                    Vector4 vec = data[(int)(i * iStep) * srcWidth + (int)(j * jStep)].ToVector4();
                    bitmap.SetPixel(j, i,
                        System.Drawing.Color.FromArgb((int)(vec.W),
                                                      (int)(vec.X),
                                                      (int)(vec.Y),
                                                      (int)(vec.Z)));
                }
            }

            textureBox.Image = bitmap;
        }

        CatTexture m_texture;

        private void textureBox_Click(object sender, EventArgs e) {
            // show dialog
            ResourceSelectorWindow resourceSelector = new ResourceSelectorWindow();
            resourceSelector.SetType(ResourceSelectorWindow.ObserveType.Texture);
            resourceSelector.CanTypeModify(false);
            resourceSelector.UpdateList();
            if (m_texture.value != null) {
                resourceSelector.Select(m_texture.value.Name);
            }
            resourceSelector.ShowDialog(this);

            if (resourceSelector.DialogResult == DialogResult.OK) {
                m_texture.value = Mgr<CatProject>.Singleton.contentManger.Load<Texture2D>("image\\" + resourceSelector.GetSelectedString());
                m_texture.value.Name = resourceSelector.GetSelectedString();
                SetObserve(m_texture);
            }
        }
    }
}
