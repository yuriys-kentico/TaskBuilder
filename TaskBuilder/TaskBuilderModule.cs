﻿using System.Web.Http;
using CMS;
using CMS.DataEngine;

using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.Msie;

using React;

[assembly: RegisterModule(typeof(TaskBuilder.TaskBuilderModule))]

namespace TaskBuilder
{
    internal class TaskBuilderModule : Module
    {
        public const string NAME = "Task builder";

        public TaskBuilderModule()
            : base(NAME)
        {
        }

        protected override void OnInit()
        {
            base.OnInit();

            ReactSiteConfiguration.Configuration
                .SetReuseJavaScriptEngines(true)
                .AddScript("~/CMSScripts/CMSModules/TaskBuilder/Components/Demo-Serialize.jsx")
                ;

            JsEngineSwitcher.Instance.EngineFactories
                .AddMsie();

            JsEngineSwitcher.Instance.DefaultEngineName = MsieJsEngine.EngineName;

            GlobalConfiguration.Configuration.Routes.MapHttpRoute("taskbuilder", "taskbuilder/{controller}/{id}", new { id = RouteParameter.Optional });
        }
    }
}