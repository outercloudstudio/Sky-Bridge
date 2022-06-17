using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkyBridge
{
    [RequireComponent(typeof(NetworkedObject))]
    public class NetworkedTransform : MonoBehaviour
    {
        public enum InterpolationMode
        {
            None,
            Interpolate,
            Extrapolate,
        }

        [SerializeField] private int ticksPerUpdate = 4;

        [SerializeField] private InterpolationMode interpolationMode = InterpolationMode.Interpolate;
        [SerializeField] private float interpolationSpeed = 1f;

        private int ticksTillUpdate;

        private NetworkedObject networkObject;

        private float packetTickDelay = 0;
        private float ticksSinceLastPacket = 0;

        public Vector3 targetPostion;
        public Quaternion targetRotation;

        private Vector3 lastPostion;
        private Quaternion lastRotation;

        private Quaternion Assert(Quaternion quaternion)
        {
            if (quaternion.x + quaternion.y + quaternion.z + quaternion.w == 0) return Quaternion.identity;

            return quaternion;
        }

        private void Awake()
        {
            networkObject = GetComponent<NetworkedObject>();

            ticksTillUpdate = ticksPerUpdate;
        }

        private void Start()
        {
            lastPostion = transform.position;
            lastRotation = transform.rotation;

            targetPostion = transform.position;
            targetRotation = transform.rotation;
        }

        public void OnUpdate(Vector3 pos, Quaternion rot)
        {
            packetTickDelay = (packetTickDelay + ticksSinceLastPacket) / 2f;

            ticksSinceLastPacket = 0;

            targetPostion = pos;
            targetRotation = Assert(rot);

            lastPostion = transform.position;
            lastRotation = transform.rotation;

            if (interpolationMode == InterpolationMode.None)
            {
                transform.position = targetPostion;
                transform.rotation = targetRotation;
            }
        }

        private void FixedUpdate()
        {
            if (!networkObject.isRegistered) return;

            if (networkObject.isOwner)
            {
                if (ticksTillUpdate <= 0)
                {
                    SkyBridge.SendEveryone(new Packet("NETWORKED_TRANSFORM_UPDATE").AddValue(networkObject.ID).AddValue(transform.position).AddValue(transform.rotation), SkyBridge.PacketPersistance.UNPERSISTENT, Connection.PacketReliability.UNRELIABLE);
                    SkyBridge.Send(new Packet("NETWORKED_TRANSFORM_UPDATE").AddValue(networkObject.ID).AddValue(transform.position).AddValue(transform.rotation), SkyBridge.client.ID, Connection.PacketReliability.UNRELIABLE);

                    ticksTillUpdate = ticksPerUpdate;
                }

                ticksTillUpdate--;
            }
            else
            {
                if (interpolationMode == InterpolationMode.Interpolate)
                {
                    transform.position = Vector3.Lerp(lastPostion, targetPostion, ticksSinceLastPacket / Mathf.Max(ticksPerUpdate, 1f) * interpolationSpeed);
                    transform.rotation = Assert(Quaternion.Lerp(Assert(lastRotation), Assert(targetRotation), ticksSinceLastPacket / Mathf.Max(ticksPerUpdate, 1f) * interpolationSpeed));
                }
                else if (interpolationMode == InterpolationMode.Extrapolate)
                {
                    transform.position = Vector3.LerpUnclamped(lastPostion, targetPostion, ticksSinceLastPacket / Mathf.Max(ticksPerUpdate, 1f) * interpolationSpeed);
                    transform.rotation = Assert(Quaternion.LerpUnclamped(Assert(lastRotation), Assert(targetRotation), ticksSinceLastPacket / Mathf.Max(ticksPerUpdate, 1f) * interpolationSpeed));
                }

                ticksSinceLastPacket++;
            }
        }
    }
}