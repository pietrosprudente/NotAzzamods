using NotAzzamods.Keybinds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniverseLib.UI;

namespace NotAzzamods.UI.Keybinds
{
    public class KeybindItem : MonoBehaviour, IPointerClickHandler
    {
        public Keybinder.Keybind keybind;

        public bool Selected
        {
            set
            {
                if (value)
                {
                    GetComponent<Image>().color = HacksUIHelper.BGColor2;
                }
                else
                {
                    GetComponent<Image>().color = HacksUIHelper.BGColor1;
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Plugin.KeybindPanel.SelectedKeybind = this;
        }
    }
}
