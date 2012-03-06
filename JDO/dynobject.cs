using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Reflection;
using System.Collections;

namespace JDO.Dynamic
{
    /// <summary>
    /// Creates a dynamic object, allowing member style access of dictionary keys. Primarily designed for
    /// use with JSON, though applicable in other scenarios.
    /// </summary>
    public class DynObject : DynamicObject, IDictionary<String, Object>
    {
        // Reference to our internal wrapped object.
        public dynamic wrappedObject { get; set; }

        /// <summary>
        /// Constructor for DynObject. If a dictionary isn't passed in, we create one, and wrap it.
        /// </summary>
        /// <param name="obj">The dictionary to wrap.</param>
        public DynObject(IDictionary<String, Object> obj=null)
        {
            if (obj != null)
            {
                wrappedObject = obj;
            }
            else
            {
                // If we weren't passed anything, create a new internal dictionary
                wrappedObject = new Dictionary<String, Object>();
            } // end if
        } // end DynObject

        /// <summary>
        /// Attempts to get the requested member as a key in our wrapped dictionary. If the key does not exist, we first
        /// try accessing the member as a member of the Dictionary class. Failing that, we set it on the dictionary, and
        /// return a new DynObject, wrapping a dictionary.
        /// </summary>
        /// <param name="binder">The GetMemberBinder containing information about the member we're attempting to access.</param>
        /// <param name="result">The results of calling/accessing the member.</param>
        /// <returns>Returns true if we successfully called or accessed the member, false otherwise.</returns>
        public override bool TryGetMember(GetMemberBinder binder, out Object result)
        {
            if (wrappedObject.ContainsKey(binder.Name))
            {
                try
                {
                    // Attempt to access the member on our internal dict.
                    result = Wrap(wrappedObject[binder.Name]);
                    return true;
                }
                catch
                {
                    // Something happened, and we failed to retrieve the key.
                    result = null;
                    return false;
                } // end try/catch
            }
            else
            {
                try
                {
                    // Attempt to access internal members
                    result = wrappedObject.GetType().GetProperty(binder.Name).GetValue(wrappedObject, null);
                    return true;
                }
                catch
                {
                    // There's no member by that name, so, set a new key in the dictionary. 
                    // This will make nesting easier.
                    result = new DynObject();
                    wrappedObject[binder.Name] = result;
                    return true;
                } // end try/catch
            } // end if
        } // end TryGetMember

        /// <summary>
        /// Attempts to set the member on the internal dictionary. If this fails, do nothing.
        /// </summary>
        /// <param name="binder">The GetMemberBinder containing information about the member we're attempting to set.</param>
        /// <param name="value">The value we're trying to assign to the member.</param>
        /// <returns>Returns true if successfully set, otherwise false.</returns>
        public override bool TrySetMember(SetMemberBinder binder, Object value)
        {
            try
            {
                // If we're trying to set DynList or DynObject, we really should only set the underlying object.
                // This really helps keep things clean, and serializable.
                if (value is DynObject)
                {
                    wrappedObject[binder.Name] = ((DynObject) value).wrappedObject;
                }
                else if (value is DynList)
                {
                    wrappedObject[binder.Name] = ((DynList) value).wrappedObject;
                }
                else
                {
                    wrappedObject[binder.Name] = value;
                } // end if
                return true;
            }
            catch
            {
                return false;
            } // end try/catch
        } // end TrySetMember

        /// <summary>
        /// Attempts to invoke the given method on the wrapped object.
        /// </summary>
        /// <param name="binder">The method we're attempting to invoke.</param>
        /// <param name="args">The arguments to invoke the method with.</param>
        /// <param name="result">The results of the method invocation.</param>
        /// <returns>True if we successfully invoked the method, false otherwise.</returns>
        public override bool TryInvokeMember(InvokeMemberBinder binder, Object[] args, out Object result)
        {
            try
            {
                result = wrappedObject.GetType().InvokeMember(binder.Name, BindingFlags.InvokeMethod, null, wrappedObject, args);
                return true;
            }
            catch
            {
                result = null;
                return false;
            } // end try/catch
        } // end TryInvokeMember


