# PainlessHttp



The HTTP client that is so easy to use that it wont give you any headache!

PainlessHttp supports:

* GET, POST
* _Very_ soon PUT and DELETE
* Plugable Serializers

Getting typed data async has never been easier

```csharp
	var client = new HttpClient("http://localhost:1337/");
	var response = client.GetAsync<Todo>("api/todos/2").Result;
	Todo todo = response.Body;
	Console.WriteLine("Mission of the day: {0}", todo.Description);
```

Can't get typed response from server? No propblem, just pass ``string`` ass generic parameter and you get the raw response.

```csharp
	var client = new HttpClient("http://localhost:1337/");
	var response = client.GetAsync<string>("api/todos/1").Result;
	string rawResponse = response.Body;
	Console.WriteLine("The server replied with: {0}", rawResponse);
```

Store new data with POST

```csharp
	var client = new HttpClient("http://localhost:1337/");
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

## Credits

Author: Pär Dahlman