using NUnit.Framework;
using TypeSync.Common.Utilities;

namespace TypeSync.Test.Utility
{
    [TestFixture]
    [Category("Utility")]
    public class NameConversionTest
    {
        [Test]
        public void ToCamelCaseTest()
        {
            Assert.AreEqual("urlValue", NameCaseConverter.ToCamelCase("URLValue"));
            Assert.AreEqual("url", NameCaseConverter.ToCamelCase("URL"));
            Assert.AreEqual("id", NameCaseConverter.ToCamelCase("ID"));
            Assert.AreEqual("i", NameCaseConverter.ToCamelCase("I"));
            Assert.AreEqual("", NameCaseConverter.ToCamelCase(""));
            Assert.AreEqual(null, NameCaseConverter.ToCamelCase(null));
            Assert.AreEqual("person", NameCaseConverter.ToCamelCase("Person"));
            Assert.AreEqual("iPhone", NameCaseConverter.ToCamelCase("iPhone"));
            Assert.AreEqual("iPhone", NameCaseConverter.ToCamelCase("IPhone"));
            Assert.AreEqual("i Phone", NameCaseConverter.ToCamelCase("I Phone"));
            Assert.AreEqual("i  Phone", NameCaseConverter.ToCamelCase("I  Phone"));
            Assert.AreEqual(" IPhone", NameCaseConverter.ToCamelCase(" IPhone"));
            Assert.AreEqual(" IPhone ", NameCaseConverter.ToCamelCase(" IPhone "));
            Assert.AreEqual("isCIA", NameCaseConverter.ToCamelCase("IsCIA"));
            Assert.AreEqual("vmQ", NameCaseConverter.ToCamelCase("VmQ"));
            Assert.AreEqual("xml2Json", NameCaseConverter.ToCamelCase("Xml2Json"));
            Assert.AreEqual("snAkEcAsE", NameCaseConverter.ToCamelCase("SnAkEcAsE"));
            Assert.AreEqual("snA__kEcAsE", NameCaseConverter.ToCamelCase("SnA__kEcAsE"));
            Assert.AreEqual("snA__ kEcAsE", NameCaseConverter.ToCamelCase("SnA__ kEcAsE"));
            Assert.AreEqual("already_snake_case_ ", NameCaseConverter.ToCamelCase("already_snake_case_ "));
            Assert.AreEqual("isJSONProperty", NameCaseConverter.ToCamelCase("IsJSONProperty"));
            Assert.AreEqual("shoutinG_CASE", NameCaseConverter.ToCamelCase("SHOUTING_CASE"));
            Assert.AreEqual("9999-12-31T23:59:59.9999999Z", NameCaseConverter.ToCamelCase("9999-12-31T23:59:59.9999999Z"));
            Assert.AreEqual("hi!! This is text. Time to test.", NameCaseConverter.ToCamelCase("Hi!! This is text. Time to test."));
        }

