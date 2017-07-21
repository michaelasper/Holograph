using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ReportPanelBehavior : MonoBehaviour
{

    public Image progressBar;
    public Text percentTextLabel;
    private Animator reportPanelAnimator;
    private int invisibleStateHash;
    // Use this for initialization
    void Start()
    {
        reportPanelAnimator = GetComponent<Animator>();
        invisibleStateHash = Animator.StringToHash("Base Layer.invisible");
    }

    // Update is called once per frame
    void Update()
    {
        if (reportPanelAnimator != null && reportPanelAnimator.isInitialized)
        {
            int stateHash = reportPanelAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash;
            if (stateHash == invisibleStateHash)
            {
                this.gameObject.SetActive(false);
            }
        }
        var progress = progressBar.fillAmount;
        percentTextLabel.text = ((int)(progress * 100)).ToString() + "%";
    }
}
