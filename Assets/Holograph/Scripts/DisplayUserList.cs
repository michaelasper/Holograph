using System;
using System.Collections.Generic;
using HoloToolkit.Sharing;
using UnityEngine;
using UnityEngine.UI;

namespace Holograph
{
    public class DisplayUserList : MonoBehaviour
    {
        public Text TextPrefab;
        public Dictionary<long, string> Users = new Dictionary<long, string>(0);

        private SessionUsersTracker usersTracker;

        private void Start()
        {
            // SharingStage should be valid at this point, but we may not be connected.
            if (SharingStage.Instance.IsConnected)
                Connected();
            else
                SharingStage.Instance.SharingManagerConnected += Connected;
        }

        private void Connected(object sender = null, EventArgs e = null)
        {
            SharingStage.Instance.SharingManagerConnected -= Connected;

            usersTracker = SharingStage.Instance.SessionUsersTracker;

            for (var i = 0; i < usersTracker.CurrentUsers.Count; i++)
            {
                Users.Add(usersTracker.CurrentUsers[i].GetID(), usersTracker.CurrentUsers[i].GetName());
                CreateUserTextIdentifier(usersTracker.CurrentUsers[i]);
            }

            usersTracker.UserJoined += NotifyUserJoined;
            usersTracker.UserLeft += NotifyUserLeft;
        }

        private void CreateUserTextIdentifier(User user)
        {
            var textObject = Instantiate(TextPrefab, transform);
            var textAsst = textObject.GetComponent<Text>();
            textAsst.text = user.GetName();
            textObject.name = "user_" + user.GetID();
            textObject.transform.position = transform.position;
            textObject.transform.rotation = transform.rotation;
        }

        private void NotifyUserJoined(User user)
        {
            if (user.IsValid())
                if (!Users.ContainsKey(user.GetID()))
                {
                    Users.Add(user.GetID(), user.GetName());

                    CreateUserTextIdentifier(user);
                }
        }

        private void NotifyUserLeft(User user)
        {
            if (user.IsValid())
            {
                var outName = string.Empty;

                if (Users.TryGetValue(user.GetID(), out outName))
                {
                    for (var i = 0; i < transform.childCount; i++)
                    {
                        var child = transform.GetChild(i);
                        var userObject = child.GetComponent<Text>();

                        if (userObject.text.Equals(outName))
                            Destroy(child.gameObject);
                    }

                    Users.Remove(user.GetID());
                }
            }
        }

        private void OnDestroy()
        {
            if (usersTracker != null)
            {
                usersTracker.UserJoined -= NotifyUserJoined;
                usersTracker.UserLeft -= NotifyUserLeft;
            }

            usersTracker = null;
        }
    }
}