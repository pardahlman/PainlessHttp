# PainlessHttp

_No external libraries! No over engineered method signatures! No uber verbose setup! Just a HTTP client that is so easy to use that it won’t ever give you any headache!_

* async ``GET``, ``POST``, ``PUT`` and ``DELETE``
* Content Negotiation
* Authentication
* Plugable serializers
* No external references to NuGets (_just Microsoft Core Libs!_)

Getting typed metadata async has never been easier

```csharp
	var client = new HttpClient("http://localhost:1337/");
	var response = client.GetAsync<Todo>("api/todos/2").Result;
	Todo todo = response.Body;
	Console.WriteLine("Mission of the day: {0}", todo.Description);
```

Don't feel like specifying the type of there response body? No problem, just pass ``string`` as generic parameter and you get the raw response.

```csharp
	var client = new HttpClient("http://localhost:1337/");
	var response = client.GetAsync<string>("api/todos/1").Result;
	string rawResponse = response.Body;
	Console.WriteLine("The server replied with: {0}", rawResponse);
```

Store new data with ``POST``

```csharp
	var client = new HttpClient("http://localhost:1337/");
	var tomorrow = new Todo { Description = "Sleep in" };
	Todo created = client.PostAsync<Todo>("api/todos", tomorrow).Result;
	Console.WriteLine("Latest todo: {0}", created.Description);
```

... or update it with ``PUT``

```csharp
	// retrieve data
	var client = new HttpClient(config);
	var response = client.GetAsync<Todo>("api/todos/2").Result;
	var completedTodo = response.Body;

	// update it
	completedTodo.IsCompleted = true;
	client.Put<string>("api/todos", completedTodo);
```

``DELETE`` works the same way

```csharp
	var client = new HttpClient("http://localhost:1337/");
	var response = client.DeleteAsync<string>("api/todos/2").Result;
	if (response.StatusCode == HttpStatusCode.OK)
	{
		Console.WriteLine("Todo successfully deleted");
	}
```

There is nothing in the [Hypertext Transfer Protocol HTTP/1.1 spec](http://www.w3.org/Protocols/rfc2616/rfc2616-sec9.html) that forbids the request to have a body. In fact, some [well-known APIs](https://developer.atlassian.com/static/rest/stash/3.6.0/stash-branch-utils-rest.html) require the ``DELETE`` request to contain a body. PainlessHttp supports this out-of-the-box

```csharp
  var client = new HttpClient("http://localhost:1337/rest/branch-utils/1.0/");
  var requestBody = new
  {
    name = "/refs/heads/my-branch",
    dryRun = false
  };
  var response = client.Delete<string>("projects/web/repos/painless/branches", requestBody);
```

## Configuration
### Serializers
Painless Http comes with a set of serializers for the standard formats (``application/xml``, ``application/json``). These serializers are registered in the client by default. This means that if you don't really care about how serialization is done, you can jump to the next section.

If you want to override serializers, just say so in the configuration object
```csharp
  var config = new HttpClientConfiguration
	{
		BaseUrl = "http://localhost:1337/",
		Advanced =
		{
			Serializers = new List<IContentSerializer>
			{
				new Serializer<NewtonSoft>(ContentType.ApplicationJson),
				new Serializer<DefaultXml>(ContentType.ApplicationXml)
			}
		}
	};
  var client = new HttpClient(config);
```

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
#### The NewtonSoft serializer

One of the most popular json serializers is Newtonsoft's JsonNet library. Here's how you configure it

```csharp
 	// create a Newtonsof configuration object
	var newtonSoftSettings = new JsonSerializerSettings
	{
		ContractResolver = new CamelCasePropertyNamesContractResolver()
	};

	// register the change
	NewtonSoft.UpdateSettings(new NewtonsoftSettings {Settings = newtonSoftSettings});

	// instanciate
	var serializer = new Serializer<NewtonSoft>(ContentType.ApplicationJson);

	// Go crazy, oh baby!
	var tomorrowJson = serializer.Serialize(tomorrow);
	var tommorowObj = serializer.Deserialize<Todo>(tomorrowJson);
```

_Note that the Jsonsoft `ContentConverter is not part of the core lib. Download the nuget_ ``PainlessHttp.Serializers.JsonNet``.

### Content Negotiation
If the request has a body, the default behaviour is to serialize it to json before sending the request. If you know right away that the endpoint you're requesting only supports another content type, you can change the default setting in the configuration object
```csharp
  var client = new HttpClient(new HttpClientConfiguration
  {
    BaseUrl = "http://localhost:1337/",
    Advanced = { ContentType = ContentType.ApplicationJson }
	});
```
If the server responds with status code ``UnsupportedMediaType`` or the Accept headers that does not contain the supplied content type, the default behaviour is to resend the request with supported content type (based on Accept headers). This behaviour can be overridden by supplying a content type in the request
```csharp
  var newTodo = new Todo { Description = "Write tests" };
  var overrideType = ContentType.ApplicationJson;
  var created = _client.PostAsync<string>("/api/todo", newTodo, null, overrideType);
```

### Customizing request
The default behavior of PainlessHttp should satisfy most of the developer out there. However, if you for some reason want to control headers and other properties of the request that is sent, there is a way to do so. With the ``WebrequestModifier`` you get access to the raw request and can do all sorts of things, like adding you custom request header.
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

## Credits

Author: Pär Dahlman