// <copyright file="StringHelperFixture.cs" company="Thales e-Security Ltd">
// Copyright (c) 2010 Thales e-Security Ltd
// All rights reserved. Company confidential.
// </copyright>
//
// NUnit Test Cases for [Description of StringHelperFixture]

using NRA.Util;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace NRATest.Util
{
    /// <summary>
    /// Test Cases for [Description of StringHelperFixture]
    /// </summary>
    [TestFixture(Description = "Test Cases for [Description of StringHelperFixture]")]
    public class StringHelperFixtureTest
    {
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
            string prefix = null;

            // Test
            var exception = Assert.Throws<ArgumentNullException>(
                () =>
                {
                    var result = StringHelper.EnsureStartsWith(text, prefix);
                }
                );

            // Assert
            ClassicAssert.AreEqual(exception.ParamName, "prefix");
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
            ClassicAssert.AreEqual("Hello", result);
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
            ClassicAssert.AreEqual("OHello", result);
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
            ClassicAssert.AreEqual("Hello WorldHello", result);
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
            ClassicAssert.AreEqual("Hello", result);
        }

        #endregion

        #endregion
    }
}
