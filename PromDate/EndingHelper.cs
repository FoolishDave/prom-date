﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;
using static EventManager;

namespace PromDate
{
    public static class EndingHelper
    {
        public struct FinalSceneInfo
        {
            public int FromPromScene;
            public int SceneId;
            public string AudioClip;
        }

        private static List<EventManager.CEventFlow> modEndings = new List<EventManager.CEventFlow>();
        private static List<FinalSceneInfo> finalScenes = new List<FinalSceneInfo>();

        public static bool IsModEnding(int endingIndex)
        {
            return modEndings.Contains(EventManager.Instance.Events[endingIndex]);
        }

        public static FinalSceneInfo GetFinalSceneInfo(int endingIndex)
        {
            return finalScenes.First(sce => sce.FromPromScene == endingIndex);
        }

        public static void LoadModEndings()
        {
            DirectoryInfo[] directories = new DirectoryInfo(ModConstants.MODS_LOCATION).GetDirectories();
            List<CSecretEndingConditions> modEndingConditions = new List<CSecretEndingConditions>();

            finalScenes.Clear();
            modEndings.Clear();

            foreach (DirectoryInfo directory in directories)
            {
                FileInfo modInfo = directory.GetFiles("*.xml").FirstOrDefault();
                if (modInfo == null) continue;
                CustomEventMod mod = CustomEventMod.Load(modInfo.FullName);
                if (!EventLoader.EventLoader.EventModLoaded(mod)) continue;
                string[] files = Directory.GetFiles(directory.FullName + "/EndingConditions", "*.xml");
                foreach (string file in files)
                {
                    GeneralManager.Instance.LogToFileOrConsole("[PromDate] Loading ending conditions from " + file);
                    EndingContainer endingContainer = EndingContainer.Load(file);
                    foreach (Ending ending in endingContainer.Endings)
                    {
                        GeneralManager.Instance.LogToFileOrConsole("\t[PromDate] Loading ending " + ending.SecretEndingName);
                        EventManager.CSecretEndingConditions endCond = EndingContainer.convertEndingConditions(ending, mod);
                        modEndingConditions.Add(endCond);
                        EndingHelper.modEndings.Add(EventManager.Instance.Events[endCond.cSecretEndingIndex]);
                        FinalSceneInfo sceneInfo = new FinalSceneInfo() { FromPromScene = endCond.cSecretEndingIndex, SceneId = endCond.cSecretEndingIndex + 1 };
                        if (EventManager.Instance.Events[endCond.cSecretEndingIndex].ArgumentTags.Any(arg => arg.Contains("SFX")))
                        {
                            sceneInfo.AudioClip = EventManager.Instance.Events[endCond.cSecretEndingIndex].ArgumentTags.First(arg => arg.Contains("SFX")).Split('_')[1];
                        }
                        EndingHelper.finalScenes.Add(sceneInfo);
                    }
                }
            }
            EventManager.Instance.SecretEndingsConditions = EventManager.Instance.SecretEndingsConditions.Concat(modEndingConditions.ToArray()).ToArray();
        }
    }
}
