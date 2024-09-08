using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UniverseLib.UI;
using UniverseLib.UI.Panels;

namespace NotAzzamods.UI.Keybinds
{
    public abstract class KeybindPanel : PanelBase
    {
        public KeybindPanel(UIBase owner) : base(owner)
        {
        }

        public override string Name => "Edit Keybinds";

        public override int MinWidth => 612;

        public override int MinHeight => 418;

        public override Vector2 DefaultAnchorMin => new(.5f, .5f);

        public override Vector2 DefaultAnchorMax => new(.5f, .5f);

        protected override void ConstructPanelContent()
        {
            var root = UIFactory.CreateVerticalGroup(ContentRoot, "root", false, false, true, true, 0, default, new(.102f, .157f, .216f));
            UIFactory.SetLayoutElement(root, 0, 0, 9999, 9999);

            var keybinds = UIFactory.CreateScrollView(root, "keybinds", out var keybindContent, out var _, new(.114f, .129f, .161f));
            UIFactory.SetLayoutElement(keybinds, 0, MinHeight - 64, 9999, 0);
        }

        protected abstract void ConstructKeybindUI();

        public void _SetActive(bool active)
        {
            SetDefaultSizeAndPosition();
            SetActive(active);
        }
    }
}
