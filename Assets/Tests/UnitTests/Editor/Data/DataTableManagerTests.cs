﻿using NUnit.Framework;
using QuickUnity.Data;
using System.Collections.Generic;
using UnityEngine;
using QuickUnity.Extensions;

namespace QuickUnity.UnitTests.Data
{
    /// <summary>
    /// Unit test cases for class <see cref="QuickUnity.Data.DataTableManager"/>.
    /// </summary>
    [TestFixture]
    internal class DataTableManagerTests
    {
        /// <summary>
        /// Test for the method DataTableManager.GetDataTableRow.
        /// </summary>
        [Test]
        public void GetDataTableRowTest()
        {
            TestData testData = DataTableManager.instance.GetDataTableRow<TestData>(1L);
            DataTableManager.instance.Dispose();

            if (testData != null)
            {
                if (testData.testVector2.ToVector2() == new Vector2(1, 2) &&
                    testData.testVector3.ToVector3() == new Vector3(1, 2, 3) &&
                    testData.testQuaternion.ToQuaternion() == new Quaternion(1, 2, 3, 4) &&
                    testData.testInt == 2147483647)
                {
                    Assert.Pass();
                }
            }
            else
            {
                Assert.Fail();
            }
        }

        /// <summary>
        /// Test for the method DataTableManager.GetDataTableRows.
        /// </summary>
        [Test]
        public void GetDataTableRowsTest()
        {
            List<BoxDBQueryCondition> conditions = new List<BoxDBQueryCondition>()
            {
                new BoxDBQueryCondition("testUShort", (ushort)0),
                new BoxDBQueryCondition("testBoolean", false)
            };

            List<BoxDBMultiConditionOperator> multiConditionOps = new List<BoxDBMultiConditionOperator>()
            {
                BoxDBMultiConditionOperator.Or
            };

            TestData[] results = DataTableManager.instance.GetDataTableRows<TestData>(conditions, multiConditionOps);
            DataTableManager.instance.Dispose();

            if (results != null)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }
        }

        /// <summary>
        /// Test for the method DataTableManager.GetAllDataTableRow.
        /// </summary>
        [Test]
        public void GetAllDataTableRowsTest()
        {
            TestData[] array = DataTableManager.instance.GetAllDataTableRows<TestData>();
            DataTableManager.instance.Dispose();
            Assert.IsNotNull(array);
        }

        /// <summary>
        /// Test for the method DataTableManager.GetAllDataTableRowsCount.
        /// </summary>
        [Test]
        public void GetAllDataTableRowsCountTest()
        {
            long count = DataTableManager.instance.GetAllDataTableRowsCount<TestData>();
            DataTableManager.instance.Dispose();
            Assert.AreEqual(3L, count);
        }

        /// <summary>
        /// Test for the method DataTableManager.GetDataTableRowsCount.
        /// </summary>
        [Test]
        public void GetDataTableRowsCountTest()
        {
            List<BoxDBQueryCondition> conditions = new List<BoxDBQueryCondition>()
            {
                new BoxDBQueryCondition("testInt", 2147483647),
                new BoxDBQueryCondition("testBoolean", true),
                new BoxDBQueryCondition("testUInt", (uint)0)
            };

            List<BoxDBMultiConditionOperator> multiConditionOps = new List<BoxDBMultiConditionOperator>()
            {
                BoxDBMultiConditionOperator.Or,
                BoxDBMultiConditionOperator.And
            };

            long count = DataTableManager.instance.GetDataTableRowsCount<TestData>(conditions, multiConditionOps);
            DataTableManager.instance.Dispose();
            Assert.Greater(count, 0L);
        }
    }
}