﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;
using UniverseLib.UI.Models;

namespace NotAzzamods.Keybinds
{
    public class SliderKeybinder : Keybinder
    {
        public Slider slider;

        public override void OnRightClicked()
        {
            Plugin.KeybindPanel.Enabled = true;
            Plugin.KeybindPanel.Keybinder = this;
        }

        public override Keybind CreateKeybind()
        {
            var keybind = new SliderKeybind(this);
            keybinds.Add(keybind);
            return keybind;
        }

        public class SliderKeybind : Keybind
        {
            public SliderKeybind(Keybinder keybinder) : base(keybinder)
            {

            }

            public override void OnPressed()
            {
                var sliderKeybinder = keybinder as SliderKeybinder;

                if (sliderKeybinder != null)
                {
                    sliderKeybinder.slider.value = value;
                }
            }

            private Text text;
            private float value = 0;
            private Slider slider;

            public override void CreateScrollItem(GameObject root)
            {
                UIFactory.SetLayoutElement(UIFactory.CreateUIObject("spacer", root), 0, 6, 9999, 0);

                var dropdownGroup = UIFactory.CreateUIObject("dropdownGroup", root);
                UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(dropdownGroup, false, false, true, true);

                UIFactory.SetLayoutElement(UIFactory.CreateUIObject("spacer", dropdownGroup), 6, 0, 0, 9999);

                var button = UIFactory.CreateButton(dropdownGroup, "button", "Set Keybind", HacksUIHelper.ButtonColor);
                button.OnClick = StartDetectKeybind;
                UIFactory.SetLayoutElement(button.GameObject, 128, 48, 0, 0);

                UIFactory.SetLayoutElement(UIFactory.CreateUIObject("spacer", dropdownGroup), 6, 0, 0, 9999);

                text = UIFactory.CreateLabel(dropdownGroup, "text", "No Keys Selected");
                UIFactory.SetLayoutElement(text.gameObject, 9999, 48, 9999, 0);

                UIFactory.SetLayoutElement(UIFactory.CreateUIObject("spacer", root), 0, 6, 9999, 0);

                var inputGroup = UIFactory.CreateHorizontalGroup(root, "inputGroup", true, false, false, true);
                UIFactory.SetLayoutElement(inputGroup, 0, 0, 9999, 9999);

                UIFactory.SetLayoutElement(UIFactory.CreateUIObject("spacer", inputGroup), 6, 0, 0, 9999);

                var sliderObj = UIFactory.CreateSlider(inputGroup, "slider", out slider);
                slider.onValueChanged.AddListener((value) => this.value = value);
                UIFactory.SetLayoutElement(sliderObj, 0, 32, 9999, 0);

                UIFactory.SetLayoutElement(UIFactory.CreateUIObject("spacer", inputGroup), 6, 0, 0, 9999);

                UIFactory.SetLayoutElement(UIFactory.CreateUIObject("spacer", root), 0, 6, 9999, 0);
            }

            public override void RefreshScrollItem()
            {
                if(!primaryKey.HasValue)
                {
                    return;
                }

                slider.maxValue = ((SliderKeybinder)keybinder).slider.maxValue;
                slider.minValue = ((SliderKeybinder)keybinder).slider.minValue;
                text.text = string.Join(" + ", Array.ConvertAll(secondaryKeys.ToArray(), key => key.ToString())) + " + " + ((KeyCode)primaryKey).ToString();
            }

            public override void StartDetectKeybind()
            {
                base.StartDetectKeybind();
                text.text = "Press Any Key...";
            }

            public override void StopDetectKeybind()
            {
                base.StopDetectKeybind();
                
                if(primaryKey == null)
                {
                    text.text = "No Keys Selected";
                    return;
                }

                text.text = string.Join(" + ", Array.ConvertAll(secondaryKeys.ToArray(), key => key.ToString())) + " + " + ((KeyCode)primaryKey).ToString();
            }
        }
    }
}
