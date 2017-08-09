namespace Holograph
{
    using System.Collections;
    using System.Collections.Generic;
    using HoloToolkit.Unity.InputModule;

    using UnityEngine;

    public class HudPanelBehavior : MonoBehaviour, IFocusable
    {
        private HudManager hudManager;


        public void OnFocusEnter()
        {
            hudManager.IsGazedAt = true;
            
        }

        public void OnFocusExit()
        {
            hudManager.IsGazedAt = false;
           
        }

        // Use this for initialization
        void Start()
        {
            hudManager = GetComponentInParent<HudManager>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}