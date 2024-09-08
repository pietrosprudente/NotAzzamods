using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NotAzzamods.Hacks.Custom
{
    public class ControllerManager : BaseHack
    {
        public override string Name => "Player Controller Manager";

        public override string Description => "";

        public override void ConstructUI(GameObject root)
        {
            var ui = new HacksUIHelper(root);

            ui.AddSpacer(6);

            ui.CreateToggle("ClothingAbilitiesToggle", "Enable Clothing Abilities", SetClothingAbilitiesEnabled, true);
            ui.CreateToggle("AllowRespawning", "Allow Respawning", (b) => Player.Controller.SetAllowedToRespawn(this, b));

            ui.AddSpacer(6);
        }

        public override void RefreshUI()
        {
        }

        public override void Update()
        {
        }

        public void SetClothingAbilitiesEnabled(bool enabled)
        {
            if(Player != null)
                Player.Controller.ServerSetAllowedCustomClothingAbilities(enabled);
        }
    }
}
