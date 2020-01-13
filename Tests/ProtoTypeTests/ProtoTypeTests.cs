using System;
using FluentAssertions;
using NUnit.Framework;

namespace Prototype
{
    [TestFixture]
    public class ProtoTypeTests
    {
        [Test]
        public void ProtoTyped_Object_Will_Be_Equivalent()
        {
            var baz = new Baz();

            baz.Should().BeEquivalentTo(baz.Prototype());
        }

        [Test]
        public void ProtoTyped_Object_Will_Not_Be_Equivalent_After_Use()
        {
            var baz = new Baz();

            var protoBaz = baz.Prototype();

            protoBaz.DoIt.Invoke();

            baz.Should().NotBeEquivalentTo(protoBaz);
        }

        [Test]
        public void None_Of_A_ProtoTyped_Objects_References_Will_Match_The_Original()
        {
            var baz = new Baz();
            var protoBaz= baz.Prototype();

            baz.Should().BeEquivalentTo(baz.Prototype());
            baz.Should().NotBeSameAs(baz.Prototype());
        }

        [Test]
        public void ProtoTyped_Object_Will_Retain_Method_And_Delegate_Functionality()
        {
            var baz = new Baz();
            var proProtoTypeOriginal = baz.IntVal;

            baz.DoIt.Invoke();

            var preProtoTypeVariance = baz.IntVal - proProtoTypeOriginal;

            var protoBaz = baz.Prototype();

            protoBaz.IntVal.Should().Be(baz.IntVal);

            var postProtoTypeOriginal = protoBaz.IntVal;

            protoBaz.DoIt.Invoke();

            var postProtoTypeVariance = protoBaz.IntVal - postProtoTypeOriginal;

            preProtoTypeVariance.Should().Be(postProtoTypeVariance);


        }
    }
}