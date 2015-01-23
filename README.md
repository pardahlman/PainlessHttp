# PainlessHttp



The HTTP client that is so easy to use that it wont give you any headache!

PainlessHttp supports:

* GET, POST
* _Very_ soon PUT and DELETE
* Plugable Serializers

To instansiate the client:
```
	var config = new HttpClientConfiguration
	{
		BaseUrl = "http://localhost:1337/"
	};
	var client = new HttpClient(config);
```
Getting typed data async has never been easier

```
	var response = http.GetAsync<Todo>("api/todos/1").Result;
	var todo = response.RawContent;
	Console.WriteLine("Name {0}", todo);
```

If you're not sure what kind of data you'll get back - just use the string

```
	var response = http.GetAsync<string>("api/todos/1").Result;
	string rawResponse = response.Body;
	Console.WriteLine("The server replied with: {0}", rawResponse);
```

Store new data with POST

```
	var tomorrow = new Todo { Description = "Sleep in" };
	Todo created = client.PostAsync<Todo>("api/todos", tomorrow).Result;
	Console.WriteLine("Latest todo: {0}", created.Description);
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
```

There are more ways to create customized serializers.

```csharp
var typedJson = new DefaultJsonSerializer();
var defaultJson = new Serializer<DefaultJson>(ContentType.ApplicationJson);
var customJson = SerializeSettings
	.For(ContentType.ApplicationJson)
		.Serialize(NewtonSoft.Serialize) // use NewtonSoft's serializer
		.Deserialize(DefaultJson.Deserialize); // ...but the normal deserializer
```

Of course, you can create your own class  that implements ``IContentSerializer`` and register that one.

## Credits

Author: Pär Dahlman