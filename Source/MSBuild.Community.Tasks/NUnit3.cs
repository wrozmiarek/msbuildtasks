#region Copyright � 2005 Paul Welter. All rights reserved.
/*
Copyright � 2005 Paul Welter. All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions
are met:

1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright
   notice, this list of conditions and the following disclaimer in the
   documentation and/or other materials provided with the distribution.
3. The name of the author may not be used to endorse or promote products
   derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE AUTHOR "AS IS" AND ANY EXPRESS OR
IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
*/
#endregion

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Win32;



namespace MSBuild.Community.Tasks
{
    /// <summary>
    /// Run NUnit 3.0 on a group of assemblies.
    /// </summary>
    /// <example>Run NUnit tests.
    /// <code><![CDATA[
    /// <ItemGroup>
    ///     <TestAssembly Include="C:\Program Files\NUnit 2.4\bin\*.tests.dll" />
    /// </ItemGroup>
    /// <Target Name="NUnit">
    ///     <NUnit3 Assemblies="@(TestAssembly)" />
    /// </Target>
    /// ]]></code>
    /// </example>
    public class NUnit3 : ToolTask
    {
        #region Constants

        /// <summary>
        /// The default relative path of the NUnit installation.
        /// The value is <c>@"NUnit 2.4\bin"</c>.
        /// </summary>
        public const string DEFAULT_NUNIT_DIRECTORY = @"NUnit.org\bin";        

        #endregion Constants

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:NUnit"/> class.
        /// </summary>
        public NUnit3()
        {
        }

        #endregion Constructor

        #region Properties
        private ITaskItem[] _assemblies;

        /// <summary>
        /// Gets or sets the assemblies.
        /// </summary>
        /// <value>The assemblies.</value>
        [Required]
        public ITaskItem[] Assemblies
        {
            get { return _assemblies; }
            set { _assemblies = value; }
        }

        private string _testNames;

        /// <summary>
        /// Gets or sets the categories to include.
        /// </summary>
        /// <remarks>Multiple values must be separated by a comma ","</remarks>
        public string TestNames
        {
            get { return _testNames; }
            set { _testNames = value; }
        }

        private string _testList;

        /// <summary>
        /// Gets or sets the categories to exclude.
        /// </summary>
        /// <remarks>Multiple values must be separated by a comma ","</remarks>
        public string TestList
        {
            get { return _testList; }
            set { _testList = value; }
        }

        private string _where;

        /// <summary>
        /// Gets or sets the fixture.
        /// </summary>
        /// <value>The fixture.</value>
        public string Where
        {
            get { return _where; }
            set { _where = value; }
        }

        private string _config;

        /// <summary>
        /// Gets or sets the XSLT transform file.
        /// </summary>
        /// <value>The XSLT transform file.</value>
        public string Config
        {
            get { return _config; }
            set { _config = value; }
        }

        private string _process;

        /// <summary>
        /// Gets or sets the output XML file.
        /// </summary>
        /// <value>The output XML file.</value>
        public string Process
        {
            get { return _process; }
            set { _process = value; }
        }

        private bool _inProcess;

        /// <summary>
        /// The file to receive test error details.
        /// </summary>
        public bool InProcess
        {
            get { return _inProcess; }
            set { _inProcess = value; }
        }


        private string _agents;

        /// <summary>
        /// The file to redirect standard output to.
        /// </summary>
        public string Agents
        {
            get { return _agents; }
            set { _agents = value; }
        }


        private string _workingDirectory;

        /// <summary>
        /// Gets or sets the working directory.
        /// </summary>
        /// <value>The working directory.</value>
        /// <returns>
        /// The directory in which to run the executable file, or a null reference (Nothing in Visual Basic) if the executable file should be run in the current directory.
        /// </returns>
        public string WorkingDirectory
        {
            get { return _workingDirectory; }
            set { _workingDirectory = value; }
        }

        private string _domain;

        /// <summary>
        /// Determines whether assemblies are copied to a shadow folder during testing.
        /// </summary>
        /// <remarks>Shadow copying is enabled by default. If you want to test the assemblies "in place",
        /// you must set this property to <c>True</c>.</remarks>
        public string Domain
        {
            get { return _domain; }
            set { _domain = value; }
        }

        private string _framework;

        /// <summary>
        /// Determines the framework to run aganist.
        /// </summary>
        public string Framework
        {
            get { return _framework; }
            set { _framework = value; }
        }

        private bool _force32Bit;

        /// <summary>
        /// Determines whether the tests are run in a 32bit process on a 64bit OS.
        /// </summary>
        public bool Force32Bit
        {
            get { return _force32Bit; }
            set { _force32Bit = value; }
        }

