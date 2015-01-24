# PainlessHttp

A Http client that is so easy to use that it wont ever give you any headache!

PainlessHttp supports:

* ``GET``, ``POST``, ``PUT`` and ``DELETE``
* Plugable serializers
* No external references to NuGets (_or any other libraries for that matter!_)

Getting typed medata async has never been easier

```csharp
	var client = new HttpClient("http://localhost:1337/");
	var response = client.GetAsync<Todo>("api/todos/2").Result;
	Todo todo = response.Body;
	Console.WriteLine("Mission of the day: {0}", todo.Description);
```

Don't feel like specifying the type of theresponse body? No problem, just pass ``string`` as generic parameter and you get the raw response.

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
	// retrive data
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
## Credits

Author: Pär Dahlman