﻿//___________________________________________________________________________________
//
//  Copyright (C) 2019, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community at GITTER: https://gitter.im/mpostol/OPC-UA-OOI
//___________________________________________________________________________________

using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UAOOI.Networking.Core;
using UAOOI.Networking.UDPMessageHandler.Diagnostic;

namespace UAOOI.Networking.UDPMessageHandler
{
  /// <summary>
  /// Class BinaryUDPPackageWriter - custom implementation of the <see cref="BinaryEncoder"/> using UDP protocol.
  /// </summary>
  internal sealed class BinaryUDPPackageWriter : IBinaryDataTransferGraphSender
  {

    #region constructor
    internal BinaryUDPPackageWriter(string remoteHostName, int remotePort)
    {
      UDPMessageHandlerSemanticEventSource.Log.EnteringMethod(nameof(BinaryUDPPackageWriter), $"{nameof(BinaryUDPPackageWriter)}(RemoteHostName={remoteHostName},RemotePort={remotePort})");
      State = new MyState(this);
      m_RemoteHostName = remoteHostName;
      m_remotePort = remotePort;
    }
    #endregion

    #region BinaryMessageEncoder
    public IAssociationState State
    {
      get;
      set;
    }
    public void AttachToNetwork()
    {
      UDPMessageHandlerSemanticEventSource.Log.EnteringMethod(nameof(BinaryUDPPackageWriter), nameof(AttachToNetwork));
    }
    public void SendFrame(byte[] buffer)
    {
      lock (this)
      {
        UDPMessageHandlerSemanticEventSource.Log.EnteringMethod(nameof(BinaryUDPPackageWriter), $"{nameof(SendFrame)} buffer.Length = {buffer.Length}");
        UdpClient _UdpClient = m_UdpClient;
        if (_UdpClient == null)
          return;
        try
        {
          IPEndPoint _IPEndPoint = new IPEndPoint(m_IPAddresses, m_remotePort);
          UDPMessageHandlerSemanticEventSource.Log.SentMessageContent(_IPEndPoint, buffer.Length, buffer);
          _UdpClient.Send(buffer, buffer.Length, _IPEndPoint);
        }
        catch (Exception _e)
        {
          UDPMessageHandlerSemanticEventSource.Log.LogException(nameof(BinaryUDPPackageWriter), nameof(SendFrame), _e);
        }
      }
    }
    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    public void Dispose()
    {
      lock (this)
      {
        UDPMessageHandlerSemanticEventSource.Log.EnteringMethod(nameof(BinaryUDPPackageWriter), $"Entering {nameof(BinaryUDPPackageWriter)}.{nameof(Dispose)} method");
        if (m_UdpClient == null)
          return;
        UDPMessageHandlerSemanticEventSource.Log.EnteringMethod(nameof(BinaryUDPPackageWriter), nameof(m_UdpClient.Close));
        m_UdpClient.Close();
        m_UdpClient = null;
      }
    }
    #endregion

    #region private
    //types
    private class MyState : IAssociationState
    {

      #region IAssociationState
      /// <summary>
      /// Initializes a new instance of the <see cref="MyState"/> class.
      /// </summary>
      public MyState(BinaryUDPPackageWriter parent)
      {
        State = HandlerState.Disabled;
        m_Parent = parent;
      }
      /// <summary>
      /// Gets the current state <see cref="HandlerState" /> of the <see cref="Association" /> instance.
      /// </summary>
      /// <value>The state of <see cref="HandlerState" /> type.</value>
      public HandlerState State
      {
        get;
        private set;
      }
      /// <summary>
      /// This method is used to enable a configured <see cref="Association" /> object. If a normal operation is possible, the state changes into <see cref="HandlerState.Operational" /> state.
      /// In the case of an error situation, the state changes into <see cref="HandlerState.Error" />. The operation is rejected if the current <see cref="State" />  is not <see cref="HandlerState.Disabled" />.
      /// </summary>
      /// <exception cref="System.ArgumentException">Wrong state</exception>
      public void Enable()
      {
        if (State != HandlerState.Disabled)
          throw new ArgumentException("Wrong state");
        State = HandlerState.Operational;
        m_Parent.OnEnable();
      }
      /// <summary>
      /// This method is used to disable an already enabled <see cref="Association" /> object.
      /// This method call shall be rejected if the current State is <see cref="HandlerState.Disabled" /> or <see cref="HandlerState.NoConfiguration" />.
      /// </summary>
      /// <exception cref="System.ArgumentException">Wrong state</exception>
      public void Disable()
      {
        if (State != HandlerState.Operational)
          throw new ArgumentException("Wrong state");
        State = HandlerState.Disabled;
        m_Parent.Dispose();
      }
      #endregion

      private BinaryUDPPackageWriter m_Parent;

    }
    //vars
    private UdpClient m_UdpClient;
    private IPAddress m_IPAddresses;
    private readonly int m_remotePort = 4800;
    private readonly string m_RemoteHostName;
    //Methods
    private void OnEnable()
    {
      UDPMessageHandlerSemanticEventSource.Log.EnteringMethod(nameof(BinaryUDPPackageWriter), nameof(OnEnable));
      Debug.Assert(m_UdpClient == null);
      // Get DNS host information.
      IPAddress[] _remoteHostAddresses = Dns.GetHostAddresses(m_RemoteHostName);
      // Get the DNS IP addresses associated with the host.
      // Get first IPAddress in list returned by the DNS.
      m_IPAddresses = _remoteHostAddresses.Where<IPAddress>(x => x.AddressFamily == AddressFamily.InterNetwork).First<IPAddress>();
      Debug.Assert(m_IPAddresses != null);
      UDPMessageHandlerSemanticEventSource.Log.EnteringMethod(nameof(BinaryUDPPackageWriter), $"{nameof(UdpClient)} m_RemoteHostName: {m_RemoteHostName} Ip : {m_IPAddresses.ToString()}");
      m_UdpClient = new UdpClient();
    }

    #endregion

  }
}
