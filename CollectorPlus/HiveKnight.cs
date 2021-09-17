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
    public class HiveKnight : MonoBehaviour
    {

        void OnEnable()
        {
            PlayMakerFSM control = gameObject.LocateMyFSM("Control");
            control.FsmVariables.FindFsmFloat("Ground Y").Value = gameObject.transform.position.y - 5;
            control.FsmVariables.FindFsmFloat("Left X").Value = CollectorPlusMod.MinX;
            control.FsmVariables.FindFsmFloat("Right X").Value = CollectorPlusMod.MaxX;

            foreach (var v in GetComponents<EnemyDeathEffects>()) Destroy(v);

            GameObject globs = Instantiate(CollectorPlusMod.HK_Globs);
            Globs(globs);
            globs.SetActive(true);

            control.FsmVariables.FindFsmGameObject("Globs Container").Value = globs;
            GameObject droppers = Instantiate(CollectorPlusMod.HK_Droppers);
            Dropper(droppers);
            droppers.transform.position = Vector3.zero;
            droppers.SetActive(true);
            control.FsmVariables.FindFsmGameObject("Droppers").Value = droppers;

            CollectorPlusMod.DestroyOnDead(gameObject, globs, droppers);
        }

        void Update()
        {
            FSMUtility.SendEventToGameObject(gameObject, "WAKE");
        }
        void Globs(GameObject HK_Globs)
        {
            HK_Globs.GetFSMActionOnState<SetPosition>("Set L").x = 44;
            HK_Globs.GetFSMActionOnState<SetPosition>("Set M").x = 50;
            HK_Globs.GetFSMActionOnState<SetPosition>("Set R").x = 60;
            HK_Globs.transform.position = new Vector3(0, transform.position.y - 5);
        }
        void Dropper(GameObject HK_Droppers)
        {
            for (int i = 0; i < HK_Droppers.transform.childCount; i++)
            {
                PlayMakerFSM pm = HK_Droppers.transform.GetChild(i).gameObject.LocateMyFSM("Control");
                if (pm != null)
                {
                    pm.FsmVariables.FindFsmFloat("Start Y").Value = 110;
                    pm.FsmVariables.FindFsmFloat("X Left").Value = CollectorPlusMod.MinX - 5;
                    pm.FsmVariables.FindFsmFloat("X Right").Value = CollectorPlusMod.MaxX + 5;
                }
            }
        }

    }
}