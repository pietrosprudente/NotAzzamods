using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NotAzzamods.Keybinds
{
    public abstract class Keybinder : MonoBehaviour, IPointerClickHandler
    {
        public List<Keybind> keybinds;

        public void OnPointerClick(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Right)
            {
                OnRightClicked();
            }
        }

        public abstract void OnRightClicked();

        public abstract class Keybind
        {
            public KeyCode primaryKey;
            public List<KeyCode> secondaryKeys;
            public Keybinder keybinder;

            public Keybind(Keybinder keybinder)
            {
                this.keybinder = keybinder;
                KeybindManager.AddKeybind(this);
            }

            public virtual bool IsPressed()
            {
                if(!Input.GetKeyDown(primaryKey))
                {
                    return false;
                }

                foreach(var key in secondaryKeys)
                {
                    if(!Input.GetKey(key))
                    {
                        return false;
                    }
                }

                return true;
            }

            public abstract void OnPressed();
        }
    }
}
