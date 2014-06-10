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
    public partial class NumericWidget : UserControl {
        public NumericWidget() {
            InitializeComponent();
        }

        public void SetObserve(CatFloat _value) {
            m_fValue = _value;
            m_iValue = null;
            // x.xxx
            inputBox.DecimalPlaces = 3;
            UpdateShowValue();
            
        }

        public void SetObserve(CatInteger _value) {
            m_iValue = _value;
            m_fValue = null;
            // x
            inputBox.DecimalPlaces = 0;
            UpdateShowValue();
        }

        public void UpdateShowValue() {
            if (m_fValue != null) {
                inputBox.Value = new System.Decimal(m_fValue.GetValue());
            }
            else if (m_iValue != null) {
                inputBox.Value = new System.Decimal(m_iValue.GetValue());
            }
            
        }

        private void NumericWidget_Load(object sender, EventArgs e) {

        }

        private void inputBox_ValueChanged(object sender, EventArgs e) {
            if (m_fValue != null) {
                m_fValue.FromString(inputBox.Value.ToString());                
            }
            else if (m_iValue != null) {
                
                //m_iValue.(inputBox.Value.ToString());
            }
        }

        CatFloat m_fValue;
        CatInteger m_iValue;
    }
}
