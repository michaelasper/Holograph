﻿namespace Holograph
{
    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    public class CaseListButton : MonoBehaviour
    {

        public int CaseID;

        public StoryManager StoryManagerObject;


        public void callStory()
        {
            StoryManagerObject.TriggerStory(StoryManager.StoryAction.EnterDefaultStory, CaseID);
        }
    }


}