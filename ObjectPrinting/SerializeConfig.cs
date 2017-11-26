using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ObjectPrinting
{
    public class SerializeConfig<TOwner, TPropType> : ISerializeConfig<TOwner, TPropType>
    {
        private readonly PrintingConfig<TOwner> printingConfig;
        private readonly Action<Delegate> ruleAddition;
        private readonly Action<CultureInfo> cultureInfoAddition;

        public SerializeConfig(PrintingConfig<TOwner> printingConfig, Action<Delegate> ruleAddition, Action<CultureInfo> cultureInfoAddition=null)
        {
            this.ruleAddition = ruleAddition;
            this.cultureInfoAddition = cultureInfoAddition;
            this.printingConfig = printingConfig;
        }

        public SerializeConfig(PrintingConfig<TOwner> printingConfig, Action<CultureInfo> cultureInfoAddition)
        {
            this.printingConfig = printingConfig;
            this.cultureInfoAddition = cultureInfoAddition;
        }

        public PrintingConfig<TOwner> Using(Func<TPropType, string> serealize)
        {
            ruleAddition(serealize);
            return printingConfig;
        }

        public PrintingConfig<TOwner> Using(CultureInfo cultureInfo)
        {
            cultureInfoAddition(cultureInfo);
            return printingConfig;
        }

        PrintingConfig<TOwner> ISerializeConfig<TOwner, TPropType>.PrintingConfig => printingConfig;
    }

    public interface ISerializeConfig<TOwner, TPropType>
    {
        PrintingConfig<TOwner> PrintingConfig { get; }
    }
}
