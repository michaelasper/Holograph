// NetowkrMessages based on the HoloToolkit CustomMessages

using System;
using System.Collections.Generic;
using HoloToolkit.Unity;
using UnityEngine;
using HoloToolkit.Sharing;

namespace Holograph
{
    /// <summary>
    /// Test class for demonstrating how to send custom messages between clients.
    /// </summary>
    public class NetworkMessages : Singleton<NetworkMessages>
    {
        /// <summary>
        /// Message enum containing our information bytes to share.
        /// The first message type has to start with UserMessageIDStart
        /// so as not to conflict with HoloToolkit internal messages.
        /// </summary>
        public enum MessageID : byte
        {
            HeadTransform = HoloToolkit.Sharing.MessageID.UserMessageIDStart,
            PresenterId,
            AnimationHash,
            MenuAnimationHash,
            MapRotation,
            RadialMenu,
            RadialMenuClickIcon,
            FirstNodeTransform,
            StoryControl,
            Button1Animation,
            Max
        }

        public enum AnimationTypes : int
        {
            Boolean = 0,
            Integer,
            Float,
            Trigger
        }

        public enum UserMessageChannels
        {
            Anchors = MessageChannel.UserMessageChannelStart
        }

        /// <summary>
        /// Cache the local user's ID to use when sending messages
        /// </summary>
        public long LocalUserID
        {
            get; set;
        }

        public delegate void MessageCallback(NetworkInMessage msg);
        private Dictionary<MessageID, MessageCallback> messageHandlers = new Dictionary<MessageID, MessageCallback>();
        public Dictionary<MessageID, MessageCallback> MessageHandlers
        {
            get
            {
                return messageHandlers;
            }
        }

        /// <summary>
        /// Helper object that we use to route incoming message callbacks to the member
        /// functions of this class
        /// </summary>
        private NetworkConnectionAdapter connectionAdapter;

        /// <summary>
        /// Cache the connection object for the sharing service
        /// </summary>
        private NetworkConnection serverConnection;

        private void Start()
        {
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
            InitializeMessageHandlers();
        }

        private void InitializeMessageHandlers()
        {
            SharingStage sharingStage = SharingStage.Instance;

            if (sharingStage == null)
            {
                Debug.Log("Cannot Initialize NetworkMessages. No SharingStage instance found.");
                return;
            }

            serverConnection = sharingStage.Manager.GetServerConnection();
            if (serverConnection == null)
            {
                Debug.Log("Cannot initialize NetworkMessages. Cannot get a server connection.");
                return;
            }

            connectionAdapter = new NetworkConnectionAdapter();
            connectionAdapter.MessageReceivedCallback += OnMessageReceived;

            // Cache the local user ID
            LocalUserID = SharingStage.Instance.Manager.GetLocalUser().GetID();

            for (byte index = (byte)MessageID.HeadTransform; index < (byte)MessageID.Max; index++)
            {
                if (MessageHandlers.ContainsKey((MessageID)index) == false)
                {
                    MessageHandlers.Add((MessageID)index, null);
                }

                serverConnection.AddListener(index, connectionAdapter);
            }
        }

        private NetworkOutMessage CreateMessage(byte messageType)
        {
            NetworkOutMessage msg = serverConnection.CreateMessage(messageType);
            msg.Write(messageType);
            // Add the local userID so that the remote clients know whose message they are receiving
            msg.Write(LocalUserID);
            return msg;
        }

        public void SendHeadTransform(Vector3 position, Quaternion rotation)
        {
            // If we are connected to a session, broadcast our head info
            if (serverConnection != null && serverConnection.IsConnected())
            {
                // Create an outgoing network message to contain all the info we want to send
                NetworkOutMessage msg = CreateMessage((byte)MessageID.HeadTransform);

                AppendTransform(msg, position, rotation);

                // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.
                serverConnection.Broadcast(
                    msg,
                    MessagePriority.Immediate,
                    MessageReliability.UnreliableSequenced,
                    MessageChannel.Avatar);
            }
        }

        public void SendPresenterId(long userId)
        {
            if (serverConnection != null && serverConnection.IsConnected())
            {
                NetworkOutMessage msg = CreateMessage((byte)MessageID.PresenterId);
                msg.Write(userId);
                // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.
                serverConnection.Broadcast(
                    msg,
                    MessagePriority.Immediate,
                    MessageReliability.Unreliable,
                    MessageChannel.Avatar);
            }
        }

        public void SendAnimationHash(int animationHash, AnimationTypes type, float value = -1)
        {
            if (serverConnection != null && serverConnection.IsConnected())
            {
                NetworkOutMessage msg = CreateMessage((byte)MessageID.AnimationHash);
                msg.Write(animationHash);
                msg.Write((int)type);
                msg.Write(value);

                // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.
                serverConnection.Broadcast(
                    msg,
                    MessagePriority.Immediate,
                    MessageReliability.Unreliable,
                    MessageChannel.Default);
            }
        }

