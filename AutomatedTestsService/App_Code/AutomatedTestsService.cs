using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service" in code, svc and config file together.
public class AutomatedTestsService : IAutomatedTestsService
{
    public string RunInFireFox()
    {
        AutomatedTestsLib.SeleniumTestDriver seleniumTestDriver = new AutomatedTestsLib.SeleniumTestDriver(AutomatedTestsLib.SeleniumTestDriver.DriverType.FireFox);
        return seleniumTestDriver.RunTestCase();
    }

    public string RunInGoogleChrome()
    {
        AutomatedTestsLib.SeleniumTestDriver seleniumTestDriver = new AutomatedTestsLib.SeleniumTestDriver(AutomatedTestsLib.SeleniumTestDriver.DriverType.GoogleChrome);
        return seleniumTestDriver.RunTestCase();
    }

    public string RunInIE()
    {
        AutomatedTestsLib.SeleniumTestDriver seleniumTestDriver = new AutomatedTestsLib.SeleniumTestDriver(AutomatedTestsLib.SeleniumTestDriver.DriverType.IE);
        return seleniumTestDriver.RunTestCase();
    }
}
