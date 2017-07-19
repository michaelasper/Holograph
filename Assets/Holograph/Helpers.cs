using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Holograph
{
    class Helpers
    {
        public static byte[] GameObjectArrayToByteArray(GameObject[] array)
        {
            byte[] byteArray = new byte[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                byteArray[i] = Convert.ToByte(array[i]);
            }

            return byteArray;
        }

        public static GameObject[] ByteArrayToGameObjectArray(byte[] array)
        {
            return array.Cast<GameObject>().ToArray();
        }

        public static GameObject[] GameObjectLinkListToArray(LinkedList<GameObject> linkList)
        {
            GameObject[] gameObjectArray = new GameObject[linkList.Count];

            linkList.CopyTo(gameObjectArray, 0);

            return gameObjectArray;
        }
    }
}
