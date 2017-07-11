using UnityEngine;
using HoloToolkit.Unity.InputModule;

namespace Holograph
{
    public class Icon : MonoBehaviour, IInputClickHandler
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

        public void OnInputClicked(InputClickedEventData eventData)
        {
            menubehvaior.Invoke(this.Message, 0);
        }

    }
}