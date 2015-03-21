using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using PainlessHttp.Http;
using PainlessHttp.Http.Contracts;
using PainlessHttp.Serializers.Defaults;

namespace PainlessHttp.Cache
{
	public class FileCache : CacheBase
	{
		public string CacheDirectory { get; private set; }
		private readonly Type _cacheObjectType = typeof (CachedOnDisk);
		private static readonly SHA1CryptoServiceProvider HashProvider = new SHA1CryptoServiceProvider();

		public FileCache(string cacheDirectory = null)
		{
			CacheDirectory = cacheDirectory ?? GetPathToCacheDirectory();
		}
		
		public override CachedObject Get(HttpWebRequest req)
		{
			if (req == null)
			{
				return null;
			}
			if (!string.Equals(req.Method, HttpMethods.Get, StringComparison.InvariantCultureIgnoreCase))
			{
				return null;
			}

			var key = GetCacheKey(req);
			var path = GetKeyPath(key);
			if (!File.Exists(path))
			{
				return null;
			}
			var xmlString = File.ReadAllText(path);
			var cachedOnDisk = (CachedOnDisk)DefaultXml.Deserialize(xmlString, _cacheObjectType);
			return new CachedObject(() => new MemoryStream(Encoding.UTF8.GetBytes(cachedOnDisk.Value)))
					{
						ModifiedDate = cachedOnDisk.LastModified
					};
		}

		public override async Task AddAsync(HttpWebRequest rawRequest, IHttpWebResponse rawResponse)
		{
			if (rawResponse == null)
			{
				return;
			}
			if (rawResponse.StatusCode != HttpStatusCode.OK)
			{
				return;
			}

			string content;
			using (var responseStream = rawResponse.GetResponseStream())
			{
				using (var reader = new StreamReader(responseStream))
				{
					content = await reader.ReadToEndAsync();
				}
			}

			rawResponse.SetResponseStream(new MemoryStream(Encoding.UTF8.GetBytes(content)));

			var cacheTarget = DefaultXml.Serialize(new CachedOnDisk
									{
										PersistDate = DateTime.Now,
										LastModified = rawResponse.LastModified,
										Value = content
									});

			var key = GetCacheKey(rawResponse);
			var path = GetKeyPath(key);
			File.WriteAllText(path, cacheTarget);
		}

		public void Clear()
		{
			var files = Directory.EnumerateFiles(CacheDirectory);
			foreach (var file in files)
			{
				var path = Path.Combine(CacheDirectory, file);
				File.Delete(path);
			}
		}

		private static string GetPathToCacheDirectory()
		{
			var defaultCacheDirectory = Environment.CurrentDirectory + "//PainlessCache";
			if (Directory.Exists(defaultCacheDirectory))
			{
				return defaultCacheDirectory;
			}
			Directory.CreateDirectory(defaultCacheDirectory);
			return defaultCacheDirectory;
		}

		string GetKeyPath(string key)
		{
			// Copy key to byte array
			var bytes = new byte[key.Length * sizeof(char)];
			Buffer.BlockCopy(key.ToCharArray(), 0, bytes, 0, bytes.Length);
			
			// Calculate file name
			var hashBytes = HashProvider.ComputeHash(bytes);
			var fileName = BitConverter.ToString(hashBytes);
			
			// Finalize Path
			var path = Path.Combine(CacheDirectory, fileName);
			return path;
		}

		public class CachedOnDisk
		{
			public string Value { get; set; }
			public DateTime LastModified { get; set; }
			public DateTime PersistDate { get; set; }
		}
	}
}
