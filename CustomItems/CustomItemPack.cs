using HawkNetworking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using CustomItems;
using Steamworks.Ugc;

namespace NotAzzamods.CustomItems
{
    public class CustomItemPack
    {
        public string path;
        public string packName;
        public List<CustomItem> items = new();
        public AssetBundle assetBundle;
        public Assembly assembly;

        public CustomItemPack(string path)
        {
            this.path = path;

            var jsonPath = $"{path}/data.json";
            var json = File.ReadAllText(jsonPath);
            var data = JsonUtility.FromJson<JsonData>(json);

            packName = data.name;

            if (data.assemblyPath != "")
            {
                var assemblyPath = $"{path}/{data.assemblyPath}";
                assembly = Assembly.LoadFile(assemblyPath);
            }

            if (data.assetPath != "")
            {
                var assetPath = $"{path}/{data.assetPath}";
                assetBundle = AssetBundle.LoadFromFile(assetPath);

                foreach(var obj in assetBundle.LoadAllAssets<GameObject>())
                {
                    if(obj.TryGetComponent<CustomItem>(out var item))
                    {
                        items.Add(item);
                    }
                }
            }

            foreach(var item in items)
            {
                HawkNetworkManager.DefaultInstance.RegisterPrefab(item);
            }
        }

        public class JsonData
        {
            public string name;
            public string assetPath;
            public string assemblyPath;
        }
    }
}
