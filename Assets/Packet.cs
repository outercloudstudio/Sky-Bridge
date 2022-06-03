using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SkyBridge
{
    [Serializable]
    public class Packet
    {
        public enum PacketType
        {
            DEBUG_PACKET,
            HOST_GAME,
            READY,
            JOIN_GAME,
            ERROR,
            SIGNAL_JOIN
        }

        [Serializable]
        public class SerializedValue
        {
            public enum Type
            {
                FLOAT,
                INT,
                BOOLEAN,
                STRING,
                VECTOR3,
                VECTOR2,
                VECTOR3INT,
                VECTOR2INT,
                QUATERNION,
            }

            public Type valueType;

            public byte[] serializedValue;

            public object unserializedValue;

            public SerializedValue(float value)
            {
                valueType = Type.FLOAT;

                serializedValue = BitConverter.GetBytes(value);

                unserializedValue = value;
            }

            public SerializedValue(int value)
            {
                valueType = Type.INT;

                serializedValue = BitConverter.GetBytes(value);

                unserializedValue = value;
            }

            public SerializedValue(bool value)
            {
                valueType = Type.BOOLEAN;

                serializedValue = BitConverter.GetBytes(value);

                unserializedValue = value;
            }

            public SerializedValue(string value)
            {
                valueType = Type.STRING;

                serializedValue = Encoding.ASCII.GetBytes(value);

                unserializedValue = value;
            }

            public SerializedValue(Vector3 value)
            {
                valueType = Type.VECTOR3;

                byte[] bytes = new byte[12];

                Buffer.BlockCopy(BitConverter.GetBytes(value.x), 0, bytes, 0, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(value.y), 0, bytes, 4, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(value.z), 0, bytes, 4 + 4, 4);

                serializedValue = bytes;

                unserializedValue = value;
            }

            public SerializedValue(Vector2 value)
            {
                valueType = Type.VECTOR2;

                byte[] bytes = new byte[8];

                Buffer.BlockCopy(BitConverter.GetBytes(value.x), 0, bytes, 0, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(value.y), 0, bytes, 4, 4);

                serializedValue = bytes;

                unserializedValue = value;
            }

            public SerializedValue(Vector3Int value)
            {
                valueType = Type.VECTOR3INT;

                byte[] bytes = new byte[12];

                Buffer.BlockCopy(BitConverter.GetBytes(value.x), 0, bytes, 0, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(value.y), 0, bytes, 4, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(value.z), 0, bytes, 4 + 4, 4);

                serializedValue = bytes;

                unserializedValue = value;
            }

            public SerializedValue(Vector2Int value)
            {
                valueType = Type.VECTOR2INT;

                byte[] bytes = new byte[8];

                Buffer.BlockCopy(BitConverter.GetBytes(value.x), 0, bytes, 0, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(value.y), 0, bytes, 4, 4);

                serializedValue = bytes;

                unserializedValue = value;
            }

            public SerializedValue(Quaternion value)
            {
                valueType = Type.QUATERNION;

                byte[] bytes = new byte[16];

                Buffer.BlockCopy(BitConverter.GetBytes(value.x), 0, bytes, 0, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(value.y), 0, bytes, 4, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(value.z), 0, bytes, 4 + 4, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(value.w), 0, bytes, 4 + 4 + 4, 4);

                serializedValue = bytes;

                unserializedValue = value;
            }

            public byte[] GetBytes()
            {
                byte[] bytes = new byte[4 + 4 + serializedValue.Length];

                Buffer.BlockCopy(BitConverter.GetBytes(bytes.Length), 0, bytes, 0, 4);

                Buffer.BlockCopy(BitConverter.GetBytes((int)valueType), 0, bytes, 4, 4);

                Buffer.BlockCopy(serializedValue, 0, bytes, 4 + 4, serializedValue.Length);

                return bytes;
            }

            public static SerializedValue Deserialize(byte[] bytes)
            {
                byte[] valueTypeBytes = bytes[4..8];

                Type valueType = (Type)BitConverter.ToInt32(valueTypeBytes);

                switch (valueType)
                {
                    case Type.FLOAT:
                        return new SerializedValue(BitConverter.ToSingle(bytes[8..12]));
                    case Type.INT:
                        return new SerializedValue(BitConverter.ToInt32(bytes[8..12]));
                    case Type.BOOLEAN:
                        return new SerializedValue(BitConverter.ToBoolean(bytes[8..9]));
                    case Type.STRING:
                        return new SerializedValue(Encoding.ASCII.GetString(bytes[8..(bytes.Length)]));
                    case Type.VECTOR3:
                        return new SerializedValue(new Vector3(BitConverter.ToSingle(bytes[8..12]), BitConverter.ToSingle(bytes[12..16]), BitConverter.ToSingle(bytes[16..20])));
                    case Type.VECTOR2:
                        return new SerializedValue(new Vector2(BitConverter.ToSingle(bytes[8..12]), BitConverter.ToSingle(bytes[12..16])));
                    case Type.VECTOR3INT:
                        return new SerializedValue(new Vector3(BitConverter.ToInt32(bytes[8..12]), BitConverter.ToInt32(bytes[12..16]), BitConverter.ToInt32(bytes[16..20])));
                    case Type.VECTOR2INT:
                        return new SerializedValue(new Vector3(BitConverter.ToInt32(bytes[8..12]), BitConverter.ToInt32(bytes[12..16])));
                    case Type.QUATERNION:
                        return new SerializedValue(new Quaternion(BitConverter.ToSingle(bytes[8..12]), BitConverter.ToSingle(bytes[12..16]), BitConverter.ToSingle(bytes[16..20]), BitConverter.ToSingle(bytes[20..24])));
                }

                return null;
            }
        }

        public PacketType packetType;

        public List<SerializedValue> values = new List<SerializedValue>();

        public Packet(PacketType _packetType)
        {
            packetType = _packetType;
        }

        public Packet(byte[] bytes)
        {
            byte[] packetLengthBytes = bytes[0..4];

            int packetLength = BitConverter.ToInt32(packetLengthBytes);

            byte[] packetTypeBytes = bytes[4..8];

            packetType = (PacketType)BitConverter.ToInt32(packetTypeBytes);

            for (int i = 4 + 4; i < packetLength;)
            {
                byte[] valueLengthBytes = bytes[i..(i + 4)];
                int valueLength = BitConverter.ToInt32(valueLengthBytes);

                byte[] valueBytes = bytes[i..(i + valueLength)];

                SerializedValue value = SerializedValue.Deserialize(valueBytes);

                values.Add(value);

                i += valueLength;
            }
        }

        public Packet AddValue(float value)
        {
            values.Add(new SerializedValue(value));

            return this;
        }

        public Packet AddValue(int value)
        {
            values.Add(new SerializedValue(value));

            return this;
        }

        public Packet AddValue(bool value)
        {
            values.Add(new SerializedValue(value));

            return this;
        }

        public Packet AddValue(string value)
        {
            values.Add(new SerializedValue(value));

            return this;
        }

        public Packet AddValue(Vector3 value)
        {
            values.Add(new SerializedValue(value));

            return this;
        }

        public Packet AddValue(Vector2 value)
        {
            values.Add(new SerializedValue(value));

            return this;
        }

        public Packet AddValue(Vector3Int value)
        {
            values.Add(new SerializedValue(value));

            return this;
        }

        public Packet AddValue(Vector2Int value)
        {
            values.Add(new SerializedValue(value));

            return this;
        }

        public Packet AddValue(Quaternion value)
        {
            values.Add(new SerializedValue(value));

            return this;
        }

        public byte[] ToBytes()
        {
            int packetLength = 4 + 4;

            foreach (SerializedValue serializedValue in values)
            {
                packetLength += serializedValue.GetBytes().Length;
            }

            byte[] bytes = new byte[packetLength];

            Buffer.BlockCopy(BitConverter.GetBytes(packetLength), 0, bytes, 0, 4);

            Buffer.BlockCopy(BitConverter.GetBytes((int)packetType), 0, bytes, 4, 4);

            int writePos = 4 + 4;

            foreach (SerializedValue serializedValue in values)
            {
                byte[] valueBytes = serializedValue.GetBytes();

                Buffer.BlockCopy(valueBytes, 0, bytes, writePos, valueBytes.Length);

                writePos += valueBytes.Length;
            }

            return bytes;
        }
    }
}