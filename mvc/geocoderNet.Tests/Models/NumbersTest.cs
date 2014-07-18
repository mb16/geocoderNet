using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using geocoderNet.Models;

namespace geocoderNet.Tests
{
    [TestClass]
    public class NumbersTest
    {
        [TestMethod]
        public void Ordinals()
        {
            var ordinals = Numbers.Ordinals();
            Assert.AreEqual(ordinals[0], "zeroth", " Ordinal missing");
            Assert.AreEqual(ordinals[20], "twentieth", " Ordinal missing");
            Assert.AreEqual(ordinals[31], "thirty-first", " Ordinal missing");

        }
           
        [TestMethod]
        public void Cardinals()
        {
            var cardinals = Numbers.Cardinals();
            Assert.AreEqual(cardinals[0], "zero", " Cardinal missing");
            Assert.AreEqual(cardinals[20], "twenty", " Cardinal missing");
            Assert.AreEqual(cardinals[31], "thirty-one", " Cardinal missing");

        }
    }
}
