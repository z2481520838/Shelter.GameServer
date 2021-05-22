using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServerLib.Maps
{
    class MapObject
    {
        /// <summary>
        /// The Name of the Map Object, as read from room.dsc (Content/LeagueSandbox-Default/Maps/Map(x)/Scene)
        /// </summary>
        public String Name{ get; private set; }

        /// <summary>
        /// The Central Point of the Object in the Map, Loaded from : Content/LeagueSandbox-Default/Maps/Map(x)
        /// </summary>
        public Vector3 CentralPoint { get; private set; }

    }
}