        public void SendMenuAnimationHash(int animationHash, AnimationTypes type, float value = -1)
        {
            if (serverConnection != null && serverConnection.IsConnected())
            {
                NetworkOutMessage msg = CreateMessage((byte)MessageID.MenuAnimationHash);
                msg.Write(animationHash);
                msg.Write((int)type);
                msg.Write(value);

                // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.
                serverConnection.Broadcast(
                    msg,
                    MessagePriority.Immediate,
                    MessageReliability.Unreliable,
                    MessageChannel.Default);
            }
        }

        public void SendMapRotation(Quaternion rotation)
        {
            if (serverConnection != null && serverConnection.IsConnected())
            {
                NetworkOutMessage msg = CreateMessage((byte)MessageID.MapRotation);
                msg.Write(rotation.x);
                msg.Write(rotation.y);
                msg.Write(rotation.z);
                msg.Write(rotation.w);

                serverConnection.Broadcast(
                    msg,
                    MessagePriority.Immediate,
                    MessageReliability.Unreliable,
                    MessageChannel.Default);
            }
        }

        public void SendRadialMenu(int nodeId, bool setActive)
        {
            if (serverConnection != null && serverConnection.IsConnected())
            {
                NetworkOutMessage msg = CreateMessage((byte)MessageID.RadialMenu);
                msg.Write(nodeId);
                msg.Write(Convert.ToByte(setActive));
                //msg.Write(Convert.ToByte(isParentTransform));

                serverConnection.Broadcast(
                    msg,
                    MessagePriority.Immediate,
                    MessageReliability.Unreliable,
                    MessageChannel.Default);
            }
        }

        public void SendRadialMenuClickIcon(string methodName)
        {
            if (serverConnection != null && serverConnection.IsConnected())
            {
                NetworkOutMessage msg = CreateMessage((byte)MessageID.RadialMenuClickIcon);
                msg.Write(methodName.Length);
                foreach (char c in methodName.ToCharArray())
                {
                    msg.Write(Convert.ToByte(c));
                }

                serverConnection.Broadcast(
                    msg,
                    MessagePriority.Immediate,
                    MessageReliability.Unreliable,
                    MessageChannel.Default);
            }

        }

        public void SendFirstNodeTransform() // (Transform transform)
        {
            if (serverConnection != null && serverConnection.IsConnected())
            {
                NetworkOutMessage msg = CreateMessage((byte)MessageID.FirstNodeTransform);
                //AppendTransform(msg, transform.position, transform.rotation);

                serverConnection.Broadcast(
                    msg,
                    MessagePriority.Immediate,
                    MessageReliability.Unreliable,
                    MessageChannel.Default);
            }
        }

        public void SendStoryControl(byte action, int[] args)
        {
            if (serverConnection != null && serverConnection.IsConnected())
            {
                NetworkOutMessage msg = CreateMessage((byte)MessageID.StoryControl);
                msg.Write(action);
                if (args == null)
                {
                    msg.Write(0);
                }
                else
                {
                    msg.Write(args.Length);
                    foreach (int x in args)
                    {
                        msg.Write(x);
                    }
                }

                serverConnection.Broadcast(
                    msg,
                    MessagePriority.Immediate,
                    MessageReliability.Unreliable,
                    MessageChannel.Default);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (serverConnection != null)
            {
                for (byte index = (byte)MessageID.HeadTransform; index < (byte)MessageID.Max; index++)
                {
                    serverConnection.RemoveListener(index, connectionAdapter);
                }
                connectionAdapter.MessageReceivedCallback -= OnMessageReceived;
            }
        }

        private void OnMessageReceived(NetworkConnection connection, NetworkInMessage msg)
        {
            byte messageType = msg.ReadByte();
            MessageCallback messageHandler = MessageHandlers[(MessageID)messageType];
            if (messageHandler != null)
            {
                messageHandler(msg);
            }
        }

        #region HelperFunctionsForWriting

        private void AppendTransform(NetworkOutMessage msg, Vector3 position, Quaternion rotation)
        {
            AppendVector3(msg, position);
            AppendQuaternion(msg, rotation);
        }

        private void AppendVector3(NetworkOutMessage msg, Vector3 vector)
        {
            msg.Write(vector.x);
            msg.Write(vector.y);
            msg.Write(vector.z);
        }

        private void AppendQuaternion(NetworkOutMessage msg, Quaternion rotation)
        {
            msg.Write(rotation.x);
            msg.Write(rotation.y);
            msg.Write(rotation.z);
            msg.Write(rotation.w);
        }

        #endregion

        #region HelperFunctionsForReading

        public Vector3 ReadVector3(NetworkInMessage msg)
        {
            return new Vector3(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
        }

        public Quaternion ReadQuaternion(NetworkInMessage msg)
        {
            return new Quaternion(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
        }

        #endregion
    }
}