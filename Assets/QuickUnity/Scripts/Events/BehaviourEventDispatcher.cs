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

namespace QuickUnity.Events
{
    /// <summary>
    /// The BehaviourEventDispatcher class is the class for all classes that both inherits from
    /// MonoBehaviour and need to dispatch events.
    /// </summary>
    /// <seealso cref="QuickUnity.MonoBehaviourBase"/>
    /// <seealso cref="QuickUnity.Events.IEventDispatcher"/>
    public class BehaviourEventDispatcher : MonoBehaviourBase, IEventDispatcher
    {
        /// <summary>
        /// The event dispatcher.
        /// </summary>
        protected IEventDispatcher m_eventDispatcher = null;

        #region Protected Functions

        /// <summary>
        /// Called when script receive message Awake.
        /// </summary>
        protected override void OnAwake()
        {
            m_eventDispatcher = new EventDispatcher();
        }

        #endregion Protected Functions

        #region Messages

        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        protected virtual void OnDestroy()
        {
            m_eventDispatcher = null;
        }

        #endregion Messages

        #region IEventDispatcher Implementations

        /// <summary>
        /// Registers an event listener object with an EventDispatcher object so that the listener
        /// receives notification of an event.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the parameter of the method that this delegate encapsulates.
        /// </typeparam>
        /// <param name="eventType">The type of event.</param>
        /// <param name="listener">The listener function that processes the event.</param>
        public void AddEventListener<T>(string eventType, Action<T> listener) where T : Event
        {
            if (m_eventDispatcher != null)
            {
                m_eventDispatcher.AddEventListener(eventType, listener);
            }
        }

        /// <summary>
        /// Dispatches the event.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the parameter of the method that this delegate encapsulates.
        /// </typeparam>
        /// <param name="eventObject">The event object.</param>
        public void DispatchEvent<T>(T eventObject) where T : Event
        {
            if (m_eventDispatcher != null)
            {
                m_eventDispatcher.DispatchEvent(eventObject);
            }
        }

        /// <summary>
        /// Checks whether the EventDispatcher object has listener registered for a specific type of event.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the parameter of the method that this delegate encapsulates.
        /// </typeparam>
        /// <param name="eventType">The type of event.</param>
        /// <param name="listener">The listener function that processes the event.</param>
        /// <returns>
        /// A value of <c>true</c> if a listener of the specified type is registered; <c>false</c> otherwise.
        /// </returns>
        public bool HasEventListener<T>(string eventType, Action<T> listener) where T : Event
        {
            if (m_eventDispatcher != null)
            {
                return m_eventDispatcher.HasEventListener(eventType, listener);
            }

            return false;
        }

        /// <summary>
        /// Checks whether the EventDispatcher object has listeners registered for a specific type of event.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <returns>
        /// <c>true</c> if [has event listener] [the specified event type]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasEventListeners(string eventType)
        {
            if (m_eventDispatcher != null)
            {
                return m_eventDispatcher.HasEventListeners(eventType);
            }

            return false;
        }

        /// <summary>
        /// Checks whether the EventDispatcher object has listeners registered for a target object.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <returns>
        /// <c>true</c> if [has event listeners] for [the specified target]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasEventListeners(object target)
        {
            if (m_eventDispatcher != null)
            {
                return m_eventDispatcher.HasEventListeners(target);
            }

            return false;
        }

        /// <summary>
        /// Checks whether the EventDispatcher object has any listener registered.
        /// </summary>
        /// <returns><c>true</c> if [has event listeners]; otherwise, <c>false</c>.</returns>
        public bool HasAnyEventListener()
        {
            if (m_eventDispatcher != null)
            {
                return m_eventDispatcher.HasAnyEventListener();
            }

            return false;
        }

        /// <summary>
        /// Removes a listener from the EventDispatcher object.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the parameter of the method that this delegate encapsulates.
        /// </typeparam>
        /// <param name="eventType">The type of event.</param>
        /// <param name="listener">The listener object to remove.</param>
        public void RemoveEventListener<T>(string eventType, Action<T> listener) where T : Event
        {
            if (m_eventDispatcher != null)
            {
                m_eventDispatcher.RemoveEventListener(eventType, listener);
            }
        }

        /// <summary>
        /// Removes the event listener by event type.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        public void RemoveEventListener(string eventType)
        {
            if (m_eventDispatcher != null)
            {
                m_eventDispatcher.RemoveEventListener(eventType);
            }
        }

        /// <summary>
        /// Removes listeners from the EventDispatcher object by matching target.
        /// </summary>
        /// <param name="target">The target object.</param>
        public void RemoveEventListeners(object target)
        {
            if (m_eventDispatcher != null)
            {
                m_eventDispatcher.RemoveEventListeners(target);
            }
        }

        /// <summary>
        /// Removes all event listeners.
        /// </summary>
        public void RemoveAllEventListeners()
        {
            if (m_eventDispatcher != null)
            {
                m_eventDispatcher.RemoveAllEventListeners();
            }
        }

        #endregion IEventDispatcher Implementations
    }
}