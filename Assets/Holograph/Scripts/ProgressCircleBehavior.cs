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

        private void Start()
        {
            reportPanelAnimator = GetComponent<Animator>();
            invisibleStateHash = Animator.StringToHash("Base Layer.invisible");
        }

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