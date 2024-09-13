using Newtonsoft.Json.Linq;
using ShadowLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.Widgets.ScrollView;

namespace NotAzzamods.UI.TabMenus
{
    public class PropSpawnerTab : BaseTab
    {
        public string SelectedProp
        {
            set
            {
                selectedObject = value;
                spawnBtn.ButtonText.text = $"Spawn Prop ({value})";
            }
        }

        private ScrollPool<PropCell> scrollPool;
        private string selectedObject;
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
                /*var player = PlayerUtils.GetMyPlayer();
                if (player == null || selectedObject == "") return;

                var character = player.GetPlayerCharacter();
                var pos = character.GetPlayerPosition() + character.GetPlayerForward();
                Debug.Log(selectedObject);
                NetworkPrefab.SpawnNetworkPrefab("Game/Prefabs/Props/" + selectedObject, position: pos);*/

                Plugin._StartCoroutine(coroutine());
            };
            UIFactory.SetLayoutElement(spawnBtn.GameObject, 0, 32, 9999, 0);
        }

        private IEnumerator coroutine()
        {
            foreach (var prop in cellHandler.propList)
            {
                try
                {
                    var player = PlayerUtils.GetMyPlayer();
                    if (player == null || prop == "") continue;

                    var character = player.GetPlayerCharacter();
                    var pos = character.GetPlayerPosition() + character.GetPlayerForward();
                    Debug.Log(selectedObject);
                    NetworkPrefab.SpawnNetworkPrefab("Game/Prefabs/Props/" + selectedObject, position: pos);
                } catch (Exception ex)
                {
                    Debug.LogError($"Error while instantiating prop \"{prop}\": {ex.Message}");
                }

                yield return null;
            }

            yield break;
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

        public string prop;
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

        public void ConfigureCell(string prop)
        {
            this.prop = prop;
            text.text = prop;
        }
    }

    public class PropCellHandler : ICellPoolDataSource<PropCell>
    {
        public int ItemCount => propList.Count;
        public PropSpawnerTab parentTab;

        public List<string> propList = new();

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
            Plugin._StartCoroutine(RefreshCoroutine());
        }

        public IEnumerator RefreshCoroutine()
        {
            propList.Clear();

            string jsonData = "";

            try
            {
                jsonData = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/NotAzzamods_prefabs.json");
            }
            catch(Exception e)
            {
                Plugin.LogSource.LogError("Could not load prefab locations: " + e.Message);
            }

            if (jsonData == "") yield break;

            foreach (var array in LoadPrefabData(jsonData))
            {
                propList.AddRange(array);
            }
        }

        public static string[][] LoadPrefabData(string jsonData)
        {
            var jsonObject = JObject.Parse(jsonData);
            var prefabArrayList = new List<string[]>();

            foreach (var property in jsonObject.Properties())
            {
                var prefabArray = property.Value.ToObject<string[]>();
                prefabArrayList.Add(prefabArray);
            }

            return prefabArrayList.ToArray();
        }
    }
}
