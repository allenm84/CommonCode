using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;

namespace System.Common.References
{
  /// <summary>
  /// 
  /// </summary>
  public class RouteTable
  {
    #region AddRoute

    /// <summary>
    /// Adds the specified route to the routine table
    /// </summary>
    /// <param name="remoteNetworkAddr"></param>
    /// <param name="remoteNetworkMask"></param>
    /// <param name="remoteNetworkGateway"></param>
    /// <exception cref="ArgumentException">Thrown if arguments are not valid Internet address strings</exception>
    /// <returns>0 if success, or error code if failed.</returns>
    public static int AddRoute(string remoteNetworkAddr, string remoteNetworkMask, string remoteNetworkGateway)
    {

      int result;

      IPAddress addr, mask, gate;
      try { addr = IPAddress.Parse(remoteNetworkAddr); }
      catch (Exception ex) { throw new ArgumentException("remoteNetworkAddr", ex); }
      try { mask = IPAddress.Parse(remoteNetworkMask); }
      catch (Exception ex) { throw new ArgumentException("remoteNetworkMask", ex); }
      try { gate = IPAddress.Parse(remoteNetworkGateway); }
      catch (Exception ex) { throw new ArgumentException("remoteNetworkGateway", ex); }

      addr = ApplyMask(addr, mask); // make sure zero bits of the mask are zero in the address.

      // figure out which adapter out of the list is the best to use
      uint bestAdapterIndex = 0;
      result = NativeMethods.GetBestInterface(IPAddressAsUint(gate), out bestAdapterIndex);
      if (result != 0)
        return result;

      // Load the existing route table
      NativeMethods.IpForwardRow[] rows = NativeMethods.GetIpForwardTable();
      if (rows == null)
        return (int)NativeMethods.IPErrorCodes.ERROR_FAILURE; // should never happen

      // Make sure the address isn't already covered in the route table
      foreach (NativeMethods.IpForwardRow row in rows)
      {
        if ((row.dwForwardMask != 0) && ((IPAddressAsUint(addr) & row.dwForwardMask) == row.dwForwardDest))
        {
          return (int)NativeMethods.IPErrorCodes.ERROR_ALREADY_EXISTS;
        }
      }

      // for some versions of Windows, the value of Metric1 must be greater than or equal to
      // the value of the adapter metric.  Unfortunately, it is somewhat difficult to get this value.
      // We will work around this by duplicating the metric of any other route that uses this adapter,
      // or, if none exist, use the maximum value of any route in the table.  If the table is empty, we
      // will use a fixed value of 255, and hope it works ok.
      uint dwMetric = 0;
      foreach (NativeMethods.IpForwardRow row in rows)
      {
        uint rowIndex = row.dwForwardIfIndex;
        uint rowMetric = row.dwForwardMetric1;
        if (rowIndex == bestAdapterIndex)
        {
          dwMetric = rowMetric;
          break;
        }
        else if (rowMetric > dwMetric)
        {
          dwMetric = rowMetric;
        }
      }
      // if we didn't find anything relevant, use 255 - this should be OK
      dwMetric = (dwMetric == 0) ? 255 : dwMetric;


      // Everything looks good - create the route.
      NativeMethods.IpForwardRow newRoute = new NativeMethods.IpForwardRow();
      newRoute.dwForwardDest = IPAddressAsUint(addr);
      newRoute.dwForwardMask = IPAddressAsUint(mask);
      newRoute.dwForwardPolicy = 0; // policy - always ;
      newRoute.dwForwardNextHop = IPAddressAsUint(gate);
      newRoute.dwForwardIfIndex = bestAdapterIndex;
      newRoute.ForwardType = NativeMethods.IpForwardType.MIB_IPROUTE_TYPE_INDIRECT;
      newRoute.ForwardProto = NativeMethods.IpForwardProto.MIB_IPPROTO_NETMGMT;
      newRoute.dwForwardAge = 0;
      newRoute.dwForwardNextHopAS = 0;
      newRoute.dwForwardMetric1 = dwMetric;
      newRoute.dwForwardMetric2 = 0;
      newRoute.dwForwardMetric3 = 0;
      newRoute.dwForwardMetric4 = 0;
      newRoute.dwForwardMetric5 = 0;

      return NativeMethods.CreateIpForwardEntry(ref newRoute);

    }

