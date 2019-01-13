﻿using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.WebHost;
using System.Web.Routing;
using System.Web.SessionState;

using CMS;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TaskBuilder.Functions;
using TaskBuilder.Models.Diagram;
using TaskBuilder.Services;
using TaskBuilder.Tasks;

using RequestContext = System.Web.Routing.RequestContext;

[assembly: RegisterModule(typeof(TaskBuilder.TaskBuilder))]

namespace TaskBuilder
{
    internal class TaskBuilder : Module
    {
        public TaskBuilder() : base(nameof(TaskBuilder))
        {
        }

        protected override void OnInit()
        {
            base.OnInit();

            InitializeFunctions();

            TaskInfo.TYPEINFO.Events.Insert.Before += HandleImportTask;
            FunctionInfo.TYPEINFO.Events.Insert.Before += EnsureUniqueClass;

            var reactConfig = TaskBuilderHelper.Environment.Configuration;

            reactConfig.JsonSerializerSettings
                .ContractResolver = new CamelCasePropertyNamesContractResolver();

            reactConfig.JsonSerializerSettings
                .Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter(true));

            // Map route directly to RouteTable to enable session access
            RouteTable.Routes
                .MapHttpRoute("taskbuilder", "taskbuilder/{controller}/{action}")
                .RouteHandler = new SessionRouteHandler();
        }

        private void EnsureUniqueClass(object sender, ObjectEventArgs e)
        {
            var function = e.Object as FunctionInfo;
            if (function != null)
            {
                var existingFunction = FunctionInfoProvider
                                        .GetFunctions()
                                        .WhereEquals("FunctionClass", function.FunctionClass)
                                        .TopN(1)
                                        .FirstOrDefault();

                if (existingFunction != null)
                {
                    throw new InfoObjectException(function, ResHelper.GetString("taskbuilder.validation.functionalreadyexists"));
                }
            }
        }

        private void HandleImportTask(object sender, ObjectEventArgs e)
        {
            var task = e.Object as TaskInfo;

            if (task != null && !string.IsNullOrEmpty(task.TaskGraph))
            {
                var taskDiagram = JsonConvert.DeserializeObject<Diagram>(task.TaskGraph);

                taskDiagram.Id = task.TaskGuid;
                task.TaskGraph = taskDiagram.ToJSON();
            }
        }

        private void InitializeFunctions()
        {
            var initializer = new FunctionInitializer(Service.Resolve<IFunctionModelService>());
            initializer.RunAsync();
        }

        public class SessionRouteHandler : IRouteHandler
        {
            public IHttpHandler GetHttpHandler(RequestContext requestContext)
            {
                return new SessionControllerHandler(requestContext.RouteData);
            }
        }

        public class SessionControllerHandler : HttpControllerHandler, IRequiresSessionState
        {
            public SessionControllerHandler(RouteData routeData)
                : base(routeData)
            { }
        }
    }
}