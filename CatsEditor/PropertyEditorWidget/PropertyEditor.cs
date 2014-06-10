using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Catsland.Core;

namespace Catsland.CatsEditor.PropertyEditorWidget {
    public partial class PropertyEditor : UserControl {
        public PropertyEditor() {
            InitializeComponent();
        }

        public void SetObserve(CatMaterial _material) {
            m_material = _material;
            UpdateShowValue();
        }

        public void UpdateShowValue() {
            // material
            if (m_material != null) {
                // now we clear all the table
                Controls.Clear();
                // and create a new world

                Dictionary<string, IEffectParameter> parameters = m_material.GetMaterialParameters();
                CatMaterialTemplate template = m_material.GetMaterialTemplate();
                int curY = 0;
                int labelWidth = 200;
                foreach(KeyValuePair<string, IEffectParameter> keyValue
                    in parameters) {
                    // check whether it is masked
                    if (template.IsParameterMaskedInEditor(keyValue.Key)) {
                        continue;
                    }

                    // label
                    Label label = CreateLabel(keyValue.Key);
                    label.Left = 0;
                    label.Top = curY;
                    label.Width = labelWidth;
                    Controls.Add(label);
                    int rowHeight = label.Height;
                    int leftWidth = label.Width;
                    // property editor
                    UserControl property = CreateMaterialPropertyWidget(keyValue.Value);
                    if (property != null){
                        property.Location = new Point(leftWidth + 2, curY);
                        property.Width = Width - leftWidth - 4;
                        Controls.Add(property);
                        rowHeight = property.Height;
                    }

                    curY += rowHeight + 2;
                }
            }
        }

        private Label CreateLabel(string _text) {
            Label label = new Label();
            label.Text = _text;
            label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            label.AutoSize = true;
            label.TabIndex = 0;
            return label;
        }

        private UserControl CreateMaterialPropertyWidget(IEffectParameter _parameter) {
            UserControl property = null;
            if (_parameter.GetType().ToString() == typeof(CatFloat).ToString()) {
                property = new NumericWidget();
                property.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
                ((NumericWidget)property).SetObserve((CatFloat)(_parameter));
            }
            else if (_parameter.GetType().ToString() == typeof(CatTexture).ToString()) {
                property = new TextureWidget();
                property.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
                ((TextureWidget)property).SetObserve((CatTexture)(_parameter));
            }
            else if (_parameter.GetType().ToString() == typeof(CatVector4).ToString()) {
                property = new VectorWidget();
                property.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
                ((VectorWidget)property).SetObserve((CatVector4)(_parameter));
            }
            else if (_parameter.GetType().ToString() == typeof(CatColor).ToString()) {
                property = new ColorWidget();
                property.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
                ((ColorWidget)property).SetObserve((CatColor)(_parameter));
            }
            // TODO: more here

            return property;
        }

        CatMaterial m_material;
    }
}