    #endregion

    #region DeleteRoute
    /// <summary>
    /// Removes an entry from the route forwarding table.
    /// </summary>
    /// <param name="remoteNetworkAddr">Address of the route to delete</param>
    /// <param name="remoteNetworkMask">Mask of the route to delete</param>
    /// <returns>0 if successful, or error code if not.</returns>
    public static int DeleteRoute(string remoteNetworkAddr, string remoteNetworkMask)
    {
      IPAddress addr, mask;
      try { addr = IPAddress.Parse(remoteNetworkAddr); }
      catch (Exception ex) { throw new ArgumentException("remoteNetworkAddr", ex); }
      try { mask = IPAddress.Parse(remoteNetworkMask); }
      catch (Exception ex) { throw new ArgumentException("remoteNetworkMask", ex); }

      addr = ApplyMask(addr, mask); // make sure zero bits of the mask are zero in the address.

      // Load the existing route table
      NativeMethods.IpForwardRow[] rows = NativeMethods.GetIpForwardTable();
      if (rows == null)
        return (int)NativeMethods.IPErrorCodes.ERROR_FAILURE; // should never happen

      // Find the address in the route table
      foreach (NativeMethods.IpForwardRow row in rows)
      {
        if ((IPAddressAsUint(addr) == row.dwForwardDest) && (IPAddressAsUint(mask) == row.dwForwardMask))
        {
          // found, so delete it.
          NativeMethods.IpForwardRow foundRow = row;
          return NativeMethods.DeleteIpForwardEntry(ref foundRow);
        }
      }

      // if we got this far, the specified route was not in the table.
      return (int)(NativeMethods.IPErrorCodes.ERROR_NOT_FOUND);

    }

    #endregion

    #region private Methods

    /// <summary>
    /// Returns the addr with the mask applied.
    /// </summary>
    /// <param name="addr"></param>
    /// <param name="mask"></param>
    /// <returns>Masked Address</returns>
    private static IPAddress ApplyMask(IPAddress addr, IPAddress mask)
    {
      byte[] addrBytes = addr.GetAddressBytes();
      byte[] maskBytes = mask.GetAddressBytes();
      int nBytes = Math.Min(addrBytes.Length, maskBytes.Length);
      for (int i = 0; i < nBytes; i++)
      {
        addrBytes[i] &= maskBytes[i];
      }
      return new IPAddress(addrBytes);
    }

    /// <summary>
    /// Returns the first 4 bytes of the address as a uint
    /// </summary>
    /// <param name="addr"></param>
    /// <returns>uint</returns>
    /// <remarks>
    /// This replaces the depricated IPAddress.Address property, but
    /// will only work on IPV4 addresses.
    /// </remarks>
    private static uint IPAddressAsUint(IPAddress addr)
    {
      uint result = 0;
      byte[] addrBytes = addr.GetAddressBytes();
      for (int i = 0; i < sizeof(uint); i++)
      {
        result |= ((uint)(addrBytes[i])) << 8 * i;
      }
      return result;
    }

    #endregion
  }

  #region NativeMethods

  /// <summary>
  /// Native functions to get the network route tables. There are not corresponding DotNet framework versions.
  /// </summary>
  internal class NativeMethods
  {
    // This DLL name is the same on CE and PC
    const string IphlpapiDllName = "Iphlpapi.dll";

    #region enums and structures

    /// <summary>
    /// Constants used in the program as defined in IPExport.h and WinError.h.
    /// </summary>
    public enum IPErrorCodes : int
    {
      ERROR_FAILURE = -1,
      ERROR_SUCCESS = 0,
      ERROR_BUFFER_OVERFLOW = 111,
      ERROR_INSUFFICIENT_BUFFER = 122,
      ERROR_ALREADY_EXISTS = 183,
      ERROR_NOT_FOUND = 1168
    }

