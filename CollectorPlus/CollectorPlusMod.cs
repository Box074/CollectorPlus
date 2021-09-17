using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modding;
using UnityEngine;
using ModCommon;
using ModCommon.Util;
using HutongGames.PlayMaker.Actions;

namespace CollectorPlus
{
    public class CollectorPlusMod : Mod
    {
        public static readonly List<GameObject> enemiesPrefab = new List<GameObject>();
        public static GameObject HK_Globs = null;
        public static GameObject HK_Droppers = null;
        public static GameObject CG_Beam = null;
        public static GameObject CG_Beam_Ball = null;
        public static GameObject GR_BAT = null;
        public const float MinX = 42.94f;
        public const float MaxX = 65.98f;
        public const float MaxY = 109;

        public static int BossHP
        {
            get
            {
                int hp = 400;
                if (BossSceneController.Instance != null)
                {
                    switch (BossSceneController.Instance.BossLevel)
                    {
                        case 1:
                            hp = 180;
                            break;
                        case 2:
                            hp = 280;
                            break;
                        case 3:
                            hp = 320;
                            break;
                    }
                }
                return hp;
            }
        }

        public static void DestroyOnDead(GameObject e,params GameObject[] gos)
        {
            HealthManager hm = e.GetComponent<HealthManager>();
            if (hm != null)
            {
                HealthManager.DeathEvent onDead = () =>
                {
                    foreach(var v in gos)
                    {
                        UnityEngine.Object.Destroy(v);
                    }
                };
                hm.OnDeath += onDead;
            }
        }
        public override List<(string,string)> GetPreloadNames()
        {
            return new List<(string, string)>
            {
                ("GG_Hive_Knight","Battle Scene/Hive Knight"),
                ("GG_Hive_Knight","Battle Scene/Globs"),
                ("GG_Hive_Knight","Battle Scene/Droppers"),

                ("GG_Collector","Battle Scene/Jar Collector"),

                ("GG_Crystal_Guardian_2", "Battle Scene/Zombie Beam Miner Rematch"),
                ("GG_Crystal_Guardian_2", "Battle Scene/Zombie Beam Miner Rematch/Beam Ball"),
                ("GG_Crystal_Guardian_2", "Battle Scene/Zombie Beam Miner Rematch/Beam"),

                ("GG_Grimm", "Grimm Scene/Grimm Boss"),
                ("GG_Grimm", "Grimm Bats/Real Bat")
            };
        }
        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            

            HiveKnight(preloadedObjects["GG_Hive_Knight"]);
            Collector(preloadedObjects["GG_Collector"]);
            Crystal(preloadedObjects["GG_Crystal_Guardian_2"]);
            try
            {
                Grimm(preloadedObjects["GG_Grimm"]);
            }
            catch (Exception e)
            {
                LogError(e);
            }
            On.SpawnJarControl.SetEnemySpawn += SpawnJarControl_SetEnemySpawn;
            On.HutongGames.PlayMaker.Actions.GetTagCount.OnEnter += GetTagCount_OnEnter;
            ModHooks.ObjectPoolSpawnHook += ModHooks_ObjectPoolSpawnHook;
        }

        public void Grimm(Dictionary<string,GameObject> go)
        {
            GR_BAT = go["Grimm Bats/Real Bat"];

            GameObject grimm = go["Grimm Scene/Grimm Boss"];
            grimm.AddComponent<Grimm>();
            enemiesPrefab.Add(grimm);
        }
        private void GetTagCount_OnEnter(On.HutongGames.PlayMaker.Actions.GetTagCount.orig_OnEnter orig, GetTagCount self)
        {
            if(self.tag.Value == "Boss")
            {
                if(self.Fsm.GameObject.name.StartsWith("Jar Collector"))
                {
                    self.storeResult.Value = 1;
                    self.Finish();
                    return;
                }
            }
            orig(self);
        }

        private void SpawnJarControl_SetEnemySpawn(On.SpawnJarControl.orig_SetEnemySpawn orig, SpawnJarControl self, 
            GameObject prefab, int health)
        {
            
            orig(self, prefab, BossHP);
        }

        public void Crystal(Dictionary<string,GameObject> go)
        {
            GameObject boss = go["Battle Scene/Zombie Beam Miner Rematch"];
            boss.AddComponent<Crystal>();
            PlayMakerFSM control = boss.LocateMyFSM("Beam Miner");
            control.FsmVariables.FindFsmFloat("Jump Max X").Value = MinX;
            control.FsmVariables.FindFsmFloat("Jump Min X").Value = MaxX;
            boss.GetFSMActionOnState<ActivateGameObject>("Wake").activate.Value = false;

            CG_Beam = go["Battle Scene/Zombie Beam Miner Rematch/Beam"];
            CG_Beam_Ball = go["Battle Scene/Zombie Beam Miner Rematch/Beam Ball"];

            enemiesPrefab.Add(boss);
        }

        public void Collector(Dictionary<string,GameObject> go)
        {
            GameObject jar = go["Battle Scene/Jar Collector"];
            jar.AddComponent<Jar>();
            enemiesPrefab.Add(jar);
            enemiesPrefab.Add(jar);
        }

        public void HiveKnight(Dictionary<string,GameObject> go)
        {
            GameObject hk = go["Battle Scene/Hive Knight"];
            
            hk.AddComponent<HiveKnight>();

            enemiesPrefab.Add(hk);
            enemiesPrefab.Add(hk);

            HK_Globs = go["Battle Scene/Globs"];
            

            HK_Droppers = go["Battle Scene/Droppers"];
            
        }

        bool inSpawn = false;
        private GameObject ModHooks_ObjectPoolSpawnHook(GameObject arg)
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.StartsWith("GG_Collector"))
            {
                if (arg.GetComponent<HealthManager>() != null && enemiesPrefab.Count > 0 && !inSpawn)
                {
                    inSpawn = true;
                    try
                    {
                        int id = UnityEngine.Random.Range(0, enemiesPrefab.Count);
                        GameObject prefab = enemiesPrefab[id];
                        GameObject go = UnityEngine.Object.Instantiate(prefab);
                        go.transform.position = arg.transform.position + new Vector3(0, 5, 0);
                        go.SetActive(true);
                        UnityEngine.Object.Destroy(arg);
                        return go;
                    }
                    finally
                    {
                        inSpawn = false;
                    }
                    
                }
                else
                {
                    return arg;
                }

            }
            else
            {
                return arg;
            }
        }
    }
}
