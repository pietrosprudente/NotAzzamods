using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace NotAzzamods.Hacks.Custom
{
    public class ActionEnterExitInteract : BaseHack
    {
        public override string Name => "Enter Exit Interact";

        public override string Description => "";

        private global::ActionEnterExitInteract action;

        private GameObject root;

        private Toggle knockoutToggle;
        private Toggle lockToggle;
        private Toggle interactableToggle;

        public override void ConstructUI(GameObject root)
        {
            this.root = root;
            var ui = new HacksUIHelper(root);

            ui.AddSpacer(6);

            knockoutToggle = ui.CreateToggle("knockoutToggle", "Should knockout if going fast", (b) => action.SetShouldKnockoutbPlayerIfGoingFast(b), true);

            ui.AddSpacer(6);

            ui.CreateLBBTrio("Evacuate Players", "Evactuate", () => action.EvacuateAll(), "Evacuate All Players", () => action.EvacuateAllExceptDriver(), "Evacuate All Except Driver");

            ui.AddSpacer(6);

            ui.CreateLBDuo("Evacuate Selected Player", "evacuatePlayer", () => action.EvacuatePlayer(Player.Controller), "Evacuate");

            ui.AddSpacer(6);

            ui.CreateToggle("lock", "Is vehicle locked", (b) => action.SetLocked(b));
            ui.CreateToggle("interactable", "Is vehicle interactable", (b) => action.SetInteractable(b));

            ui.AddSpacer(6);
        }

        public override void RefreshUI()
        {
            if(Player.Controller.GetPlayerControllerInteractor().GetEnteredAction() != null)
            {
                var action = Player.Controller.GetPlayerControllerInteractor().GetEnteredAction();

                if(action is global::ActionEnterExitInteract)
                {
                    this.action = (global::ActionEnterExitInteract) action;
                }

                knockoutToggle.isOn = (bool)typeof(global::ActionEnterExitInteract).GetField("bKnockoutPlayerIfGoingFast", Plugin.Flags).GetValue(this.action);
                lockToggle.isOn = this.action.IsLocked(Player.Controller);
                interactableToggle.isOn = (bool)typeof(global::ActionEnterExitInteract).GetField("bInteractable", Plugin.Flags).GetValue(this.action);

                root.SetActive(true);
            }
            else
            {
                root.SetActive(false);
            }
        }

        public override void Update()
        {
        }
    }
}
