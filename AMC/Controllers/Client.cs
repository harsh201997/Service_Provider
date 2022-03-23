using System;
namespace TemplatePattern
{
    public class Client
    {

        static void Main(string[] args)
        {
            //Calling customerViewPlans
            viewPlanTemplate plans = new customerViewPlans();
            plans.viewPlan();

            //Calling serviceProviderViewPlans
            plans = new serviceProviderViewPlans();
            plans.viewPlan();
        }
    }
}