        private bool _disposeRunners;

        /// <summary>
        /// Determines whether each test runner is disposed after it has finished running its tests
        /// </summary>
        public bool DisposeRunners
        {
            get { return _disposeRunners; }
            set { _disposeRunners = value; }
        }

        private string _timeout;

        /// <summary>
        /// Set timeout for each test case in milliseconds.
        /// </summary>
        public string Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        private string _seed;

        /// <summary>
        /// Set the random seed used to generate test cases.
        /// </summary>
        public string Seed
        {
            get { return _seed; }
            set { _seed = value; }
        }

        private string _workers;

        /// <summary>
        /// Specify the number of worker threads to be used in running tests.
        /// </summary>
        public string Workers
        {
            get { return _workers; }
            set { _workers = value; }
        }

        private bool _stopOnError;

        /// <summary>
        /// Determines whether the tests are run in a 32bit process on a 64bit OS.
        /// </summary>
        public bool StopOnError
        {
            get { return _stopOnError; }
            set { _stopOnError = value; }
        }

        private bool _debug;

        /// <summary>
        /// Causes NUnit to break into the debugger immediately before it executes your tests. This is particularly useful when the tests are running in a separate process to which you would otherwise have to attach.
        /// </summary>
        public bool Debug
        {
            get { return _debug; }
            set { _debug = value; }
        }

        private string _outputDirectory;

        /// <summary>
        /// Path of the directory to use for output files.
        /// </summary>
        public string OutputDirectory
        {
            get { return _outputDirectory; }
            set { _outputDirectory = value; }
        }

        private string _errorOutputPath;

        /// <summary>
        /// File path to contain error output from the tests.
        /// </summary>
        public string ErrorOutputPath
        {
            get { return _errorOutputPath; }
            set { _errorOutputPath = value; }
        }

        private string _outputPath;

        /// <summary>
        /// File path to contain text output from the tests.
        /// </summary>
        public string OutputPath
        {
            get { return _outputPath; }
            set { _outputPath = value; }
        }

        private string _resultSpec;

        /// <summary>
        /// An output spec for saving the test results.
        /// </summary>
        public string ResultSpec
        {
            get { return _resultSpec; }
            set { _resultSpec = value; }
        }

        private string _exploreSpec;

        /// <summary>
        /// Display or save test info rather than running tests. Optionally provide an output SPEC for saving the test info.
        /// </summary>
        public string ExploreSpec
        {
            get { return _exploreSpec; }
            set { _exploreSpec = value; }
        }

        private bool _noResult;

        /// <summary>
        /// Don't save any test results.
        /// </summary>
        public bool NoResult
        {
            get { return _noResult; }
            set { _noResult = value; }
        }

        
        private string _labels;

        /// <summary>
        /// Specify whether to write test case names to the output. Values: Off, On, All
        /// </summary>
        public string Labels
        {
            get { return _labels; }
            set { _labels = value; }
        }

        private string _trace;

        /// <summary>
        /// Set internal trace LEVEL. Values: Off, Error, Warning, Info, Verbose (Debug)
        /// </summary>
        public string Trace
        {
            get { return _trace; }
            set { _trace = value; }
        }

        private bool _shadowCopy;

        /// <summary>
        /// Tells .NET to copy loaded assemblies to the shadowcopy directory.
        /// </summary>
        public bool ShadowCopy
        {
            get { return _shadowCopy; }
            set { _shadowCopy = value; }
        }

        private bool _teamCity;

        /// <summary>
        /// Turns on use of TeamCity service messages.
        /// </summary>
        public bool TeamCity
        {
            get { return _teamCity; }
            set { _teamCity = value; }
        }

        private bool _noHeader;

        /// <summary>
        /// Suppress display of program information at start of run.
        /// </summary>
        public bool NoHeader
        {
            get { return _noHeader; }
            set { _noHeader = value; }
        }

        private bool _verbose;

        /// <summary>
        /// Display additional information as the test runs.
        /// </summary>
        public bool Verbose
        {
            get { return _verbose; }
            set { _verbose = value; }
        }

        #endregion

