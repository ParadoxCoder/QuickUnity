﻿using NUnit.Framework;
using QuickUnity.Core.Miscs;
using QuickUnity.Data;
using System.Collections.Generic;
using UnityEngine;

namespace QuickUnity.Tests.UnitTests
{
    /// <summary>
    /// Unit test cases for class BoxDBAdapter.
    /// </summary>
    internal class BoxDBAdapterTestVO
    {
        public int id;

        public string name;

        public float value;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxDBAdapterTestVO"/> class.
        /// </summary>
        public BoxDBAdapterTestVO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxDBAdapterTestVO"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public BoxDBAdapterTestVO(int id, string name, float value)
        {
            this.id = id;
            this.name = name;
            this.value = value;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("id={0}, name={1}, value={2}", id, name, value);
        }
    }

    /// <summary>
    /// Unit test cases for class BoxDBAdapter.
    /// </summary>
    [TestFixture]
    [Category("QuickUnity Tests/Unit Tests/BoxDBAdapter Tests")]
    internal class BoxDBAdapterTests
    {
        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        private string tableName
        {
            get { return "BoxDBAdapterTestTable"; }
        }

        /// <summary>
        /// Test for the method BoxDBAdapterTest.Insert.
        /// </summary>
        [Test]
        public void InsertTest()
        {
            BoxDBAdapter db = GetBoxDBAdapter();
            BoxDBAdapterTestVO[] voList = new BoxDBAdapterTestVO[3]
            {
                new BoxDBAdapterTestVO((int)db.MakeNewId(), "test1", 1f),
                new BoxDBAdapterTestVO((int)db.MakeNewId(), "测试2", 2f),
                new BoxDBAdapterTestVO((int)db.MakeNewId(), "test3", 3f)
            };
            bool success = db.Insert(tableName, voList);
            db.Dispose();
            Assert.IsTrue(success);
        }

        /// <summary>
        /// Test for the method BoxDBAdapterTest.SelectCount.
        /// </summary>
        [Test]
        public void SelectCountTest()
        {
            BoxDBAdapter db = GetBoxDBAdapter();
            long count = db.SelectCount(tableName);
            db.Dispose();
            Assert.AreEqual(3, count);
        }

        /// <summary>
        /// Test for the method BoxDBAdapterTest.Select when on multi-conditions query.
        /// </summary>
        [Test]
        public void MuiltiConditionsSelectCountTest()
        {
            BoxDBAdapter db = GetBoxDBAdapter();
            List<BoxDBQueryCondition> list = new List<BoxDBQueryCondition>()
            {
                new BoxDBQueryCondition("id", 1),
                new BoxDBQueryCondition("id", 2)
            };

            long count = db.SelectCount(tableName, list,
                new List<BoxDBMultiConditionOperator>(new BoxDBMultiConditionOperator[1] { BoxDBMultiConditionOperator.Or }));
            db.Dispose();
            Assert.AreEqual(2, count);
        }

        /// <summary>
        /// Test for the method BoxDBAdapterTest.Select.
        /// </summary>
        [Test]
        public void SelectTest()
        {
            BoxDBAdapter db = GetBoxDBAdapter();
            BoxDBAdapterTestVO vo = db.Select<BoxDBAdapterTestVO>(tableName, 1);
            DebugLogger.Log(vo.ToString());
            db.Dispose();
            Assert.IsNotNull(vo);
        }

        /// <summary>
        /// Test for the method BoxDBAdapterTest.Select when on multi-conditions query.
        /// </summary>
        [Test]
        public void MuiltiConditionsSelectTest()
        {
            BoxDBAdapter db = GetBoxDBAdapter();
            List<BoxDBQueryCondition> list = new List<BoxDBQueryCondition>()
            {
                new BoxDBQueryCondition("id", 1),
                new BoxDBQueryCondition("id", 2)
            };

            List<BoxDBAdapterTestVO> result = db.Select<BoxDBAdapterTestVO>(tableName, list,
                new List<BoxDBMultiConditionOperator>(new BoxDBMultiConditionOperator[1] { BoxDBMultiConditionOperator.Or }));
            db.Dispose();
            Assert.AreEqual(2f, result[0].value);
        }

        /// <summary>
        /// Test for the method BoxDBAdapterTest.SelectAll.
        /// </summary>
        [Test]
        public void SelectAllTest()
        {
            BoxDBAdapter db = GetBoxDBAdapter();
            List<BoxDBAdapterTestVO> list = db.SelectAll<BoxDBAdapterTestVO>(tableName);

            foreach (BoxDBAdapterTestVO vo in list)
            {
                DebugLogger.Log(vo.ToString());
            }

            db.Dispose();
            Assert.AreEqual(3, list.Count);
        }

        /// <summary>
        /// Test for the method BoxDBAdapterTest.Update.
        /// </summary>
        [Test]
        public void UpdateTest()
        {
            BoxDBAdapter db = GetBoxDBAdapter();
            BoxDBAdapterTestVO[] voList = new BoxDBAdapterTestVO[3]
            {
                new BoxDBAdapterTestVO(1, "test1_update", 1f),
                new BoxDBAdapterTestVO(2, "测试2_update", 2f),
                new BoxDBAdapterTestVO(4, "test3_update", 3f)
            };
            db.Update(tableName, voList);

            BoxDBAdapterTestVO vo = db.Select<BoxDBAdapterTestVO>(tableName, 1);
            db.Dispose();

            Assert.AreEqual("test1_update", vo.name);
        }

        /// <summary>
        /// Test for the method BoxDBAdapterTest.Delete.
        /// </summary>
        [Test]
        public void DeleteTest()
        {
            BoxDBAdapter db = GetBoxDBAdapter();
            db.Delete(tableName, 3);
            long count = db.SelectCount(tableName);
            Assert.AreEqual(2, count);
        }

        /// <summary>
        /// Gets the box database adapter.
        /// </summary>
        /// <returns>BoxDBAdapter.</returns>
        private BoxDBAdapter GetBoxDBAdapter()
        {
            BoxDBAdapter db = new BoxDBAdapter(Application.persistentDataPath);
            db.EnsureTable<BoxDBAdapterTestVO>(tableName, "id");
            db.Open();
            return db;
        }
    }
}