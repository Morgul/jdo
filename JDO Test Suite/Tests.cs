using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Web.Script.Serialization;

using System.Dynamic;

using JDO.Dynamic;

namespace JDO_Test_Suite
{
    public partial class Tests : Form
    {
        public Tests()
        {
            InitializeComponent();

            string json = "{\"foo\": 3, \"bar\": [9, 8, 7], \"baz\": [{\"red_balloons\":99}]}";

            var serializer = new JavaScriptSerializer();

            dynamic foobar = serializer.DeserializeObject(json);

            Console.WriteLine("So far... So Good.");

            Console.WriteLine("foobar type: {0}", foobar.GetType());

            foreach (var key in foobar.Keys)
            {
                Console.WriteLine("foobar: {0}", key);
            }

            Console.WriteLine("foo is: {0}, bar is: {1}, and bar[2] is: {2}.", foobar["foo"], foobar["bar"], foobar["bar"][2]);


            // dynobj tests

            dynamic foo = DynObject.Wrap(foobar);

            IList<object> bar = foo.bar as IList<object>;
            Console.WriteLine("bar: {0}", bar[2]);

            Console.WriteLine("DynObj foo.foo: {0}", foo.foo);

            Console.WriteLine("[DYNOBJ] foo is: {0}.", foo.foo);
            Console.WriteLine("[DYNOBJ] foo is: {0}, bar is: {1}.", foo.foo, foo.bar);
            Console.WriteLine("[DYNOBJ] foo is: {0}, bar is: {1}, bar[2] is: {2}.", foo.foo, foo.bar, foo.bar[2]);
            Console.WriteLine("[DYNOBJ] foo is: {0}, bar is: {1}, bar[2] is: {2}, baz.red_balloons is: {3}.", foo.foo, foo.bar, foo.bar[2], foo.baz[0].red_balloons);

        }
    }
}
