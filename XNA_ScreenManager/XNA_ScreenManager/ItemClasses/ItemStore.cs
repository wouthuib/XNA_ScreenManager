using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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

        public void loadItems(string file)
        {
            string dir = @"c:\Temp";
            string serializationFile = Path.Combine(dir, file);

            //deserialize
            using (Stream stream = File.Open(serializationFile, FileMode.Open))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                item_list = (List<Item>)bformatter.Deserialize(stream);
            }
        }

        public void saveItem(string file)
        {
            string dir = @"c:\Temp";
            string serializationFile = Path.Combine(dir, file);

            //serialize
            using (Stream stream = File.Open(serializationFile, FileMode.Create))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                bformatter.Serialize(stream, item_list);
            }
        }
    }
}
