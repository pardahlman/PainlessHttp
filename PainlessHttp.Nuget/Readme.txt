Painless Http - so easy you don't event need a manual

var client = new HttpClient("https://www.google.com");
var response = await client.GetAsync<string>("?q=painless");
var body = response.Body;