    /// <summary>
    /// The route type as described in RFC 1354.
    /// </summary>
    internal enum IpForwardType
    {
      /// <summary>Some other type not specified in RFC 1354.</summary>
      MIB_IPROUTE_TYPE_OTHER = 1,
      /// <summary>An invalid route. This value can result from a route added by an ICMP redirect.</summary>
      MIB_IPROUTE_TYPE_INVALID = 2,
      /// <summary>A local route where the next hop is the final destination (a local interface).</summary>
      MIB_IPROUTE_TYPE_DIRECT = 3,
      /// <summary>The remote route where the next hop is not the final destination (a remote destination).</summary>
      MIB_IPROUTE_TYPE_INDIRECT = 4,
    }

    /// <summary>
    /// The protocol or routing mechanism that generated the route as described in RFC 1354.
    /// </summary>
    internal enum IpForwardProto
    {
      /// <summary>Some other protocol not specified in RFC 1354.</summary>
      MIB_IPPROTO_OTHER = 1,
      /// <summary>A local interface.</summary>
      MIB_IPPROTO_LOCAL = 2,
      /// <summary>A static route.</summary>
      MIB_IPPROTO_NETMGMT = 3,
      /// <summary>The result of ICMP redirect.</summary>
      MIB_IPPROTO_ICMP = 4,
      /// <summary>The Exterior Gateway Protocol (EGP), a dynamic routing protocol.</summary>
      MIB_IPPROTO_EGP = 5,
      /// <summary>The Gateway-to-Gateway Protocol (GGP), a dynamic routing protocol.</summary>
      MIB_IPPROTO_GGP = 6,
      /// <summary>The Hellospeak protocol, a dynamic routing protocol.</summary>
      MIB_IPPROTO_HELLO = 7,
      /// <summary>The Berkeley Routing Information Protocol (RIP) or RIP-II, a dynamic routing protocol.</summary>
      MIB_IPPROTO_RIP = 8,
      /// <summary>The Intermediate System-to-Intermediate System (IS-IS) protocol, a dynamic routing protocol.</summary>
      MIB_IPPROTO_IS_IS = 9,
      /// <summary>The End System-to-Intermediate System (ES-IS) protocol, a dynamic routing protocol.</summary>
      MIB_IPPROTO_ES_IS = 10,
      /// <summary>The Cisco Interior Gateway Routing Protocol (IGRP), a dynamic routing protocol.</summary>
      MIB_IPPROTO_CISCO = 11,
      /// <summary>The Bolt, Beranek, and Newman (BBN) Interior Gateway Protocol (IGP) that used the Shortest Path First (SPF) algorithm. This was an early dynamic routing protocol.</summary>
      MIB_IPPROTO_BBN = 12,
      /// <summary>The Open Shortest Path First (OSPF) protocol, a dynamic routing protocol.</summary>
      MIB_IPPROTO_OSPF = 13,
      /// <summary>The Border Gateway Protocol (BGP), a dynamic routing protocol.</summary>
      MIB_IPPROTO_BGP = 14,
      /// <summary>A Windows specific entry added originally by a routing protocol, but which is now static.</summary>
      MIB_IPPROTO_NT_AUTOSTATIC = 10002,
      /// <summary>A Windows specific entry added as a static route from the routing user interface or a routing command.</summary>
      MIB_IPPROTO_NT_STATIC = 10006,
      /// <summary>A Windows specific entry added as an static route from the routing user interface or a routing command, except these routes do not cause Dial On Demand (DOD).</summary>
      MIB_IPPROTO_NT_STATIC_NON_DOD = 10007,
    }

