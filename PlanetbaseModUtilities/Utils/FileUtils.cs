using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace PlanetbaseModUtilities
{
    public static class FileUtils
    {
        /// <summary>
        ///  Returns the location of the mod's dll file
        /// </summary>
        public static string getModLocation()
        {
            return Assembly.GetEntryAssembly().Location;
        }
        /// <summary>
        /// Returns the location of the game's exe file
        /// </summary>
        public static string getGameLocation()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }
    }
}
