using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Web.Script.Serialization;

using JDO.Dynamic;

namespace JDO
{
    public class JSON
    {
        // Our internal serializer object.
        private static JavaScriptSerializer _serializer = null;
        private static JavaScriptSerializer serializer
        {
            get
            {
                if (_serializer == null)
                {
                    _serializer = new JavaScriptSerializer();
                    _serializer.RegisterConverters(new List<JavaScriptConverter>{new DynObjectConverter()});
                } // end if
                return _serializer;
            } // end get
        } // end serializer

        /// <summary>
        /// Deserializes a json string, and returns an object hierarchy.
        /// </summary>
        /// <param name="jsonString">A string containing valid json.</param>
        /// <returns>An object hierachry, wrapped in a DynObject.</returns>
        public static dynamic Load(String jsonString)
        {
            return DynObject.Wrap(serializer.DeserializeObject(jsonString));
        } // end Load

        /// <summary>
        /// Serializes an object hierarchy into a JSON string.
        /// </summary>
        /// <param name="dynObject">The object hierarchy to serialize.</param>
        /// <returns>The json representation of the object hierarchy as a string.</returns>
        public static string Dump(dynamic dynObject)
        {
            // We can't serialize DynObject or DynList. So, we check for them, and get the underlying objects.
            if (dynObject is DynObject)
            {
                return serializer.Serialize(dynObject.wrappedObject);
            }
            else if (dynObject is DynList)
            {
                return serializer.Serialize(dynObject.wrappedObject);
            }
            else
            {
                return serializer.Serialize(dynObject);
            } // end if
        } // end TrySetMember

        #region Aliases

        /// <summary>
        /// (Alias for JSON.Load.) Deserializes a json string, and returns an object hierarchy.
        /// </summary>
        /// <param name="jsonString">A string containing valid json.</param>
        /// <returns>An object hierachry, wrapped in a DynObject.</returns>
        public static dynamic Deserialize(String jsonString)
        {
            return Load(jsonString);
        } // end Load

        /// <summary>
        /// (Alias for JSON.Dump.) Serializes an object hierarchy into a JSON string.
        /// </summary>
        /// <param name="dynObject">The object hierarchy to serialize.</param>
        /// <returns>The json representation of the object hierarchy as a string.</returns>
        public static string Serialize(dynamic dynObject)
        {
            return Dump(dynObject);
        } // end TrySetMember

        #endregion

    } // end JSON
} // end namespace
