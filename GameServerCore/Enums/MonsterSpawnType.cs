namespace GameServerCore.Enums
{
    public enum MonsterSpawnType : byte
    {
        //OLD SUMMONERS RIFT && OLD TWISTED TREELINE

        /// <summary>
        /// Old Baron
        /// </summary>
        WORM = 0x00,

        /// <summary>
        /// Old Dragon
        /// </summary>
        DRAGON = 0x01,

        /// <summary>
        /// Old Gromp
        /// </summary>
        WRAITH = 0x02,

        /// <summary>
        /// Old Blue Buff
        /// </summary>
        ANCIENT_GOLEM = 0x03,

        /// <summary>
        /// Old Blue Buff's Guards
        /// </summary>
        YOUNG_LIZARD_ANCIENT = 0x04,

        /// <summary>
        /// Old Big/Main Wolf
        /// </summary>
        GIANT_WOLF = 0x05,

        /// <summary>
        /// Old Smaller Wolfs
        /// </summary>
        WOLF = 0x06,

        /// <summary>
        /// Old Chickens
        /// </summary>
        GREAT_WRAITH = 0x07,

        /// <summary>
        /// Old Smaller Chickens
        /// </summary>
        LESSER_WRAITH = 0x08,

        /// <summary>
        /// Old Red Buff
        /// </summary>
        ELDER_LIZARD = 0x09,

        /// <summary>
        /// Old Blue Buff's Guards
        /// </summary>
        YOUNG_LIZARD_ELDER = 0x0A,

        /// <summary>
        /// Old Krugs
        /// </summary>
        GOLEM = 0X0B,

        /// <summary>
        /// Old Smaller Krug
        /// </summary>
        LESSER_GOLEM = 0X0C,


        //NEW SUMMONERS RIFT

        /// <summary>
        /// New Baron
        /// </summary>
        SRU_BARON = 0x0D,

        /// <summary>
        /// New Dragon
        /// </summary>
        SRU_DRAGON = 0x0E,

        /// <summary>
        /// Gromp
        /// </summary>
        SRU_GROMP = 0x0F,

        /// <summary>
        /// New Blue Buff
        /// </summary>
        SRU_BLUE = 0x10,

        /// <summary>
        /// New Blue Buff Guards
        /// </summary>
        SRU_BLUEMINI = 0x11,

        /// <summary>
        /// New Blue Buff Guards
        /// </summary>
        SRU_BLUEMINI2 = 0x12,

        /// <summary>
        /// New Wolfs
        /// </summary>
        SRU_MURKWOLF = 0x13,

        /// <summary>
        /// New Smaller Wolfs
        /// </summary>
        SRU_MURKWOLF_MINI = 0x14,

        /// <summary>
        /// New Big Chicken
        /// </summary>
        SRU_RAZORBEAK = 0x15,

        /// <summary>
        /// New Smaller Chickens
        /// </summary>
        SRU_RAZORBRAK_MINI = 0x16,

        /// <summary>
        /// New Red Buff
        /// </summary>
        SRU_RED = 0x17,

        /// <summary>
        /// New Red Buff Guards
        /// </summary>
        SRU_RED_MINI = 0x18,

        /// <summary>
        /// New Krug
        /// </summary>
        SRU_KRUG = 0x19,

        /// <summary>
        /// New Smaller Krug
        /// </summary>
        SRU_KRUG_MINI = 0x1A,

        //NEW TWISTED TREELINE

        /// <summary>
        /// Vilemaw
        /// </summary>
        TT_SPIDERBOSS = 0x1B,

        /// <summary>
        /// 
        /// </summary>
        TT_GOLEM = 0x1C,

        /// <summary>
        /// 
        /// </summary>
        TT_GOLEM2 = 0x1D,

        /// <summary>
        /// 
        /// </summary>
        TT_NWOLF = 0x1E,

        /// <summary>
        /// 
        /// </summary>
        TT_NWOLF2 = 0x1F,

        /// <summary>
        /// 
        /// </summary>
        TT_NWRAITH = 0x20,

        /// <summary>
        /// 
        /// </summary>
        TT_NWRAITH2 = 0x21,

        /// <summary>
        /// 
        /// </summary>
        TT_RELIC = 0x22,
    }

    public enum MonsterCampType : byte
    {
        //TODO: FIGURE OUT WHY THIS HAS TO BE SEPARATED BY BLUE/RED TEAM, SINCE NOT USING SEPARATION BREACK THE MINIMAP ICONS
        DRAGON = 0x00,
        BARON = 0x01,
        TT_SPIDERBOSS = 0X02,
        BLUE_RED_BUFF = 0x03,
        BLUE_BLUE_BUFF = 0x04,
        BLUE_GROMP = 0x05,
        BLUE_WRAITHS = 0x06,
        BLUE_GOLEMS = 0x07,
        BLUE_WOLVES = 0x08,
        RED_RED_BUFF = 0x09,
        RED_BLUE_BUFF = 0x0A,
        RED_GROMP = 0x0B,
        RED_WRAITHS = 0x0C,
        RED_GOLEMS = 0x0D,
        RED_WOLVES = 0x0E
    }
}