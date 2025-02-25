#region Copyright (c) 2011-2020 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2020 Technosoftware GmbH. All rights reserved
// Web: https://www.technosoftware.com 
// 
// The source code in this file is covered under a dual-license scenario:
//   - Owner of a purchased license: SCLA 1.0
//   - GPL V3: everybody else
//
// SCLA license terms accompanied with this source code.
// See SCLA 1.0://technosoftware.com/license/Source_Code_License_Agreement.pdf
//
// GNU General Public License as published by the Free Software Foundation;
// version 3 of the License are accompanied with this source code.
// See https://technosoftware.com/license/GPLv3License.txt
//
// This source code is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE.
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2011-2020 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Globalization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.IO;
using Microsoft.Win32;
using System.Runtime.InteropServices;
#endregion

namespace Technosoftware.DaAeHdaClient.Utilities
{
	/// <summary>
	/// Defines various static utility functions.
	/// </summary>
	internal static class Utils
    {
        #region Trace Support
        #if DEBUG
        private static int s_traceOutput = (int)TraceOutput.DebugAndFile;
        private static int s_traceMasks = (int)TraceMasks.All;
        #else
        private static int s_traceOutput = (int)TraceOutput.FileOnly;
        private static int s_traceMasks = (int)TraceMasks.None;
        #endif

        private static string s_traceFileName = null;
        private static long s_BaseLineTicks = DateTime.UtcNow.Ticks;
        private static object s_traceFileLock = new object();

        /// <summary>
        /// The possible trace output mechanisms.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public enum TraceOutput
        {
            /// <summary>
            /// No tracing
            /// </summary>
            Off = 0,

            /// <summary>
            /// Only write to file (if specified). Default for Release mode.
            /// </summary>
            FileOnly = 1,

            /// <summary>
            /// Write to debug trace listeners and a file (if specified). Default for Debug mode.
            /// </summary>
            DebugAndFile = 2,

            /// <summary>
            /// Write to trace listeners and a file (if specified).
            /// </summary>
            StdOutAndFile = 3
        }

        /// <summary>
        /// The masks used to filter trace messages.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public static class TraceMasks
        {
            /// <summary>
            /// Do not output any messages.
            /// </summary>
            public const int None = 0x0;
            
            /// <summary>
            /// Output error messages.
            /// </summary>
            public const int Error = 0x1;

            /// <summary>
            /// Output informational messages.
            /// </summary>
            public const int Information = 0x2;

            /// <summary>
            /// Output stack traces.
            /// </summary>
            public const int StackTrace = 0x4;

            /// <summary>
            /// Output basic messages for service calls.
            /// </summary>
            public const int Service = 0x8;

            /// <summary>
            /// Output detailed messages for service calls.
            /// </summary>
            public const int ServiceDetail = 0x10;

            /// <summary>
            /// Output basic messages for each operation.
            /// </summary>
            public const int Operation = 0x20;

            /// <summary>
            /// Output detailed messages for each operation.
            /// </summary>
            public const int OperationDetail = 0x40;

            /// <summary>
            /// Output messages related to application initialization or shutdown
            /// </summary>
            public const int StartStop = 0x80;

            /// <summary>
            /// Output messages related to a call to an external system.
            /// </summary>
            public const int ExternalSystem = 0x100;

            /// <summary>
            /// Output messages related to security
            /// </summary>
            public const int Security = 0x200;

            /// <summary>
            /// Output all messages.
            /// </summary>
            public const int All = 0x7FFFFFFF;
        }

        /// <summary>
        /// Sets the output for tracing (thead safe).
        /// </summary>
        public static void SetTraceOutput(TraceOutput output)
        {
            lock (s_traceFileLock)
            {
                s_traceOutput = (int)output;
            }
        }

        /// <summary>
        /// Gets the current trace mask settings.
        /// </summary>
        public static int TraceMask
        {
            get { return s_traceMasks; }
        }

        /// <summary>
        /// Sets the mask for tracing (thead safe).
        /// </summary>
        public static void SetTraceMask(int masks)
        {
            s_traceMasks = (int)masks;
        }

        /// <summary>
        /// Returns Tracing class instance for event attaching.
        /// </summary>
        public static Tracing Tracing
        {
            get
            { return Tracing.Instance; }
        }

