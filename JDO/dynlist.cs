using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace JDO.Dynamic
{
    /// <summary>
    /// A List wrapper, which returns its contents wrapped by DynObject. This _specifically_ retains the list syntax for member access.
    /// </summary>
    public class DynList : IList<Object>
    {
        // Our internal list
        public IList<Object> wrappedObject {get; set;}

        /// <summary>
        /// Constructor for DynList.
        /// </summary>
        /// <param name="list">The IList we are going to be wrapping.</param>
        public DynList(IList<Object> list=null)
        {
            if (list == null)
            {
                wrappedObject = new List<Object>();
            }
            else
            {
                wrappedObject = list;
            } // end if
        } // end DynList

        #region DynList Overrides

        /// <summary>
        /// Accessor method of members of the list. Returns those objects wrapped by DynObject.
        /// </summary>
        /// <param name="index">The index of the time we're retrieving.</param>
        /// <returns>The object, as returned by DynObject.Wrap.</returns>
        public object this[int index]
        {
            get
            {
                return DynObject.Wrap(wrappedObject[index]);
            } // end get
            set
            {
                // If we're trying to set DynList or DynObject, we really should only set the underlying object.
                // This really helps keep things clean, and serializable.
                if (value is DynObject)
                {
                    wrappedObject[index] = ((DynObject) value).wrappedObject;
                }
                else if (value is DynList)
                {
                    wrappedObject[index] = ((DynList) value).wrappedObject;
                }
                else
                {
                    wrappedObject[index] = value;
                } // end if

            } // end set
        } // end object this[]

        #endregion

        #region IList Members

        // Most of this is just requirements for being an IList. We pass it all through
        // to our underlying list object.
        
        public void Add(object value)
        {
            wrappedObject.Add(value);
        } // end Add
    
        public void Clear()
        {
            wrappedObject.Clear();
        } // end Clear
    
        public bool Contains(object value)
        {
            return wrappedObject.Contains(value);
        } // end Contains
    
        public int IndexOf(object value)
        {
            return wrappedObject.IndexOf(value);
        } // end IndexOf
    
        public void Insert(int index, object value)
        {
            wrappedObject.Insert(index, value);
        } // end Insert
    
        public bool IsReadOnly
        {
            get
            {
                return wrappedObject.IsReadOnly;
            } // end get
        } // end IsReadOnly
    
        public bool Remove(object value)
        {
            return wrappedObject.Remove(value);
        } // end Remove
    
        public void RemoveAt(int index)
        {
            wrappedObject.RemoveAt(index);
        } // end RemoveAt

        #endregion

        #region ICollection Members
    
        public void CopyTo(object[] array, int index)
        {
            wrappedObject.CopyTo(array, index);
        } // end CopyTo
    
        public int Count
        {
            get
            {
                return wrappedObject.Count;
            } // end get
        } // end Count

        #endregion

        #region IEnumerable Members
        // (aka: LOLWUT?)

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (this as IEnumerable<object>).GetEnumerator();
        } // end GetEnumerator

        IEnumerator<object> IEnumerable<object>.GetEnumerator()
        {
            foreach (object obj in wrappedObject)
            {
                yield return DynObject.Wrap(obj);
            } // end foreach
        } // end GetEnumerator

        #endregion

    } // end DynList
} // end namespace
