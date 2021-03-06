﻿using QuickUnity.Patterns;
using UnityEngine;

namespace QuickUnity.Examples.Singleton
{
    /// <summary>
    /// Class TestSingletonOne.
    /// </summary>
    public sealed class TestSingletonOne : Singleton<TestSingletonOne>
    {
        /// <summary>
        /// Prevents a default sInstance of the <see cref="TestSingletonOne"/> class from being created.
        /// </summary>
        private TestSingletonOne()
        {
            UnityEngine.Debug.Log("TestSingletonOne");
        }

        public void Run()
        {
            UnityEngine.Debug.Log("TestSingletonOne Run!");
        }
    }
}