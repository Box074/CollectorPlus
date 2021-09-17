using System.Collections;
using UnityEngine;
using ModCommon;
using HutongGames.PlayMaker.Actions;
namespace CollectorPlus
{
    public class Jar : MonoBehaviour
    {
        void OnEnable()
        {
            PlayMakerFSM control = gameObject.LocateMyFSM("Control");
            control.SetState("Init");
            gameObject.GetFSMActionOnState<SendEventByName>("Roar").sendEvent.Value = "";
            gameObject.GetFSMActionOnState<Wait>("Roar").time = 0.5f;
        }
        void Update()
        {
            FSMUtility.SendEventToGameObject(gameObject, "WAKE");
        }
    }
}