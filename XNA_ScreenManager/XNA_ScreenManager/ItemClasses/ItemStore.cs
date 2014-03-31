using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace XNA_ScreenManager.ItemClasses
{
    public sealed class ItemStore
    {
        public List<Item> item_list { get; set; }

        private static ItemStore instance;
        private ItemStore()
        {
            item_list = new List<Item>();
        }

        public static ItemStore Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ItemStore();
                }
                return instance;
            }
        }

        public void addItem(Item addItem)
        {
            item_list.Add(addItem);
        }

        public void removeItem(string name)
        {
            item_list.Remove(new Item() { itemName = name });
        }

        public Item getItem(int ID)
        {
            return this.item_list.Find(delegate(Item item) { return item.itemID == ID; });
        }

        public void loadItems(string dir, string file)
        {
            using (var reader = new StreamReader(Path.Combine(dir, file)))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    try
                    {
                        if (values[0] != "atkModifier")
                        {
                            this.addItem(Item.create(Convert.ToInt32(values[0]), values[1], (ItemType)Enum.Parse(typeof(ItemType), values[14])));

                            // Link item to item database
                            Item item = this.getItem(Convert.ToInt32(values[0]));

                            // Fill in the item properties if applicable
                            item.itemDescription = Regex.Replace(values[2], "\"", "");
                            item.itemSpritePath = @"" + Regex.Replace(values[3], "\"", "");
                            item.itemSpritePath = Regex.Replace(item.itemSpritePath, " ", "");
                            item.equipSpritePath = @"" + Regex.Replace(values[4], "\"", "");
                            item.equipSpritePath = Regex.Replace(item.equipSpritePath, " ", "");
                            item.SpriteFrameX = Convert.ToInt32(values[5]);
                            item.SpriteFrameY = Convert.ToInt32(values[6]);

                            item.DEF = Convert.ToInt32(values[7]);
                            item.ATK = Convert.ToInt32(values[8]);
                            item.Magic = Convert.ToInt32(values[9]);
                            item.Speed = Convert.ToInt32(values[10]);
                            item.Price = Convert.ToInt32(values[11]);
                            item.WeaponLevel = Convert.ToInt32(values[12]);
                            item.Class = (ItemClass)Enum.Parse(typeof(ItemClass), values[15]);
                            item.Slot = (ItemSlot)Enum.Parse(typeof(ItemSlot), values[16]);
                            item.WeaponType = (WeaponType)Enum.Parse(typeof(WeaponType), values[17]);
                        }
                    }
                    catch (Exception ee)
                    {
                        string exception = ee.ToString();
                    }
                }
            }
        }

        public void saveItem(string dir, string file)
        {
            Type itemType = typeof(Item);
            var props = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                .OrderBy(p => p.Name);

            using (var writer = new StreamWriter(Path.Combine(dir, file)))
            {
                writer.WriteLine(string.Join("; ", props.Select(p => p.Name)));

                foreach (var item in item_list)
                {
                    foreach (PropertyInfo propertyInfo in item.GetType().GetProperties())
                    {
                        if (propertyInfo.Name != "itemID")
                            writer.Write("; ");

                        string value;

                        if (propertyInfo.Name == "itemSpritePath" || propertyInfo.Name == "equipSpritePath" ||
                            propertyInfo.Name == "itemDescription")
                        {
                            value = "\"" + propertyInfo.GetValue(item, null) + "\"";
                            value = Regex.Replace(value, @"\t|\r|\n", "");
                        }
                        else
                        {
                            var getvalue = propertyInfo.GetValue(item, null);
                            value = getvalue.ToString();
                        }

                        writer.Write(value);
                    }

                    writer.WriteLine(";");
                }
            }
        }
    }
}
