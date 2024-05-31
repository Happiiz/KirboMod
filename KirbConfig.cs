using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Terraria.ModLoader.Config;
using RangeAttribute = Terraria.ModLoader.Config.RangeAttribute;

namespace KirboMod
{
    public class KirbConfig : ModConfig
    {
        // ConfigScope.ClientSide should be used for client side, usually visual or audio tweaks.
        // ConfigScope.ServerSide should be used for basically everything else, including disabling items or changing NPC behaviours
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(true)] // This sets the configs default value.
        //[ReloadRequired] // Marking it with [ReloadRequired] makes tModLoader force a mod reload if the option is changed. It should be used for things like item toggles, which only take effect during mod loading
        public bool HyperzoneClouds; // The option
    }
}