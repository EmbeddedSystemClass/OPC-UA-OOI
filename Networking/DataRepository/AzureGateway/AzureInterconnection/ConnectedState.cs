﻿//___________________________________________________________________________________
//
//  Copyright (C) 2020, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community at GITTER: https://gitter.im/mpostol/OPC-UA-OOI
//___________________________________________________________________________________

using Microsoft.Azure.Devices.Provisioning.Client;
using System;
using System.Threading.Tasks;

namespace UAOOI.Networking.DataRepository.AzureGateway.AzureInterconnection
{
  internal class ConnectedState : CommunicationContext.StateBase
  {
    public ConnectedState(CommunicationContext communicationContext) : base(communicationContext)
    {
    }

    #region StateBase

    public override ProvisioningRegistrationStatusType GetProvisioningRegistrationStatusType => ProvisioningRegistrationStatusType.Assigned;

    public override Task<bool> Connect()
    {
      throw new ApplicationException($"The operation {nameof(Connect)} is not allowed in the {nameof(ConnectedState)}");
    }

    public override Task<RegisterResult> Register()
    {
      throw new ApplicationException($"The operation {nameof(Register)} is not allowed in the {nameof(ConnectedState)}");
    }

    public override void TransferData(IDTOProvider dataProvider, string repositoryGroup)
    {
      TransitionTo(new DataTransferingState(dataProvider, repositoryGroup, _paretContext));
    }

    public override void DisconnectRequest()
    {
      DeviceClient.CloseAsync().Wait();
      TransitionTo(new AssigneddState(_paretContext));
    }

    #endregion StateBase
  }
}