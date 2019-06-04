using System.ComponentModel.DataAnnotations;

namespace WoA.Lib.Blizzard
{
    public enum WowClass
    {
        Warrior = 1,
        Paladin = 2,
        Hunter = 3,
        Rogue = 4,
        Priest = 5,
        [Display(Name = "Death Knight")]
        DeathKnight = 6,
        Shaman = 7,
        Mage = 8,
        Warlock = 9,
        Monk = 10,
        Druid = 11,
        [Display(Name = "Demon Hunter")]
        DemonHunter = 12
    }
    public enum WowRace
    {
        Human = 1,
        Orc = 2,
        Dwarf = 3,
        [Display(Name = "Night Elf")]
        NightElf = 4,
        Undead = 5,
        Tauren = 6,
        Gnome = 7,
        Troll = 8,
        Goblin = 9,
        [Display(Name = "Blood Elf")]
        BloodElf = 10,
        Draenei = 11,
        [Display(Name = "Fel Orc")]
        FelOrc = 12,
        Naga = 13,
        Broken = 14,
        Skeleton = 15,
        Vrykul = 16,
        Tuskarr = 17,
        [Display(Name = "Forest Troll")]
        ForestTroll = 18,
        Taunka = 19,
        [Display(Name = "Northrend Skeleton")]
        NorthrendSkeleton = 20,
        [Display(Name = "Ice Troll")]
        IceTroll = 21,
        Worgen = 22,
        Gilnean = 23,
        Pandaren = 24,
        [Display(Name = "Pandaren")]
        Pandaren2 = 25,
        [Display(Name = "Pandaren")]
        Pandaren3 = 26,
        Nightborne = 27,
        [Display(Name = "Highmountain Tauren")]
        HighmountainTauren = 28,
        [Display(Name = "Void Elf")]
        VoidElf = 29,
        [Display(Name = "Lightforged Draenei")]
        LightforgedDraenei = 30,
        [Display(Name = "Zandalari Troll")]
        ZandalariTroll = 31,
        [Display(Name = "Kul Tiran")]
        KulTiran = 32,
        [Display(Name = "Thin Human")]
        ThinHuman = 33,
        [Display(Name = "Dark Iron Dwarf")]
        DarkIronDwarf = 34,
        Vulpera = 35,
        [Display(Name = "Mag'har Orc")]
        MagharOrc = 36,
        Mechagnome = 37
    }
}