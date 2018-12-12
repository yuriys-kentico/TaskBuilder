﻿using System;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.Helpers;
using CMS.UIControls;

using TaskBuilder;
using TaskBuilder.Services;

[Title("taskbuilder.ui.edittask")]
[UIElement("TaskBuilder", "TaskBuilder")]
[EditedObject(TaskInfo.OBJECT_TYPE, "objectid")]
public partial class TaskBuilder_EditTask : CMSPage
{
    private readonly IFunctionModelService _functionModelService = Service.Resolve<IFunctionModelService>();

    protected void Page_Init()
    {
        ScriptHelper.RegisterScriptFile(this, "CMSModules/TaskBuilder/Vendor/lodash.min.js", false);
        ScriptHelper.RegisterScriptFile(this, "CMSModules/TaskBuilder/Vendor/react.development.js", false);
        ScriptHelper.RegisterScriptFile(this, "CMSModules/TaskBuilder/Vendor/react-dom.development.js", false);
        ScriptHelper.RegisterScriptFile(this, "CMSModules/TaskBuilder/Vendor/main.js", false);

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "TaskBuilder",
            DiagramFactory.Environment.Babel.TransformFile("~/CMSScripts/CMSModules/TaskBuilder/Components/TaskDiagramArea.jsx") +
            DiagramFactory.Environment.Babel.TransformFile("~/CMSScripts/CMSModules/TaskBuilder/Components/TaskDiagram.jsx") +
            DiagramFactory.Environment.Babel.TransformFile("~/CMSScripts/CMSModules/TaskBuilder/Components/BaseNodeFactory.jsx") +
            DiagramFactory.Environment.Babel.TransformFile("~/CMSScripts/CMSModules/TaskBuilder/Components/BaseNodeModel.jsx"), true);

        CssRegistration.RegisterCssLink(this, "~/CMSModules/TaskBuilder/Stylesheets/style.min.css");
        CssRegistration.RegisterCssLink(this, "~/CMSModules/TaskBuilder/Stylesheets/TaskBuilder.css");
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        // Get props from IFunctionModelService for all available functions
        // Get task diagram from database (via EditedObject)
        // Pass both as props (functions array, diagram string)

        var reactComponent = DiagramFactory.GetReactComponent("TaskDiagramArea");
        network.Text = reactComponent.RenderHtml(true);

        initScript.Text = ScriptHelper.GetScript(DiagramFactory.Environment.GetInitJavaScript());

        if (!RequestHelper.IsAsyncPostback())
        {
            ScriptHelper.HideVerticalTabs(this);
        }
    }
}