using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/*using System;
using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using SteamworksNative;*/

namespace wsaii
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        public static GameObject ServerName;
        public static TextMeshProUGUI ServerNameText;
        public static GameObject PlayerCount;

        public override void Load()
        {
            Harmony.CreateAndPatchAll(typeof(Plugin));

            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        [HarmonyPatch(typeof(GameUI), "Start")]
        [HarmonyPostfix]
        public static void Start(GameUI __instance)
        {
            if (!PlayerCount)
            {
                PlayerCount = GameObject.Find("GameUI/PlayerList/WindowUI/Header/Tabs/PLayerInLobby");
            }

            if (!GameObject.Find("GameUI/PlayerList/WindowUI/Header/Tabs/ServerName"))
            {
                ServerName = Object.Instantiate(PlayerCount);
                ServerName.name = "ServerName";

                Object.Destroy(ServerName.GetComponent<MonoBehaviourPublicTeplUnique>());

                RectTransform ServerNameRect = ServerName.GetComponent<RectTransform>();
                ServerNameRect.sizeDelta = new Vector2(600, 80);

                ServerNameText = ServerName.GetComponent<TextMeshProUGUI>();
                ServerNameText.text = "Couldn't load name.";

                ServerName.transform.parent = PlayerCount.transform.parent;
            }
        }

        [HarmonyPatch(typeof(PlayerList), nameof(PlayerList.UpdateList))]
        [HarmonyFinalizer]
        public static void UpdateList(PlayerList __instance)
        {
            if (ServerNameText) {
                ServerNameText.SetText(SteamworksNative.SteamMatchmaking.GetLobbyData(SteamManager.Instance.currentLobby, "LobbyName"));
            } else
            {
                Start(GameUI.Instance);
            }
        }
    }
}