using System.Collections;
using UnityEngine;

namespace CollectorPlus
{
    public class Crystal : MonoBehaviour
    {
        void OnEnbale()
        {
            PlayMakerFSM control = gameObject.LocateMyFSM("Beam Miner");
            GameObject beam = Instantiate(CollectorPlusMod.CG_Beam);
            beam.SetActive(true);
            control.FsmVariables.FindFsmGameObject("Beam").Value = beam;
            GameObject beamball = Instantiate(CollectorPlusMod.CG_Beam_Ball);
            beamball.SetActive(true);
            control.FsmVariables.FindFsmGameObject("Beam Ball").Value = beamball;

            CollectorPlusMod.DestroyOnDead(gameObject, beam, beamball);
        }
    }
}