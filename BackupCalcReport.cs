using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

// TODO: Replace the following version attributes by creating AssemblyInfo.cs. You can do this in the properties of the Visual Studio project.
[assembly: AssemblyVersion("1.0.0.1")]
[assembly: AssemblyFileVersion("1.0.0.1")]
[assembly: AssemblyInformationalVersion("1.0")]

// TODO: Uncomment the following line if the script requires write access.
// [assembly: ESAPIScript(IsWriteable = true)]

namespace VMS.TPS
{
  public class Script
  {
    public Script()
    {
    }

    [MethodImpl(MethodImplOptions.NoInlining)]

        public void Execute(ScriptContext scriptContext, Window mainWindow)
        {
            Run(scriptContext.CurrentUser,
                scriptContext.Patient,
                scriptContext.Image,
                scriptContext.StructureSet,
                scriptContext.PlanSetup,
                scriptContext.PlansInScope,
                scriptContext.PlanSumsInScope,
                mainWindow);
        }

        public void Run(
            User user,
            Patient patient,
            Image image,
            StructureSet structureSet,
            PlanSetup planSetup,
            IEnumerable<PlanSetup> planSetupsInScope,
            IEnumerable<PlanSum> planSumsInScope,
            Window mainWindow)
        {
            // Your main code now goes here

            // grab the active plansetup.

            //if (planSetup == null)
            //{
            //    throw new ApplicationException("Please load a plan.");
            //}
            //if (planSetup == null && planSumsInScope.Count() == 1)
            //{
            //    throw new ApplicationException("Please load a plan, not a plan sum.");
            //}


            //if (patient == null || structureSet == null)
            //{
            //   throw new ApplicationException("Please load a patient and plan before running this script.");
            //}
            //if (!planSetup.IsDoseValid())
            //{
            //    throw new ApplicationException("Please calculate dose for the active plan before running this script.");
            //}


            // Retrieve list of plans displayed in Scope Window
            
            if (planSetupsInScope.Count() == 0)
            {
                MessageBox.Show("Scope Window does not contain any plans.");
                return;
            }

            // Retrieve plan names
            List<string> planIDs = new List<string>();
            foreach (var ps in planSetupsInScope)
            {
                planIDs.Add(ps.Id);
            }

            // Construct output message
            string message = string.Format("Number of plans in Scope Window is {0} ({1}). \n\nWhen more than one plan are present in Scope Window a short comparison follows.",
              planSetupsInScope.Count(),
              string.Join(",", planIDs));

            // If more than one plan, create a short comparison list
            if (planSetupsInScope.Count() > 1)
            {
                // Display some additional plan information
                PlanSetup plan1 = planSetupsInScope.ElementAt(0);
                PlanSetup plan2 = planSetupsInScope.ElementAt(1);

                if (plan1.StructureSet != null && plan2.StructureSet != null)
                {
                    Image image1 = plan1.StructureSet.Image;
                    Image image2 = plan2.StructureSet.Image;
                    var structures1 = plan1.StructureSet.Structures;
                    var structures2 = plan2.StructureSet.Structures;
                    var beams1 = plan1.Beams;
                    var beams2 = plan2.Beams;
                    //Fractionation fractionation1 = plan1.UniqueFractionation;
                    //Fractionation fractionation2 = plan2.UniqueFractionation;
                    message += string.Format("\n* Image IDs for the first two plans: {0}, {1}", image1.Id, image2.Id);
                    message += string.Format("\n* Number of structures defined in plans: {0} and {1} accordingly.", structures1.Count(), structures2.Count());
                    message += string.Format("\n* Number of Beams: {0} and {1}.", beams1.Count(), beams2.Count());
                    //message += string.Format("\n* Number of Fractions: {0} and {1}.", fractionation1.NumberOfFractions, fractionation2.NumberOfFractions);
                }
            }
            MessageBox.Show(message);





            string WORKBOOK_RESULT_DIR = System.IO.Path.GetTempPath();



            string[] myPlanInfo = new string[]
                    {
                        "Name (ID): " + patient.Name.ToString(),
                        "Plan ID: "  + planSetup.Id.ToString(),
                        "Printed :" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")
                    };
            string HtmlBody = ExportToHtml(myPlanInfo);


            string outputpath = System.IO.Path.Combine(WORKBOOK_RESULT_DIR, patient.Id.ToString() + "-" + planSetup.Id.ToString() + "-" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + ".html");
            System.IO.File.WriteAllText(outputpath, HtmlBody);

            //open the output file for viewing
            System.Diagnostics.Process.Start(outputpath);
            System.Threading.Thread.Sleep(3000);




        }


        
        protected string ExportToHtml(string[] PatientAndPlan)
        {
            StringBuilder strHTMLBuilder = new StringBuilder();
            strHTMLBuilder.Append("<html >");
            strHTMLBuilder.Append("<head>");
            strHTMLBuilder.Append("</head>");
            strHTMLBuilder.Append("<body style='font-family:arial; font-size:medium'>");
            foreach (string plndata in PatientAndPlan)
            {
                strHTMLBuilder.Append("<br>");
                strHTMLBuilder.Append(plndata);
            }
            strHTMLBuilder.Append("<table border='1px' cellpadding='1' cellspacing='0' bgcolor='#F8F8F8' ; style='border:dotted 1px Silver; font-family:arial; font-size:small'>");
            strHTMLBuilder.Append("<tr align='left' valign='top'>");

            strHTMLBuilder.Append("<tr >");


            //Close tags.  
            strHTMLBuilder.Append("</table>");
            strHTMLBuilder.Append("</body>");
            strHTMLBuilder.Append("</html>");

            string Htmltext = strHTMLBuilder.ToString();

            return Htmltext;

        }





    

  }
}