        [Test]
        public void ToKebabCaseTest()
        {
            Assert.AreEqual("url-value", NameCaseConverter.ToKebabCase("URLValue"));
            Assert.AreEqual("url", NameCaseConverter.ToKebabCase("URL"));
            Assert.AreEqual("id", NameCaseConverter.ToKebabCase("ID"));
            Assert.AreEqual("i", NameCaseConverter.ToKebabCase("I"));
            Assert.AreEqual("", NameCaseConverter.ToKebabCase(""));
            Assert.AreEqual(null, NameCaseConverter.ToKebabCase(null));
            Assert.AreEqual("person", NameCaseConverter.ToKebabCase("Person"));
            Assert.AreEqual("i-phone", NameCaseConverter.ToKebabCase("iPhone"));
            Assert.AreEqual("i-phone", NameCaseConverter.ToKebabCase("IPhone"));
            Assert.AreEqual("i-phone", NameCaseConverter.ToKebabCase("I Phone"));
            Assert.AreEqual("i-phone", NameCaseConverter.ToKebabCase("I  Phone"));
            Assert.AreEqual("i-phone", NameCaseConverter.ToKebabCase(" IPhone"));
            Assert.AreEqual("i-phone", NameCaseConverter.ToKebabCase(" IPhone "));
            Assert.AreEqual("is-cia", NameCaseConverter.ToKebabCase("IsCIA"));
            Assert.AreEqual("vm-q", NameCaseConverter.ToKebabCase("VmQ"));
            Assert.AreEqual("xml2-json", NameCaseConverter.ToKebabCase("Xml2Json"));
            Assert.AreEqual("sn-ak-ec-as-e", NameCaseConverter.ToKebabCase("SnAkEcAsE"));
            Assert.AreEqual("sn-a--k-ec-as-e", NameCaseConverter.ToKebabCase("SnA--kEcAsE"));
            Assert.AreEqual("sn-a--k-ec-as-e", NameCaseConverter.ToKebabCase("SnA-- kEcAsE"));
            Assert.AreEqual("already-snake-case-", NameCaseConverter.ToKebabCase("already-snake-case- "));
            Assert.AreEqual("is-json-property", NameCaseConverter.ToKebabCase("IsJSONProperty"));
            Assert.AreEqual("shouting-case", NameCaseConverter.ToKebabCase("SHOUTING-CASE"));
            Assert.AreEqual("9999-12-31-t23:59:59.9999999-z", NameCaseConverter.ToKebabCase("9999-12-31T23:59:59.9999999Z"));
            Assert.AreEqual("hi!!-this-is-text.-time-to-test.", NameCaseConverter.ToKebabCase("Hi!! This is text. Time to test."));
        }

        [Test]
        public void ToSnakeCaseTest()
        {
            Assert.AreEqual("url_value", NameCaseConverter.ToSnakeCase("URLValue"));
            Assert.AreEqual("url", NameCaseConverter.ToSnakeCase("URL"));
            Assert.AreEqual("id", NameCaseConverter.ToSnakeCase("ID"));
            Assert.AreEqual("i", NameCaseConverter.ToSnakeCase("I"));
            Assert.AreEqual("", NameCaseConverter.ToSnakeCase(""));
            Assert.AreEqual(null, NameCaseConverter.ToSnakeCase(null));
            Assert.AreEqual("person", NameCaseConverter.ToSnakeCase("Person"));
            Assert.AreEqual("i_phone", NameCaseConverter.ToSnakeCase("iPhone"));
            Assert.AreEqual("i_phone", NameCaseConverter.ToSnakeCase("IPhone"));
            Assert.AreEqual("i_phone", NameCaseConverter.ToSnakeCase("I Phone"));
            Assert.AreEqual("i_phone", NameCaseConverter.ToSnakeCase("I  Phone"));
            Assert.AreEqual("i_phone", NameCaseConverter.ToSnakeCase(" IPhone"));
            Assert.AreEqual("i_phone", NameCaseConverter.ToSnakeCase(" IPhone "));
            Assert.AreEqual("is_cia", NameCaseConverter.ToSnakeCase("IsCIA"));
            Assert.AreEqual("vm_q", NameCaseConverter.ToSnakeCase("VmQ"));
            Assert.AreEqual("xml2_json", NameCaseConverter.ToSnakeCase("Xml2Json"));
            Assert.AreEqual("sn_ak_ec_as_e", NameCaseConverter.ToSnakeCase("SnAkEcAsE"));
            Assert.AreEqual("sn_a__k_ec_as_e", NameCaseConverter.ToSnakeCase("SnA__kEcAsE"));
            Assert.AreEqual("sn_a__k_ec_as_e", NameCaseConverter.ToSnakeCase("SnA__ kEcAsE"));
            Assert.AreEqual("already_snake_case_", NameCaseConverter.ToSnakeCase("already_snake_case_ "));
            Assert.AreEqual("is_json_property", NameCaseConverter.ToSnakeCase("IsJSONProperty"));
            Assert.AreEqual("shouting_case", NameCaseConverter.ToSnakeCase("SHOUTING_CASE"));
            Assert.AreEqual("9999-12-31_t23:59:59.9999999_z", NameCaseConverter.ToSnakeCase("9999-12-31T23:59:59.9999999Z"));
            Assert.AreEqual("hi!!_this_is_text._time_to_test.", NameCaseConverter.ToSnakeCase("Hi!! This is text. Time to test."));
        }
    }
}
