﻿/*
 *	The MIT License (MIT)
 *
 *	Copyright (c) 2017 Jerry Lee
 *
 *	Permission is hereby granted, free of charge, to any person obtaining a copy
 *	of this software and associated documentation files (the "Software"), to deal
 *	in the Software without restriction, including without limitation the rights
 *	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *	copies of the Software, and to permit persons to whom the Software is
 *	furnished to do so, subject to the following conditions:
 *
 *	The above copyright notice and this permission notice shall be included in all
 *	copies or substantial portions of the Software.
 *
 *	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *	SOFTWARE.
 */

using System;

namespace QuickUnityEditor.Attributes
{
    /// <summary>
    /// Allow an editor class to be initialized when Unity loads without action from the user.
    /// </summary>
    /// <seealso cref="System.Attribute"/>
    [AttributeUsage(AttributeTargets.Class)]
    internal class InitializeOnEditorStartupAttribute : Attribute
    {
        /// <summary>
        /// The execution order.
        /// </summary>
        private int m_executionOrder;

        /// <summary>
        /// Gets the execution order.
        /// </summary>
        /// <value>The execution order.</value>
        public int executionOrder
        {
            get { return m_executionOrder; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InitializeOnEditorStartupAttribute"/> class.
        /// </summary>
        /// <param name="executionOrder">The execution order.</param>
        public InitializeOnEditorStartupAttribute(int executionOrder = 0)
            : base()
        {
            m_executionOrder = executionOrder;
        }
    }
}