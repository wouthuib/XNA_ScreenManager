﻿using System;
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

    public enum WeaponType
    {
        Dagger,
        One_handed_Sword,
        Two_handed_Sword,
        One_handed_Spear,
        Two_handed_Spear,
        One_handed_Axe,
        Two_handed_Axe,
        Mace,
        Staff,
        Bow,
        None
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
        Weapon,         // both hands i.e. two-handed sword and bows
        Shield,         // shoulds i.e. cape
        Headgear,       // complete head i.e. helmet
        Neck,           // necklace and scarf
        Bodygear,       // complete body i.e. cloak
        Feet,           // Feet i.e. boots
        Accessory,      // rings etc..
    };

    [Serializable]
    public class Item
    {
        public int itemID { get; set; }
        public string itemName { get; set; }
        public string itemDescription { get; set; }

        public string itemSpritePath { get; set; }
        public string equipSpritePath { get; set; }
        public int SpriteFrameX { get; set; }
        public int SpriteFrameY { get; set; }

        public int defModifier { get; set; }
        public int atkModifier { get; set; }
        public int magicModifier { get; set; }
        public int speedModifier { get; set; }
        public int Value { get; set; }
        public int WeaponLevel { get; set; }
        public int RefinementBonus { get; set; }

        public ItemType itemType { get; set; }
        public ItemClass itemClass { get; set; }
        public ItemSlot itemSlot { get; set; }
        public WeaponType itemWaponType { get; set; }

        public static Item create(int identifier, string name, ItemType type)
        {
            var results = new Item();

            results.itemID = identifier;
            results.itemType = type;
            results.itemName = name;
            results.WeaponLevel = 1;
            results.RefinementBonus = 0;
            results.itemWaponType = WeaponType.None;

            return results;
        }
    }
}
