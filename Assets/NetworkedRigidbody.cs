using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkyBridge
{
    [RequireComponent(typeof(NetworkedObject))]
    [RequireComponent(typeof(Rigidbody))]
    public class NetworkedRigidbody : MonoBehaviour
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
        private Rigidbody rb;

        private float packetTickDelay = 0;
        private float ticksSinceLastPacket = 0;

        public Vector3 targetPostion;
        public Quaternion targetRotation;
        public Vector3 targetVel;
        public Vector3 targetAngVel;

        private Vector3 lastPostion;
        private Quaternion lastRotation;
        private Vector3 lastVel;
        private Vector3 lastAngVel;

        private Quaternion Assert(Quaternion quaternion)
        {
            if (quaternion.x + quaternion.y + quaternion.z + quaternion.w == 0) return Quaternion.identity;

            return quaternion;
        }

        private void Awake()
        {
            networkObject = GetComponent<NetworkedObject>();
            rb = GetComponent<Rigidbody>();

            ticksTillUpdate = ticksPerUpdate;
        }

        private void Start()
        {
            lastPostion = transform.position;
            lastRotation = transform.rotation;
            lastVel = rb.velocity;
            lastAngVel = rb.angularVelocity;

            targetPostion = transform.position;
            targetRotation = transform.rotation;
            targetVel= rb.velocity;
            targetAngVel = rb.angularVelocity;
        }

        public void OnUpdate(Vector3 pos, Quaternion rot, Vector3 vel, Vector3 angVel)
        {
            packetTickDelay = (packetTickDelay + ticksSinceLastPacket) / 2f;

            ticksSinceLastPacket = 0;

            targetPostion = pos;
            targetRotation = Assert(rot);
            targetVel = vel;
            targetAngVel = angVel;

            lastPostion = transform.position;
            lastRotation = transform.rotation;
            lastVel = rb.velocity;
            lastAngVel = rb.angularVelocity;

            if (interpolationMode == InterpolationMode.None)
            {
                rb.velocity = vel;
                rb.angularVelocity = vel;
            }

            transform.position = targetPostion;
            transform.rotation = targetRotation;
        }

        private void FixedUpdate()
        {
            if (!networkObject.isRegistered) return;

            if (networkObject.isOwner)
            {
                if (ticksTillUpdate <= 0)
                {
                    SkyBridge.SendEveryone(new Packet("NETWORKED_RIGIDBODY_UPDATE").AddValue(networkObject.ID).AddValue(transform.position).AddValue(transform.rotation).AddValue(rb.velocity).AddValue(rb.angularVelocity));

                    ticksTillUpdate = ticksPerUpdate;
                }

                ticksTillUpdate--;
            }
            else
            {
                if (interpolationMode == InterpolationMode.Interpolate)
                {
                    //transform.position = Vector3.Lerp(lastPostion, targetPostion, ticksSinceLastPacket / Mathf.Max(ticksPerUpdate, 1f) * interpolationSpeed);
                    //transform.rotation = Assert(Quaternion.Lerp(Assert(lastRotation), Assert(targetRotation), ticksSinceLastPacket / Mathf.Max(ticksPerUpdate, 1f) * interpolationSpeed));
                    rb.velocity = Vector3.Lerp(lastVel, targetVel, ticksSinceLastPacket / Mathf.Max(ticksPerUpdate, 1f) * interpolationSpeed);
                    rb.angularVelocity = Vector3.Lerp(lastAngVel, targetAngVel, ticksSinceLastPacket / Mathf.Max(ticksPerUpdate, 1f) * interpolationSpeed);
                }
                else if (interpolationMode == InterpolationMode.Extrapolate)
                {
                    //transform.position = Vector3.LerpUnclamped(lastPostion, targetPostion, ticksSinceLastPacket / Mathf.Max(ticksPerUpdate, 1f) * interpolationSpeed);
                    //transform.rotation = Assert(Quaternion.LerpUnclamped(Assert(lastRotation), Assert(targetRotation), ticksSinceLastPacket / Mathf.Max(ticksPerUpdate, 1f) * interpolationSpeed));
                    rb.velocity = Vector3.LerpUnclamped(lastVel, targetVel, ticksSinceLastPacket / Mathf.Max(ticksPerUpdate, 1f) * interpolationSpeed);
                    rb.angularVelocity = Vector3.LerpUnclamped(lastAngVel, targetAngVel, ticksSinceLastPacket / Mathf.Max(ticksPerUpdate, 1f) * interpolationSpeed);
                }

                ticksSinceLastPacket++;
            }
        }
    }
}