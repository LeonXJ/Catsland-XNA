using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MapEditorControlLibrary
{
	public partial class AnimationClipViewer : UserControl
	{
		public object m_targetClip { set; get; }

// clip name
#region 
		[Description("Clip name")]
		[DefaultValue(typeof(String), "Untitled Clip")]
		public String ClipName
		{
			get
			{
				return clipName.Text;
			}
			set
			{
				clipName.Text = value;
			}
		}

		public delegate void ClipNameChangedEventHandler(object sender, EventArgs e);
		public event ClipNameChangedEventHandler ClipNameChanged;
		protected virtual void OnClipNameChanged(EventArgs e)
		{
			if (ClipNameChanged != null)
			{
				ClipNameChanged(this, e);
			}
		}
#endregion
// min index
#region 

		[Description("Min index")]
		[DefaultValue(typeof(int), "0")]
		public int MinIndex
		{
			get
			{
				return (int)minIndex.Value;
			}
			set
			{
				minIndex.Value = value;
			}
		}

		public delegate void MinIndexChangedEventHandler(object sender, EventArgs e);
		public event MinIndexChangedEventHandler MinIndexChanged;
		protected virtual void OnMinIndexChanged(EventArgs e)
		{
			if (MinIndexChanged != null)
			{
				MinIndexChanged(this, e);
			}
		}
#endregion
// max index
#region 
		[Description("Max index")]
		[DefaultValue(typeof(int), "0")]
		public int MaxIndex
		{
			get
			{
				return (int)maxIndex.Value;
			}
			set
			{
				maxIndex.Value = value;
			}
		}

		public delegate void MaxIndexChangedEventHandler(object sender, EventArgs e);
		public event MaxIndexChangedEventHandler MaxIndexChanged;
		protected virtual void OnMaxIndexChanged(EventArgs e)
		{
			if (MaxIndexChanged != null)
			{
				MaxIndexChanged(this, e);
			}
		}
#endregion
// clip mode
#region 
		[Description("Clip mode")]
		[DefaultValue(typeof(int), "0")]
		public int ClipMode
		{
			get
			{
				return clipMode.SelectedIndex;
			}
			set
			{
				clipMode.SelectedIndex = value;
			}
		}

		public delegate void ClipModeChangedEventHandler(object sender, EventArgs e);
		public event ClipModeChangedEventHandler ClipModeChanged;
		protected virtual void OnClipModeChanged(EventArgs e)
		{
			if (ClipModeChanged != null)
			{
				ClipModeChanged(this, e);
			}
		}
#endregion
// delete button
		public delegate void DeleteButtonClickedEventHandler(object sender, EventArgs e);
		public event DeleteButtonClickedEventHandler DeleteButtonClicked;
		protected virtual void OnDeleteButtonClicked(EventArgs e)
		{
			if (DeleteButtonClicked != null)
			{
				DeleteButtonClicked(this, e);
			}
		}

		public AnimationClipViewer()
		{
			InitializeComponent();
		}

		private void AnimationClipViewer_Load(object sender, EventArgs e)
		{

		}

		private void clipName_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == 13)
			{
				OnClipNameChanged(e);
			}
		}

		private void minIndex_ValueChanged(object sender, EventArgs e)
		{
			OnMinIndexChanged(e);
		}

		private void maxIndex_ValueChanged(object sender, EventArgs e)
		{
			OnMaxIndexChanged(e);
		}

		private void clipMode_SelectedIndexChanged(object sender, EventArgs e)
		{
			OnClipModeChanged(e);
		}

		private void deleteBtn_Click(object sender, EventArgs e)
		{
			OnDeleteButtonClicked(e);
		}
	}
}
