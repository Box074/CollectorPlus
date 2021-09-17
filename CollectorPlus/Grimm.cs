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
    public class Grimm : MonoBehaviour
    {
        GameObject bat = null;
        void Awake()
        {
            bat = Instantiate(CollectorPlusMod.GR_BAT);
            bat.LocateMyFSM("Control").FsmVariables.FindFsmGameObject("Grimm").Value = gameObject;

            PlayMakerFSM control = gameObject.LocateMyFSM("Control");
            control.FsmVariables.FindFsmGameObject("Real Bat Obj").Value = bat;
            control.FsmVariables.FindFsmFloat("AD Min X").Value = CollectorPlusMod.MinX;
            control.FsmVariables.FindFsmFloat("AD Max X").Value = CollectorPlusMod.MaxX;
            control.FsmVariables.FindFsmFloat("Min X").Value = CollectorPlusMod.MinX;
            control.FsmVariables.FindFsmFloat("Max X").Value = CollectorPlusMod.MaxX;
            control.FsmVariables.FindFsmFloat("Ground Y").Value = 98;
            SetPosition balloonP = gameObject.GetFSMActionOnState<SetPosition>("Balloon Pos");
            balloonP.x = (CollectorPlusMod.MaxX - CollectorPlusMod.MinX) / 2 + CollectorPlusMod.MinX;
            balloonP.y = CollectorPlusMod.MaxY - 4;

            gameObject.GetFSMActionOnState<SetHP>("Set 1").hp.Value = CollectorPlusMod.BossHP;
            gameObject.GetFSMActionOnState<SetHP>("Set 2").hp.Value = CollectorPlusMod.BossHP;
            gameObject.GetFSMActionOnState<SetHP>("Set 3").hp.Value = CollectorPlusMod.BossHP;
            gameObject.GetFSMActionOnState<SetHP>("Set 4").hp.Value = CollectorPlusMod.BossHP;
            gameObject.GetFSMActionOnState<SetHP>("Set 5").hp.Value = CollectorPlusMod.BossHP;

            RandomFloat FRTL = gameObject.GetFSMActionOnState<RandomFloat>("FR Tele L");
            FRTL.min = CollectorPlusMod.MinX + 4;
            FRTL.max = CollectorPlusMod.MinX + 5;

            RandomFloat FRTR = gameObject.GetFSMActionOnState<RandomFloat>("FR Tele R");
            FRTR.min = CollectorPlusMod.MaxX - 5;
            FRTR.max = CollectorPlusMod.MaxX - 4;

            CollectorPlusMod.DestroyOnDead(gameObject, bat);
        }
        void Update()
        {
            if (bat != null)
            {
                FSMUtility.SendEventToGameObject(bat, "BOSS AWAKE");
            }
        }
    }
}