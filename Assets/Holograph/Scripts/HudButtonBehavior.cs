// /********************************************************
// *                                                       *
// *   Copyright (C) Microsoft. All rights reserved.       *
// *                                                       *
// ********************************************************/

namespace Holograph
{
    using System;

    using HoloToolkit.Unity.InputModule;

    using UnityEngine;

    public class HudButtonBehavior : MonoBehaviour, IFocusable, IInputHandler
    {
        public Texture hoverTexture;

        // public GlobeBehavior globe;
        public Texture idleTexture;

        public Texture selectedTexture;

        private Material buttonMaterial;

        private HudManager hudManager;

        private bool isGazedAt;

        private bool isSelected;

        private int texturePropertyId;

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

        public void OnInputDown(InputEventData eventData)
        {
            // this function doesn't work
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

        private void Start()
        {
            buttonMaterial = GetComponent<MeshRenderer>().material;
            texturePropertyId = Shader.PropertyToID("_MainTex");
            hudManager = GetComponentInParent<HudManager>();
        }

        private void Update()
        {
        }
    }
}