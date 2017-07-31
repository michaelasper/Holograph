using System;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace Holograph
{
    [Obsolete("Not used anymore", true)]
    public class Icon : MonoBehaviour, IInputHandler
    {
        public MenuBehavior menubehvaior;

        public Icon(string TextureName)
        {
            this.TextureName = TextureName;
        }

        public string TextureName { get; set; }
        public string Hash { get; set; }
        public string Message { get; set; }

        public void OnInputDown(InputEventData eventData)
        {
        }

        public void OnInputUp(InputEventData eventData)
        {
            menubehvaior.Invoke(Message, 0);
            NetworkMessages.Instance.SendRadialMenuClickIcon(Message);
        }

    }
}