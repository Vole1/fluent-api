using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace ObjectPrinting.Tests
{
    [TestFixture]
    public class ObjectPrinterAcceptanceTests
    {
        [Test]
        public void Demo()
        {
            var person = new Person { Name = "Alex", Age = 19 };

            var printer = ObjectPrinter.For<Person>()
            //1. Исключить из сериализации свойства определенного типа
            .Excluding<int>()
            //2. Указать альтернативный способ сериализации для определенного типа
            .Printing<int>().Using(p => p.ToString())
            //3. Для числовых типов указать культуру
            .Printing<double>().Using(CultureInfo.CurrentCulture)
            //4. Настроить сериализацию конкретного свойства
            .Printing(p => p.Age).Using(prop => prop.ToString())
            //5. Настроить обрезание строковых свойств (метод должен быть виден только для строковых свойств)
            .Printing<string>().TrimmedToLength(10)
            //6. Исключить из сериализации конкретное свойства
            .Excluding(p => p.Id);



            string s1 = printer.PrintToString(person);

            //7. Синтаксический сахар в виде метода расширения, сериализующего по-умолчанию
            string s2 = person.PrintToString();

            //8. ...с конфигурированием
            string s3 = person.PrintToString(s => s.Excluding(p => p.Age));

            Console.WriteLine(s1);
            Console.WriteLine(s2);
            Console.WriteLine(s3);
        }

        [Test]
        public void TestForTypeExcuding()
        {
            var person = new Person { Name = "Lalka", Age = 19, Height = 163.2 };

            var printer = ObjectPrinter.For<Person>()
            .Excluding<int>();

            var targetStr = string.Format(
                "Person\r\n" +
                "\t{0} = {1}" + "\r\n" +
                "\t{2} = {3}\r\n",
                "Name", person.Name, "Height", person.Height);

            printer.PrintToString(person).ShouldBeEquivalentTo(targetStr);
        }

        [Test]
        public void TestForPropertyExcluding()
        {
            var person = new Person { Name = "Lalka", Age = 19, Height = 163.2 };

            var printer = ObjectPrinter.For<Person>()
                .Excluding(p => p.Name);

            var targetStr = string.Format(
                "Person\r\n" +
                "\t{0} = {1}" + "\r\n" +
                "\t{2} = {3}\r\n",
                "Height", person.Height,
                "Age", person.Age);

            printer.PrintToString(person).ShouldBeEquivalentTo(targetStr);
        }

        [Test]
        public void TestForCultureSettingForNumericTypes()
        {
            var person = new Person { Name = "Lalka", Age = 19, Height = 163.2 };

            var printer = ObjectPrinter.For<Person>()
                .Printing<double>().Using(new CultureInfo(1));

            var targetStr = string.Format(
                "Person\r\n" +
                "\t{0} = {1}" + "\r\n" +
                "\t{2} = {3}" + "\r\n" +
                "\t{4} = {5}\r\n",
                "Name", person.Name,
                "Height", person.Height.ToString(new CultureInfo(1)),
                "Age", person.Age);

            printer.PrintToString(person).ShouldBeEquivalentTo(targetStr);
        }

        [Test]
        public void TestForAlternativeSerializationRuleForSpecificType()
        {
            var person = new Person { Name = "Lalka", Age = 19, Height = 163.2 };

            var printer = ObjectPrinter.For<Person>()
                .Printing<double>().Using(p => p+"d");

            var targetStr = string.Format(
                "Person\r\n" +
                "\t{0} = {1}" + "\r\n" +
                "\t{2} = {3}" + "\r\n" +
                "\t{4} = {5}\r\n",
                "Name", person.Name,
                "Height", person.Height+"d",
                "Age", person.Age);

            printer.PrintToString(person).ShouldBeEquivalentTo(targetStr);
        }

        [Test]
        public void TestForAlternativeSerializationRuleForSpecificProperty()
        {
            var person = new Person { Name = "Lalka", Age = 19, Height = 163.2 };

            var printer = ObjectPrinter.For<Person>()
                .Printing(p => p.Name).Using(p => p+" Is a Person");

            var targetStr = string.Format(
                "Person\r\n" +
                "\t{0} = {1}" + "\r\n" +
                "\t{2} = {3}" + "\r\n" +
                "\t{4} = {5}\r\n",
                "Name", person.Name + " Is a Person",
                "Height", person.Height,
                "Age", person.Age);

            printer.PrintToString(person).ShouldBeEquivalentTo(targetStr);
        }


        [Test]
        public void TestForStringTrimming()
        {
            var person = new Person { Name = "Lalka", Age = 19, Height = 163.2 };

            var printer = ObjectPrinter.For<Person>()
                .Printing<string>().TrimmedToLength(3);

            var targetStr = string.Format(
                "Person\r\n" +
                "\t{0} = {1}" + "\r\n" +
                "\t{2} = {3}" + "\r\n" +
                "\t{4} = {5}\r\n",
                "Name", person.Name.Substring(0,3),
                "Height", person.Height,
                "Age", person.Age);

            printer.PrintToString(person).ShouldBeEquivalentTo(targetStr);
        }

    }
}