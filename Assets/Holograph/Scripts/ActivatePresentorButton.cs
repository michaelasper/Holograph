// /********************************************************
// *                                                       *
// *   Copyright (C) Microsoft. All rights reserved.       *
// *                                                       *
// ********************************************************/

namespace Holograph
{
    using System;

    using HoloToolkit.Unity;
    using HoloToolkit.Unity.InputModule;

    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Text))]
    public class ActivatePresentorButton : MonoBehaviour, IInputClickHandler
    {
        private Text textAsset;

        private DisplayUserList userList;

        public void OnInputClicked(InputClickedEventData eventData)
        {
            foreach (var user in userList.Users)
            {
                if (user.Value.Equals(textAsset.text))
                {
                    NetworkMessages.Instance.SendPresenterId(user.Key);

                    var headInfo = HeadManager.Instance.GetRemoteHeadInfo(user.Key);

                    if (headInfo != null)
                    {
                        for (var i = 0; i < HeadManager.Instance.Panels.Length; i++)
                        {
                            var billboard = HeadManager.Instance.Panels[i].GetComponent<Billboard>();

                            billboard.TargetTransform = headInfo == null ? Camera.main.transform : headInfo.HeadObject.transform;
                        }
                    }
                }
            }
        }

        private void Start()
        {
            userList = GetComponentInParent<DisplayUserList>();
            textAsset = GetComponent<Text>();
        }
    }
}