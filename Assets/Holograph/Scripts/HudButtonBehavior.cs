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
        public Texture idleTexture;
        private Material buttonMaterial;
        public Texture hoverTexture;
        public Texture selectedTexture;
        private int texturePropertyId;
        private bool isSelected;
        private bool isGazedAt;
        private HudManager hudManager;

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
            if (isSelected)
            {
                switchSelected(false);
            }
            else
            {
                hudManager.selectButton(transform);
            }
        }

        public void switchSelected(bool select)
        {
            isSelected = select;
            if (select)
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
            //throw new NotImplementedException();
        }
    }
}