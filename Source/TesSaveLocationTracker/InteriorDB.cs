using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesSaveLocationTracker
{
    /// <summary>
    /// Interior database for plugins.
    /// </summary>
    public class InteriorDB
    {
        public string PluginName { get; private set; }

        Dictionary<int, Tuple<float, float, float>> database;

        private InteriorDB()
        {

        }

        public Tuple<float, float, float> GetInteriorPosition(int formID)
        {
            Tuple<float, float, float> value;

            if (database.TryGetValue(formID, out value))
                return value;
            return null;
        }

        public static InteriorDB Parse(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            InteriorDB db = new InteriorDB();

            using (var reader = new BinaryReader(stream))
            {
                int version = reader.ReadInt32();
                int pluginNameLength = reader.ReadUInt16();
                string pluginName = Encoding.ASCII.GetString(reader.ReadBytes(pluginNameLength));

                db.PluginName = pluginName;
                db.database = new Dictionary<int, Tuple<float, float, float>>();

                while (stream.Position < stream.Length) {
                    int formID = reader.ReadInt32();
                    float x = reader.ReadSingle();
                    float y = reader.ReadSingle();
                    float z = reader.ReadSingle();

                    db.database.Add(formID, new Tuple<float, float, float>(x, y, z));
                }
            }

            return db;
        }
    }
}
