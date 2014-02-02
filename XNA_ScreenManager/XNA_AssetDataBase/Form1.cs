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
        ItemStore itemDB = ItemStore.Instance;

        public Form1()
        {
            InitializeComponent();
        }

        private void ItemAddButton_Click(object sender, EventArgs e)
        {
            // String builder
            StringBuilder description = new StringBuilder();
            foreach(var line in this.ItemDescription.Lines)
                description.Append(line.ToString() + Environment.NewLine);

            // Create the an empty item in item database
            if (itemDB.item_list.FindAll(delegate(Item item) { return item.itemID == Convert.ToInt32(this.ItemIDNum.Value); }).Count == 0)
            {
                try
                {
                    // Get the itemtype text to enum
                    ItemType itemType = (ItemType)Enum.Parse(typeof(ItemType), this.ItemTypeBox.Text.ToString());

                    itemDB.addItem(Item.create(Convert.ToInt32(this.ItemIDNum.Value), this.ItemName.Text.ToString(), itemType));

                    // Link item to item database
                    Item item = itemDB.getItem(Convert.ToInt32(this.ItemIDNum.Value));

                    // Fill in the item properties if applicable
                    item.itemDescription = description.ToString();
                    item.itemSpritePath = @"" + ItemSpritePath.Text;
                    item.equipSpritePath = @"" + EquipSpitePath.Text;
                    item.SpriteFrameX = Convert.ToInt32(this.itemSpriteFrameX.Value);
                    item.SpriteFrameY = Convert.ToInt32(this.itemSpriteFrameY.Value);
                    item.defModifier = Convert.ToInt32(this.ItemDEFNum.Value);
                    item.atkModifier = Convert.ToInt32(this.ItemATKNum.Value);
                    item.magicModifier = Convert.ToInt32(this.ItemMATKNum.Value);
                    item.speedModifier = Convert.ToInt32(this.ItemSPDNum.Value);
                    item.Value = Convert.ToInt32(this.ItemGoldNum.Value);
                    item.itemClass = (ItemClass)Enum.Parse(typeof(ItemClass), this.ItemJobBox.Text.ToString());
                    item.itemSlot = (ItemSlot)Enum.Parse(typeof(ItemSlot), this.ItemSlotBox.Text.ToString());
                }
                catch (Exception ee)
                {
                    WinformPopup popup = new WinformPopup(ee.ToString());
                    DialogResult dialogresult = popup.ShowDialog();
                }

                updateGrid();
            }
        }

        private void ItemClearButton_Click(object sender, EventArgs e)
        {
            clearInput();
        }

        private void LoadXMLButton_Click(object sender, EventArgs e)
        {
            itemDB.loadItems(@"..\..\..\XNA_ScreenManagerContent\itemDB\","itemtable.bin");
            updateGrid();
        }

        private void SaveXMLButton_Click(object sender, EventArgs e)
        {
            itemDB.saveItem(@"..\..\..\XNA_ScreenManagerContent\itemDB\", "itemtable.bin");
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            updateGrid();
        }

        private void updateGrid()
        {
            this.dataGridView1.DataSource = null;
            this.dataGridView1.DataSource = itemDB.item_list;
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
            this.itemSpriteFrameX.Value = 0;
            this.itemSpriteFrameY.Value = 0;

            this.ItemSlotBox.Text = null;
            this.ItemJobBox.Text = null;
            this.ItemTypeBox.Text = null;
            this.ItemName.Text = null;
            this.ItemDescription.Text = null;
            this.ItemSpritePath.Text = @"gfx\effects\item_spritesheet1";
            this.EquipSpitePath.Text = @"gfx\player\costume\hunter_clothes01";

        }
    }
}
