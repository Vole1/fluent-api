using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectPrinting
{
	public static class SerializeConfigExtensions
	{
	    public static PrintingConfig<TOwner> TrimmedToLength<TOwner>(this SerializeConfig<TOwner, string> propConfig, int maxLen)
	    {
	        return ((ISerializeConfig<TOwner, string>)propConfig).PrintingConfig;
	    }

	    public static string PrintToString<T>(this T obj, Func<PrintingConfig<T>, PrintingConfig<T>> config)
	    {
	        return config(ObjectPrinter.For<T>()).PrintToString(obj);
	    }

        public static PrintingConfig<TOwner> Using<TOwner>(
			this SerializeConfig<TOwner, double> config,
			CultureInfo cultureInfo)
		{
            
			return ((ISerializeConfig<TOwner, double>)config).PrintingConfig;
		}

		public static PrintingConfig<TOwner> Using<TOwner>(
			this SerializeConfig<TOwner, int> config,
			CultureInfo cultureInfo)
		{
			return ((ISerializeConfig<TOwner, double>)config).PrintingConfig;
		}

		public static PrintingConfig<TOwner> Using<TOwner>(
			this SerializeConfig<TOwner, long> config,
			CultureInfo cultureInfo)
		{
			return ((ISerializeConfig<TOwner, double>)config).PrintingConfig;
		}

		public static PrintingConfig<TOwner> Using<TOwner>(
			this SerializeConfig<TOwner, string> config,
			CultureInfo cultureInfo)
		{
			return ((ISerializeConfig<TOwner, string>)config).PrintingConfig;
		}
	}
}
