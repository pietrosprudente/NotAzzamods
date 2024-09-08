using BepInEx;
using UnityEngine;
using NotAzzamods.Hacks.Free;
using NotAzzamods.Hacks.Paid;
using UniverseLib.UI;
using UniverseLib;
using UniverseLib.Config;
using NotAzzamods.UI;
using System.Collections.Generic;
using NotAzzamods.UI.TabMenus;
using NotAzzamods.Hacks;
using NotAzzamods.Hacks.Custom;
using System.Reflection;
using System.Collections;
using NotAzzamods.Hacks.Custom.JobManager;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System;

namespace NotAzzamods
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;

        public static PlayerController playerController;
        public static PlayerCharacter playerCharacter;

        public static Plugin Instance { get; private set; }

        public static UIBase UiBase { get; private set; }
        public static MainPanel MainPanel { get; private set; }

        public static List<BaseTab> TabMenus { get; private set; } = new();
        public static List<BaseHack> Hacks { get; private set; } = new();

        public static HacksTab PlayerHacksTab { get; private set; }
        public static HacksTab ServerHacksTab { get; private set; }
        public static HacksTab VehicleHacksTab { get; private set; }
        public static HacksTab SaveHacksTab { get; private set; }
        public static HacksTab ExtraHacksTab { get; private set; }

        public static PropSpawnerTab PropSpawnerTab { get; private set; }

        private void Awake()
        {
            WobblyServerUtilCompat.Init();

            Instance = this;

            PlayerHacksTab = new("Player Mods");
            PlayerHacksTab.Hacks.Add(new ChangePlayerName());
            PlayerHacksTab.Hacks.Add(new MovementManager());
            PlayerHacksTab.Hacks.Add(new GiveMoney());
            PlayerHacksTab.Hacks.Add(new ClothesManager());
            PlayerHacksTab.Hacks.Add(new CompleteJob());
            PlayerHacksTab.Hacks.Add(new RagdollManager());
            PlayerHacksTab.Hacks.Add(new CharacterManager());
            PlayerHacksTab.Hacks.Add(new ControllerManager());
            PlayerHacksTab.Hacks.Add(new TeleportAllPlayers());
            PlayerHacksTab.Hacks.Add(new SmitePlayer());
            PlayerHacksTab.Hacks.Add(new Hacks.Paid.PropSpawner());
            //PlayerHacksTab.Hacks.Add(new JobHacks());
            //PlayerHacksTab.Hacks.Add(new CrashGame()); // !!! REMOVE BEFORE RELEASE !!!

            VehicleHacksTab = new("Vehicle Mods");
            VehicleHacksTab.Hacks.Add(new Hacks.Custom.ActionEnterExitInteract());
            VehicleHacksTab.Hacks.Add(new RoadVehicle());

            ServerHacksTab = new("Server Mods", false);
            ServerHacksTab.Hacks.Add(new PreventDrowning());
            ServerHacksTab.Hacks.Add(new SetGravity());
            ServerHacksTab.Hacks.Add(new ServerSettings());
            ServerHacksTab.Hacks.Add(new WeatherEditor());
            ServerHacksTab.Hacks.Add(new SetTime());

            SaveHacksTab = new("Save File Mods", false);
            SaveHacksTab.Hacks.Add(new MissionComplete());
            SaveHacksTab.Hacks.Add(new PresentUnlocker());
            SaveHacksTab.Hacks.Add(new Hacks.Paid.AchievementManager());
            SaveHacksTab.Hacks.Add(new MuseumManager());

            ExtraHacksTab = new("Extra Mods", false);
            ExtraHacksTab.Hacks.Add(new Debt());
            ExtraHacksTab.Hacks.Add(new Gambling());
            ExtraHacksTab.Hacks.Add(new BuyUnlimitedHouses());
            ExtraHacksTab.Hacks.Add(new FirstPerson());

            PropSpawnerTab = new();

            GameInstance.onAssignedPlayerController += AssignPlayerController;
            GameInstance.onAssignedPlayerCharacter += AssignPlayerCharacter;

            new JobTimerManager();
            new ArtStudioJobManager();
            new ConstructionBuildingJobManager();
            new ConstructionDestructionJobManager();
            new DeliveryJobManager();
            new DiscoJobManager();
            new FarmHarvestingJobManager();
            new FarmPlowingJobManager();
            new FarmSeedingJobManager();
            new FireFighterJobManager();
            new FishingJobManager();
            new GarbageJobManager();
            new IceCreamJobManager();
            new MinerJobManager();
            new NewsRoundManager();
            new PowerPlantJobManager();
            new QuizMasterJobManager();
            new RaceJobManager();
            new ScienceMachineJobManager();
            new TaxiJobManager();
            new WeatherResearcherJobManager();
            new WoodCutterJobManager();

            float startupDelay = 1f;
            UniverseLibConfig config = new()
            {
                Disable_EventSystem_Override = false,
                Force_Unlock_Mouse = true
            };

            Universe.Init(startupDelay, () =>
            {
                UiBase = UniversalUI.RegisterUI("lstwo.NotAzza", null);

                MainPanel = new(UiBase);

                UiBase.Enabled = false;
            }, null, config);

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        private async Task DownloadPrefabJSON()
        {
            string githubUrl = "https://raw.githubusercontent.com/lstwo/NotAzzamods/main/Data/NotAzzamods_prefabs.json";
            string fileName = "NotAzzamods_prefabs.json";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(githubUrl);
                    response.EnsureSuccessStatusCode();
                    string jsonContent = await response.Content.ReadAsStringAsync();

                    string exeFolderPath = AppDomain.CurrentDomain.BaseDirectory;
                    string filePath = Path.Combine(exeFolderPath, fileName);

                    File.WriteAllText(filePath, jsonContent);

                    Console.WriteLine($"File saved to: {filePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public static Coroutine _StartCoroutine(IEnumerator routine)
        {
            return Instance.StartCoroutine(routine);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.F2))
            {
                UiBase.Enabled = !UiBase.Enabled;
                MainPanel.Refresh();
            }

            foreach(var hack in Hacks)
            {
                hack.Update();
            }
        }


        private static void AssignPlayerController(PlayerController controller)
        {
            if(controller.networkObject.IsOwner())
            {
                playerController = controller;
            }
        }

        private static void AssignPlayerCharacter(PlayerCharacter character)
        {
            if (character.networkObject.IsOwner())
            {
                playerCharacter = character;
            }
        }
    }
}