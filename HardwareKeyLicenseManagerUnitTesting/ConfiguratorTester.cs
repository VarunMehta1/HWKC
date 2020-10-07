using HardwareKeyCreationTool;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace HardwareConfiguratorUnitTesting
{
    [TestClass]
    public class ConfiguratorTester
    {
        public HardwareKeyConfigurator configurator { get; set; }
        public ConfiguratorTester()
        {
            configurator = new HardwareKeyConfigurator();
        }
        [TestMethod]
        public void TestWriteLicenseFile()
        {
            string path = @"C:\Users\1288954\Desktop\lservrc";
            string[] licenses = File.ReadAllLines(path);
            try
            {
                configurator.DeleteHiddenMemory();
                configurator.CreateHiddenMemory(1000);
                configurator.SaveInHiddenMemory(licenses);
            }
            catch (HardwareConfiguratorException e)
            {
                Assert.IsTrue(false);
            }
        }
        [TestMethod]
        public void TestCreateHiddenMemory()
        {
            try
            {
                configurator.DeleteHiddenMemory();
                configurator.CreateHiddenMemory(1000);
            }
            catch (HardwareConfiguratorException e)
            {
                Assert.IsTrue(false);
            }
        }
        [TestMethod]
        public void TestCreateDeleteHiddenMemory()
        {
            try
            {
                configurator.DeleteHiddenMemory();
            }
            catch (HardwareConfiguratorException e)
            {
                Assert.IsTrue(false);
            }
        }
    }
}
