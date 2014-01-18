using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XNA_ScreenManager.ItemClasses;
using XNA_ScreenManager.ScriptClasses;
using System.Reflection;

namespace XNA_AssetDataBase
{
    public partial class Form1 : Form
    {
        // we use the inventory class for the ITEM database
        Inventory inventory = Inventory.Instance;
        ScriptInterpreter scriptManager = ScriptInterpreter.Instance;
        //int scriptIndex = 1;

        public Form1()
        {
            InitializeComponent();
        }

        private void ItemAddButton_Click(object sender, EventArgs e)
        {
            int itemID = Convert.ToInt32(this.ItemIDNum.Value);
            string itemName = this.ItemName.Text.ToString();
            
            StringBuilder description = new StringBuilder();
            foreach(var line in this.ItemDescription.Lines)
                description.Append(line.ToString() + Environment.NewLine);

            int itemDEF = Convert.ToInt32(this.ItemDEFNum.Value),
                itemATK = Convert.ToInt32(this.ItemATKNum.Value),
                itemMATK = Convert.ToInt32(this.ItemMATKNum.Value),
                itemSPD = Convert.ToInt32(this.ItemSPDNum.Value),
                itemGold = Convert.ToInt32(this.ItemGoldNum.Value);

            ItemType itemType = (ItemType)Enum.Parse(typeof(ItemType), this.ItemTypeBox.Text.ToString());
            ItemClass itemJob = (ItemClass)Enum.Parse(typeof(ItemClass), this.ItemJobBox.Text.ToString());
            ItemSlot itemSlot = (ItemSlot)Enum.Parse(typeof(ItemSlot), this.ItemSlotBox.Text.ToString());


            inventory.addItem(Item.create(
                                       itemID, 
                                       itemName,
                                       description.ToString(),
                                       itemDEF,
                                       itemATK,
                                       itemMATK,
                                       itemSPD,
                                       itemGold,
                                       itemType,
                                       itemJob,
                                       itemSlot));
            updateGrid();
        }

        private void ItemClearButton_Click(object sender, EventArgs e)
        {
            clearInput();
        }

        private void LoadXMLButton_Click(object sender, EventArgs e)
        {
            inventory.loadItems("itemtable.bin");
            updateGrid();
        }

        private void SaveXMLButton_Click(object sender, EventArgs e)
        {
            inventory.saveItem("itemtable.bin");
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            updateGrid();
        }

        private void updateGrid()
        {
            this.dataGridView1.DataSource = null;
            this.dataGridView1.DataSource = inventory.item_list;
            clearInput();
        }

        private void clearInput()
        {
            this.ItemIDNum.Value = 0;
            this.ItemDEFNum.Value = 0;
            this.ItemATKNum.Value = 0;
            this.ItemMATKNum.Value = 0;
            this.ItemSPDNum.Value = 0;
            this.ItemGoldNum.Value = 0;

            this.ItemSlotBox.Text = null;
            this.ItemJobBox.Text = null;
            this.ItemTypeBox.Text = null;
            this.ItemName.Text = null;
            this.ItemDescription.Text = null;

        }
    }
}
