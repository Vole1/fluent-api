using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.Internal.Execution;

namespace ObjectPrinting
{

    public class PrintingConfig<TOwner>
    {
        private readonly HashSet<Type> excludeTypes = new HashSet<Type>();
        private readonly HashSet<PropertyInfo> excludeOwnerProperties = new HashSet<PropertyInfo>();
        private readonly Dictionary<Type, Delegate> typeSerializeRulesDict = new Dictionary<Type, Delegate>();
        private readonly Dictionary<PropertyInfo, Delegate> propertySerializeRulesDict = new Dictionary<PropertyInfo, Delegate>();
        private readonly Dictionary<Type, CultureInfo> cultureSpecialization = new Dictionary<Type, CultureInfo>();

        private void AddTypeSerializationRule(Type type, Delegate rule)
        {
            typeSerializeRulesDict[type] = rule;
        }

        private void AddPropertySerializationRule(PropertyInfo propertyInfo, Delegate rule)
        {
            propertySerializeRulesDict[propertyInfo] = rule;
        }

        private void AddCultureSpecification(Type type, CultureInfo cultureInfo)
        {
            cultureSpecialization[type] = cultureInfo;
        }

        public PrintingConfig<TOwner> Excluding<TPropType>()
        {
            excludeTypes.Add(typeof(TPropType));
            return this;
        }

        public PrintingConfig<TOwner> Excluding<TPropType>(
        Expression<Func<TOwner, TPropType>> propertySelector)
        {
            excludeOwnerProperties.Add(((MemberExpression)propertySelector.Body).Member as PropertyInfo);
            return this;
        }

        public SerializeConfig<TOwner, TPropType> Printing<TPropType>()
        {
            var culturelmd = new Action<CultureInfo>(culInf => AddCultureSpecification(typeof(TPropType), culInf));
            var serialLmd = new Action<Delegate>(rule => AddTypeSerializationRule(typeof(TPropType), rule));
            return new SerializeConfig<TOwner, TPropType>(this, serialLmd, culturelmd);
        }

        public SerializeConfig<TOwner, TPropType> Printing<TPropType>(
        Expression<Func<TOwner, TPropType>> memberSelector)
        {
            var propInfo = ((MemberExpression) memberSelector.Body).Member as PropertyInfo;
            var lmd = new Action<Delegate>((serializeFunc) => AddPropertySerializationRule(propInfo, serializeFunc));
            return new SerializeConfig<TOwner, TPropType>(this, lmd);
        }

        public PrintingConfig<TOwner> SetStringCutting(Func<string, string> rule)
        {
            return this;
        }

        public string PrintToString(TOwner obj)
        {
            return PrintToString(obj, 0);
        }

        private string PrintToString(object obj, int nestingLevel)
        {

            //TODO apply configurations
            if (obj == null)
                return "null" + Environment.NewLine;


            var finalTypes = new List<Type>
            {
            typeof(int), typeof(double), typeof(float), typeof(string),
            typeof(DateTime), typeof(TimeSpan)
            };

            for (var i = 0; i < finalTypes.Count; i++)
            {
                if (excludeTypes.Contains(finalTypes[i]))
                    finalTypes.RemoveAt(i);
            }

            if (finalTypes.Contains(obj.GetType()))
                return obj + Environment.NewLine;

            var identation = new string('\t', nestingLevel + 1);
            var sb = new StringBuilder();
            var type = obj.GetType();
            sb.AppendLine(type.Name);
            foreach (var propertyInfo in type.GetProperties())
            {
                if (!finalTypes.Contains(propertyInfo.PropertyType))
                    continue;
                if (excludeOwnerProperties.Contains(propertyInfo))
                    continue;

                object objValue = propertyInfo.GetValue(obj);
                if (propertySerializeRulesDict.ContainsKey(propertyInfo))
                    objValue = propertySerializeRulesDict[propertyInfo].DynamicInvoke(objValue);

                else if (typeSerializeRulesDict.ContainsKey(propertyInfo.PropertyType))
                    objValue = typeSerializeRulesDict[propertyInfo.PropertyType].DynamicInvoke(objValue);


                sb.Append(identation + propertyInfo.Name + " = " +
                        PrintToString(objValue,
                            nestingLevel + 1));
            }
            return sb.ToString();
        }
    }
}