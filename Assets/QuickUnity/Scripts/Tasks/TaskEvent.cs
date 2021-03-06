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

namespace QuickUnity.Tasks
{
    /// <summary>
    /// When you use Task component, Task component will dispatch TaskEvent.
    /// </summary>
    public class TaskEvent : Events.Event
    {
        /// <summary>
        /// When task start to run, it will dispatch this event.
        /// </summary>
        public const string TaskStart = "taskStart";

        /// <summary>
        /// When task stop to run, it will dispatch this event.
        /// </summary>
        public const string TaskStop = "taskStop";

        /// <summary>
        /// When task pause, it will dispatch this event.
        /// </summary>
        public const string TaskPause = "taskPause";

        /// <summary>
        /// When task resume, it will dispatch this event.
        /// </summary>
        public const string TaskResume = "taskResume";

        /// <summary>
        /// Gets the task object.
        /// </summary>
        /// <value>The task.</value>
        public ITask task
        {
            get { return (ITask)m_target; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskEvent"/> class.
        /// </summary>
        /// <param name="type">The type of event.</param>
        /// <param name="target">The target of event.</param>
        public TaskEvent(string type, object target = null)
            : base(type, target)
        {
        }
    }
}