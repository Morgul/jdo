## Basic Usage

JDO is rather simple to use, which is part of it's point. Still, documentation never hurt anyone. At this point, I highly recommend you look at the [Design Overview](DesignOverview). If you haven't yet, you might want to, just to get a better sense of what I'm talking about.

**Deserialization**

The driving force for creating this project, for me, was to easy working with JSON on a project for work. Primarily what (I think) most people will want to use JDO for is simply getting a JSON string into something that can be worked with in C#. JDO does this surprisingly well:

_Deserialize JSON_
{code: c#}
using JDO;

dynamic json = JSON.Deserialize("\foo"\":\"bar\",\"baz\":3");

Console.WriteLine("foo is: {0}, baz is: {1}", json.foo, json.baz);
{code: c#}

As you can see, there's not a lot going on here. We import our namespace, and then we simply pass the JSON string to {{JSON.Deserialize}}, and store that in a {{dynamic}} variable. Our use of dynamic here is important; if we used {{object}} (which is all a dynamic really is) then none of our attribute access would work, since {{Object}} doesn't have the members defined.

Really, that's all there is to deserialization; You pass a valid JSON string to {{JSON.Deserialize}}, and stick the result in a {{dynamic}}. From there, it's just working with JDO's {{DynObject}}. (More on that below.)

_Note: There is another method, {{JSON.Load}}, which identical to {{JSON.Deserialize}}. The reason both exist is that I come from a python background, and am more used to using the terms `load` and `dump` to `deserialize` and `serialize`. So, when I initially implemented JDO, I wrote load and dump methods. I've since been convinced that a significant chunk of programers would appreciate better names, so I wrote {{ JSON.Deserialize}} and {{JSON.Serialize}} as aliases, which just call {{JSON.Load}} and {{JSON.Dump}}. Hope that helps._

**Serialization**

Since most JSON communication is two way, having a good, sane method for serializing C# objects was the second priority of JDO. There are a _lot_ of schemes for doing this out there, and I'm sure mine has its own flaws, but to me (a python and javascript fan), JDO's method seems the simplest, most straight-forward out of all of them.

Here's an example of serialization, using the basic, run of the mill property setting of {{DynObject}}. Really, it's about as straight-forward as you can get:

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

Now, I'm not going to get into a discussion about how {{DynObject}} works just yet (see below), but what's going on should be pretty obvious. We create a new {{DynObject}}, and then we set some properties on it, and pass it to {{JSON.Serialize}}. We get back a JSON string. That's it.

**DynObject**

The real magic of JDO resides in {{DynObject}}. Basically, {{DynObject}} is a thing wrapper around {{Dictionary<String, Object>}}. What it allows is member access on the internal dictionary, as well as dynamic creation of members. For example:

_Member access_
{code: c#}
using JDO.Dynamic;

dynamic foo = new DynObject();
foo.bar = "test";
foo.baz = 3;

Console.WriteLine(foo.bar is :{0}, foo.baz is: {1}", foo.bar, foo.baz);
{code: c#}

_Dynamic member creation_
{code: c#}
using JDO.Dynamic;

dynamic foo = new DynObject();
foo.bar = "test";

// This creates a dictionary for each member up to 'other'.
foo.baz.bar.again.foo.other = 3;

Console.WriteLine(foo.bar is :{0}, foo.baz is: {1}", foo.bar, foo.baz.bar.again.foo.other);
{code: c#}

As you can see, you are free to extend the member creation as far as you want. The above example will produce the following JSON when serialized:

{code: javascript}
{
    "bar": "test",
    "baz": {
        "bar": {
            "again": {
                "foo": {
                    "other": 3
                }
           }
        }
    }
}
{code: javascript}

To paraphrase a Winston Churchill anecdote: "It's dictionaries, all the way down." This makes building something that can easily be turned into JSON very simple; there's no need to initialize new dictionaries at every step; {{DynObject}} does it for you.

**Collection Initializer Syntax**

I've added in support for something I've only recently learned the name of: [Collection Initializer Syntax](http://msdn.microsoft.com/en-us/library/bb384062.aspx). Basically, it's a shortcut for creating/initializing objects in C#. (Since it's less typing, and marginally more readable, I worked really hard on making sure that {{DynObject}} supports it.)

Most people who work in C# have seen something like this:

{code: c#}
List<int> = new List<int> {1, 2, 3, 4, 5, 6, 7};
{code: c#}

_(It's actually most common when working with delegates.)_ Well, building dictionaries for use with JSON can be done the same way. To build a dictionary representing our rather long, nested JSON from above, you would do:

{code: c#}
// Note: I couldn't write this without the help of VS's brace matching.
Dictionary<String, Object> foo = new Dictionary<String, Object>
{
    {"bar", "test"},
    {"baz", new Dictionary<String, Object> 
        {
            {"bar", new Dictionary<String, Object>
                {
                    {"again", new Dictionary<String, Object>
                        {
                            {"foo", new Dictionary<String, Object>
                                {
                                    {"other", 3}
                                }
                            }
                        }
                    }
                }
            }
        }
    }
};
{code: c#}

Now, just to remind you, the same thing using DynObject is:

{code: c#}
dynamic foo = new DynObject();
foo.bar = "test";
foo.baz.bar.again.foo.other = 3;
{code: c#}

However, we can write this using Collection Initializer Syntax and {{DynObject}}:

{code: c#}
dynamic foo = new DynObject
{
    {"bar", "test"},
    {"baz", new DynObject 
        {
            {"bar", new DynObject
                {
                    {"again", new DynObject
                        {
                            {"foo", new DynObject
                                {
                                    {"other", 3}
                                }
                            }
                        }
                    }
                }
            }
        }
    }
};
{code: c#}

Still pretty nasty. However, if we combine the two, we start seeing the benefits of using the strengths of each:

{code: c#}
dynamic foo = new DynObject {{"bar", "test"}};
foo.baz.bar.again.foo.other = 3;
{code: c#}

We've actually managed to save a line of code! Now, keep in mind, this example was specifically chosen to highlight the problem with Collection Initializer Syntax, given deeply nested documents. That being said, I'm sure that you can see how you would leverage this, along with the other strengths of {{DynObject}} where appropriate.

**Wrapping**

Wrapping something in a {{DynObject}} is rather simple. {{DynObject}} has a static function, {{DynObject.Wrap}} which you pass an {{object}} to, and it returns an {{object}}. {{DynObject}} is designed so that in all but two specific cases, every call to {{DynObject.Wrap}} returns the same object passed in. This is to use the minimal amount of memory, and reduce the actual wrapping involved. Those two special cases are anything that's an {{IDictionary<String, Object>}} and {{IList<Object>}}. Those both return a {{DynObject}} and a {{DynList}} respectively.

{code: c#}

// foo will be the int 3.
dynamic foo = DynObject.Wrap(3)

// foo will be the string "test".
dynamic foo = DynObject.Wrap("test")

// foo will be an instance of the class SomeClass.
dynamic foo = DynObject.Wrap(new SomeClass())

// foo will be a DynList instance.
dynamic foo = DynObject.Wrap(new List<Object>())

// foo will be a DynObject.
dynamic foo = DynObject.Wrap(new Dictionary<String, Object>())
{code: c#}

This mechanic is fairly simple, and follows the 'principal of least surprise'. If you pass something to {{DynObject.Wrap}}, you can work with whatever it returns as if it was the original object.

**DynList**

I should probably mention {{DynList}} at this point. In short, all {{DynList}} is, is an {{IList}} that wraps an existing list (or creates a new internal one) and returns its items, wrapped by {{DynObject}}. This allows us to do this:

{code: c#}
dynamic foo = new DynObject {{"bar", "test"}, {"list", 
        new DynList {1, 2, 3, new DynObject {{"bar", "test"}}}
}};

// test will have the value "test"
string test = foo.list[3](3).bar;
{code: c#}

And, as you can see, {{DynList}} also supports Collection Initializer Syntax.