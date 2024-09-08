using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniverseLib.UI.Models;

namespace NotAzzamods.Keybinds
{
    public class ButtonKeybinder : Keybinder
    {
        public ButtonRef button;

        public override void OnRightClicked()
        {

        }

        public class ButtonKeybind : Keybind
        {
            public ButtonKeybind(Keybinder keybinder) : base(keybinder)
            {

            }

            public override void OnPressed()
            {
                var buttonKeybinder = keybinder as ButtonKeybinder;

                if (buttonKeybinder != null)
                {
                    buttonKeybinder.button.OnClick();
                }
            }
        }
    }
}