        /// <summary>
        /// Writes a trace statement.
        /// </summary>
        private static void TraceWriteLine(string message, params object[] args)
        {
            // null strings not supported.
            if (String.IsNullOrEmpty(message))
            {
                return;
            }

            // format the message if format arguments provided.
            string output = message;

            if (args != null && args.Length > 0)
            {
                try
                {
                    output = String.Format(CultureInfo.InvariantCulture, message, args);
                }
                catch (Exception)
                {
                    output = message;
                } 
            }

            // write to the log file.
            lock (s_traceFileLock)
            {
                // write to debug trace listeners.
                if (s_traceOutput == (int)TraceOutput.DebugAndFile)
                {
                    System.Diagnostics.Debug.WriteLine(output);
                }

                // write to trace listeners.
                if (s_traceOutput == (int)TraceOutput.StdOutAndFile)
                {
                    System.Diagnostics.Trace.WriteLine(output);
                }

                string traceFileName = s_traceFileName;

                if (s_traceOutput != (int)TraceOutput.Off && !String.IsNullOrEmpty(traceFileName))
                {
                    try
                    {
                        FileInfo file = new FileInfo(traceFileName);

                        // limit the file size. hard coded for now - fix later.
                        bool truncated = false;

                        if (file.Exists && file.Length > 10000000)
                        {
                            file.Delete();
                            truncated = true;
                        }

                        using (StreamWriter writer = new StreamWriter(File.Open(traceFileName, FileMode.Append)))
                        {
                            if (truncated)
                            {
                                writer.WriteLine("WARNING - LOG FILE TRUNCATED.");
                            }

                            writer.WriteLine(output);
                            writer.Close();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Could not write to trace file. Error={0}\r\nFilePath={1}", e.Message, traceFileName);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the path to the log file to use for tracing.
        /// </summary>
        public static void SetTraceLog(string filePath, bool deleteExisting)
        {
            // turn tracing on.
            lock (s_traceFileLock)
            {
                // check if tracing is being turned off.
                if (String.IsNullOrEmpty(filePath))
                {
                    s_traceFileName = null;
                    return;
                }

                s_traceFileName = GetAbsoluteFilePath(filePath, true, false, true);

                if (s_traceOutput == (int)TraceOutput.Off)
                {
                    s_traceOutput = (int)TraceOutput.FileOnly;
                }

                try
                {
                    FileInfo file = new FileInfo(s_traceFileName);

                    if (deleteExisting && file.Exists)
                    {
                        file.Delete();
                    }

                    // write initial log message.
                    TraceWriteLine(
                        "\r\nPID:{2} {1} Logging started at {0} {1}",
                        DateTime.Now,
                        new String('*', 25),
                        Process.GetCurrentProcess().Id);
                }
                catch (Exception e)
                {
                    TraceWriteLine(e.Message, null);
                }
            }
        }
        
        /// <summary>
        /// Writes an informational message to the trace log.
        /// </summary>
        public static void Trace(string format, params object[] args)
        {
            Trace((int)TraceMasks.Information, format, false, args);
        }

        /// <summary>
        /// Writes an exception/error message to the trace log.
        /// </summary>
        public static void Trace(Exception e, string format, params object[] args)
        {
            Trace(e, format, false, args);
        }

        /// <summary>
        /// Writes a general message to the trace log.
        /// </summary>
        public static void Trace(int traceMask, string format, params object[] args)
        {
            Trace(traceMask, format, false, args);
        }

        /// <summary>
        /// Writes an exception/error message to the trace log.
        /// </summary>
        internal static void Trace(Exception e, string format, bool handled, params object[] args)
        {
            StringBuilder message = new StringBuilder();

            // format message.            
            if (args != null && args.Length > 0)
            {
                try
                {
                    message.AppendFormat(CultureInfo.InvariantCulture, format, args);
                }
                catch (Exception)
                {
                    message.Append(format);
                }
            }
            else
            {
                message.Append(format);
            }

            // append exception information.
            if (e != null)
            {
                OpcResultException sre = e as OpcResultException;

                if (sre != null)
                {
                    message.AppendFormat(CultureInfo.InvariantCulture, " {0} '{1}'", sre.Result.Code, sre.Message);
                }
                else
                {
                    message.AppendFormat(CultureInfo.InvariantCulture, " {0} '{1}'", e.GetType().Name, e.Message);
                }

                // append stack trace.
                if ((s_traceMasks & (int)TraceMasks.StackTrace) != 0)
                {
                    message.AppendFormat(CultureInfo.InvariantCulture, "\r\n\r\n{0}\r\n", new String('=', 40));
                    message.Append(e.StackTrace.ToString());
                    message.AppendFormat(CultureInfo.InvariantCulture, "\r\n{0}\r\n", new String('=', 40));
                }
            }

            // trace message.
            Trace((int)TraceMasks.Error, message.ToString(), handled, null);
        }

        /// <summary>
        /// Writes the message to the trace log.
        /// </summary>
        private static void Trace(int traceMask, string format, bool handled, params object[] args)
        {
            if (!handled)
            {
                Tracing.Instance.RaiseTraceEvent(new TraceEventArgs(traceMask, format, string.Empty, null, args));
            }

            // do nothing if mask not enabled.
            if ((s_traceMasks & traceMask) == 0)
            {
                return;
            }

            double seconds = ((double)(HiResClock.UtcNow.Ticks - s_BaseLineTicks))/TimeSpan.TicksPerSecond;
            
            StringBuilder message = new StringBuilder();

            // append process and timestamp.
            #if !SILVERLIGHT
            message.AppendFormat("{0} - ", Process.GetCurrentProcess().Id);
            #endif
            message.AppendFormat("{0:d} {0:HH:mm:ss.fff} ", HiResClock.UtcNow.ToLocalTime());

            // format message.
            if (args != null && args.Length > 0)
            {
                try
                {
                    message.AppendFormat(CultureInfo.InvariantCulture, format, args);
                }
                catch (Exception)
                {
                    message.Append(format);
                } 
            }
            else
            {
                message.Append(format);
            }

            TraceWriteLine(message.ToString(), null);
        }

        #endregion

        #region File Access

        /// <summary>
        /// Replaces a prefix enclosed in '%' with a special folder or environment variable path (e.g. %ProgramFiles%\MyCompany).
        /// </summary>
        public static string ReplaceSpecialFolderNames(string input)
        {
            // nothing to do for nulls.
            if (String.IsNullOrEmpty(input))
            {
                return null;
            }

            // check for absolute path.
            if (input.Length > 1 && ((input[0] == '\\' && input[1] == '\\') || input[1] == ':'))
            {
                return input;
            }

            // check for special folder prefix.
            if (input[0] != '%')
            {
                return input;
            }

            // extract special folder name.
            string folder = null;
            string path = null;

            int index = input.IndexOf('%', 1);

            if (index == -1)
            {
                folder = input.Substring(1);
                path = String.Empty;
            }
            else
            {
                folder = input.Substring(1, index - 1);
                path = input.Substring(index + 1);
            }

            StringBuilder buffer = new StringBuilder();

            // check for special folder.
            try
            {
                var specialFolder = (Environment.SpecialFolder)Enum.Parse(
                    typeof (Environment.SpecialFolder),
                    folder,
                    true);

                buffer.Append(Environment.GetFolderPath(specialFolder));
            }

            // check for generic environment variable.
            catch (Exception)
            {
                string value = Environment.GetEnvironmentVariable(folder);

                if (value != null)
                {
                    buffer.Append(value);
                }
            }

            // construct new path.
            buffer.Append(path);
            return buffer.ToString();
        }

        /// <summary>
        /// Checks if the file path is a relative path and returns an absolute path relative to the EXE location.
        /// </summary>
        public static string GetAbsoluteFilePath(string filePath)
        {
            return GetAbsoluteFilePath(filePath, false, true, false);
        }

        /// <summary>
        /// Checks if the file path is a relative path and returns an absolute path relative to the EXE location.
        /// </summary>
        public static string GetAbsoluteFilePath(string filePath, bool checkCurrentDirectory, bool throwOnError, bool createAlways)
        {
            filePath = Utils.ReplaceSpecialFolderNames(filePath);

            if (!String.IsNullOrEmpty(filePath))
            {
                FileInfo file = new FileInfo(filePath);

                // check for absolute path.
                bool isAbsolute = filePath.StartsWith("\\\\", StringComparison.Ordinal) || filePath.IndexOf(':') == 1;

                if (isAbsolute)
                {
                    if (file.Exists)
                    {
                        return filePath;
                    }

                    if (createAlways)
                    {
                        return CreateFile(file, filePath, throwOnError);
                    }
                }

                if (!isAbsolute)
                {
                    // look current directory.
                    if (checkCurrentDirectory)
                    {
                        if (!file.Exists)
                        {
                            file = new FileInfo(Utils.Format("{0}\\{1}", Environment.CurrentDirectory, filePath));
                        }

                        if (file.Exists)
                        {
                            return file.FullName;
                        }

                        if (createAlways)
                        {
                            return CreateFile(file, filePath, throwOnError);
                        }
                    }

                    // look executable directory.
                    if (!file.Exists)
                    {
#if !SILVERLIGHT
                        string executablePath = Environment.GetCommandLineArgs()[0];
                        FileInfo executable = new FileInfo(executablePath);

                        if (executable.Exists)
                        {
                            file = new FileInfo(Utils.Format("{0}\\{1}", executable.DirectoryName, filePath));
                        }

                        if (file.Exists)
                        {
                            return file.FullName;
                        }
#endif

                        if (createAlways)
                        {
                            return CreateFile(file, filePath, throwOnError);
                        }
                    }
                }
            }

            // file does not exist.
            if (throwOnError)
            {
                throw new OpcResultException(new OpcResult((int)OpcResult.CONNECT_E_NOCONNECTION.Code, OpcResult.FuncCallType.SysFuncCall, null), String.Format("File does not exist: {0}\r\nCurrent directory is: {1}", filePath, Environment.CurrentDirectory));
            }

            return null;
        }

        /// <summary>
        /// Creates an empty file.
        /// </summary>
        private static string CreateFile(FileInfo file, string filePath, bool throwOnError)
        {
            try
            {
                // create the directory as required.
                if (!file.Directory.Exists)
                {
                    Directory.CreateDirectory(file.DirectoryName);
                }

                // open and close the file.
                using (Stream ostrm = file.Open(FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    return filePath;
                }
            }
            catch (Exception e)
            {
                if (throwOnError)
                {
                    throw;
                }

                return filePath;
            }
        }

        /// <summary>
        /// Formats a message using the invariant locale.
        /// </summary>
        public static string Format(string text, params object[] args)
        {
            return String.Format(CultureInfo.InvariantCulture, text, args);
        }
        #endregion
    }

    #region Tracing Class
    /// <summary>
    /// Used as underlying tracing object for event processing.
    /// </summary>
    public class Tracing
    {
        #region Private Members
        private static object m_syncRoot = new Object();
        private static Tracing s_instance;
        #endregion Private Members

        #region Singleton Instance
        /// <summary>
        /// Private constructor.
        /// </summary>
        private Tracing()
        { }

        /// <summary>
        /// Internal Singleton Instance getter.
        /// </summary>
        internal static Tracing Instance
        {
            get
            {
                if (s_instance == null)
                {
                    lock (m_syncRoot)
                    {
                        if (s_instance == null)
                        {
                            s_instance = new Tracing();
                        }
                    }
                }
                return s_instance;
            }
        }
        #endregion Singleton Instance

        #region Public Events
        /// <summary>
        /// Occurs when a trace call is made.
        /// </summary>
        public event EventHandler<TraceEventArgs> TraceEventHandler;
        #endregion Public Events

        internal void RaiseTraceEvent(TraceEventArgs eventArgs)
        {
            if (TraceEventHandler != null)
            {
                try
                {
                    TraceEventHandler(this, eventArgs);
                }
                catch (Exception ex)
                {
                    Utils.Trace(ex, "Exception invoking Trace Event Handler", true, null);
                }
            }
        }
    }
    #endregion Tracing Class

    #region TraceEventArgs Class
    /// <summary>
    /// The event arguments provided when a trace event is raised.
    /// </summary>
    public class TraceEventArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the TraceEventArgs class.
        /// </summary>
        /// <param name="traceMask">The trace mask.</param>
        /// <param name="format">The format.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="args">The arguments.</param>
        internal TraceEventArgs(int traceMask, string format, string message, Exception exception, object[] args)
        {
            TraceMask = traceMask;
            Format = format;
            Message = message;
            Exception = exception;
            Arguments = args;
        }
        #endregion Constructors

        #region Public Properties
        /// <summary>
        /// Gets the trace mask.
        /// </summary>
        public int TraceMask { get; private set; }

        /// <summary>
        /// Gets the format.
        /// </summary>
        public string Format { get; private set; }

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        public object[] Arguments { get; private set; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Gets the exception.
        /// </summary>
        public Exception Exception { get; private set; }
        #endregion Public Properties
    }
    #endregion TraceEventArgs Class

    /// <summary>
    /// Utility functions used by COM applications.
    /// </summary>
    public static class ConfigUtils
    {
        /// <summary>
        /// Gets the log file directory and ensures it is writeable.
        /// </summary>
        public static string GetLogFileDirectory()
        {
            // try the program data directory.
            string logFileDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            logFileDirectory += "\\Technosoftware\\Logs";

            try
            {
                // create the directory.
                if (!Directory.Exists(logFileDirectory))
                {
                    Directory.CreateDirectory(logFileDirectory);
                }
            }
            catch (Exception)
            {
                // try the MyDocuments directory instead.
                logFileDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                logFileDirectory += "Technosoftware\\Logs";

                if (!Directory.Exists(logFileDirectory))
                {
                    Directory.CreateDirectory(logFileDirectory);
                }
            }

            return logFileDirectory;
        }

        /// <summary>
        /// Enable the trace.
        /// </summary>
        /// <param name="path">The path to use.</param>
        /// <param name="filename">The filename.</param>
        public static void EnableTrace(string path, string filename)
        {
            Utils.SetTraceOutput(Utils.TraceOutput.FileOnly);
            Utils.SetTraceMask(Int32.MaxValue);

            string logFilePath = path + "\\" + filename;
            Utils.SetTraceLog(logFilePath, false);
            Utils.Trace("Log File Set to: {0}", logFilePath);
        }
    }
}
