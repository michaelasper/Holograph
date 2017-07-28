using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

namespace Holograph
{
    public class HudButtonBehavior : MonoBehaviour,
                                            IFocusable,
                                            IInputHandler
    {
        //public GlobeBehavior globe;
        public Texture idleTexture;
        public Texture hoverTexture;
        public Texture selectedTexture;
        private int texturePropertyId;
        private bool isSelected;
        private bool isGazedAt;
        private HudManager hudManager;
        private Material buttonMaterial;

        public void OnFocusEnter()
        {
            isGazedAt = true;
            hudManager.isGazedAt = true;
            if (!isSelected)
            {
                buttonMaterial.SetTexture(texturePropertyId, hoverTexture);
            }
        }

        public void OnFocusExit()
        {
            isGazedAt = false;
            hudManager.isGazedAt = false;
            if (!isSelected)
            {
                buttonMaterial.SetTexture(texturePropertyId, idleTexture);
            }
        }

        public void OnInputUp(InputEventData eventData)
        {
            hudManager.clickButtonUp(transform);
        }

        public void switchSelected(bool select)
        {
            isSelected = select && !isSelected;
            if (isSelected)
            {
                buttonMaterial.SetTexture(texturePropertyId, selectedTexture);
            }
            else
            {
                buttonMaterial.SetTexture(texturePropertyId, isGazedAt ? hoverTexture : idleTexture);
            }
        }

        void Start()
        {
            buttonMaterial = GetComponent<MeshRenderer>().material;
            texturePropertyId = Shader.PropertyToID("_MainTex");
            hudManager = GetComponentInParent<HudManager>();
        }

        void Update()
        {

        }

        public void OnInputDown(InputEventData eventData)
        {
            //this function doesn't work
        }
    }
}