using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Collections.ObjectModel;

namespace JDO.Dynamic
{
    class DynObjectConverter : JavaScriptConverter
    {
        public override IEnumerable<Type> SupportedTypes
        {
            //Define the ListItemCollection as a supported type.
            get
            {
                return new ReadOnlyCollection<Type>(new List<Type>(new Type[] { typeof(DynObject) })); 
            } // end get
        } // end SupportedTypes

        public override Object Deserialize(IDictionary<String, Object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            //return DynObject.Wrap(dictionary);
            throw new NotImplementedException("This will never get called, since it's really just a dictionary.");
        } // end Deserialize

        public override IDictionary<String, Object> Serialize(Object obj, JavaScriptSerializer serializer)
        {
            return ((DynObject) obj).wrappedObject;
        } // end Serialize
    } // end DynObjectConverter
} // end namespace
