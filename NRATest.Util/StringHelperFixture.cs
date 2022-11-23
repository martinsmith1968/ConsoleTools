// <copyright file="StringHelperFixture.cs" company="Thales e-Security Ltd">
// Copyright (c) 2010 Thales e-Security Ltd
// All rights reserved. Company confidential.
// </copyright>
//
// NUnit Test Cases for [Description of StringHelperFixture]

using System;
using System.Collections.Generic;
using NRA.Util;
using NUnit.Framework;

namespace NRATest.Util
{
    /// <summary>
    /// Test Cases for [Description of StringHelperFixture]
    /// </summary>
    [TestFixture(Description = "Test Cases for [Description of StringHelperFixture]")]
    public class StringHelperFixtureTest
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Fixture Level Initialisation / Termination

        /// <summary>
        /// Sets up this instance.
        /// </summary>
        /// <remarks>
        /// For testing frameworks (E.g. NUnit) this method is called once before any [Test] methods
        /// </remarks>
        [TestFixtureSetUp]
        public void FixtureSetup()
        {
        }

        /// <summary>
        /// Tears down this instance.
        /// </summary>
        /// <remarks>
        /// For testing frameworks (E.g. NUnit) this method is called once after all [Test] methods
        /// </remarks>
        [TestFixtureTearDown]
        public void FixtureTeardown()
        {
        }

        #endregion

        #region Individual Test Level Initialisation / Termination

        /// <summary>
        /// Sets up the test.
        /// </summary>
        /// <remarks>
        /// For testing frameworks (E.g. NUnit) this method is called before each [Test] method
        /// </remarks>
        [SetUp]
        public void TestSetup()
        {
        }

        /// <summary>
        /// Tears down the test.
        /// </summary>
        /// <remarks>
        /// For testing frameworks (E.g. NUnit) this method is called after each [Test] method
        /// </remarks>
        [TearDown]
        public void TestTeardown()
        {
        }

        #endregion

        #region Test Methods

        #region EnsureStartsWith

        /// <summary>
        /// Check_s the ensure starts with_ null prefix_ returns unchanged.
        /// </summary>
        [Test]
        public void Check_EnsureStartsWith_NullPrefix_Throws()
        {
            // Setup
            var text = "Hello";
            var prefix = (string)null;

            // Test
            var exception = Assert.Throws<ArgumentNullException>(
                () =>
                {
                    var result = StringHelper.EnsureStartsWith(text, prefix);
                }
                );

            // Assert
            Assert.AreEqual("prefix", exception.ParamName);
        }

        /// <summary>
        /// Check_s the ensure starts with_ existing prefix_ returns unchanged.
        /// </summary>
        [Test]
        public void Check_EnsureStartsWith_ExistingPrefix_ReturnsUnchanged()
        {
            // Setup
            var text = "Hello";
            var prefix = "H";

            // Test
            var result = StringHelper.EnsureStartsWith(text, prefix);

            // Assert
            Assert.AreEqual("Hello", result);
        }

        /// <summary>
        /// Check_s the ensure starts with_ nonexisting prefix_ returns prefixed.
        /// </summary>
        [Test]
        public void Check_EnsureStartsWith_NonexistingPrefix_ReturnsPrefixed()
        {
            // Setup
            var text = "Hello";
            var prefix = "O";

            // Test
            var result = StringHelper.EnsureStartsWith(text, prefix);

            // Assert
            Assert.AreEqual("OHello", result);
        }

        /// <summary>
        /// Check_s the ensure starts with_ prefix larger than text_ returns unchanged.
        /// </summary>
        [Test]
        public void Check_EnsureStartsWith_PrefixLargerThanText_ReturnsPrefixed()
        {
            // Setup
            var text = "Hello";
            var prefix = "Hello World";

            // Test
            var result = StringHelper.EnsureStartsWith(text, prefix);

            // Assert
            Assert.AreEqual("Hello WorldHello", result);
        }

        /// <summary>
        /// Check_s the ensure starts with_ empty prefix_ returns unchanged.
        /// </summary>
        [Test]
        public void Check_EnsureStartsWith_EmptyPrefix_ReturnsUnchanged()
        {
            // Setup
            var text = "Hello";
            var prefix = "";

            // Test
            var result = StringHelper.EnsureStartsWith(text, prefix);

            // Assert
            Assert.AreEqual("Hello", result);
        }

        #endregion

        #endregion
    }
}
