using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NotAzzamods.Keybinds
{
    public class KeybindManager : MonoBehaviour
    {
        private static List<Keybinder.Keybind> Keybinds = new List<Keybinder.Keybind>();

        void Update()
        {
            if (Plugin.UiBase == null || Plugin.UiBase.Enabled)
            {
                return;
            }

            foreach (var keybind in Keybinds)
            {
                if(keybind.IsPressed())
                {
                    keybind.OnPressed();
                }
            }
        }

        public static Keybinder.Keybind AddKeybind(Keybinder.Keybind keybind)
        {
            Keybinds.Add(keybind);
            return keybind;
        }

        public static void RemoveKeybind(Keybinder.Keybind keybind)
        {
            Keybinds.Remove(keybind);
        }
    }
}
