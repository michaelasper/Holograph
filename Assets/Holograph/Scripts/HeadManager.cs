// HeadManager based on the HoloToolkit RemoteHeadManager

using System;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Sharing.Tests;
using HoloToolkit.Sharing;
using HoloToolkit.Unity;

namespace Holograph
{
    public class RemoteHeadInfo
    {
        public long UserID;
        public GameObject HeadObject;
    }
    public class HeadManager : Singleton<HeadManager>
    {

        public GameObject HeadPrefab;
        public GameObject[] Panels;

        /// <summary>
        /// Keep a list of the remote heads, indexed by XTools userID
        /// </summary>
        private Dictionary<long, RemoteHeadInfo> remoteHeads = new Dictionary<long, RemoteHeadInfo>();

        private void Start()
        {
            NetworkMessages.Instance.MessageHandlers[NetworkMessages.MessageID.HeadTransform] = UpdateHeadTransform;
            NetworkMessages.Instance.MessageHandlers[NetworkMessages.MessageID.PresenterId] = UpdatePresenterId;

            // SharingStage should be valid at this point, but we may not be connected.
            if (SharingStage.Instance.IsConnected)
            {
                Connected();
            }
            else
            {
                SharingStage.Instance.SharingManagerConnected += Connected;
            }
        }

        private void Connected(object sender = null, EventArgs e = null)
        {
            SharingStage.Instance.SharingManagerConnected -= Connected;

            SharingStage.Instance.SessionUsersTracker.UserJoined += UserJoinedSession;
            SharingStage.Instance.SessionUsersTracker.UserLeft += UserLeftSession;
        }

        private void Update()
        {
            // Grab the current head transform and broadcast it to all the other users in the session
            Transform headTransform = Camera.main.transform;

            // Transform the head position and rotation from world space into local space
            Vector3 headPosition = transform.InverseTransformPoint(headTransform.position);
            Quaternion headRotation = Quaternion.Inverse(transform.rotation) * headTransform.rotation;

            NetworkMessages.Instance.SendHeadTransform(headPosition, headRotation);
        }

        protected override void OnDestroy()
        {
            if (SharingStage.Instance != null)
            {
                if (SharingStage.Instance.SessionUsersTracker != null)
                {
                    SharingStage.Instance.SessionUsersTracker.UserJoined -= UserJoinedSession;
                    SharingStage.Instance.SessionUsersTracker.UserLeft -= UserLeftSession;
                }
            }

            base.OnDestroy();
        }

        /// <summary>
        /// Called when a new user is leaving the current session.
        /// </summary>
        /// <param name="user">User that left the current session.</param>
        private void UserLeftSession(User user)
        {
            int userId = user.GetID();
            if (userId != SharingStage.Instance.Manager.GetLocalUser().GetID())
            {
                RemoveRemoteHead(remoteHeads[userId].HeadObject);
                remoteHeads.Remove(userId);
            }
        }

        /// <summary>
        /// Called when a user is joining the current session.
        /// </summary>
        /// <param name="user">User that joined the current session.</param>
        private void UserJoinedSession(User user)
        {
            if (user.GetID() != SharingStage.Instance.Manager.GetLocalUser().GetID())
            {
                GetRemoteHeadInfo(user.GetID());
            }
        }

        /// <summary>
        /// Gets the data structure for the remote users' head position.
        /// </summary>
        /// <param name="userId">User ID for which the remote head info should be obtained.</param>
        /// <returns>RemoteHeadInfo for the specified user.</returns>
        public RemoteHeadInfo GetRemoteHeadInfo(long userId)
        {
            RemoteHeadInfo headInfo;

            if (userId == SharingStage.Instance.Manager.GetLocalUser().GetID())
            {
                return null;
            }

            // Get the head info if its already in the list, otherwise add it
            if (!remoteHeads.TryGetValue(userId, out headInfo))
            {
                headInfo = new RemoteHeadInfo();
                headInfo.UserID = userId;
                headInfo.HeadObject = CreateRemoteHead();

                remoteHeads.Add(userId, headInfo);
            }

            return headInfo;
        }

        /// <summary>
        /// Called when a remote user sends a head transform.
        /// </summary>
        /// <param name="msg"></param>
        private void UpdateHeadTransform(NetworkInMessage msg)
        {
            // Parse the message
            long userID = msg.ReadInt64();
            Vector3 headPos = NetworkMessages.Instance.ReadVector3(msg);

            Quaternion headRot = NetworkMessages.Instance.ReadQuaternion(msg);

            RemoteHeadInfo headInfo = GetRemoteHeadInfo(userID);
            headInfo.HeadObject.transform.localPosition = headPos;
            headInfo.HeadObject.transform.localRotation = headRot;
        }

        private void UpdatePresenterId(NetworkInMessage msg)
        {
            long userId = msg.ReadInt64(); //The user that sent the msg.
            long presenterId = msg.ReadInt64(); // The user that we want to set as presenter.

            RemoteHeadInfo headInfo = GetRemoteHeadInfo(presenterId);

            for (var i = 0; i < Panels.Length; i++)
            {
                var billboard = Panels[i].GetComponent<Billboard>();

                billboard.TargetTransform = headInfo == null ? Camera.main.transform : headInfo.HeadObject.transform;
            }
        }

        /// <summary>
        /// Creates a new game object to represent the user's head.
        /// </summary>
        /// <returns></returns>
        private GameObject CreateRemoteHead()
        {
            return Instantiate(HeadPrefab, transform);
        }

        /// <summary>
        /// When a user has left the session this will cleanup their
        /// head data.
        /// </summary>
        /// <param name="remoteHeadObject"></param>
        private void RemoveRemoteHead(GameObject remoteHeadObject)
        {
            DestroyImmediate(remoteHeadObject);
        }
    }
}