using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNA_ScreenManager.ItemClasses
{
    public enum ItemType
    {
        Collectable,
        Consumable,
        Weapon,
        Armor,
        Accessory,
        KeyItem
    };

    public enum ItemClass
    {
        Archer,
        Fighter,
        Wizard,
        Priest,
        Monk,
        ArcherFighter,
        ArcherFighterMonk,
        WizardPriest,
        PriestMonk,
        WizardPriestMonk,
        FighterMonk,
        All
    };

    public enum ItemSlot
    {
        UpperHead,
        LowerHead,
        Head,           // complete head i.e. helmet
        Neck,           // necklace and scarf
        Shoulders,      // shoulds i.e. cape
        UpperBody,
        LowerBody,
        Body,           // complete body i.e. cloak
        Feet,           // Feet i.e. boots
        leftHand,
        rightHand,
        Hands,          // both hands i.e. two-handed sword and bows
        accessory1,
        accessory2
    };

    [Serializable]
    public class Item
    {
        public int itemID { get; set; }
        public string itemName { get; set; }
        public string itemDescription { get; set; }

        public int defModifier { get; set; }
        public int atkModifier { get; set; }
        public int magicModifier { get; set; }
        public int speedModifier { get; set; }
        public int Value { get; set; }

        public ItemType itemType { get; set; }
        public ItemClass itemClass { get; set; }
        public ItemSlot itemSlot { get; set; }

        public static Item create(
            int identifier,
            string name, string description,
            int defMod, int atkMod, int mgcMod, int spdMod, int value,
            ItemType type, ItemClass itemclass, ItemSlot slot)
        {
            var results = new Item();

            results.itemID = identifier;
            results.itemType = type;
            results.itemName = name;
            results.itemDescription = description;
            results.defModifier = defMod;
            results.atkModifier = atkMod;
            results.magicModifier = mgcMod;
            results.speedModifier = spdMod;
            results.Value = value;
            results.itemClass = itemclass;
            results.itemSlot = slot;

            return results;
        }
    }
}
