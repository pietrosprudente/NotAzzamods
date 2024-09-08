using NotAzzamods.UI.TabMenus;
using ShadowLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NotAzzamods.Hacks
{
    public abstract class BaseHack
    {
        public abstract string Name { get; }
        public abstract string Description { get; }

        public PlayerRef Player { get; set; }

        public BaseHack()
        {
            Plugin.Hacks.Add(this);
        }

        public abstract void ConstructUI(GameObject root);

        public abstract void Update();

        public abstract void RefreshUI();
    }
}
