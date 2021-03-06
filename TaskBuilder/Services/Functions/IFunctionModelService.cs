﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CMS;
using CMS.Base;
using CMS.DataEngine;
using TaskBuilder.Models.Function;
using TaskBuilder.Services.Functions;

[assembly: RegisterImplementation(typeof(IFunctionModelService), typeof(FunctionModelService), Priority = CMS.Core.RegistrationPriority.Fallback)]

namespace TaskBuilder.Services.Functions
{
    public interface IFunctionModelService
    {
        IEnumerable<IFunctionModel> AllFunctionModels { get; }

        /// <summary>
        /// Finds all of the function types, gets their ports and creates a model in the cache.
        /// When a task builder is opened, the React app pulls in the models for deserialization and creating new ones in the side drawer.
        /// </summary>
        Task<IEnumerable<IFunctionModel>> RegisterFunctionModels();

        /// <summary>
        /// Get all <see cref="IFunctionModel"/>s authorized for given user and site.
        /// </summary>
        IEnumerable<Guid> AuthorizedFunctionGuids(IUserInfo user, SiteInfoIdentifier siteIdentifier);
    }
}