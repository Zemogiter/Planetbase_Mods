using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;

namespace PlanetbaseMultiplayer.Model.Packets
{
    [Serializable]
    public abstract class Packet
    {
        private static readonly JsonSerializerOptions serializerOptions;

        static Packet()
        {
            serializerOptions = new JsonSerializerOptions
            {
                WriteIndented = false
            };
        }

        public byte[] Serialize()
        {
            return JsonSerializer.SerializeToUtf8Bytes(this, this.GetType(), serializerOptions);
        }

        public static Packet Deserialize(byte[] data, Type type)
        {
            return (Packet)JsonSerializer.Deserialize(data, type, serializerOptions);
        }
    }
}
