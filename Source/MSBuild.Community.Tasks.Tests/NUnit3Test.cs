

using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Build.Utilities;
using Microsoft.Win32;
using NUnit.Framework;

namespace MSBuild.Community.Tasks.Tests
{
    /// <summary>
    /// Summary description for NUnitTest
    /// </summary>
    [TestFixture]
    public class NUnit3Test
    {
        private string _nunitPath;

        [Test]
        public void DummyTest()
        {
            Assert.Pass("This is dummy test that is used to test NUnit3 task.");
        }

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {            
            _nunitPath = @"C:\Users\krecik\Downloads\NUnit-3.0.1\bin";// Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), NUnit3.DEFAULT_NUNIT_DIRECTORY);
        }

        [Test(Description = "Excute NUnit3 tests of the NUnit framework")]
        public void NUnitExecute()
        {
            #region Find NUnit installation

            if (!Directory.Exists(_nunitPath))
            {
                Assert.Inconclusive("{0} - not found", _nunitPath);
            }
            
            #endregion Find NUnit installation

            MockBuild buildEngine = new MockBuild();

            string testDirectory = TaskUtility.makeTestDirectory(buildEngine);

            NUnit3 task = new NUnit3();
            task.ToolPath = _nunitPath;
            
            task.BuildEngine = buildEngine;
            task.Assemblies = TaskUtility.StringArrayToItemArray(Assembly.GetExecutingAssembly().Location);
            task.Where = "test == MSBuild.Community.Tasks.Tests.NUnit3Test.DummyTest";
            task.WorkingDirectory = testDirectory;
            task.OutputFile = Path.Combine(testDirectory, @"nunit.framework.tests-results.xml");
            Assert.IsTrue(task.Execute(), "Execute Failed");
        }

        [Test(Description = "Excute NUnit tests of the NUnit framework")]
        [TestCase(2, 6, 3)]
        [TestCase(3, 0, 0)]
        public void NUnitExecuteWhenToolPathIsDefined(int majorVersion, int minorVersion, int number)
        {
            if (!Directory.Exists(_nunitPath))
            {
                Assert.Inconclusive("{0} - not found", _nunitPath);
            }

            MockBuild buildEngine = new MockBuild();
            string testDirectory = TaskUtility.makeTestDirectory(buildEngine);

            NUnit3 task = new NUnit3();
            task.ToolPath = _nunitPath;
            task.BuildEngine = buildEngine;
            task.Where = "test == MSBuild.Community.Tasks.Tests.NUnit3Test.DummyTest";
            task.Assemblies = TaskUtility.StringArrayToItemArray(Path.Combine(_nunitPath, "nunit.framework.tests.dll"));
            task.WorkingDirectory = testDirectory;
            Assert.IsTrue(task.Execute(), "Execute Failed");
        }
    }
}
