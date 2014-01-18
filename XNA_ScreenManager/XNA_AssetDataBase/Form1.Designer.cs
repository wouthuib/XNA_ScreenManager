namespace XNA_AssetDataBase
{
    partial class Form1
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
            this.SaveXMLButton = new System.Windows.Forms.Button();
            this.LoadXMLButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.ItemsTab = new System.Windows.Forms.TabPage();
            this.ItemIDNum = new System.Windows.Forms.NumericUpDown();
            this.ItemName = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.ItemClearButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.ItemAddButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.ItemDescription = new System.Windows.Forms.TextBox();
            this.ItemSlotBox = new System.Windows.Forms.ComboBox();
            this.ItemJobBox = new System.Windows.Forms.ComboBox();
            this.ItemDEFNum = new System.Windows.Forms.NumericUpDown();
            this.ItemTypeBox = new System.Windows.Forms.ComboBox();
            this.ItemATKNum = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.ItemMATKNum = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.ItemSPDNum = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.ItemGoldNum = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.TabController = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label14 = new System.Windows.Forms.Label();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.comboBoxCondition = new System.Windows.Forms.ComboBox();
            this.labelValue = new System.Windows.Forms.Label();
            this.listBoxActionName = new System.Windows.Forms.ListBox();
            this.textBoxScriptName = new System.Windows.Forms.TextBox();
            this.labelScriptName = new System.Windows.Forms.Label();
            this.AddScriptButton = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.textBoxActionValue = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.LoadScriptButton = new System.Windows.Forms.Button();
            this.SaveScriptButton = new System.Windows.Forms.Button();
            this.SwitchButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.ItemsTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ItemIDNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemDEFNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemATKNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemMATKNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemSPDNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemGoldNum)).BeginInit();
            this.TabController.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // SaveXMLButton
            // 
            this.SaveXMLButton.Location = new System.Drawing.Point(262, 293);
            this.SaveXMLButton.Name = "SaveXMLButton";
            this.SaveXMLButton.Size = new System.Drawing.Size(85, 29);
            this.SaveXMLButton.TabIndex = 26;
            this.SaveXMLButton.Text = "Save to XML";
            this.SaveXMLButton.UseVisualStyleBackColor = true;
            this.SaveXMLButton.Click += new System.EventHandler(this.SaveXMLButton_Click);
            // 
            // LoadXMLButton
            // 
            this.LoadXMLButton.Location = new System.Drawing.Point(371, 293);
            this.LoadXMLButton.Name = "LoadXMLButton";
            this.LoadXMLButton.Size = new System.Drawing.Size(91, 28);
            this.LoadXMLButton.TabIndex = 27;
            this.LoadXMLButton.Text = "Load from XML";
            this.LoadXMLButton.UseVisualStyleBackColor = true;
            this.LoadXMLButton.Click += new System.EventHandler(this.LoadXMLButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.AutoSize = true;
            this.groupBox1.Controls.Add(this.dataGridView1);
            this.groupBox1.Location = new System.Drawing.Point(14, 14);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(5);
            this.groupBox1.Size = new System.Drawing.Size(984, 265);
            this.groupBox1.TabIndex = 28;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Item Table";
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(5, 18);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(974, 242);
            this.dataGridView1.TabIndex = 1;
            // 
            // ItemsTab
            // 
            this.ItemsTab.Controls.Add(this.ItemIDNum);
            this.ItemsTab.Controls.Add(this.LoadXMLButton);
            this.ItemsTab.Controls.Add(this.SaveXMLButton);
            this.ItemsTab.Controls.Add(this.ItemName);
            this.ItemsTab.Controls.Add(this.label11);
            this.ItemsTab.Controls.Add(this.ItemClearButton);
            this.ItemsTab.Controls.Add(this.label1);
            this.ItemsTab.Controls.Add(this.ItemAddButton);
            this.ItemsTab.Controls.Add(this.label2);
            this.ItemsTab.Controls.Add(this.label10);
            this.ItemsTab.Controls.Add(this.label3);
            this.ItemsTab.Controls.Add(this.label9);
            this.ItemsTab.Controls.Add(this.ItemDescription);
            this.ItemsTab.Controls.Add(this.ItemSlotBox);
            this.ItemsTab.Controls.Add(this.ItemJobBox);
            this.ItemsTab.Controls.Add(this.ItemDEFNum);
            this.ItemsTab.Controls.Add(this.ItemTypeBox);
            this.ItemsTab.Controls.Add(this.ItemATKNum);
            this.ItemsTab.Controls.Add(this.label8);
            this.ItemsTab.Controls.Add(this.ItemMATKNum);
            this.ItemsTab.Controls.Add(this.label7);
            this.ItemsTab.Controls.Add(this.ItemSPDNum);
            this.ItemsTab.Controls.Add(this.label6);
            this.ItemsTab.Controls.Add(this.ItemGoldNum);
            this.ItemsTab.Controls.Add(this.label5);
            this.ItemsTab.Controls.Add(this.label4);
            this.ItemsTab.Location = new System.Drawing.Point(4, 22);
            this.ItemsTab.Name = "ItemsTab";
            this.ItemsTab.Padding = new System.Windows.Forms.Padding(3);
            this.ItemsTab.Size = new System.Drawing.Size(973, 337);
            this.ItemsTab.TabIndex = 1;
            this.ItemsTab.Text = "Create Items";
            this.ItemsTab.UseVisualStyleBackColor = true;
            // 
            // ItemIDNum
            // 
            this.ItemIDNum.Location = new System.Drawing.Point(138, 21);
            this.ItemIDNum.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.ItemIDNum.Name = "ItemIDNum";
            this.ItemIDNum.Size = new System.Drawing.Size(180, 20);
            this.ItemIDNum.TabIndex = 27;
            // 
            // ItemName
            // 
            this.ItemName.Location = new System.Drawing.Point(462, 20);
            this.ItemName.Name = "ItemName";
            this.ItemName.Size = new System.Drawing.Size(180, 20);
            this.ItemName.TabIndex = 26;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(26, 23);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(41, 13);
            this.label11.TabIndex = 25;
            this.label11.Text = "Item ID";
            // 
            // ItemClearButton
            // 
            this.ItemClearButton.Location = new System.Drawing.Point(138, 293);
            this.ItemClearButton.Name = "ItemClearButton";
            this.ItemClearButton.Size = new System.Drawing.Size(96, 28);
            this.ItemClearButton.TabIndex = 24;
            this.ItemClearButton.Text = "Clear";
            this.ItemClearButton.UseVisualStyleBackColor = true;
            this.ItemClearButton.Click += new System.EventHandler(this.ItemClearButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(350, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Item Name";
            // 
            // ItemAddButton
            // 
            this.ItemAddButton.Location = new System.Drawing.Point(29, 293);
            this.ItemAddButton.Name = "ItemAddButton";
            this.ItemAddButton.Size = new System.Drawing.Size(85, 28);
            this.ItemAddButton.TabIndex = 23;
            this.ItemAddButton.Text = "Add";
            this.ItemAddButton.UseVisualStyleBackColor = true;
            this.ItemAddButton.Click += new System.EventHandler(this.ItemAddButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Item Type";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(26, 78);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(48, 13);
            this.label10.TabIndex = 22;
            this.label10.Text = "Item Slot";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(26, 105);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Description";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(350, 54);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 13);
            this.label9.TabIndex = 21;
            this.label9.Text = "Profession";
            // 
            // ItemDescription
            // 
            this.ItemDescription.Location = new System.Drawing.Point(138, 102);
            this.ItemDescription.Multiline = true;
            this.ItemDescription.Name = "ItemDescription";
            this.ItemDescription.Size = new System.Drawing.Size(524, 94);
            this.ItemDescription.TabIndex = 5;
            // 
            // ItemSlotBox
            // 
            this.ItemSlotBox.FormattingEnabled = true;
            this.ItemSlotBox.Items.AddRange(new object[] {
            "UpperHead",
            "LowerHead",
            "Neck",
            "Shoulders",
            "UpperBody",
            "LowerBody",
            "Feet",
            "leftHand",
            "rightHand"});
            this.ItemSlotBox.Location = new System.Drawing.Point(138, 75);
            this.ItemSlotBox.Name = "ItemSlotBox";
            this.ItemSlotBox.Size = new System.Drawing.Size(180, 21);
            this.ItemSlotBox.TabIndex = 20;
            // 
            // ItemJobBox
            // 
            this.ItemJobBox.FormattingEnabled = true;
            this.ItemJobBox.Items.AddRange(new object[] {
            "Archer",
            "Fighter",
            "Wizard",
            "Priest",
            "Monk",
            "ArcherFighter",
            "ArcherFighterMonk",
            "WizardPriest",
            "PriestMonk",
            "WizardPriestMonk",
            "FighterMonk",
            "All"});
            this.ItemJobBox.Location = new System.Drawing.Point(462, 51);
            this.ItemJobBox.Name = "ItemJobBox";
            this.ItemJobBox.Size = new System.Drawing.Size(180, 21);
            this.ItemJobBox.TabIndex = 19;
            // 
            // ItemDEFNum
            // 
            this.ItemDEFNum.Location = new System.Drawing.Point(138, 202);
            this.ItemDEFNum.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.ItemDEFNum.Name = "ItemDEFNum";
            this.ItemDEFNum.Size = new System.Drawing.Size(180, 20);
            this.ItemDEFNum.TabIndex = 6;
            // 
            // ItemTypeBox
            // 
            this.ItemTypeBox.FormattingEnabled = true;
            this.ItemTypeBox.Items.AddRange(new object[] {
            "Collectable",
            "Consumable",
            "Weapon",
            "Armor",
            "Accessory",
            "KeyItem"});
            this.ItemTypeBox.Location = new System.Drawing.Point(138, 48);
            this.ItemTypeBox.Name = "ItemTypeBox";
            this.ItemTypeBox.Size = new System.Drawing.Size(180, 21);
            this.ItemTypeBox.TabIndex = 18;
            // 
            // ItemATKNum
            // 
            this.ItemATKNum.Location = new System.Drawing.Point(138, 229);
            this.ItemATKNum.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.ItemATKNum.Name = "ItemATKNum";
            this.ItemATKNum.Size = new System.Drawing.Size(180, 20);
            this.ItemATKNum.TabIndex = 7;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(350, 260);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "Gold Value";
            // 
            // ItemMATKNum
            // 
            this.ItemMATKNum.Location = new System.Drawing.Point(462, 204);
            this.ItemMATKNum.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.ItemMATKNum.Name = "ItemMATKNum";
            this.ItemMATKNum.Size = new System.Drawing.Size(180, 20);
            this.ItemMATKNum.TabIndex = 8;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(350, 233);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(67, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "SPD Modifer";
            // 
            // ItemSPDNum
            // 
            this.ItemSPDNum.Location = new System.Drawing.Point(462, 231);
            this.ItemSPDNum.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.ItemSPDNum.Name = "ItemSPDNum";
            this.ItemSPDNum.Size = new System.Drawing.Size(180, 20);
            this.ItemSPDNum.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(350, 206);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "MATK Modifier";
            // 
            // ItemGoldNum
            // 
            this.ItemGoldNum.Location = new System.Drawing.Point(462, 258);
            this.ItemGoldNum.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.ItemGoldNum.Name = "ItemGoldNum";
            this.ItemGoldNum.Size = new System.Drawing.Size(180, 20);
            this.ItemGoldNum.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(26, 231);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "ATK Modifier";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(26, 204);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "DEF Modifier";
            // 
            // TabController
            // 
            this.TabController.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TabController.Controls.Add(this.ItemsTab);
            this.TabController.Controls.Add(this.tabPage1);
            this.TabController.Location = new System.Drawing.Point(12, 287);
            this.TabController.Name = "TabController";
            this.TabController.SelectedIndex = 0;
            this.TabController.Size = new System.Drawing.Size(981, 363);
            this.TabController.TabIndex = 25;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label14);
            this.tabPage1.Controls.Add(this.listBox2);
            this.tabPage1.Controls.Add(this.comboBoxCondition);
            this.tabPage1.Controls.Add(this.labelValue);
            this.tabPage1.Controls.Add(this.listBoxActionName);
            this.tabPage1.Controls.Add(this.textBoxScriptName);
            this.tabPage1.Controls.Add(this.labelScriptName);
            this.tabPage1.Controls.Add(this.AddScriptButton);
            this.tabPage1.Controls.Add(this.label13);
            this.tabPage1.Controls.Add(this.textBoxActionValue);
            this.tabPage1.Controls.Add(this.label12);
            this.tabPage1.Controls.Add(this.LoadScriptButton);
            this.tabPage1.Controls.Add(this.SaveScriptButton);
            this.tabPage1.Controls.Add(this.SwitchButton);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(973, 337);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Create Script";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(648, 61);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(34, 13);
            this.label14.TabIndex = 45;
            this.label14.Text = "Value";
            // 
            // listBox2
            // 
            this.listBox2.FormattingEnabled = true;
            this.listBox2.Items.AddRange(new object[] {
            "Gold",
            "Level",
            "ItemNum"});
            this.listBox2.Location = new System.Drawing.Point(356, 61);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(120, 30);
            this.listBox2.TabIndex = 44;
            // 
            // comboBoxCondition
            // 
            this.comboBoxCondition.AutoCompleteCustomSource.AddRange(new string[] {
            "bigger than",
            "equal to",
            "smaller than"});
            this.comboBoxCondition.FormattingEnabled = true;
            this.comboBoxCondition.Location = new System.Drawing.Point(505, 62);
            this.comboBoxCondition.Name = "comboBoxCondition";
            this.comboBoxCondition.Size = new System.Drawing.Size(121, 21);
            this.comboBoxCondition.TabIndex = 42;
            // 
            // labelValue
            // 
            this.labelValue.AutoSize = true;
            this.labelValue.Location = new System.Drawing.Point(300, 61);
            this.labelValue.Name = "labelValue";
            this.labelValue.Size = new System.Drawing.Size(34, 13);
            this.labelValue.TabIndex = 41;
            this.labelValue.Text = "Value";
            // 
            // listBoxActionName
            // 
            this.listBoxActionName.FormattingEnabled = true;
            this.listBoxActionName.Items.AddRange(new object[] {
            "dialogue",
            "next",
            "close",
            "chooise",
            "if",
            "goto",
            "label"});
            this.listBoxActionName.Location = new System.Drawing.Point(135, 61);
            this.listBoxActionName.Name = "listBoxActionName";
            this.listBoxActionName.Size = new System.Drawing.Size(134, 69);
            this.listBoxActionName.TabIndex = 39;
            // 
            // textBoxScriptName
            // 
            this.textBoxScriptName.Location = new System.Drawing.Point(135, 26);
            this.textBoxScriptName.Name = "textBoxScriptName";
            this.textBoxScriptName.Size = new System.Drawing.Size(224, 20);
            this.textBoxScriptName.TabIndex = 38;
            // 
            // labelScriptName
            // 
            this.labelScriptName.AutoSize = true;
            this.labelScriptName.Location = new System.Drawing.Point(54, 26);
            this.labelScriptName.Name = "labelScriptName";
            this.labelScriptName.Size = new System.Drawing.Size(65, 13);
            this.labelScriptName.TabIndex = 37;
            this.labelScriptName.Text = "Script Name";
            // 
            // AddScriptButton
            // 
            this.AddScriptButton.Location = new System.Drawing.Point(135, 284);
            this.AddScriptButton.Name = "AddScriptButton";
            this.AddScriptButton.Size = new System.Drawing.Size(96, 29);
            this.AddScriptButton.TabIndex = 36;
            this.AddScriptButton.Text = "Add Line";
            this.AddScriptButton.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(52, 151);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(67, 13);
            this.label13.TabIndex = 35;
            this.label13.Text = "Action Value";
            // 
            // textBoxActionValue
            // 
            this.textBoxActionValue.Location = new System.Drawing.Point(135, 151);
            this.textBoxActionValue.Multiline = true;
            this.textBoxActionValue.Name = "textBoxActionValue";
            this.textBoxActionValue.Size = new System.Drawing.Size(567, 126);
            this.textBoxActionValue.TabIndex = 34;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(51, 61);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(68, 13);
            this.label12.TabIndex = 33;
            this.label12.Text = "Action Name";
            // 
            // LoadScriptButton
            // 
            this.LoadScriptButton.Location = new System.Drawing.Point(542, 284);
            this.LoadScriptButton.Name = "LoadScriptButton";
            this.LoadScriptButton.Size = new System.Drawing.Size(91, 28);
            this.LoadScriptButton.TabIndex = 31;
            this.LoadScriptButton.Text = "Load from XML";
            this.LoadScriptButton.UseVisualStyleBackColor = true;
            // 
            // SaveScriptButton
            // 
            this.SaveScriptButton.Location = new System.Drawing.Point(424, 283);
            this.SaveScriptButton.Name = "SaveScriptButton";
            this.SaveScriptButton.Size = new System.Drawing.Size(85, 29);
            this.SaveScriptButton.TabIndex = 30;
            this.SaveScriptButton.Text = "Save to XML";
            this.SaveScriptButton.UseVisualStyleBackColor = true;
            // 
            // SwitchButton
            // 
            this.SwitchButton.Location = new System.Drawing.Point(27, 285);
            this.SwitchButton.Name = "SwitchButton";
            this.SwitchButton.Size = new System.Drawing.Size(85, 28);
            this.SwitchButton.TabIndex = 28;
            this.SwitchButton.Text = "Switch Grid";
            this.SwitchButton.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1012, 662);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.TabController);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ItemsTab.ResumeLayout(false);
            this.ItemsTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ItemIDNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemDEFNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemATKNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemMATKNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemSPDNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemGoldNum)).EndInit();
            this.TabController.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button SaveXMLButton;
        private System.Windows.Forms.Button LoadXMLButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TabPage ItemsTab;
        private System.Windows.Forms.Button ItemClearButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ItemAddButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox ItemDescription;
        private System.Windows.Forms.ComboBox ItemSlotBox;
        private System.Windows.Forms.ComboBox ItemJobBox;
        private System.Windows.Forms.NumericUpDown ItemDEFNum;
        private System.Windows.Forms.ComboBox ItemTypeBox;
        private System.Windows.Forms.NumericUpDown ItemATKNum;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown ItemMATKNum;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown ItemSPDNum;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown ItemGoldNum;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabControl TabController;
        private System.Windows.Forms.TextBox ItemName;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown ItemIDNum;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button LoadScriptButton;
        private System.Windows.Forms.Button SaveScriptButton;
        private System.Windows.Forms.Button SwitchButton;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button AddScriptButton;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textBoxActionValue;
        private System.Windows.Forms.TextBox textBoxScriptName;
        private System.Windows.Forms.Label labelScriptName;
        private System.Windows.Forms.ListBox listBoxActionName;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.ComboBox comboBoxCondition;
        private System.Windows.Forms.Label labelValue;
    }
}

