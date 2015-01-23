using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PainlessHttp.Http;
using PainlessHttp.Serializers.Contracts;

namespace PainlessHttp.Serializers.Custom
{
	public class Serializer<TSerializer> : IContentSerializer
	{
		private readonly MethodInfo _serializeMethod;
		private readonly MethodInfo _deserialize;

		public Serializer(params ContentType[] contentTypes)
		{
			if (contentTypes == null || contentTypes.Length == 0)
			{
				throw new ArgumentException("The serializer must at leaste contain one ContentType.");
			}
			ContentType = contentTypes;

			var methods = typeof(TSerializer).GetMethods();
			_serializeMethod = methods.FirstOrDefault(m =>
			{
				var correctArguments = ArgumentsMatch(m.GetParameters(), new[] { typeof(object) });
				var correctReturn = ReturnTypeIs<string>(m.ReturnType);

				return correctArguments && correctReturn;
			});
			_deserialize = methods.FirstOrDefault(m =>
			{
				var correctArguments = ArgumentsMatch(m.GetParameters(), new[] { typeof(string), typeof(Type) });
				var correctReturn = ReturnTypeIs<object>(m.ReturnType);

				return correctArguments && correctReturn;
			});

			if (_serializeMethod == null)
			{
				throw new MissingMethodException(string.Format("The type {0} does not contain a method with signature 'string Method(object obj)'. Please make sure that this is a serializer.", typeof(TSerializer).Name));
			}
			if (_deserialize == null)
			{
				throw new MissingMethodException(string.Format("The type {0} does not contain a method with signature 'object Method(string data, Type type)'. Please make sure that this is a serializer.", typeof(TSerializer).Name));
			}
		}

		private static bool ArgumentsMatch(IList<ParameterInfo> parameterInfos, IList<Type> types)
		{
			if (parameterInfos.Count != types.Count)
			{
				return false;
			}

			for (var i = 0; i < parameterInfos.Count; i++)
			{
				if (parameterInfos[i].ParameterType != types[i])
				{
					return false;
				}
			}
			return true;
		}

		private static bool ReturnTypeIs<TReturnType>(Type type)
		{
			return type == typeof(TReturnType);
		}

		public IEnumerable<ContentType> ContentType { get; private set; }
		public string Serialize(object data)
		{
			var res = _serializeMethod.Invoke(null, new[] { data });
			return (string)res;
		}

		public TObject Deserialize<TObject>(string data)
		{
			var res = _deserialize.Invoke(null, new object[] { data, typeof(TObject) });
			return (TObject)res;
		}
	}
}
