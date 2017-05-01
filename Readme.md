**Project Description**
JSON Dynamic Object (JDO) is a small and lightweight library that provides a simple paradigm of working with JSON objects in .Net. Everything is stored as a native type, and made easily accessible through member access notation and internally uses the JavaScriptSerializer Class.


**JSON Dynamic Object**

At it's heart, JDO is just a wrapper around {{ JavaScriptSerializer }}. It utilizes C# 4.0's {{ dynamic }} keyword, as well as the new {{ DynamicObject }} class to provide a simple to use wrapper class around JSON objects. Not only does it make deserializing JSON into native types simple, but it makes generating JSON much more like writing javascript than C# code.

It's easiest to understand JDO with a few basic examples:

_Reading JSON_
{code: c#}
using JDO;

dynamic json = JSON.Deserialize("\foo"\":\"bar\",\"baz\":3");
Console.WriteLine("foo is: {0}, baz is: {1}", json.foo, json.baz);
{code: c#}

_Creating JSON with accessor syntax_
{code: c#}
using JDO;
using JDO.Dynamic;

// Using accessor notation
dynamic json = new DynObject();
json.foo = "bar";
json.baz = 3;

Console.WriteLine("json is: \"{0}\"", JSON.Serialize(json));
{code: c#}

_Creating JSON with Collection Initializer syntax_
{code: c#}
using JDO;
using JDO.Dynamic;

// Using Collection Initializer notation
dynamic json = new DynObject {{"foo", "bar"}, {"baz", 3}};

Console.WriteLine("json is: \"{0}\"", JSON.Serialize(json));
{code: c#}