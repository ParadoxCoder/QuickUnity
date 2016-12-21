﻿/*
 *	The MIT License (MIT)
 *
 *	Copyright (c) 2016 Jerry Lee
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

namespace QuickUnity.Core.Security
{
    /// <summary>
    /// Represents int value that should be protected.
    /// </summary>
    public struct SecureInt
    {
        /// <summary>
        /// The encrypted int value.
        /// </summary>
        private int m_value;

        /// <summary>
        /// The check value.
        /// </summary>
        private int m_check;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecureInt"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public SecureInt(int value)
        {
            m_value = MemDataSecurity.EncryptIntValue(value, out m_check);
        }

        /// <summary>
        /// Gets the original int value.
        /// </summary>
        /// <returns>System.Int32 The original int value.</returns>
        public int GetValue()
        {
            return MemDataSecurity.DecryptIntValue(m_value, m_check);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            return GetValue().ToString();
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="SecureInt"/> to <see cref="System.Int32"/>.
        /// </summary>
        /// <param name="value">The value of SercureInt.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator int(SecureInt value)
        {
            return value.GetValue();
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Int32"/> to <see cref="SecureInt"/>.
        /// </summary>
        /// <param name="value">The int value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator SecureInt(int value)
        {
            return new SecureInt(value);
        }

        /// <summary>
        /// Implements the operator ++.
        /// </summary>
        /// <param name="value">The SecureInt value.</param>
        /// <returns>The result of the operator.</returns>
        public static SecureInt operator ++(SecureInt value)
        {
            SecureInt sint = new SecureInt(value + 1);
            value.m_check = sint.m_check;
            value.m_value = sint.m_value;
            return sint;
        }

        /// <summary>
        /// Implements the operator --.
        /// </summary>
        /// <param name="value">The SecureInt value.</param>
        /// <returns>The result of the operator.</returns>
        public static SecureInt operator --(SecureInt value)
        {
            SecureInt sint = new SecureInt(value - 1);
            value.m_check = sint.m_check;
            value.m_value = sint.m_value;
            return sint;
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="a">The SecureInt a.</param>
        /// <param name="b">The SecureInt b.</param>
        /// <returns>The result of the operator.</returns>
        public static SecureInt operator +(SecureInt a, SecureInt b)
        {
            int result = a.GetValue() + b.GetValue();
            return new SecureInt(result);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="a">The SecureInt a.</param>
        /// <param name="b">The SecureInt b.</param>
        /// <returns>The result of the operator.</returns>
        public static SecureInt operator -(SecureInt a, SecureInt b)
        {
            int result = a.GetValue() - b.GetValue();
            return new SecureInt(result);
        }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="a">The SecureInt a.</param>
        /// <param name="b">The SecureInt b.</param>
        /// <returns>The result of the operator.</returns>
        public static SecureInt operator *(SecureInt a, SecureInt b)
        {
            int result = a.GetValue() * b.GetValue();
            return new SecureInt(result);
        }

        /// <summary>
        /// Implements the operator /.
        /// </summary>
        /// <param name="a">The SecureInt a.</param>
        /// <param name="b">The SecureInt b.</param>
        /// <returns>The result of the operator.</returns>
        public static SecureInt operator /(SecureInt a, SecureInt b)
        {
            int result = a.GetValue() / b.GetValue();
            return new SecureInt(result);
        }

        /// <summary>
        /// Implements the operator %.
        /// </summary>
        /// <param name="a">The SecureInt a.</param>
        /// <param name="b">The SecureInt b.</param>
        /// <returns>The result of the operator.</returns>
        public static SecureInt operator %(SecureInt a, SecureInt b)
        {
            int result = a.GetValue() % b.GetValue();
            return new SecureInt(result);
        }
    }
}