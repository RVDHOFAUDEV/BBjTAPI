/*
 * Wrapper of the tapi functionalities - interacting with TAPI drivers:
 * - Mark Smith JULMAR ATAPI .Net TAPI2.0 https://github.com/markjulmar/atapi.net
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JulMar.Atapi;

namespace BBjTapiClient.utils
{
    public class Tapi : IDisposable
    {
        public string adapter = "atapi";

        private TapiManager mgr;
        private TapiLine[] lineCollection;
        public TapiLine currentLine;
        private TapiAddress[] addressCollection;
        public TapiAddress currentAddress;
        private ITapiCall currentOutgoingCall;
        private ITapiCall currentIncomingCall;
        string callNumberAsSoonAsPossible = "";
        public string allLinesAndAddresses = "";

        public Tapi()
        {
            App.log(String.Format("using adapter {0}", adapter));
            mgr = new TapiManager("BBjTAPIClient.Net");
        }


        /* is called when main win is loaded */
        public void init()
        {
            if (mgr.Initialize())
            {
                App.Setup.Lines.Clear();
                App.Setup.Addresses.Clear();

                currentLine = null;
                lineCollection = mgr.Lines;

                currentAddress = null;
                addressCollection = null;

                currentOutgoingCall = null;
                currentIncomingCall = null;

                allLinesAndAddresses = "";

                App.log(String.Format("{0}x TSP (Telephony service provider) lines detected", lineCollection.Count()));
                foreach (TapiLine line in lineCollection)
                {
                    App.Setup.Lines.Add(line.Name);
                    line.Changed += Line_Changed;
                    line.NewCall += Line_NewCall;
                    line.Ringing += Line_Ringing;
                    allLinesAndAddresses += "|" + line.Name;
                    if (line.Addresses!=null)
                        if (line.Addresses.Count()>0)
                            foreach (TapiAddress adr in line.Addresses)
                            {
                                allLinesAndAddresses += "~" + adr.Address;
                            }
                }
                if (allLinesAndAddresses != "")
                    allLinesAndAddresses = allLinesAndAddresses.Substring(1);

                if (App.Setup.Line != "")
                    setCurrentLine(App.Setup.Line);
                if (App.Setup.Address != "")
                    setCurrentAddress(App.Setup.Address);

                App.isRefreshingTapiSession = true; // start session
            }
        }


        /* is called when the item in the Device combobox has been changed */
        public void setCurrentLine(string lineName)
        {
            if (lineCollection.Count() > 0)
            {
                TapiLine item = lineCollection.FirstOrDefault(i => i.Name == lineName);
                if (item!=null)
                {
                    currentLine = item;
                    App.log(String.Format("Selected TAPI line '{0}'", currentLine.Name));
                    addressCollection = (TapiAddress[])item.Addresses;
                    App.Setup.Addresses.Clear();
                    if (addressCollection.Count() == 1)
                    {
                        App.log(String.Format("Selected TAPI line includes one address {0}", addressCollection[0].Address));
                        App.Setup.Addresses.Clear();
                        App.Setup.Addresses.Add(addressCollection[0].Address); // refresh Addresses combobox content 
                        setCurrentAddress(addressCollection[0].Address);
                    }
                    else
                    {
                        App.log(String.Format("Selected TAPI line includes {0}x addresses", addressCollection.Count()));
                        App.Setup.Addresses.Clear();
                        foreach (var address in addressCollection)
                        {
                            App.Setup.Addresses.Add(address.Address); // refresh Addresses combobox content 
                        }
                    }
                    App.registry.write("Device", currentLine.Name);
                }
                else
                    App.log(String.Format("Unable to find line '{0}'", lineName));
            }
        }


        /* is called when the item in the Address combobox has been changed */
        public void setCurrentAddress(string addressName)
        {
            bool found = false;
            if (addressCollection.Count() > 0)
            {
                foreach (var item in addressCollection)
                {
                    if (item.Address == addressName)
                    {
                        currentAddress = item;
                        App.log(String.Format("Selected TAPI address '{0}'", currentAddress.Address));
                        App.registry.write("Address", currentAddress.Address);
                        found = true;
                        break;
                    }
                }
                if (!found)
                    App.log(String.Format("Unable to apply address '{0}'", addressName));
            }
        }


        /* rock'n roll */
        public void startSession()
        {
            App.log("TAPI - Start session");
            if (!App.Setup.IsTapiSessionActive)
            {
                if (currentLine != null)
                {
                    try
                    {
                        currentLine.Open(currentLine.Capabilities.MediaModes);
                        App.log(String.Format("Opened TAPI line (MediaModes)"));
                    }
                    catch (TapiException)
                    {
                        currentLine.Open(MediaModes.DataModem);
                        App.log(String.Format("Opened TAPI line (alternative DataModem)"));
                    }
                    finally
                    {
                        App.Setup.IsTapiSessionActive = true;
                        refreshStateFlags();
                    }
                }
            }
        }


        /* what starts ends */
        public void stopSession()
        {
            App.log("TAPI - Stop session");
            if (App.Setup.IsTapiSessionActive)
            {
                if (currentLine != null)
                {
                    if (currentLine.IsOpen)
                    {
                        currentLine.Close();
                        App.log(String.Format("TAPI line - closed"));
                        App.Setup.IsTapiSessionActive = false;
                        refreshStateFlags();
                    }
                }
            }

        }

        /* drop outgoing call */
        public void dropCall()
        {
            App.log("TAPI address - Dropping outgoing call");
            if (currentOutgoingCall != null)
            {
                if (currentOutgoingCall.Line != null)
                {
                    if (currentOutgoingCall.Line.IsOpen)
                    {
                        try
                        {
                            currentOutgoingCall.Drop();
                            App.log("TAPI address - Dropped outgoing call");
                        }
                        catch (Exception ex)
                        {
                            App.log("TAPI address - Unable to drop call. " + ex.Message);
                        }
                    }
                }
            }
        }

        /* make outgoing call */
        public void makeCall(string plainPhoneNumber)
        {
            App.log("TAPI address - Making outgoing call to '" + plainPhoneNumber + "'");
            if (currentAddress != null)
            {
                if (currentAddress.Address != "")
                {
                    if (!App.Setup.CanMakeCall)
                    {
                        dropCall();
                        callNumberAsSoonAsPossible = plainPhoneNumber;
                    }
                    else
                    {
                        try
                        {
                            currentOutgoingCall = currentAddress.MakeCall(plainPhoneNumber);
                            App.log(String.Format("TAPI address - Made call to '{0}'", plainPhoneNumber));
                        }
                        catch (Exception ex)
                        {
                            App.log(ex.Message);
                        }
                    }
                }
            }
        }


        /*
         * dummy so far 
         */
        private void Line_Ringing(object sender, RingEventArgs e)
        {
            App.log(String.Format("TAPI line - Ringing event - Count {0}", e.RingCount));
        }


        /*
         * Incoming call event handler
         */
        private void Line_NewCall(object sender, NewCallEventArgs e)
        {
            currentIncomingCall = e.Call;
            string callingPhoneNumber = currentIncomingCall.CallerId;
            App.log(String.Format("TAPI line - Incoming call event - Caller ID '{0}' - Called ID '{1} - Call {2} - Privilege {3}", currentIncomingCall.CallerId, currentIncomingCall.CalledId, e.Call, e.Privilege));
            App.network.incomingCall(callingPhoneNumber);
            e.Call.Line.Monitor(); // will stop the execution of this method here using a inner exception.
            currentIncomingCall.Line.Close();
            currentIncomingCall.Line.Dispose();
            currentIncomingCall.Dispose();
            currentIncomingCall = null;
        }


        /* tapi line changed event handler */
        private void Line_Changed(object sender, LineInfoChangeEventArgs e)
        {
            App.log("TAPI line - State changed event");
            if (sender == currentLine)
                refreshStateFlags();
        }


        /* cleanup the outgoing call object */
        private void disposeOutgoingCall()
        {
            if (currentOutgoingCall!=null)
            {
                //App.log("TAPI address - disposing outgoing call");
                currentOutgoingCall.Dispose();
                App.log("TAPI address - Disposed outgoing call");
                currentOutgoingCall = null;
            }
        }


        /* final cleanup */
        public void Dispose()
        {
            disposeOutgoingCall();
            if (mgr!=null)
            {
                if (currentLine!=null)
                {
                    currentLine.Close();
                    currentLine.Dispose();
                    App.log("TAPI line - Closed and disposed line");
                }
                mgr.Shutdown();
                App.log("TAPI manager - Shut down");
            }
        }


        /* refresh the flags on line changed event */
        private void refreshStateFlags()
        {
            /* on line level */
            App.Setup.IsTapiSessionConnected = currentLine.Status.Connected; // must have
                    /* extras optional */
                    //bool state;
                    //state = currentLine.Status.Locked;
                    //if (state)
                    //    App.log(String.Format("TAPI line locked"));
                    //state = currentLine.Status.MessageWaitingLampState;
                    //App.log(String.Format("TAPI line message waiting lamp state is {0}", state));
                    //state = currentLine.Status.InService;
                    //if (state)
                    //    App.log(String.Format("TAPI line is in service"));
                    //state = (currentLine.Capabilities.SupportsForwarding && currentLine.IsOpen);
                    //App.log(String.Format("TAPI line support forwarding state is {0}", state));
            /* on address level*/
            App.Setup.CanMakeCall=(currentAddress != null && currentAddress.Status.CanMakeCall);
            App.log(String.Format("TAPI address - Can make call state is '{0}'", App.Setup.CanMakeCall));
                    //state = (currentAddress != null && currentAddress.Status.CanPickupCall);
                    //App.log(String.Format("TAPI address can pickup call state is {0}", state));
                    //state = (currentAddress != null && currentAddress.Status.CanPickupCall);
                    //App.log(String.Format("TAPI address can pickup call state is {0}", state));
                    //state = (currentAddress != null && currentAddress.Status.CanUnparkCall);
                    //App.log(String.Format("TAPI address can unpark call state is {0}", state));
            if (callNumberAsSoonAsPossible != "" && App.Setup.CanMakeCall == true)
                makeCall(callNumberAsSoonAsPossible);
            callNumberAsSoonAsPossible = "";
        }
    }
}