    /// <summary>
    /// Describes one row of an IP Address table as returned by <see cref="GetIpForwardTable"/>.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct IpForwardRow
    {
      /// <summary>The destination IPv4 address of the route..</summary>
      public uint dwForwardDest;
      /// <summary>The IPv4 subnet mask.</summary>
      public uint dwForwardMask;
      /// <summary>Should always be zero.</summary>
      public uint dwForwardPolicy;
      /// <summary> The IPv4 address of the next system en route.</summary>
      public uint dwForwardNextHop;
      /// <summary>The index of the local interface through which the next hop of this route should be reached.</summary>
      public uint dwForwardIfIndex;
      /// <summary>The route type as described in RFC 1354.</summary>
      public IpForwardType ForwardType;
      /// <summary>The protocol or routing mechanism that generated the route as described in RFC 1354.</summary>
      public IpForwardProto ForwardProto;
      /// <summary>The number of seconds since the route was added or modified in the network routing table.</summary>
      public uint dwForwardAge;
      /// <summary>The autonomous system number of the next hop. (Set to zero.)</summary>
      public uint dwForwardNextHopAS;
      /// <summary>Metric used to calculate best route</summary>
      public uint dwForwardMetric1;
      /// <summary>Unused</summary>
      public uint dwForwardMetric2;
      /// <summary>Unused</summary>
      public uint dwForwardMetric3;
      /// <summary>Unused</summary>
      public uint dwForwardMetric4;
      /// <summary>Unused</summary>
      public uint dwForwardMetric5;
    }

    #endregion

    #region CreateIpForwardEntry

    // The declaration of this function is the same on CE and PC
    [DllImport(IphlpapiDllName, CharSet = CharSet.Auto, EntryPoint = "CreateIpForwardEntry")]
    internal static extern int CreateIpForwardEntry(ref IpForwardRow ipForwardRow);

    #endregion

    #region DeleteIpForwardEntry

    // The declaration of this function is the same on CE and PC
    [DllImport(IphlpapiDllName, CharSet = CharSet.Auto, EntryPoint = "DeleteIpForwardEntry")]
    internal static extern int DeleteIpForwardEntry(ref IpForwardRow ipForwardRow);

    #endregion

    #region GetBestInterface
    // The declaration of this function is the same on CE and PC
    [DllImport(IphlpapiDllName, CharSet = CharSet.Auto, EntryPoint = "GetBestInterface")]
    internal static extern int GetBestInterface(uint ipAddr, out uint bestIndex);

    #endregion

    #region GetIpForwardTable

    // The declaration of this function is the same on CE and PC
    [DllImport(IphlpapiDllName, CharSet = CharSet.Auto, EntryPoint = "GetIpForwardTable")]
    private static extern int _GetIpForwardTable(IntPtr table, ref uint size, bool sort);

    /// <summary>
    /// Retrieves the interface to IP address mapping table.
    /// </summary>
    /// <returns>A table that describes the mapping between IP addresses and adapters.</returns>
    internal static IpForwardRow[] GetIpForwardTable()
    {
      IpForwardRow[] rows = null;

      // figure out how big of a buffer we need
      uint size = 0;
      int ret = _GetIpForwardTable(IntPtr.Zero, ref size, true);

      // Check to see if the call returned a valid result
      if (ret != (int)IPErrorCodes.ERROR_SUCCESS &&
        ret != (int)IPErrorCodes.ERROR_INSUFFICIENT_BUFFER)
      {
        Debug.Assert(false, @"Error calling GetIpForwardTable(): " + ret);
        return null;
      }

      // allocate the ip address table buffer
      IntPtr buf = Marshal.AllocHGlobal((int)size);

      try
      {
        // Get the ip address table
        ret = _GetIpForwardTable(buf, ref size, true);

        // Check to see if the call returned a valid result
        if (ret != (int)IPErrorCodes.ERROR_SUCCESS)
        {
          Debug.Assert(false, @"Error calling GetIpForwardTable(): " + ret);
          return null;
        }

        // Copy the data into a managed array of IpAddressRow structures
        int numEntries = Marshal.ReadInt32(buf);
        int pRows = 4 + (int)buf;
        rows = new IpForwardRow[(int)numEntries];
        for (int i = 0; i < numEntries; i++)
        {
          rows[i] = (IpForwardRow)Marshal.PtrToStructure((IntPtr)pRows, typeof(IpForwardRow));
          pRows += Marshal.SizeOf(typeof(IpForwardRow));
        }
      }
      finally
      {
        // free the buffer
        Marshal.FreeHGlobal(buf);
      }

      return rows;
    }

    #endregion

  }

  #endregion
}