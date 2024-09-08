using NotAzzamods.Hacks;
using ShadowLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UniverseLib.UI;

namespace NotAzzamods.UI.TabMenus
{
    public class BaseTab
    {
        public BaseTab()
        {
            Plugin.TabMenus.Add(this);
        }

        public string Name;

        protected HacksUIHelper ui;

        private GameObject root;

        public virtual void ConstructUI(GameObject root)
        {
            this.root = root;
            ui = new(root);

            root.SetActive(false);
        }

        public virtual GameObject ConstructTabButton(HacksUIHelper ui)
        {
            var btn = ui.CreateButton( "<b>" + Name + "</b>  ", () =>
            {
                SetTabActive(!root.activeSelf);
            });
            UIFactory.SetLayoutElement(btn.GameObject, minHeight: 32);

            return btn.GameObject;
        }

        public virtual void SetTabActive(bool active)
        {
            root.SetActive(active);
            if(active)
            {
                Plugin.MainPanel.CurrentTab = this;

                if (Plugin.MainPanel.oldTab != null && Plugin.MainPanel.oldTab != this)
                    Plugin.MainPanel.oldTab.SetTabActive(false);

                Plugin.MainPanel.oldTab = this;

                RefreshUI();
            }
        }

        public virtual void UpdateUI()
        {

        }

        public virtual void RefreshUI()
        {

        }
    }
}
