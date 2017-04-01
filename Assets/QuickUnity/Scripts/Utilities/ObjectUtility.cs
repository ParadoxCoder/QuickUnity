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
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace QuickUnity.Utilities
{
    /// <summary>
    /// A class to process object things. This class cannot be inherited.
    /// </summary>
    public sealed class ObjectUtility
    {
        /// <summary>
        /// Deeps clone.
        /// </summary>
        /// <typeparam name="T">The type definition of clone object.</typeparam>
        /// <param name="source">The object of source.</param>
        /// <returns>The cloned object.</returns>
        public static T DeepClone<T>(T source)
        {
            BinaryFormatter fomatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.Clone));
            MemoryStream stream = new MemoryStream();
            fomatter.Serialize(stream, source);
            stream.Position = 0;
            object clone = fomatter.Deserialize(stream);
            stream.Close();
            return (T)clone;
        }

        /// <summary>
        /// Try parse the string object to target type value.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="source">The source string.</param>
        /// <returns>The object of target type.</returns>
        public static T TryParse<T>(string source)
        {
            Type targetType = typeof(T);
            T value = default(T);
            object[] args = new object[2] { source, value };

            ReflectionUtility.InvokeStaticMethod(targetType, "TryParse", new Type[2] {
                    typeof(string), targetType.MakeByRefType()
                }, ref args);

            return (T)args[1];
        }
    }
}