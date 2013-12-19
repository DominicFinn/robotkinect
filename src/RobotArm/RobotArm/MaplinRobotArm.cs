using System;
using System.Runtime.InteropServices;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace RobotArm
{
    class MaplinRobotArm
    {

        private UsbDevice RobotArm;
        // State properties
        // Ints set to -1 for reverse direction and 1 for forward direction of motor, 0 to stop
        public int GripperState;
        public int WristState;
        public int ElbowState;
        public int ShoulderState;
        public int BaseState;

        public bool LightState;
        // Concatenates property values into three-byte control packet
        private byte[] BuildControlPacket(int code)
        {
            byte val1 = 0;
            byte val2 = 0;
            byte val3 = 0;

            if (GripperState == -1)
                val1 += 1;
            if (GripperState == 1)
                val1 += 2;

            if (WristState == 1)
                val1 += 4;
            if (WristState == -1)
                val1 += 8;
            if (ElbowState == 1)
                val1 += 16;
            if (ElbowState == -1)
                val1 += 32;
            if (ShoulderState == 1)
                val1 += 64;
            if (ShoulderState == -1)
                val1 += 128;

            if (BaseState == -1)
                val2 += 1;
            if (BaseState == 1)
                val2 += 2;

            if (LightState)
                val3 = 1;

            return new byte[] {
            (byte) code,
            val2,
            val3
        };


        }

        public void ResetArm()
        {
            GripperState=0;
            WristState=0;
            ElbowState=0;
            ShoulderState=0;
            BaseState=0;
            LightState = false;
        }

        // Connect to the arm
        public bool Connect()
        {
            // Vendor ID/Product ID here
            UsbDeviceFinder USBFinder = new UsbDeviceFinder(0x1267, 0x0);

            // Try to open the device
            RobotArm = UsbDevice.OpenUsbDevice(USBFinder);

            // Did we connect OK?
            if ((RobotArm == null))
                return false;

            // If this is a "whole" usb device (libusb-win32, linux libusb)
            // it will have an IUsbDevice interface. If not (WinUSB) the 
            // variable will be null indicating this is an interface of a 
            // device.
            IUsbDevice wholeUsbDevice = RobotArm as IUsbDevice;
            if (!ReferenceEquals(wholeUsbDevice, null))
            {
                // This is a "whole" USB device. Before it can be used, 
                // the desired configuration and interface must be selected.

                // Select config #1
                wholeUsbDevice.SetConfiguration(1);

                // Claim interface #0.
                wholeUsbDevice.ClaimInterface(0);
            }

            // Connected and have interface to the arm
            return true;

        }

        // Call this function to send a control packet to the arm
        public void Update(int code)
        {
            var usbPacket = default(UsbSetupPacket);
            var data = BuildControlPacket(code);

            int transferred = 0;

            var ptr = Marshal.AllocHGlobal(3);
            // alloc 3 bytes
            Marshal.Copy(data, 0, ptr, 3);
            // copy control packet

            usbPacket = new UsbSetupPacket(Convert.ToByte(UsbRequestType.TypeVendor), 6, 0x100, 0, 0);
            RobotArm.ControlTransfer(ref usbPacket, ptr, data.Length, out transferred);
        }

        // Cleanup method - call this when you've finished 
        public void Cleanup()
        {
            if ((RobotArm == null)) return;
            if (!RobotArm.IsOpen) return;

            dynamic wholeUsbDevice = RobotArm as IUsbDevice;
            if (!ReferenceEquals(wholeUsbDevice, null))
            {
                // This is a "whole" USB device. Before it can be used, 
                // the desired configuration and interface must be selected.

                // Select config #1
                wholeUsbDevice.ReleaseInterface(0);
            }

            RobotArm.Close();
            UsbDevice.Exit();
        }
    }
}
