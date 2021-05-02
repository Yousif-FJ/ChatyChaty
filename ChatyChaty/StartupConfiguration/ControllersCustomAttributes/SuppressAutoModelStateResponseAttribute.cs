using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatyChaty.StartupConfiguration.ControllersCustomAttributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SuppressAutoModelStateResponseAttribute : Attribute, IControllerModelConvention
    {
        private static readonly Type ModelStateInvalidFilterFactory = typeof(ModelStateInvalidFilter).Assembly.GetType("Microsoft.AspNetCore.Mvc.Infrastructure.ModelStateInvalidFilterFactory");

        public void Apply(ControllerModel controller)
        {
            foreach (var action in controller.Actions)
            {
                for (var i = 0; i < action.Filters.Count; i++)
                {
                    if (action.Filters[i] is ModelStateInvalidFilter || action.Filters[i].GetType() == ModelStateInvalidFilterFactory)
                    {
                        action.Filters.RemoveAt(i);
                        break;
                    }
                }
            }
        }
    }
}
