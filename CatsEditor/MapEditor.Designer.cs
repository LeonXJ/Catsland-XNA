namespace Catsland.Editor
{
	partial class MapEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapEditor));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btn_play = new System.Windows.Forms.ToolStripButton();
            this.btn_pause = new System.Windows.Forms.ToolStripButton();
            this.btn_stop = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.movingAxis = new System.Windows.Forms.ToolStripSplitButton();
            this.xYToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.assistXYheight = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.resolution_selector = new System.Windows.Forms.ToolStripComboBox();
            this.resolution_size_label = new System.Windows.Forms.ToolStripLabel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scenesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.publishProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteGameObjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addGameObjectToPrefabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteProfabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gameObjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compoundGameObjectMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.gameObjectFromPrefabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_component = new System.Windows.Forms.ToolStripMenuItem();
            this.resourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rescanPluginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.renderArea = new System.Windows.Forms.PictureBox();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.gameObjectTree = new System.Windows.Forms.TreeView();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.prefabTree = new System.Windows.Forms.TreeView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.modelList = new System.Windows.Forms.ListBox();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.materialList = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.attr_tab = new System.Windows.Forms.TabControl();
            this.attr_tab_scene = new System.Windows.Forms.TabPage();
            this.pg_scene = new System.Windows.Forms.PropertyGrid();
            this.attr_tab_model = new System.Windows.Forms.TabPage();
            this.attr_ml_group = new System.Windows.Forms.GroupBox();
            this.attr_ml_name = new System.Windows.Forms.TextBox();
            this.label42 = new System.Windows.Forms.Label();
            this.attr_mtl_group = new System.Windows.Forms.GroupBox();
            this.propertyBox = new System.Windows.Forms.Panel();
            this.attr_ml_mtrl = new System.Windows.Forms.ComboBox();
            this.attr_clips_group = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.label43 = new System.Windows.Forms.Label();
            this.label44 = new System.Windows.Forms.Label();
            this.label45 = new System.Windows.Forms.Label();
            this.label46 = new System.Windows.Forms.Label();
            this.attr_ani_autoplay = new System.Windows.Forms.CheckBox();
            this.attr_ani_mpf = new System.Windows.Forms.TextBox();
            this.attr_ani_tiltwidth = new System.Windows.Forms.TextBox();
            this.attr_ani_tiltheight = new System.Windows.Forms.TextBox();
            this.attr_ani_defaultclip = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.attr_ani_clips = new System.Windows.Forms.Panel();
            this.attr_tab_material = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.label47 = new System.Windows.Forms.Label();
            this.label48 = new System.Windows.Forms.Label();
            this.label49 = new System.Windows.Forms.Label();
            this.attr_mtrl_name = new System.Windows.Forms.TextBox();
            this.attr_mtrl_effect = new System.Windows.Forms.ComboBox();
            this.picbox_preview = new System.Windows.Forms.PictureBox();
            this.btn_textureSelector = new System.Windows.Forms.Button();
            this.attr_tab_newgo = new System.Windows.Forms.TabPage();
            this.attr_tab_camera = new System.Windows.Forms.TabPage();
            this.attr_postprocesses = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.attr_camera = new System.Windows.Forms.PropertyGrid();
            this.toolStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.renderArea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.panel1.SuspendLayout();
            this.attr_tab.SuspendLayout();
            this.attr_tab_scene.SuspendLayout();
            this.attr_tab_model.SuspendLayout();
            this.attr_ml_group.SuspendLayout();
            this.attr_mtl_group.SuspendLayout();
            this.attr_clips_group.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.attr_tab_material.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbox_preview)).BeginInit();
            this.attr_tab_camera.SuspendLayout();
            this.attr_postprocesses.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripButton3,
            this.toolStripSeparator2,
            this.btn_play,
            this.btn_pause,
            this.btn_stop,
            this.toolStripSeparator3,
            this.movingAxis,
            this.assistXYheight,
            this.toolStripSeparator4,
            this.resolution_selector,
            this.resolution_size_label});
            this.toolStrip1.Location = new System.Drawing.Point(0, 25);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1028, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(126, 22);
            this.toolStripButton1.Text = "Create GameObject";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(92, 22);
            this.toolStripButton2.Text = "Create Model";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(102, 22);
            this.toolStripButton3.Text = "Create Material";
            this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btn_play
            // 
            this.btn_play.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btn_play.Image = ((System.Drawing.Image)(resources.GetObject("btn_play.Image")));
            this.btn_play.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_play.Name = "btn_play";
            this.btn_play.Size = new System.Drawing.Size(35, 22);
            this.btn_play.Text = "Play";
            this.btn_play.Click += new System.EventHandler(this.btn_play_Click);
            // 
            // btn_pause
            // 
            this.btn_pause.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btn_pause.Enabled = false;
            this.btn_pause.Image = ((System.Drawing.Image)(resources.GetObject("btn_pause.Image")));
            this.btn_pause.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_pause.Name = "btn_pause";
            this.btn_pause.Size = new System.Drawing.Size(46, 22);
            this.btn_pause.Text = "Pause";
            this.btn_pause.Click += new System.EventHandler(this.btn_pause_Click);
            // 
            // btn_stop
            // 
            this.btn_stop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btn_stop.Enabled = false;
            this.btn_stop.Image = ((System.Drawing.Image)(resources.GetObject("btn_stop.Image")));
            this.btn_stop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_stop.Name = "btn_stop";
            this.btn_stop.Size = new System.Drawing.Size(39, 22);
            this.btn_stop.Text = "Stop";
            this.btn_stop.Click += new System.EventHandler(this.btn_stop_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // movingAxis
            // 
            this.movingAxis.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.movingAxis.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.xYToolStripMenuItem,
            this.zToolStripMenuItem});
            this.movingAxis.Image = ((System.Drawing.Image)(resources.GetObject("movingAxis.Image")));
            this.movingAxis.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.movingAxis.Name = "movingAxis";
            this.movingAxis.Size = new System.Drawing.Size(117, 22);
            this.movingAxis.Text = "Moving Axis: XY";
            this.movingAxis.ButtonClick += new System.EventHandler(this.movingAxis_ButtonClick);
            // 
            // xYToolStripMenuItem
            // 
            this.xYToolStripMenuItem.Name = "xYToolStripMenuItem";
            this.xYToolStripMenuItem.Size = new System.Drawing.Size(91, 22);
            this.xYToolStripMenuItem.Text = "XY";
            this.xYToolStripMenuItem.Click += new System.EventHandler(this.xYToolStripMenuItem_Click);
            // 
            // zToolStripMenuItem
            // 
            this.zToolStripMenuItem.Name = "zToolStripMenuItem";
            this.zToolStripMenuItem.Size = new System.Drawing.Size(91, 22);
            this.zToolStripMenuItem.Text = "XZ";
            this.zToolStripMenuItem.Click += new System.EventHandler(this.zToolStripMenuItem_Click);
            // 
            // assistXYheight
            // 
            this.assistXYheight.Name = "assistXYheight";
            this.assistXYheight.Size = new System.Drawing.Size(100, 25);
            this.assistXYheight.Text = "0";
            this.assistXYheight.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.assistXYheight_KeyPress);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // resolution_selector
            // 
            this.resolution_selector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.resolution_selector.Name = "resolution_selector";
            this.resolution_selector.Size = new System.Drawing.Size(121, 25);
            this.resolution_selector.SelectedIndexChanged += new System.EventHandler(this.resolution_selector_SelectedIndexChanged);
            // 
            // resolution_size_label
            // 
            this.resolution_size_label.Name = "resolution_size_label";
            this.resolution_size_label.Size = new System.Drawing.Size(26, 22);
            this.resolution_size_label.Text = "xxx";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 557);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1028, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.insertToolStripMenuItem,
            this.resourceToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1028, 25);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectToolStripMenuItem,
            this.openProjectToolStripMenuItem,
            this.saveProjectToolStripMenuItem,
            this.scenesToolStripMenuItem,
            this.publishProjectToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(39, 21);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newProjectToolStripMenuItem
            // 
            this.newProjectToolStripMenuItem.Name = "newProjectToolStripMenuItem";
            this.newProjectToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.newProjectToolStripMenuItem.Text = "New Project";
            this.newProjectToolStripMenuItem.Click += new System.EventHandler(this.newProjectToolStripMenuItem_Click);
            // 
            // openProjectToolStripMenuItem
            // 
            this.openProjectToolStripMenuItem.Name = "openProjectToolStripMenuItem";
            this.openProjectToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.openProjectToolStripMenuItem.Text = "Open Project";
            this.openProjectToolStripMenuItem.Click += new System.EventHandler(this.openProjectToolStripMenuItem_Click);
            // 
            // saveProjectToolStripMenuItem
            // 
            this.saveProjectToolStripMenuItem.Name = "saveProjectToolStripMenuItem";
            this.saveProjectToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.saveProjectToolStripMenuItem.Text = "Save";
            this.saveProjectToolStripMenuItem.Click += new System.EventHandler(this.saveProjectToolStripMenuItem_Click);
            // 
            // scenesToolStripMenuItem
            // 
            this.scenesToolStripMenuItem.Name = "scenesToolStripMenuItem";
            this.scenesToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.scenesToolStripMenuItem.Text = "Scenes";
            this.scenesToolStripMenuItem.Click += new System.EventHandler(this.scenesToolStripMenuItem_Click);
            // 
            // publishProjectToolStripMenuItem
            // 
            this.publishProjectToolStripMenuItem.Name = "publishProjectToolStripMenuItem";
            this.publishProjectToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.publishProjectToolStripMenuItem.Text = "Publish Project";
            this.publishProjectToolStripMenuItem.Click += new System.EventHandler(this.publishProjectToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(158, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteGameObjectToolStripMenuItem,
            this.addGameObjectToPrefabToolStripMenuItem,
            this.deleteProfabToolStripMenuItem,
            this.cameraToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(42, 21);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.DropDownOpening += new System.EventHandler(this.editToolStripMenuItem_DropDownOpening);
            // 
            // deleteGameObjectToolStripMenuItem
            // 
            this.deleteGameObjectToolStripMenuItem.Name = "deleteGameObjectToolStripMenuItem";
            this.deleteGameObjectToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.deleteGameObjectToolStripMenuItem.Text = "Delete GameObject";
            this.deleteGameObjectToolStripMenuItem.Click += new System.EventHandler(this.deleteGameObjectToolStripMenuItem_Click);
            // 
            // addGameObjectToPrefabToolStripMenuItem
            // 
            this.addGameObjectToPrefabToolStripMenuItem.Name = "addGameObjectToPrefabToolStripMenuItem";
            this.addGameObjectToPrefabToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.addGameObjectToPrefabToolStripMenuItem.Text = "Add GameObject to Prefab";
            this.addGameObjectToPrefabToolStripMenuItem.Click += new System.EventHandler(this.addGameObjectToPrefabToolStripMenuItem_Click);
            // 
            // deleteProfabToolStripMenuItem
            // 
            this.deleteProfabToolStripMenuItem.Name = "deleteProfabToolStripMenuItem";
            this.deleteProfabToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.deleteProfabToolStripMenuItem.Text = "Delete Profab";
            this.deleteProfabToolStripMenuItem.Click += new System.EventHandler(this.deleteProfabToolStripMenuItem_Click);
            // 
            // cameraToolStripMenuItem
            // 
            this.cameraToolStripMenuItem.Name = "cameraToolStripMenuItem";
            this.cameraToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            this.cameraToolStripMenuItem.Text = "Camera";
            this.cameraToolStripMenuItem.Click += new System.EventHandler(this.cameraToolStripMenuItem_Click);
            // 
            // insertToolStripMenuItem
            // 
            this.insertToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gameObjectToolStripMenuItem,
            this.compoundGameObjectMenu,
            this.gameObjectFromPrefabToolStripMenuItem,
            this.menu_component});
            this.insertToolStripMenuItem.Name = "insertToolStripMenuItem";
            this.insertToolStripMenuItem.Size = new System.Drawing.Size(53, 21);
            this.insertToolStripMenuItem.Text = "Insert";
            this.insertToolStripMenuItem.DropDownOpened += new System.EventHandler(this.insertToolStripMenuItem_DropDownOpened);
            // 
            // gameObjectToolStripMenuItem
            // 
            this.gameObjectToolStripMenuItem.Name = "gameObjectToolStripMenuItem";
            this.gameObjectToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.gameObjectToolStripMenuItem.Text = "GameObject";
            // 
            // compoundGameObjectMenu
            // 
            this.compoundGameObjectMenu.Name = "compoundGameObjectMenu";
            this.compoundGameObjectMenu.Size = new System.Drawing.Size(224, 22);
            this.compoundGameObjectMenu.Text = "Compound GameObject";
            // 
            // gameObjectFromPrefabToolStripMenuItem
            // 
            this.gameObjectFromPrefabToolStripMenuItem.Name = "gameObjectFromPrefabToolStripMenuItem";
            this.gameObjectFromPrefabToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.gameObjectFromPrefabToolStripMenuItem.Text = "GameObject From Prefab";
            this.gameObjectFromPrefabToolStripMenuItem.Click += new System.EventHandler(this.gameObjectFromPrefabToolStripMenuItem_Click);
            // 
            // menu_component
            // 
            this.menu_component.Name = "menu_component";
            this.menu_component.Size = new System.Drawing.Size(224, 22);
            this.menu_component.Text = "Component";
            this.menu_component.Click += new System.EventHandler(this.menu_component_Click);
            // 
            // resourceToolStripMenuItem
            // 
            this.resourceToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rescanPluginToolStripMenuItem});
            this.resourceToolStripMenuItem.Name = "resourceToolStripMenuItem";
            this.resourceToolStripMenuItem.Size = new System.Drawing.Size(74, 21);
            this.resourceToolStripMenuItem.Text = "Resource";
            // 
            // rescanPluginToolStripMenuItem
            // 
            this.rescanPluginToolStripMenuItem.Name = "rescanPluginToolStripMenuItem";
            this.rescanPluginToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.rescanPluginToolStripMenuItem.Text = "Rescan Plugin";
            this.rescanPluginToolStripMenuItem.Click += new System.EventHandler(this.rescanPluginToolStripMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 50);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Size = new System.Drawing.Size(1028, 507);
            this.splitContainer1.SplitterDistance = 658;
            this.splitContainer1.TabIndex = 3;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.BackColor = System.Drawing.Color.Black;
            this.splitContainer2.Panel1.Controls.Add(this.renderArea);
            this.splitContainer2.Panel1.Resize += new System.EventHandler(this.splitContainer2_Panel1_Resize);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer2.Size = new System.Drawing.Size(658, 507);
            this.splitContainer2.SplitterDistance = 335;
            this.splitContainer2.TabIndex = 0;
            // 
            // renderArea
            // 
            this.renderArea.Location = new System.Drawing.Point(156, 107);
            this.renderArea.Name = "renderArea";
            this.renderArea.Size = new System.Drawing.Size(326, 177);
            this.renderArea.TabIndex = 2;
            this.renderArea.TabStop = false;
            this.renderArea.MouseClick += new System.Windows.Forms.MouseEventHandler(this.renderArea_MouseClick);
            this.renderArea.MouseDown += new System.Windows.Forms.MouseEventHandler(this.renderArea_MouseDown);
            this.renderArea.MouseEnter += new System.EventHandler(this.renderArea_MouseEnter);
            this.renderArea.MouseLeave += new System.EventHandler(this.renderArea_MouseLeave);
            this.renderArea.MouseMove += new System.Windows.Forms.MouseEventHandler(this.renderArea_MouseMove);
            this.renderArea.MouseUp += new System.Windows.Forms.MouseEventHandler(this.renderArea_MouseUp);
            this.renderArea.Resize += new System.EventHandler(this.renderArea_SizeChanged);
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.tabControl2);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer3.Size = new System.Drawing.Size(658, 168);
            this.splitContainer3.SplitterDistance = 211;
            this.splitContainer3.TabIndex = 1;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage3);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(0, 0);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(211, 168);
            this.tabControl2.TabIndex = 1;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.gameObjectTree);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(203, 142);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "GameObject";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // gameObjectTree
            // 
            this.gameObjectTree.AllowDrop = true;
            this.gameObjectTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gameObjectTree.LabelEdit = true;
            this.gameObjectTree.Location = new System.Drawing.Point(3, 3);
            this.gameObjectTree.Name = "gameObjectTree";
            this.gameObjectTree.Size = new System.Drawing.Size(197, 136);
            this.gameObjectTree.TabIndex = 1;
            this.gameObjectTree.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.gameObjectTree_AfterLabelEdit);
            this.gameObjectTree.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.gameObjectTree_AfterCollapse);
            this.gameObjectTree.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.gameObjectTree_AfterExpand);
            this.gameObjectTree.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.gameObjectTree_ItemDrag);
            this.gameObjectTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.gameObjectTree_NodeMouseClick);
            this.gameObjectTree.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.gameObjectTree_NodeMouseDoubleClick);
            this.gameObjectTree.DragDrop += new System.Windows.Forms.DragEventHandler(this.gameObjectTree_DragDrop);
            this.gameObjectTree.DragEnter += new System.Windows.Forms.DragEventHandler(this.gameObjectTree_DragEnter);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(443, 168);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.prefabTree);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(435, 142);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Prefab";
            // 
            // prefabTree
            // 
            this.prefabTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.prefabTree.Location = new System.Drawing.Point(3, 3);
            this.prefabTree.Name = "prefabTree";
            this.prefabTree.Size = new System.Drawing.Size(429, 136);
            this.prefabTree.TabIndex = 1;
            this.prefabTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.prefabTree_NodeMouseClick);
            this.prefabTree.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.prefabTree_NodeMouseDoubleClick);
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Controls.Add(this.modelList);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(435, 142);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Model";
            // 
            // modelList
            // 
            this.modelList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modelList.FormattingEnabled = true;
            this.modelList.ItemHeight = 12;
            this.modelList.Location = new System.Drawing.Point(3, 3);
            this.modelList.Name = "modelList";
            this.modelList.Size = new System.Drawing.Size(429, 136);
            this.modelList.Sorted = true;
            this.modelList.TabIndex = 0;
            this.modelList.MouseClick += new System.Windows.Forms.MouseEventHandler(this.modelList_MouseClick);
            this.modelList.SelectedIndexChanged += new System.EventHandler(this.modelList_SelectedIndexChanged);
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.materialList);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(435, 142);
            this.tabPage5.TabIndex = 2;
            this.tabPage5.Text = "Material";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // materialList
            // 
            this.materialList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.materialList.FormattingEnabled = true;
            this.materialList.ItemHeight = 12;
            this.materialList.Location = new System.Drawing.Point(0, 0);
            this.materialList.Name = "materialList";
            this.materialList.Size = new System.Drawing.Size(435, 142);
            this.materialList.Sorted = true;
            this.materialList.TabIndex = 0;
            this.materialList.MouseClick += new System.Windows.Forms.MouseEventHandler(this.materialList_MouseClick);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.attr_tab);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(366, 507);
            this.panel1.TabIndex = 0;
            // 
            // attr_tab
            // 
            this.attr_tab.Controls.Add(this.attr_tab_scene);
            this.attr_tab.Controls.Add(this.attr_tab_model);
            this.attr_tab.Controls.Add(this.attr_tab_material);
            this.attr_tab.Controls.Add(this.attr_tab_newgo);
            this.attr_tab.Controls.Add(this.attr_tab_camera);
            this.attr_tab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.attr_tab.Location = new System.Drawing.Point(0, 0);
            this.attr_tab.Name = "attr_tab";
            this.attr_tab.SelectedIndex = 0;
            this.attr_tab.Size = new System.Drawing.Size(364, 505);
            this.attr_tab.TabIndex = 0;
            // 
            // attr_tab_scene
            // 
            this.attr_tab_scene.Controls.Add(this.pg_scene);
            this.attr_tab_scene.Location = new System.Drawing.Point(4, 22);
            this.attr_tab_scene.Name = "attr_tab_scene";
            this.attr_tab_scene.Size = new System.Drawing.Size(356, 479);
            this.attr_tab_scene.TabIndex = 3;
            this.attr_tab_scene.Text = "Scene";
            this.attr_tab_scene.UseVisualStyleBackColor = true;
            // 
            // pg_scene
            // 
            this.pg_scene.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pg_scene.Location = new System.Drawing.Point(0, 0);
            this.pg_scene.Name = "pg_scene";
            this.pg_scene.Size = new System.Drawing.Size(356, 479);
            this.pg_scene.TabIndex = 0;
            // 
            // attr_tab_model
            // 
            this.attr_tab_model.AutoScroll = true;
            this.attr_tab_model.Controls.Add(this.attr_ml_group);
            this.attr_tab_model.Controls.Add(this.attr_mtl_group);
            this.attr_tab_model.Controls.Add(this.attr_clips_group);
            this.attr_tab_model.Location = new System.Drawing.Point(4, 22);
            this.attr_tab_model.Name = "attr_tab_model";
            this.attr_tab_model.Size = new System.Drawing.Size(356, 479);
            this.attr_tab_model.TabIndex = 1;
            this.attr_tab_model.Text = "Model";
            this.attr_tab_model.UseVisualStyleBackColor = true;
            // 
            // attr_ml_group
            // 
            this.attr_ml_group.Controls.Add(this.attr_ml_name);
            this.attr_ml_group.Controls.Add(this.label42);
            this.attr_ml_group.Dock = System.Windows.Forms.DockStyle.Top;
            this.attr_ml_group.Location = new System.Drawing.Point(0, 457);
            this.attr_ml_group.Name = "attr_ml_group";
            this.attr_ml_group.Size = new System.Drawing.Size(339, 47);
            this.attr_ml_group.TabIndex = 0;
            this.attr_ml_group.TabStop = false;
            this.attr_ml_group.Text = "Model";
            // 
            // attr_ml_name
            // 
            this.attr_ml_name.Dock = System.Windows.Forms.DockStyle.Fill;
            this.attr_ml_name.Location = new System.Drawing.Point(32, 17);
            this.attr_ml_name.Name = "attr_ml_name";
            this.attr_ml_name.Size = new System.Drawing.Size(304, 21);
            this.attr_ml_name.TabIndex = 1;
            this.attr_ml_name.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.attr_ml_name_KeyPress);
            this.attr_ml_name.Leave += new System.EventHandler(this.attr_ml_name_Leave);
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Dock = System.Windows.Forms.DockStyle.Left;
            this.label42.Location = new System.Drawing.Point(3, 17);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(29, 12);
            this.label42.TabIndex = 0;
            this.label42.Text = "Name";
            this.label42.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // attr_mtl_group
            // 
            this.attr_mtl_group.Controls.Add(this.propertyBox);
            this.attr_mtl_group.Controls.Add(this.attr_ml_mtrl);
            this.attr_mtl_group.Dock = System.Windows.Forms.DockStyle.Top;
            this.attr_mtl_group.Location = new System.Drawing.Point(0, 150);
            this.attr_mtl_group.Name = "attr_mtl_group";
            this.attr_mtl_group.Size = new System.Drawing.Size(339, 307);
            this.attr_mtl_group.TabIndex = 1;
            this.attr_mtl_group.TabStop = false;
            this.attr_mtl_group.Text = "Material";
            // 
            // propertyBox
            // 
            this.propertyBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyBox.Location = new System.Drawing.Point(8, 43);
            this.propertyBox.Name = "propertyBox";
            this.propertyBox.Size = new System.Drawing.Size(324, 258);
            this.propertyBox.TabIndex = 1;
            // 
            // attr_ml_mtrl
            // 
            this.attr_ml_mtrl.Dock = System.Windows.Forms.DockStyle.Top;
            this.attr_ml_mtrl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.attr_ml_mtrl.FormattingEnabled = true;
            this.attr_ml_mtrl.Location = new System.Drawing.Point(3, 17);
            this.attr_ml_mtrl.Name = "attr_ml_mtrl";
            this.attr_ml_mtrl.Size = new System.Drawing.Size(333, 20);
            this.attr_ml_mtrl.TabIndex = 0;
            this.attr_ml_mtrl.SelectedIndexChanged += new System.EventHandler(this.attr_ml_mtrl_SelectedIndexChanged);
            // 
            // attr_clips_group
            // 
            this.attr_clips_group.AutoSize = true;
            this.attr_clips_group.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.attr_clips_group.Controls.Add(this.tableLayoutPanel7);
            this.attr_clips_group.Controls.Add(this.button1);
            this.attr_clips_group.Controls.Add(this.attr_ani_clips);
            this.attr_clips_group.Dock = System.Windows.Forms.DockStyle.Top;
            this.attr_clips_group.Location = new System.Drawing.Point(0, 0);
            this.attr_clips_group.Name = "attr_clips_group";
            this.attr_clips_group.Size = new System.Drawing.Size(339, 150);
            this.attr_clips_group.TabIndex = 1;
            this.attr_clips_group.TabStop = false;
            this.attr_clips_group.Text = "Animation";
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 4;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel7.Controls.Add(this.label43, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.label44, 0, 1);
            this.tableLayoutPanel7.Controls.Add(this.label45, 2, 1);
            this.tableLayoutPanel7.Controls.Add(this.label46, 0, 2);
            this.tableLayoutPanel7.Controls.Add(this.attr_ani_autoplay, 0, 3);
            this.tableLayoutPanel7.Controls.Add(this.attr_ani_mpf, 2, 0);
            this.tableLayoutPanel7.Controls.Add(this.attr_ani_tiltwidth, 1, 1);
            this.tableLayoutPanel7.Controls.Add(this.attr_ani_tiltheight, 3, 1);
            this.tableLayoutPanel7.Controls.Add(this.attr_ani_defaultclip, 1, 2);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(3, 40);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 4;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel7.Size = new System.Drawing.Size(333, 107);
            this.tableLayoutPanel7.TabIndex = 2;
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.tableLayoutPanel7.SetColumnSpan(this.label43, 2);
            this.label43.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label43.Location = new System.Drawing.Point(3, 0);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(183, 27);
            this.label43.TabIndex = 0;
            this.label43.Text = "Millionseconds per frame";
            this.label43.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label44.Location = new System.Drawing.Point(3, 27);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(77, 27);
            this.label44.TabIndex = 1;
            this.label44.Text = "Tilt width";
            this.label44.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label45.Location = new System.Drawing.Point(192, 27);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(71, 27);
            this.label45.TabIndex = 2;
            this.label45.Text = "Tilt Height";
            this.label45.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label46.Location = new System.Drawing.Point(3, 54);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(77, 26);
            this.label46.TabIndex = 3;
            this.label46.Text = "Default clip";
            this.label46.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // attr_ani_autoplay
            // 
            this.attr_ani_autoplay.AutoSize = true;
            this.tableLayoutPanel7.SetColumnSpan(this.attr_ani_autoplay, 4);
            this.attr_ani_autoplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.attr_ani_autoplay.Location = new System.Drawing.Point(3, 83);
            this.attr_ani_autoplay.Name = "attr_ani_autoplay";
            this.attr_ani_autoplay.Size = new System.Drawing.Size(366, 21);
            this.attr_ani_autoplay.TabIndex = 4;
            this.attr_ani_autoplay.Text = "Auto play";
            this.attr_ani_autoplay.UseVisualStyleBackColor = true;
            this.attr_ani_autoplay.CheckedChanged += new System.EventHandler(this.attr_ani_autoplay_CheckedChanged);
            // 
            // attr_ani_mpf
            // 
            this.tableLayoutPanel7.SetColumnSpan(this.attr_ani_mpf, 2);
            this.attr_ani_mpf.Dock = System.Windows.Forms.DockStyle.Fill;
            this.attr_ani_mpf.Location = new System.Drawing.Point(192, 3);
            this.attr_ani_mpf.Name = "attr_ani_mpf";
            this.attr_ani_mpf.Size = new System.Drawing.Size(177, 21);
            this.attr_ani_mpf.TabIndex = 5;
            this.attr_ani_mpf.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.attr_ani_mpf_KeyPress);
            // 
            // attr_ani_tiltwidth
            // 
            this.attr_ani_tiltwidth.Dock = System.Windows.Forms.DockStyle.Fill;
            this.attr_ani_tiltwidth.Location = new System.Drawing.Point(86, 30);
            this.attr_ani_tiltwidth.Name = "attr_ani_tiltwidth";
            this.attr_ani_tiltwidth.Size = new System.Drawing.Size(100, 21);
            this.attr_ani_tiltwidth.TabIndex = 6;
            this.attr_ani_tiltwidth.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.attr_ani_tilt_KeyPress);
            // 
            // attr_ani_tiltheight
            // 
            this.attr_ani_tiltheight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.attr_ani_tiltheight.Location = new System.Drawing.Point(269, 30);
            this.attr_ani_tiltheight.Name = "attr_ani_tiltheight";
            this.attr_ani_tiltheight.Size = new System.Drawing.Size(100, 21);
            this.attr_ani_tiltheight.TabIndex = 7;
            this.attr_ani_tiltheight.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.attr_ani_tilt_KeyPress);
            // 
            // attr_ani_defaultclip
            // 
            this.tableLayoutPanel7.SetColumnSpan(this.attr_ani_defaultclip, 3);
            this.attr_ani_defaultclip.Dock = System.Windows.Forms.DockStyle.Left;
            this.attr_ani_defaultclip.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.attr_ani_defaultclip.FormattingEnabled = true;
            this.attr_ani_defaultclip.Location = new System.Drawing.Point(86, 57);
            this.attr_ani_defaultclip.Name = "attr_ani_defaultclip";
            this.attr_ani_defaultclip.Size = new System.Drawing.Size(265, 20);
            this.attr_ani_defaultclip.TabIndex = 8;
            this.attr_ani_defaultclip.SelectedIndexChanged += new System.EventHandler(this.attr_ani_defaultclip_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Top;
            this.button1.Location = new System.Drawing.Point(3, 17);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(333, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Add Clip";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // attr_ani_clips
            // 
            this.attr_ani_clips.AutoSize = true;
            this.attr_ani_clips.Dock = System.Windows.Forms.DockStyle.Top;
            this.attr_ani_clips.Location = new System.Drawing.Point(3, 17);
            this.attr_ani_clips.Name = "attr_ani_clips";
            this.attr_ani_clips.Size = new System.Drawing.Size(333, 0);
            this.attr_ani_clips.TabIndex = 1;
            // 
            // attr_tab_material
            // 
            this.attr_tab_material.AutoScroll = true;
            this.attr_tab_material.Controls.Add(this.tableLayoutPanel8);
            this.attr_tab_material.Location = new System.Drawing.Point(4, 22);
            this.attr_tab_material.Name = "attr_tab_material";
            this.attr_tab_material.Size = new System.Drawing.Size(356, 479);
            this.attr_tab_material.TabIndex = 2;
            this.attr_tab_material.Text = "Material";
            this.attr_tab_material.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.ColumnCount = 3;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 128F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel8.Controls.Add(this.label47, 0, 0);
            this.tableLayoutPanel8.Controls.Add(this.label48, 0, 1);
            this.tableLayoutPanel8.Controls.Add(this.label49, 0, 2);
            this.tableLayoutPanel8.Controls.Add(this.attr_mtrl_name, 1, 0);
            this.tableLayoutPanel8.Controls.Add(this.attr_mtrl_effect, 1, 2);
            this.tableLayoutPanel8.Controls.Add(this.picbox_preview, 1, 1);
            this.tableLayoutPanel8.Controls.Add(this.btn_textureSelector, 2, 1);
            this.tableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel8.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 4;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel8.Size = new System.Drawing.Size(356, 479);
            this.tableLayoutPanel8.TabIndex = 0;
            // 
            // label47
            // 
            this.label47.AutoSize = true;
            this.label47.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label47.Location = new System.Drawing.Point(3, 0);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(47, 27);
            this.label47.TabIndex = 0;
            this.label47.Text = "Name";
            this.label47.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label48
            // 
            this.label48.AutoSize = true;
            this.label48.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label48.Location = new System.Drawing.Point(3, 27);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(47, 134);
            this.label48.TabIndex = 1;
            this.label48.Text = "Texture";
            this.label48.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label49
            // 
            this.label49.AutoSize = true;
            this.label49.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label49.Location = new System.Drawing.Point(3, 161);
            this.label49.Name = "label49";
            this.label49.Size = new System.Drawing.Size(47, 26);
            this.label49.TabIndex = 2;
            this.label49.Text = "Effect";
            this.label49.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // attr_mtrl_name
            // 
            this.tableLayoutPanel8.SetColumnSpan(this.attr_mtrl_name, 2);
            this.attr_mtrl_name.Dock = System.Windows.Forms.DockStyle.Fill;
            this.attr_mtrl_name.Location = new System.Drawing.Point(56, 3);
            this.attr_mtrl_name.Name = "attr_mtrl_name";
            this.attr_mtrl_name.Size = new System.Drawing.Size(302, 21);
            this.attr_mtrl_name.TabIndex = 3;
            this.attr_mtrl_name.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.attr_mtrl_name_KeyPress);
            this.attr_mtrl_name.Leave += new System.EventHandler(this.attr_mtrl_name_Leave);
            // 
            // attr_mtrl_effect
            // 
            this.tableLayoutPanel8.SetColumnSpan(this.attr_mtrl_effect, 2);
            this.attr_mtrl_effect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.attr_mtrl_effect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.attr_mtrl_effect.FormattingEnabled = true;
            this.attr_mtrl_effect.Location = new System.Drawing.Point(56, 164);
            this.attr_mtrl_effect.Name = "attr_mtrl_effect";
            this.attr_mtrl_effect.Size = new System.Drawing.Size(302, 20);
            this.attr_mtrl_effect.TabIndex = 5;
            // 
            // picbox_preview
            // 
            this.picbox_preview.BackColor = System.Drawing.Color.Magenta;
            this.picbox_preview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picbox_preview.Location = new System.Drawing.Point(56, 30);
            this.picbox_preview.Name = "picbox_preview";
            this.picbox_preview.Size = new System.Drawing.Size(122, 128);
            this.picbox_preview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picbox_preview.TabIndex = 7;
            this.picbox_preview.TabStop = false;
            // 
            // btn_textureSelector
            // 
            this.btn_textureSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_textureSelector.Location = new System.Drawing.Point(184, 30);
            this.btn_textureSelector.Name = "btn_textureSelector";
            this.btn_textureSelector.Size = new System.Drawing.Size(174, 128);
            this.btn_textureSelector.TabIndex = 6;
            this.btn_textureSelector.Text = "Select Texture";
            this.btn_textureSelector.UseVisualStyleBackColor = true;
            this.btn_textureSelector.Click += new System.EventHandler(this.btn_textureSelector_Click);
            // 
            // attr_tab_newgo
            // 
            this.attr_tab_newgo.AutoScroll = true;
            this.attr_tab_newgo.Location = new System.Drawing.Point(4, 22);
            this.attr_tab_newgo.Name = "attr_tab_newgo";
            this.attr_tab_newgo.Size = new System.Drawing.Size(356, 479);
            this.attr_tab_newgo.TabIndex = 4;
            this.attr_tab_newgo.Text = "GameObject";
            this.attr_tab_newgo.UseVisualStyleBackColor = true;
            // 
            // attr_tab_camera
            // 
            this.attr_tab_camera.Controls.Add(this.attr_postprocesses);
            this.attr_tab_camera.Controls.Add(this.attr_camera);
            this.attr_tab_camera.Location = new System.Drawing.Point(4, 22);
            this.attr_tab_camera.Name = "attr_tab_camera";
            this.attr_tab_camera.Size = new System.Drawing.Size(356, 479);
            this.attr_tab_camera.TabIndex = 5;
            this.attr_tab_camera.Text = "Camera";
            this.attr_tab_camera.UseVisualStyleBackColor = true;
            // 
            // attr_postprocesses
            // 
            this.attr_postprocesses.AutoScroll = true;
            this.attr_postprocesses.Controls.Add(this.button2);
            this.attr_postprocesses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.attr_postprocesses.Location = new System.Drawing.Point(0, 173);
            this.attr_postprocesses.Name = "attr_postprocesses";
            this.attr_postprocesses.Size = new System.Drawing.Size(356, 306);
            this.attr_postprocesses.TabIndex = 1;
            // 
            // button2
            // 
            this.button2.Dock = System.Windows.Forms.DockStyle.Top;
            this.button2.Location = new System.Drawing.Point(0, 0);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(356, 23);
            this.button2.TabIndex = 0;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // attr_camera
            // 
            this.attr_camera.Dock = System.Windows.Forms.DockStyle.Top;
            this.attr_camera.Location = new System.Drawing.Point(0, 0);
            this.attr_camera.Name = "attr_camera";
            this.attr_camera.Size = new System.Drawing.Size(356, 173);
            this.attr_camera.TabIndex = 0;
            // 
            // MapEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1028, 579);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MapEditor";
            this.Text = "CatsEngine Editor";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MapEditor_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MapEditor_KeyUp);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.renderArea)).EndInit();
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.attr_tab.ResumeLayout(false);
            this.attr_tab_scene.ResumeLayout(false);
            this.attr_tab_model.ResumeLayout(false);
            this.attr_tab_model.PerformLayout();
            this.attr_ml_group.ResumeLayout(false);
            this.attr_ml_group.PerformLayout();
            this.attr_mtl_group.ResumeLayout(false);
            this.attr_clips_group.ResumeLayout(false);
            this.attr_clips_group.PerformLayout();
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            this.attr_tab_material.ResumeLayout(false);
            this.tableLayoutPanel8.ResumeLayout(false);
            this.tableLayoutPanel8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbox_preview)).EndInit();
            this.attr_tab_camera.ResumeLayout(false);
            this.attr_postprocesses.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.SplitContainer splitContainer3;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.ToolStripButton toolStripButton1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem insertToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem gameObjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menu_component;
        private System.Windows.Forms.ListBox modelList;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.ListBox materialList;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteGameObjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addGameObjectToPrefabToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gameObjectFromPrefabToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteProfabToolStripMenuItem;
        private System.Windows.Forms.TabControl attr_tab;
        private System.Windows.Forms.TabPage attr_tab_scene;
        private System.Windows.Forms.TabPage attr_tab_model;
        private System.Windows.Forms.GroupBox attr_ml_group;
        private System.Windows.Forms.TextBox attr_ml_name;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.GroupBox attr_mtl_group;
        private System.Windows.Forms.ComboBox attr_ml_mtrl;
        private System.Windows.Forms.GroupBox attr_clips_group;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.Label label43;
        private System.Windows.Forms.Label label44;
        private System.Windows.Forms.Label label45;
        private System.Windows.Forms.Label label46;
        private System.Windows.Forms.CheckBox attr_ani_autoplay;
        private System.Windows.Forms.TextBox attr_ani_mpf;
        private System.Windows.Forms.TextBox attr_ani_tiltwidth;
        private System.Windows.Forms.TextBox attr_ani_tiltheight;
        private System.Windows.Forms.ComboBox attr_ani_defaultclip;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel attr_ani_clips;
        private System.Windows.Forms.TabPage attr_tab_material;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;
        private System.Windows.Forms.Label label47;
        private System.Windows.Forms.Label label48;
        private System.Windows.Forms.Label label49;
        private System.Windows.Forms.TextBox attr_mtrl_name;
        private System.Windows.Forms.ComboBox attr_mtrl_effect;
        private System.Windows.Forms.TabPage attr_tab_newgo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        public  System.Windows.Forms.ToolStripButton btn_play;
        public  System.Windows.Forms.ToolStripButton btn_stop;
        public  System.Windows.Forms.ToolStripButton btn_pause;
        private System.Windows.Forms.PropertyGrid pg_scene;
        private System.Windows.Forms.TreeView gameObjectTree;
        private System.Windows.Forms.TreeView prefabTree;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.TabPage attr_tab_camera;
        private System.Windows.Forms.PropertyGrid attr_camera;
        private System.Windows.Forms.ToolStripSplitButton movingAxis;
        private System.Windows.Forms.ToolStripMenuItem xYToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox assistXYheight;
        private System.Windows.Forms.ToolStripMenuItem compoundGameObjectMenu;
        private System.Windows.Forms.ToolStripMenuItem newProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scenesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripComboBox resolution_selector;
        private System.Windows.Forms.ToolStripLabel resolution_size_label;
        private System.Windows.Forms.ToolStripMenuItem resourceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rescanPluginToolStripMenuItem;
        private System.Windows.Forms.Button btn_textureSelector;
        private System.Windows.Forms.PictureBox picbox_preview;
        private System.Windows.Forms.ToolStripMenuItem publishProjectToolStripMenuItem;
        private System.Windows.Forms.Panel propertyBox;
        private System.Windows.Forms.Panel attr_postprocesses;
        private System.Windows.Forms.ToolStripMenuItem cameraToolStripMenuItem;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.PictureBox renderArea;
	}
}