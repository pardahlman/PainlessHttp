# PainlessHttp



The HTTP client that is so easy to use that it wont give you any headache!

PainlessHttp supports:

* GET
* _Very_ soon POST, PUT and DELETE

To instansiate the client:
```
	var config = new HttpClientConfiguration
	{
		BaseUrl = "http://localhost:1337/"
	};
	var client = new HttpClient(config);
```


To GET raw data:


```
	var response = http.Get("api/todos/1");
	var todo = response.RawContent;
	Console.WriteLine("Name {0}", todo);
```

To GET typed data:


```
	var response = http.Get<Todo>("api/todos/1");
	Todo todo = response.Body;
	Console.WriteLine("Todo is completed: {0}", todo.IsComplete);
```

## Credits

Author: Pär Dahlman