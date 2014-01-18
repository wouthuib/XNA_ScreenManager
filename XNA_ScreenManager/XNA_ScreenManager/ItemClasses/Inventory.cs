using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace XNA_ScreenManager.ItemClasses
{
    // Global Inventory Class.
    // Sealed modifier prevents other classes from inheriting A : B.
    // Static with get { return instance; } makes the class Global.

    public sealed class Inventory
    {
        public List<Item> item_list { get; set; }

        private static Inventory instance;
        private Inventory()
        {
            item_list = new List<Item>();
        }

        public static Inventory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Inventory();
                }
                return instance;
            }
        }

        public void addItem(Item addItem)
        {
            item_list.Add(addItem);
        }

        public void removeItem(int ID)
        {
            var onlyMatch = item_list.First(i => i.itemID == ID);
            item_list.Remove(onlyMatch);
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
