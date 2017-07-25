using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

namespace Holograph
{
    public class VoiceCommandManager : MonoBehaviour
    {
        private Dictionary<string, System.Action> Keywords;
        private KeywordRecognizer WordRecognizer;

        void Start()
        {
            Keywords = new Dictionary<string, System.Action>();
            Keywords.Add("drop", () => {
                NetworkMessages.Instance.SendPresenterId();
            });

            Debug.Log("YOYOYOYOYOOY");

            WordRecognizer = new KeywordRecognizer(Keywords.Keys.ToArray());
            WordRecognizer.OnPhraseRecognized += WordRecognizerOnPhraseRecognized;
            WordRecognizer.Start();
        }

        void WordRecognizerOnPhraseRecognized(PhraseRecognizedEventArgs args)
        {
            System.Action keywordAction;

            if (Keywords.TryGetValue(args.text, out keywordAction))
            {
                keywordAction.Invoke();
            }

        }
    }
}