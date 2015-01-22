# PainlessHttp



The HTTP client that is so easy to use that it wont give you any headache!

PainlessHttp supports:

* GET, POST
* _Very_ soon PUT and DELETE

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



## Credits

Author: Pär Dahlman