        /// <summary>
        /// Attempt to wrap an object. If it is a dictionary, it will wrap it with a DynObject. If it is a list, it will
        /// wrap it with a DynList. Otherwise, it will simply return the object. (This is an optimization, so we only wrap
        /// what is required, and let dynamic to the rest of the work.)
        /// </summary>
        /// <param name="objectToWrap">The object we're wrapping.</param>
        /// <returns>The wrapped object as either a DynObject/DynList, or the original object.</returns>
        public static dynamic Wrap(Object objectToWrap)
        {
            if (objectToWrap is IDictionary<String, Object>)
            {
                // We wrap dictionaries in DynObjects
                return new DynObject(objectToWrap as IDictionary<String, Object>);
            }
            else if (objectToWrap is IList<Object>)
            {
                // And wrap lists in DynLists
                return new DynList(objectToWrap as IList<Object>);
            }
            else
            {
                // Otherwise, we return the original object
                return objectToWrap;
            } // end if
        } // end Wrap

        /// <summary>
        /// Renders the DynObject as a json string. Warning: This renders the entire hierarchy, and therefore shouldn't
        /// be called from time-sensitive code. Calling this method repeatedly may be CPU intensive.
        /// </summary>
        /// <returns>A string, representing the DynObject as a JSON object.</returns>
        public string renderJson()
        {
            return JSON.Serialize(wrappedObject);
        } // end renderJson

        #region IDictionary Members
        public int Count
        {
            get
            {
                return wrappedObject.Count;
            } // end get
        } // end Count

        public bool IsReadOnly
        {
            get
            {
                return wrappedObject.IsReadOnly;
            } // end get
        } // end IsReadOnly

        public object this[String key]
        {
            get
            {
                return DynObject.Wrap(wrappedObject[key]);
            } // end get

            set
            {
                // If we're trying to set DynList or DynObject, we really should only set the underlying object.
                // This really helps keep things clean, and serializable.
                if (value is DynObject)
                {
                    wrappedObject[key] = ((DynObject) value).wrappedObject;
                }
                else if (value is DynList)
                {
                    wrappedObject[key] = ((DynList) value).wrappedObject;
                }
                else
                {
                    wrappedObject[key] = value;
                } // end if

            } // end set
        } // end this[]

        public ICollection<String> Keys
        {
            get
            {
                return wrappedObject.Keys;
            }
        } // end Keys

        public ICollection<Object> Values
        {
            get
            {
                return wrappedObject.Values;
            }
        } // end Keys

        public void Add(KeyValuePair<String, Object> kvp)
        {
            wrappedObject.Add(kvp);
        } // end Add

        public void Add(String key, Object value)
        {
            this[key] = value;
        } // end Add

        public bool ContainsKey(String key)
        {
            return wrappedObject.ContainsKey(key);
        } // end ContainsKey

        public bool TryGetValue(String key, out Object value)
        {
            return wrappedObject.TryGetValue(key, out value);
        } // end TryGetValue

        #endregion

        #region ICollection Members
    
        public void Clear()
        {
            wrappedObject.Clear();
        } // end Clear
    
        public bool Contains(KeyValuePair<String, Object> value)
        {
            return wrappedObject.Contains(value);
        } // end Contains
    
        public bool Remove(KeyValuePair<String, Object> value)
        {
            return wrappedObject.Remove(value);
        } // end Remove

        public bool Remove(String key)
        {
            return wrappedObject.Remove(key);
        } // end Remove
        
        public void CopyTo(KeyValuePair<String, Object>[] array, int index)
        {
            wrappedObject.CopyTo(array, index);
        } // end CopyTo
    
        #endregion

        #region IEnumerable Members
        // (aka: LOLWUT?)

        IEnumerator IEnumerable.GetEnumerator()
        {
            return wrappedObject.GetEnumerator();
        } // end GetEnumerator

        IEnumerator<KeyValuePair<String, Object>> IEnumerable<KeyValuePair<String, Object>>.GetEnumerator()
        {
            return wrappedObject.GetEnumerator();
        } // end GetEnumerator

        #endregion

    } // end DynObj
} // end namespace
