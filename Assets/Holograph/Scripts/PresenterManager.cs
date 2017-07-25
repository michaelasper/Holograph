using HoloToolkit.Sharing;
using UnityEngine;

namespace Holograph
{
    public class PresenterManager : MonoBehaviour
    {
        public long PresenterId { get; set; }

        void Start()
        {
            NetworkMessages.Instance.MessageHandlers[NetworkMessages.MessageID.SelfPresenterId] = UpdatePresenterId;
        }

        private void UpdatePresenterId(NetworkInMessage msg)
        {
            long userId = msg.ReadInt64();

            PresenterId = userId;

            Debug.Log("RECIEVED NEW PRESENTER !!!!! + " + PresenterId);
        }
    }

}