using HawkNetworking;
using Rewired;
using ShadowLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.UI;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.ObjectPool;
using UniverseLib.UI.Widgets.ScrollView;

namespace NotAzzamods.UI.TabMenus
{
    public class PropSpawnerTab : BaseTab
    {
        public GameObject SelectedProp
        {
            set
            {
                selectedObject = value;
                spawnBtn.ButtonText.text = $"Spawn Prop ({value.name.Replace(" (UnityEngine.GameObject)", "")})";
            }
        }

        private ScrollPool<PropCell> scrollPool;
        private GameObject selectedObject;
        private PropCellHandler cellHandler;
        private ButtonRef spawnBtn;

        public PropSpawnerTab()
        {
            Name = "Prop Spawner";
        }

        public override void ConstructUI(GameObject root)
        {
            base.ConstructUI(root);

            UIFactory.SetLayoutElement(UIFactory.CreateUIObject("spacer", root), 0, 5, 9999, 0);

            var scrollGroup = UIFactory.CreateHorizontalGroup(root, "group", true, false, true, true, bgColor: new(0, 0, 0, 0));
            UIFactory.SetLayoutElement(scrollGroup, 0, 620, 9999, 0);

            UIFactory.SetLayoutElement(UIFactory.CreateUIObject("spacer", scrollGroup), 5, 0, 0, 9999);

            scrollPool = UIFactory.CreateScrollPool<PropCell>(scrollGroup, "scroll", out var scrollRoot, out var scrollContent, new Color(.114f, .129f, .161f));
            UIFactory.SetLayoutElement(scrollRoot, 0, 0, 9999, 9999);
            UIFactory.SetLayoutElement(scrollContent, 0, 0, 9999, 9999);

            cellHandler = new(this);
            scrollPool.Initialize(cellHandler);

            UIFactory.SetLayoutElement(UIFactory.CreateUIObject("spacer", scrollGroup), 5, 0, 0, 9999);
            UIFactory.SetLayoutElement(UIFactory.CreateUIObject("spacer", root), 0, 5, 9999, 0);

            spawnBtn = UIFactory.CreateButton(root, "SpawnBtn", "Spawn Prop");
            spawnBtn.OnClick = () =>
            {
                var player = PlayerUtils.GetMyPlayer();
                if (player == null || !selectedObject) return;

                var character = player.GetPlayerCharacter();
                var pos = character.GetPlayerPosition() + character.GetPlayerForward();
                HawkNetworkManager.DefaultInstance.InstantiateNetworkPrefab(selectedObject, pos);
            };
            UIFactory.SetLayoutElement(spawnBtn.GameObject, 0, 32, 9999, 0);
        }

        public override void RefreshUI()
        {
            base.RefreshUI();

            cellHandler.Refresh();
        }
    }

    public class PropCell : ICell
    {
        private bool enabled;

        public bool Enabled => enabled;
        public RectTransform Rect { get; set; }
        public GameObject UIRoot { get; set; }
        public float DefaultHeight => 32;

        public GameObject prop;
        public PropSpawnerTab parentTab;

        private Text text;
        private ButtonRef button;

        public GameObject CreateContent(GameObject parent)
        {
            UIRoot = UIFactory.CreateUIObject("PropCell", parent);
            UIFactory.SetLayoutElement(UIRoot, 0, 32, 9999, 0);
            Rect = UIRoot.GetComponent<RectTransform>();

            button = UIFactory.CreateButton(UIRoot, "button", "");
            button.OnClick = OnCellButtonClicked;

            RectTransform buttonRect = button.GameObject.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0, 0);
            buttonRect.anchorMax = new Vector2(1, 1);
            buttonRect.offsetMin = Vector2.zero;
            buttonRect.offsetMax = Vector2.zero;

            text = button.ButtonText;

            return UIRoot;
        }

        public void OnCellButtonClicked()
        {
            parentTab.SelectedProp = prop;
        }

        public void Disable()
        {
            enabled = false;
            UIRoot.SetActive(false);
        }

        public void Enable()
        {
            enabled = true;
            UIRoot.SetActive(true);
        }

        public void ConfigureCell(GameObject prop)
        {
            this.prop = prop;
            text.text = prop.ToString();
        }
    }

    public class PropCellHandler : ICellPoolDataSource<PropCell>
    {
        public int ItemCount => propList.Count;
        public PropSpawnerTab parentTab;

        private List<GameObject> propList = new();

        public PropCellHandler(PropSpawnerTab parentTab)
        {
            this.parentTab = parentTab;
        }

        public void OnCellBorrowed(PropCell cell)
        {
        }

        public void SetCell(PropCell cell, int index)
        {
            if(index < propList.Count)
            {
                cell.parentTab = parentTab;
                cell.ConfigureCell(propList[index]);
                cell.Enable();
            }
            else
            {
                cell.Disable();
            }
        }

        public void Refresh()
        {
            propList.Clear();

            foreach(var behavior in UnityEngine.Object.FindObjectsOfType<HawkNetworkBehaviour>())
            {
                var obj = behavior.gameObject;
                if(obj && !obj.name.Contains("(Clone)"))
                {
                    propList.Add(obj);
                }
            }
        }
    }
}