        #region Task Overrides
        /// <summary>
        /// Returns a string value containing the command line arguments to pass directly to the executable file.
        /// </summary>
        /// <returns>
        /// A string value containing the command line arguments to pass directly to the executable file.
        /// </returns>
        protected override string GenerateCommandLineCommands()
        {
            CommandLineBuilder builder = new CommandLineBuilder();

            const string switchPrefix = "--";

            builder.AppendFileNamesIfNotNull(_assemblies, " ");

            builder.AppendSwitchIfNotNull(switchPrefix + "test", _testNames);

            builder.AppendSwitchIfNotNull(switchPrefix + "testlist", _testList);

            builder.AppendSwitchIfNotNull(switchPrefix + "where", _where);

            builder.AppendSwitchIfNotNull(switchPrefix + "config", _config);

            builder.AppendSwitchIfNotNull(switchPrefix + "process", _process);

            builder.AppendSwitchIfNotNull(switchPrefix + "agents", _agents);

            builder.AppendSwitchIfNotNull(switchPrefix + "domain", _domain);

            builder.AppendSwitchIfNotNull(switchPrefix + "framework", _framework);

            builder.AppendSwitchIfNotNull(switchPrefix + "timeout", _timeout);

            builder.AppendSwitchIfNotNull(switchPrefix + "seed", _seed);

            builder.AppendSwitchIfNotNull(switchPrefix + "workers", _workers);

            builder.AppendSwitchIfNotNull(switchPrefix + "work", _outputDirectory);

            builder.AppendSwitchIfNotNull(switchPrefix + "output", _outputPath);

            builder.AppendSwitchIfNotNull(switchPrefix + "err", _errorOutputPath);

            builder.AppendSwitchIfNotNull(switchPrefix + "result", _resultSpec);

            builder.AppendSwitchIfNotNull(switchPrefix + "explore", _exploreSpec);

            builder.AppendSwitchIfNotNull(switchPrefix + "labels", _labels);

            builder.AppendSwitchIfNotNull(switchPrefix + "trace", _trace);

            if (_force32Bit)
            {
                builder.AppendSwitch(switchPrefix + "x86");
            }

            if (_disposeRunners)
            {
                builder.AppendSwitch(switchPrefix + "dispose-runners");
            }

            if (_stopOnError)
            {
                builder.AppendSwitch(switchPrefix + "stoponerror");
            }

            if (_debug)
            {
                builder.AppendSwitch(switchPrefix + "debug");
            }

            if (_noResult)
            {
                builder.AppendSwitch(switchPrefix + "noresult");
            }

            if (_shadowCopy)
            {
                builder.AppendSwitch(switchPrefix + "shadowcopy");
            }

            if (_teamCity)
            {
                builder.AppendSwitch(switchPrefix + "teamcity");
            }

            if (_noHeader)
            {
                builder.AppendSwitch(switchPrefix + "noheader");
            }

            if (_verbose)
            {
                builder.AppendSwitch(switchPrefix + "verbose");
            }

            return builder.ToString();
        }

        private void CheckToolPath()
        {
            string nunitPath = ToolPath == null ? String.Empty : ToolPath.Trim();
            if (!String.IsNullOrEmpty(nunitPath))
            {
                ToolPath = nunitPath;
                return;
            }
            
            nunitPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), DEFAULT_NUNIT_DIRECTORY);          

            ToolPath = nunitPath;

        }

        /// <summary>
        /// Returns the fully qualified path to the executable file.
        /// </summary>
        /// <returns>
        /// The fully qualified path to the executable file.
        /// </returns>
        protected override string GenerateFullPathToTool()
        {
            CheckToolPath();
            return Path.Combine(ToolPath, ToolName);
        }

        /// <summary>
        /// Logs the starting point of the run to all registered loggers.
        /// </summary>
        /// <param name="message">A descriptive message to provide loggers, usually the command line and switches.</param>
        protected override void LogToolCommand(string message)
        {
            Log.LogCommandLine(MessageImportance.Low, message);
        }

        /// <summary>
        /// Gets the name of the executable file to run.
        /// </summary>
        /// <value></value>
        /// <returns>The name of the executable file to run.</returns>
        protected override string ToolName
        {
            get
            {
                string toolName = @"nunit3-console";
                return ToolPathUtil.MakeToolName(toolName);
            }
        }

        /// <summary>
        /// Gets the <see cref="T:Microsoft.Build.Framework.MessageImportance"></see> with which to log errors.
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref="T:Microsoft.Build.Framework.MessageImportance"></see> with which to log errors.</returns>
        protected override MessageImportance StandardOutputLoggingImportance
        {
            get
            {
                return MessageImportance.Normal;
            }
        }

        /// <summary>
        /// Returns the directory in which to run the executable file.
        /// </summary>
        /// <returns>
        /// The directory in which to run the executable file, or a null reference (Nothing in Visual Basic) if the executable file should be run in the current directory.
        /// </returns>
        protected override string GetWorkingDirectory()
        {
            return string.IsNullOrEmpty(_workingDirectory) ? base.GetWorkingDirectory() : _workingDirectory;
        }

        #endregion Task Overrides

    }
}
