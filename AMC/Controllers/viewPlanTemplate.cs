using System;
namespace TemplatePattern
{
    public abstract class viewPlanTemplate
    {
        public void viewPlan()
        {
            viewAllPlans();
            viewMyPlans();

        }

        protected abstract void viewAllPlans();
        protected abstract void viewMyPlans();
    }
}
