# PainlessHttp

_No external libraries! No over engineered method signatures! No uber verbose setup! Just a HTTP client that is so easy to use that it won’t ever give you any headache!_

* async/sync ``GET``, ``POST``, ``PUT`` and ``DELETE``
* Content negotiation, so that you don't have to think about AcceptHeaders and all that stuff
* Plugable ``If-Modified-Since`` caches for speeding up your application even more.
* Authentication
* Plugable serializers - use defaults, additional ones (like ``PainlessHttp.Serializers.JsonNet``) or build one yourself
* No external references to NuGets (_just Microsoft Core Libs!_)

## Quick introduction

```csharp
	//instanciate client
	var client = new HttpClient("http://painless.pardahlman.se");

	// create new entity
	var tomorrow = new Todo { Description = "Sleep in", IsCompleted = false};
	var created = await client.PostAsync<Todo>("/api/todos", tomorrow);

	// get it
	var response = await client.GetAsync<Todo>("/api/todos/1");
	var existing = response.Body;

	// update it
	existing.IsCompleted = true;
	var updated = await client.PutAsync<Todo>("/api/todos/1", existing);

	// delete it
	var deleted = await client.DeleteAsync<string>("/api/todos/1");
	if (deleted.StatusCode == HttpStatusCode.OK)
	{
		Console.Write("Successfully deleted todo");
	}
```
## Configuration
Want to have greater control over how things are done? Just instanciate the client with a ``Configuration`` object, and you'll have the posibility to change just about everything: 
```csharp
  //create config
  var config = new Configuration
  {
    BaseUrl = "http://painless.pardahlman.se",
    Advanced =
    {
      Serializers = new List { new PainlessJsonNet() },
      ModifiedSinceCache = new FileCache(cacheDirectory: Environment.CurrentDirectory),
      RequestTimeout = new TimeSpan(days:0, hours:0, minutes:0, seconds:2),
      ContentNegotiation = true,
      Credentials = new NetworkCredential("pardahlman", "strong-password"),
      WebrequestModifier = request => request.Headers.Add("X-Additional-Header", "Each request")
    }
  };
  var client = new HttpClient(config);
```
### Serializers
Painless Http comes with a set of serializers for the standard formats (``application/xml``, ``application/json``). These serializers are registered in the client by default. This means that if you don't really care about how serialization is done, you can jump to the next section.

If you want to override serializers, just say so in the configuration object

There are more ways to create customized serializers.

```csharp
  var typedJson = new DefaultJsonSerializer();
  var defaultJson = new Serializer<DefaultJson>(ContentType.ApplicationJson);
  var customJson = SerializeSettings
    .For(ContentType.ApplicationJson)
      .Serialize(NewtonSoft.Serialize)        // use NewtonSoft's serializer
      .Deserialize(DefaultJson.Deserialize); // ...but the normal deserializer
```

Of course, you can create your own class  that implements ``IContentSerializer`` and register that one.

#### Increase speed with pre-complied serializers
Painless Http wants to perform your requests as fast as possible. That's why all the type-specific serializers are cached in all the default implementations. If you request the same _type_ of object twice, you won't need to pay the penalty of creating a new type-specific serializer. However, in some instances you would want to reduce the overhead of creating the serializer in the first request, too.  If you want to speed up things right from the start, just register underlying serializers
```csharp
  var ignores = new XmlAttributeOverrides();
  ignores.Add(typeof(Todo), "Description", new XmlAttributes { XmlIgnore = true });
  var preCompiledSerializers = new Dictionary<Type, XmlSerializer>
  {
    {typeof(Todo), new XmlSerializer(typeof(Todo), ignores)}
  };
  var xmlSerializer = new DefaultXmlSerializer(preCompiledSerializers);

  var xmlTomorrow = xmlSerializer.Serialize(tomorrow);
```

_Note that the Jsonsoft `ContentConverter is not part of the core lib. Download the nuget_ ``PainlessHttp.Serializers.JsonNet``.

### Content Negotiation
If the server responds with status code ``UnsupportedMediaType`` or the Accept headers that does not contain the supplied content type, the default behaviour is to resend the request with supported content type (based on Accept headers). This behaviour can be overridden by supplying a content type in the request
```csharp
  var config = new Configuration
  {
    BaseUrl = "http://localhost:1337/",
    Advanced =
    {
      ContentNegotiation = true
    }
  };
  var client = new HttpClient(config);
  var newTodo = new Todo { Description = "Write tests" };
  var created = _client.PostAsync<string>("/api/todo", newTodo);
```

### Customizing request
The default behavior of PainlessHttp should satisfy most of the developer out there. However, if you for some reason want to control manipulate properties of the request that is sent, there is a way to do so. With the ``WebrequestModifier`` you get access to the raw request and do all sorts of things, like adding you custom request header.
```csharp
  var config = new HttpClientConfiguration
  {
    BaseUrl = "http://localhost:1337/",
    Advanced =
    {
      WebrequestModifier = req => req.Headers.Add("X-Custom-Header", "Custom-Value")
    }
  };
```

### Authentication
Authentication is handled with ``System.Het.NetworkCredential``, and is registered in the configuration
```csharp
  var config = new HttpClientConfiguration
  {
    BaseUrl = "http://localhost:1337/",
    Advanced =
    {
			Credentials = Credentials = new NetworkCredential("pardahlman", "password")
		}
	};
```

### If-Modified-Since Cache
If the entities that you are requesting are large, and the server supports [If-Modified-Since Headers](http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html), you can turn caching on the through PainlessHttp to make you application even faster. There are three cache types
* ``NoCache`` (does nothing)
* ``InMemoryCache`` (saves entities in working memory)
* ``FileCache`` (saves entities on disk)

This is done in the configuration view:
```csharp
  var cacheDirectory = Environment.CurrentDirectory;
  var config = new Configuration
  {
  	BaseUrl = "http://localhost:1337/",
  	Advanced =
  	{
  		ModifiedSinceCache = new FileCache(cacheDirectory)
  	}
  };
```

### Request Timeout
There is no point in waiting for a response forever. By default, The Painless HttpClient defaults to a 10 second Request Timeout. If you want to change this, just do so in the configuration
```csharp
  var config = new Configuration
  {
  	BaseUrl = "http://localhost:1337/",
  	Advanced =
  	{
  		RequestTimeout = new TimeSpan(days:0, hours:0, minutes:0, seconds:2)
  	}
  };
```
## Credits

Author: Pär Dahlman