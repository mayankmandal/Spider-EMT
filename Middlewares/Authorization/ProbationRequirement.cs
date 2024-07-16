using Microsoft.AspNetCore.Authorization;

namespace Spider_EMT.Middlewares.Authorization
{
    public class ProbationRequirement: IAuthorizationRequirement
    {
        public ProbationRequirement(int probationMonths)
        {
            ProbationMonths = probationMonths;
        }

        public int ProbationMonths { get; }
    }
    public class ProbingRequirementHandler : AuthorizationHandler<ProbationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProbationRequirement requirement)
        {
            if(!context.User.HasClaim(x => x.Type == "EmployeeJoiningDate"))
                return Task.CompletedTask;
            var empDOJ = DateTime.Parse(context.User.FindFirst(x => x.Type == "EmployeeJoiningDate").Value);
            var period = DateTime.Now - empDOJ;
            if (period.Days > 30 * requirement.ProbationMonths)
                context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
 