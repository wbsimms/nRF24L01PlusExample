#region License
// Copyright (C) 2015 by Wm. Barrett Simms (wbsimms)
// http://wbsimms.com
// 
// MIT Licence
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
// Updated version of original code from Jakub Bartkowiak (Gralin)
#endregion
using System;
using System.Collections;
using System.Text;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Touch;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;
using Gadgeteer.Modules.WBSimms;
using GHI.Pins;
using Microsoft.SPOT.Hardware;

namespace nRF24L01PlusExample
{
    public partial class Program
    {
        private NRF24L01Plus nSender = new NRF24L01Plus();
        private NRF24L01Plus nReceiver = new NRF24L01Plus();
        private string addressSender = "sbs";
        private string addressReceiver = "rbs";

        void ProgramStarted()
        {
            Debug.Print("Program Started");

            var socket1 = GT.Socket.GetSocket(1, true, null, null);
            var socket2 = GT.Socket.GetSocket(3, true, null, null);

            nSender.Initialize(socket1.SPIModule,G400.PB5,G400.PB0,G400.PA7);
            nSender.Configure(Encoding.UTF8.GetBytes(addressSender), 9);

            nReceiver.Initialize(socket2.SPIModule,G400.PB3,G400.PB1,G400.PC23);
            nReceiver.Configure(Encoding.UTF8.GetBytes(addressReceiver), 9);


            nSender.OnTransmitFailed += nSender_OnTransmitFailed;
            nSender.OnTransmitSuccess += nSender_OnTransmitSuccess;
            nReceiver.OnDataReceived += nReceiver_OnDataReceived;
            
            button.ButtonPressed += button_ButtonPressed;
            nSender.Enable();
            nReceiver.Enable();
        }

        void nReceiver_OnDataReceived(byte[] data)
        {
            Debug.Print("Received : "+new string(Encoding.UTF8.GetChars(data)));
        }

        void nSender_OnTransmitSuccess()
        {
            Debug.Print("Success");
        }

        void nSender_OnTransmitFailed()
        {
            Debug.Print("Failed");
        }

        void button_ButtonPressed(GTM.GHIElectronics.Button sender, GTM.GHIElectronics.Button.ButtonState state)
        {
            if (nSender.IsEnabled)
                nSender.SendTo(Encoding.UTF8.GetBytes(addressReceiver),Encoding.UTF8.GetBytes("blah"), Acknowledge.Yes);
        }
    }
}
