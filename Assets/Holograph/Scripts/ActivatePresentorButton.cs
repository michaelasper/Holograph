using UnityEngine;
using HoloToolkit.Unity.InputModule;
using UnityEngine.UI;
using HoloToolkit.Unity;

namespace Holograph
{
    [RequireComponent(typeof(Text))]
    public class ActivatePresentorButton : MonoBehaviour, IInputClickHandler
    {
        private DisplayUserList userList;
        private Text textAsset;

        private void Start()
        {
            userList = GetComponentInParent<DisplayUserList>();
            textAsset = GetComponent<Text>();
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            foreach (var user in userList.Users)
            {
                if (user.Value.Equals(textAsset.text))
                {
                    NetworkMessages.Instance.SendPresenterId(user.Key);

                    RemoteHeadInfo headInfo = HeadManager.Instance.GetRemoteHeadInfo(user.Key);

                    if(headInfo != null)
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
    }
}
