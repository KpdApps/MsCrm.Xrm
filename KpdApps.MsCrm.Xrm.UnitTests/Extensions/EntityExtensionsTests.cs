using System;
using KpdApps.MsCrm.Xrm.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace KpdApps.MsCrm.Xrm.UnitTests.Extensions
{
    [TestClass]
    public class EntityExtensionsTests
    {
        const string EntityLogicalName = "e_LogicalName";
        const string StringAttribute = "a_string";
        const string NumberAttribute = "a_number";
        const string DecimalAttribute = "a_decimal";
        const string FloatAttribute = "a_float";
        const string BoolAttribute = "a_bool";
        const string DateTimeAttribute = "a_datetime";
        const string EntityReferenceAttribute = "a_entityreference";
        const string MoneyAttribute = "a_money";
        const string NullAttribute = "a_null";
        const string PickListAttribute = "a_picklist";
        const string StatusCodeAttribute = "statuscode";
        //const string StateCodeAttribute = "statecode";

        private Entity GetOriginEntity()
        {
            Entity originEnt = new Entity(EntityLogicalName) { Id = Guid.NewGuid() };
            originEnt.Attributes.SetStringValue(StringAttribute, "testName");
            originEnt.Attributes.SetNumberValue(NumberAttribute, 1);
            originEnt.Attributes.SetDecimalValue(DecimalAttribute, 12.11m);
            originEnt.Attributes.SetFloatValue(FloatAttribute, 11.1F);
            originEnt.Attributes.SetBooleanValue(BoolAttribute, true);
            originEnt.Attributes.SetDateTimeValue(DateTimeAttribute, DateTime.Now);
            originEnt.Attributes.SetLookupValue(EntityReferenceAttribute, new EntityReference("entityReference", Guid.NewGuid()));
            originEnt.Attributes.SetMoneyValue(MoneyAttribute, 1333m);
            originEnt.Attributes.SetNullValue(NullAttribute);
            originEnt.Attributes.SetPicklistValue(PickListAttribute, 400103133);
            originEnt.Attributes.SetStatusValue(133111111);

            return originEnt;
        }

        [TestMethod]
        public void CloneTest()
        {
            Entity originEnt = GetOriginEntity();

            Entity actualEnt = originEnt.Clone();

            Assert.AreEqual(Guid.Empty, actualEnt.Id);
            Assert.AreEqual(originEnt.Attributes.GetStringValue(StringAttribute), actualEnt.Attributes.GetStringValue(StringAttribute));
            Assert.AreEqual(originEnt.Attributes.GetStringValue(NumberAttribute), actualEnt.Attributes.GetStringValue(NumberAttribute));
            Assert.AreEqual(originEnt.Attributes.GetStringValue(DecimalAttribute), actualEnt.Attributes.GetStringValue(DecimalAttribute));
            Assert.AreEqual(originEnt.Attributes.GetStringValue(FloatAttribute), actualEnt.Attributes.GetStringValue(FloatAttribute));
            Assert.AreEqual(originEnt.Attributes.GetStringValue(BoolAttribute), actualEnt.Attributes.GetStringValue(BoolAttribute));
            Assert.AreEqual(originEnt.Attributes.GetStringValue(DateTimeAttribute), actualEnt.Attributes.GetStringValue(DateTimeAttribute));
            Assert.AreEqual(originEnt.Attributes.GetStringValue(EntityReferenceAttribute), actualEnt.Attributes.GetStringValue(EntityReferenceAttribute));
            Assert.AreEqual(originEnt.Attributes.GetStringValue(MoneyAttribute), actualEnt.Attributes.GetStringValue(MoneyAttribute));
            Assert.AreEqual(originEnt.Attributes.GetStringValue(NullAttribute), actualEnt.Attributes.GetStringValue(NullAttribute));
            Assert.AreEqual(originEnt.Attributes.GetStringValue(PickListAttribute), actualEnt.Attributes.GetStringValue(PickListAttribute));
            Assert.AreEqual(originEnt.Attributes.GetStringValue(StatusCodeAttribute), actualEnt.Attributes.GetStringValue(StatusCodeAttribute));
        }

        [TestMethod]
        public void CreateEmptyTest()
        {
            Entity originEnt = GetOriginEntity();

            Entity actualEnt = originEnt.CreateEmpty();

            Assert.AreEqual(originEnt.Id, actualEnt.Id);

            Assert.IsFalse(actualEnt.Attributes.Contains(StringAttribute));
            Assert.IsFalse(actualEnt.Attributes.Contains(NumberAttribute));
            Assert.IsFalse(actualEnt.Attributes.Contains(DecimalAttribute));
            Assert.IsFalse(actualEnt.Attributes.Contains(FloatAttribute));
            Assert.IsFalse(actualEnt.Attributes.Contains(BoolAttribute));
            Assert.IsFalse(actualEnt.Attributes.Contains(DateTimeAttribute));
            Assert.IsFalse(actualEnt.Attributes.Contains(EntityReferenceAttribute));
            Assert.IsFalse(actualEnt.Attributes.Contains(MoneyAttribute));
            Assert.IsFalse(actualEnt.Attributes.Contains(NullAttribute));
            Assert.IsFalse(actualEnt.Attributes.Contains(PickListAttribute));
            Assert.IsFalse(actualEnt.Attributes.Contains(StatusCodeAttribute));
        }

        [TestMethod]
        public void SerializeDeserializeTest()
        {
            Entity originEnt = GetOriginEntity();

            string serializedOriginEntity = originEnt.Serialize();

            Entity actualEnt = EntityExtensions.Deserialize(serializedOriginEntity);

            Assert.AreEqual(originEnt.Id, actualEnt.Id);
            Assert.AreEqual(originEnt.Attributes.GetStringValue(StringAttribute), actualEnt.Attributes.GetStringValue(StringAttribute));
            Assert.AreEqual(originEnt.Attributes.GetStringValue(NumberAttribute), actualEnt.Attributes.GetStringValue(NumberAttribute));
            Assert.AreEqual(originEnt.Attributes.GetStringValue(DecimalAttribute), actualEnt.Attributes.GetStringValue(DecimalAttribute));
            Assert.AreEqual(originEnt.Attributes.GetStringValue(FloatAttribute), actualEnt.Attributes.GetStringValue(FloatAttribute));
            Assert.AreEqual(originEnt.Attributes.GetStringValue(BoolAttribute), actualEnt.Attributes.GetStringValue(BoolAttribute));
            Assert.AreEqual(originEnt.Attributes.GetStringValue(DateTimeAttribute), actualEnt.Attributes.GetStringValue(DateTimeAttribute));
            Assert.AreEqual(originEnt.Attributes.GetStringValue(EntityReferenceAttribute), actualEnt.Attributes.GetStringValue(EntityReferenceAttribute));
            Assert.AreEqual(originEnt.Attributes.GetStringValue(MoneyAttribute), actualEnt.Attributes.GetStringValue(MoneyAttribute));
            Assert.AreEqual(originEnt.Attributes.GetStringValue(NullAttribute), actualEnt.Attributes.GetStringValue(NullAttribute));
            Assert.AreEqual(originEnt.Attributes.GetStringValue(PickListAttribute), actualEnt.Attributes.GetStringValue(PickListAttribute));
            Assert.AreEqual(originEnt.Attributes.GetStringValue(StatusCodeAttribute), actualEnt.Attributes.GetStringValue(StatusCodeAttribute));
        }

        [TestMethod]
        public void ContainsAllTest()
        {
            Entity originEnt = GetOriginEntity();

            Assert.IsTrue(originEnt.ContainsAll(StringAttribute, NumberAttribute, EntityReferenceAttribute));
        }

        [TestMethod]
        public void ContainsAllFieldNotExistsTest()
        {
            Entity originEnt = GetOriginEntity();

            Assert.IsFalse(originEnt.ContainsAll(StringAttribute, NumberAttribute, "a_notexists"));
        }

        [TestMethod]
        public void ContainsAnyTests()
        {
            Entity originEnt = GetOriginEntity();

            Assert.IsTrue(originEnt.ContainsAny(StringAttribute, NumberAttribute, "a_notexists"));
        }

        [TestMethod]
        public void ContainsAnyFieldNotExistsTests()
        {
            Entity originEnt = GetOriginEntity();

            Assert.IsFalse(originEnt.ContainsAny("a_notexists"));
        }

        [TestMethod]
        public void ContainsNotNullTest()
        {
            Entity originEnt = GetOriginEntity();

            Assert.IsTrue(originEnt.ContainsNotNull(StringAttribute));
        }

        [TestMethod]
        public void ContainsNotNullWithNullFieldTest()
        {
            Entity originEnt = GetOriginEntity();

            Assert.IsFalse(originEnt.ContainsNotNull(NullAttribute));
        }


    }
}
