﻿//___________________________________________________________________________________
//
//  Copyright (C) 2018, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community at GITTER: https://gitter.im/mpostol/OPC-UA-OOI
//___________________________________________________________________________________

using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Reflection;
using UAOOI.Networking.DataRepository.DataLogger;
using UAOOI.Networking.Encoding;
using UAOOI.Networking.ReferenceApplication.Core.Diagnostic;
using UAOOI.Networking.SemanticData;

namespace UAOOI.Networking.ReferenceApplication.MEF
{
  internal static class DefaultServiceRegistrar
  {
    /// <summary>
    /// Registers the required services.
    /// </summary>
    /// <param name="additionalCatalog">The additional catalog to be considered for composition.</param>
    /// <returns>An instance of <see cref="AggregateCatalog"/>.</returns>
    public static AggregateCatalog RegisterServices(ComposablePartCatalog additionalCatalog)
    {
      List<ComposablePartCatalog> _catalog = GetDefaultComposablePartCatalog();
      if (additionalCatalog != null)
        _catalog.Add(additionalCatalog);
      return new AggregateCatalog(_catalog);
    }

    private static List<ComposablePartCatalog> GetDefaultComposablePartCatalog()
    {
      return new List<ComposablePartCatalog>(new ComposablePartCatalog[] { new AssemblyCatalog(Assembly.GetAssembly(typeof(AppBootstrapper))),
                                                                           new AssemblyCatalog(Assembly.GetAssembly(typeof(NetworkingEventSourceProvider))),
                                                                           new AssemblyCatalog(Assembly.GetAssembly(typeof(DataManagementSetup))),
                                                                           new AssemblyCatalog(Assembly.GetAssembly(typeof(EncodingFactoryBinarySimple))),
                                                                           new AssemblyCatalog(Assembly.GetAssembly(typeof(ConsumerCompositionSettings))),
                                                                          }
                                            );
    }
  }
}