// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Framework.Runtime;
using Microsoft.Framework.Runtime.Caching;
using Microsoft.Framework.Runtime.Loader;

namespace Microsoft.Framework.DesignTimeHost
{
    internal class DesignTimeAssemblyLoadContextFactory : IAssemblyLoadContextFactory
    {
        private readonly Project _project;
        private readonly IApplicationEnvironment _appEnv;
        private readonly ICache _cache;
        private readonly ICacheContextAccessor _cacheContextAccessor;
        private readonly INamedCacheDependencyProvider _namedDependencyProvider;

        public DesignTimeAssemblyLoadContextFactory(Project project,
                                                    IApplicationEnvironment appEnv,
                                                    ICache cache,
                                                    ICacheContextAccessor cacheContextAccessor,
                                                    INamedCacheDependencyProvider namedDependencyProvider)
        {
            _project = project;
            _appEnv = appEnv;
            _cache = cache;
            _cacheContextAccessor = cacheContextAccessor;
            _namedDependencyProvider = namedDependencyProvider;
        }

        public IAssemblyLoadContext Create(IServiceProvider serviceProvider)
        {
            return GetRuntimeFactory(serviceProvider).Create(serviceProvider);
        }

        private IAssemblyLoadContextFactory GetRuntimeFactory(IServiceProvider serviceProvider)
        {
            var cacheKey = Tuple.Create("RuntimeLoadContextFactory", _project.Name, _appEnv.Configuration, _appEnv.RuntimeFramework);

            return _cache.Get<IAssemblyLoadContextFactory>(cacheKey, ctx =>
            {
                var applicationHostContext = new ApplicationHostContext(serviceProvider,
                                                                    _project.ProjectDirectory,
                                                                    packagesDirectory: null,
                                                                    configuration: _appEnv.Configuration,
                                                                    targetFramework: _appEnv.RuntimeFramework,
                                                                    cache: _cache,
                                                                    cacheContextAccessor: _cacheContextAccessor,
                                                                    namedCacheDependencyProvider: _namedDependencyProvider);

                applicationHostContext.DependencyWalker.Walk(_project.Name, _project.Version, _appEnv.RuntimeFramework);

                // Watch all projects for project.json changes
                foreach (var library in applicationHostContext.DependencyWalker.Libraries)
                {
                    if (string.Equals(library.Type, "Project"))
                    {
                        ctx.Monitor(new FileWriteTimeCacheDependency(library.Path));
                    }
                }

                // Add a cache dependency on restore complete to reevaluate dependencies
                ctx.Monitor(_namedDependencyProvider.GetNamedDependency(_project.Name + "_Dependencies"));

                return new AssemblyLoadContextFactory(applicationHostContext.ServiceProvider);
            });
        }
    }
}