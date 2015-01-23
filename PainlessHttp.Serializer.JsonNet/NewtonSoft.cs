using System;
using Newtonsoft.Json;

namespace PainlessHttp.Serializer.JsonNet
{
	public class NewtonSoft
	{
		private static NewtonsoftSettings _settings = new NewtonsoftSettings();

		public static object Deserialize(string data, Type type)
		{
			if (_settings.Settings != null)
			{
				return JsonConvert.DeserializeObject(data, type, _settings.Settings);
			}
			if(_settings.JsonConverters != null)
			{
				return JsonConvert.DeserializeObject(data, type, _settings.JsonConverters);
			}

			return JsonConvert.DeserializeObject(data, type);
		}

		public static string Serialize(object data)
		{
			if (_settings.Settings != null)
			{
				return JsonConvert.SerializeObject(data, _settings.Settings);
			}
			if (_settings.JsonConverters != null)
			{
				return JsonConvert.SerializeObject(data, _settings.Formatting, _settings.JsonConverters);
			}

			return JsonConvert.SerializeObject(data, _settings.Formatting);
		}

		
		public static void UpdateSettings(NewtonsoftSettings settings)
		{
			_settings = settings;
		}
	}
}
