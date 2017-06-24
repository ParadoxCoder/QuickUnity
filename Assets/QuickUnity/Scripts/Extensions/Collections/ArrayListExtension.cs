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

using System.Collections;

namespace QuickUnity.Extensions.Collections
{
    /// <summary>
    /// Extension methods to the <see cref="System.Collections.ArrayList"/>.
    /// </summary>
    public static class ArrayListExtension
    {
        /// <summary>
        /// Adds a unique item to the <see cref="System.Collections.ArrayList"/>.
        /// </summary>
        /// <param name="source">A <see cref="System.Collections.ArrayList"/> oject to add item.</param>
        /// <param name="value">The Object to be added to the end of the <see cref="System.Collections.ArrayList"/>.</param>
        public static void AddUnique(this ArrayList source, object value)
        {
            (source as IList).AddUnique(value);
        }

        /// <summary>
        /// Adds the range unique collection.
        /// </summary>
        /// <param name="source">A <see cref="System.Collections.ArrayList"/> object to add some items.</param>
        /// <param name="collection">The Objects to be added to the end of the <see cref="System.Collections.ArrayList"/>.</param>
        public static void AddRangeUnique(this ArrayList source, ICollection collection)
        {
            ArrayList newCollection = new ArrayList();

            foreach (object item in collection)
            {
                if (!source.Contains(item) && !newCollection.Contains(item))
                {
                    newCollection.Add(item);
                }
            }

            source.AddRange(newCollection);
        }
    }
}