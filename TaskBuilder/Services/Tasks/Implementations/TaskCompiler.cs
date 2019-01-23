﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

using TaskBuilder.Functions;
using TaskBuilder.Models.Diagram;
using TaskBuilder.Models.Function.InputValue;
using TaskBuilder.Services.Functions;
using TaskBuilder.Services.Inputs;
using TaskBuilder.Tasks;

namespace TaskBuilder.Services.Tasks
{
    internal class TaskCompiler : ITaskCompiler
    {
        private readonly IFunctionTypeService _functionDiscoveryService;
        private readonly IInputValueService _inputValueService;

        public TaskCompiler(IFunctionTypeService functionDiscoveryService, IInputValueService inputValueService)
        {
            _functionDiscoveryService = functionDiscoveryService;
            _inputValueService = inputValueService;
        }

        public Task PrepareTask(Diagram diagram)
        {
            var startFunctionDisplayName = "Start";

            var invokables = new Dictionary<Guid, IInvokable>(diagram.Nodes.Count);
            var dispatchers = new Dictionary<Guid, IDispatcher>();

            var linkedPorts = new Dictionary<Guid, string>();
            var openInputPorts = new Dictionary<Guid, string>();
            var fieldsModels = new Dictionary<Guid, IInputValueModel>();
            var portFunctionIdentifiers = new Dictionary<Guid, string>();
            var portFunctionGuids = new Dictionary<Guid, Guid>();

            IInvokable startInvokable = null;

            var types = diagram.Nodes.ToDictionary(n => n.Id, node =>
            {
                var type = _functionDiscoveryService.GetFunctionType(node.Function.TypeIdentifier);

                var function = FormatterServices.GetUninitializedObject(type);

                invokables.Add(node.Id, function as IInvokable);

                if (function is IDispatcher)
                {
                    dispatchers.Add(node.Id, function as IDispatcher);
                }

                foreach (var port in node.Ports)
                {
                    if (port.Linked)
                    {
                        linkedPorts.Add(port.Id, port.Name);
                        continue;
                    }

                    if (port.Type.Equals(FunctionHelper.INPUT, StringComparison.OrdinalIgnoreCase))
                    {
                        openInputPorts.Add(port.Id, port.Name);

                        var fieldsModel = node.Function
                            .Inputs
                            .FirstOrDefault(i => i.Name == port.Name)
                            .FilledModel;

                        fieldsModels.Add(port.Id, fieldsModel);
                        portFunctionIdentifiers.Add(port.Id, node.Function.TypeIdentifier);
                        portFunctionGuids.Add(port.Id, node.Id);
                    }
                }

                // Find the start function and save it
                if (node.Function.DisplayName == startFunctionDisplayName)
                {
                    startInvokable = function as IInvokable;
                }

                return type;
            });

            foreach (var link in diagram.Links)
            {
                IDispatcher source = dispatchers[link.Source];
                IInvokable target = invokables[link.Target];
                string sourcePort = linkedPorts[link.SourcePort];
                string targetPort = linkedPorts[link.TargetPort];

                switch (link.Type)
                {
                    case "caller":
                        source.Dispatch = target.Invoke;
                        break;

                    case "parameter":

                        Type sourceType = types[link.Source];
                        Type targetType = types[link.Target];

                        targetType.GetProperty(targetPort).SetValue(
                            target,
                            sourceType.GetProperty(sourcePort).GetMethod.CreateDelegate(
                                targetType.GetProperty(targetPort).PropertyType,
                                source)
                        );
                        break;
                }
            }

            if (openInputPorts.Any())
            {
                foreach (var openPort in openInputPorts)
                {
                    IInputValueModel fields;
                    object value;

                    Type parentType = types[portFunctionGuids[openPort.Key]];
                    IInvokable parent = invokables[portFunctionGuids[openPort.Key]];

                    if (fieldsModels.TryGetValue(openPort.Key, out fields))
                    {
                        value = _inputValueService.BuildValue(portFunctionIdentifiers[openPort.Key], openPort.Value, fields);
                    }
                    else
                    {
                        value = null;
                    }

                    parentType.GetProperty(openPort.Value).SetValue(
                        parent,
                        Expression.Lambda(
                            Expression.Convert(
                                Expression.Constant(value),
                                parentType.GetProperty(openPort.Value).PropertyType.GenericTypeArguments[0]
                            )
                        ).Compile()
                    );
                }
            }

            return new Task(invokables, startInvokable);
        }
    }
}