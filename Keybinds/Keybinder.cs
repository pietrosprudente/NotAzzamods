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
        public List<Keybind> keybinds = new();

        public void OnPointerClick(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Right)
            {
                OnRightClicked();
            }
        }

        void Update()
        {
            foreach(var keybind in keybinds)
            {
                if(keybind.isCapturing)
                {
                    keybind.DetectKeybinds();
                }
            }
        }

        public abstract void OnRightClicked();

        public abstract Keybind CreateKeybind();

        public abstract class Keybind
        {
            public KeyCode? primaryKey = null;
            public List<KeyCode> secondaryKeys = new();
            public Keybinder keybinder;

            public Keybind(Keybinder keybinder)
            {
                this.keybinder = keybinder;
                KeybindManager.AddKeybind(this);
            }

            public virtual bool IsPressed()
            {
                if(!this.primaryKey.HasValue)
                {
                    return false;
                }

                KeyCode primaryKey = this.primaryKey.Value;

                if (!Input.GetKeyDown(primaryKey))
                {
                    Debug.Log("no primary");
                    return false;
                }

                foreach(var key in secondaryKeys)
                {
                    if(!Input.GetKey(key))
                    {
                    Debug.Log("no secondary");
                        return false;
                    }
                }

                Debug.Log("yessir");

                return true;
            }

            public bool isCapturing = false;

            public virtual void StartDetectKeybind()
            {
                primaryKey = null;
                secondaryKeys = new();
                isCapturing = true;
            }

            public virtual void StopDetectKeybind()
            {
                isCapturing = false;
            }

            public virtual void DetectKeybinds()
            {
                KeyCode? primaryKey = null;
                List<KeyCode> secondaryKeys = new();

                foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKey(key))
                    {
                        if (!IsModifierKey(key))
                        {
                            primaryKey = key;
                            continue;
                        }

                        secondaryKeys.Add(key);
                    }
                }

                if (primaryKey.HasValue)
                {
                    this.primaryKey = primaryKey.Value;
                    this.secondaryKeys = secondaryKeys;
                    StopDetectKeybind();
                }
            }

            private KeyCode[] modifierKeys = { KeyCode.LeftShift, KeyCode.RightShift, KeyCode.LeftControl, KeyCode.RightControl, KeyCode.LeftAlt, KeyCode.RightAlt };

            private bool IsModifierKey(KeyCode keyCode)
            {
                foreach (var modifier in modifierKeys)
                {
                    if (keyCode == modifier)
                        return true;
                }
                return false;
            }

            public abstract void OnPressed();

            public abstract void CreateScrollItem(GameObject root);

            public abstract void RefreshScrollItem();
        }
    }
}
