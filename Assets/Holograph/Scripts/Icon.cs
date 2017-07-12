using UnityEngine;
using HoloToolkit.Unity.InputModule;

namespace Holograph
{
    public class Icon : MonoBehaviour, IInputHandler
    {

        public string TextureName { get; set; }
        public string Hash { get; set; }
        public string Message { get; set; }
        public MenuBehavior menubehvaior;

        public Icon(string TextureName)
        {
            this.TextureName = TextureName;
        }

        void OnStart()
        {

        }

        void ChangeTexture()
        {

        }
        public void OnInputDown(InputEventData eventData)
        {

        }

        public void OnInputUp(InputEventData eventData)
        {
            menubehvaior.Invoke(this.Message, 0);
        }

    }
}