// /********************************************************
// *                                                       *
// *   Copyright (C) Microsoft. All rights reserved.       *
// *                                                       *
// ********************************************************/

namespace Holograph
{
    using System;

    using UnityEngine;
    using UnityEngine.UI;

    public class ProgressCircleBehavior : MonoBehaviour
    {
        public Text percentTextLabel;

        public Image progressBar;

        private int invisibleStateHash;

        private Animator reportPanelAnimator;

        // Use this for initialization
        private void Start()
        {
            reportPanelAnimator = GetComponent<Animator>();
            invisibleStateHash = Animator.StringToHash("Base Layer.invisible");
        }

        // Update is called once per frame
        private void Update()
        {
            if (reportPanelAnimator != null && reportPanelAnimator.isInitialized)
            {
                int stateHash = reportPanelAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash;
                if (stateHash == invisibleStateHash)
                {
                    gameObject.SetActive(false);
                }
            }

            float progress = progressBar.fillAmount;
            percentTextLabel.text = (int)(progress * 100) + "%";
        }
    }
}