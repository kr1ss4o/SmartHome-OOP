using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome
{
    class SmartDevice : IComparer<SmartDevice>
    {
        public string Name { get; set; }
        public double EnergyConsumption { get; set; }
        public void TurnOn()
        {
            Console.WriteLine($"{Name} is now ON.");
        }
        public void TurnOff()
        {
            Console.WriteLine($"{Name} is now OFF.");
        }

        public int Compare(SmartDevice x, SmartDevice y)
        {
            if (x == null || y == null)
                return 0;
            else
            {
                return this.EnergyConsumption.CompareTo(y.EnergyConsumption);
            }
        }
        public int CompareName(SmartDevice x, SmartDevice y)
        {
            if (x == null || y == null)
                return 0;
            else
            {
                return this.Name.CompareTo(y.Name);
            }
        }
    }
    class SmartLight : SmartDevice
    {
        public int Brightness { get; set; }
        public SmartLight()
        {
            this.Brightness = 0; // Default brightness
        }
        public void SetBrightness(int brightness)
        {
            Brightness = brightness;
            Console.WriteLine($"{Name} brightness set to {Brightness}.");
        }
    }
    class SmartThermostat : SmartDevice
    {
        public double Temperature { get; set; }

        public SmartThermostat()
        {
            this.Temperature = 20.0; // Default temperature
        }
        public void SetTemperature(double temperature)
        {
            Temperature = temperature;
            Console.WriteLine($"{Name} temperature set to {Temperature}°C.");
        }
    }
    class SmartHome : IEnumerable<SmartDevice>
    {
        private List<SmartDevice> HomeDevices = new List<SmartDevice>();
        
        public void AddDevice(SmartDevice device)
        {
            HomeDevices.Add(device);
        }

        public string DisplayFields(string inputClass, params string[] inputFields)
        {
            Type classType = Type.GetType(inputClass);
            
            FieldInfo[] classFields = classType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            StringBuilder stringBuilder = new StringBuilder();
            
            Object classInstance = Activator.CreateInstance(classType, new object[] { });
            stringBuilder.AppendLine($"Class: {classType.Name}");

            foreach (FieldInfo field in classFields.Where(f => inputFields.Contains(f.Name)))
            {
                stringBuilder.AppendLine($"{field.Name} = {field.GetValue(classInstance)}");
            }

            return stringBuilder.ToString().Trim();
        }   

        // Enumerator fields and class
        public IEnumerator<SmartDevice> GetEnumerator()
        {
            return new SmartHomeEnumerator(HomeDevices);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class SmartHomeEnumerator : IEnumerator<SmartDevice>
        {
            private List<SmartDevice> _devices;
            private int _position = -1;
            public SmartHomeEnumerator(List<SmartDevice> devices)
            {
                _devices = devices;
            }
            public SmartDevice Current
            {
                get
                {
                    if (_position < 0 || _position >= _devices.Count)
                        throw new InvalidOperationException();
                    return _devices[_position];
                }
            }
            object IEnumerator.Current => Current;

            
            public bool MoveNext()
            {
                _position++;
                return (_position < _devices.Count);
            }
            public void Reset()
            {
                _position = -1;
            }
            public void Dispose()
            {
                // No resources to dispose
            }
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            // 1. Create objects of SmartLight and SmartThermostat
            SmartLight readingLight = new SmartLight() { Name = "Reading Light", EnergyConsumption = 5.5, Brightness = 70 };
            SmartThermostat livingRoomThermostat = new SmartThermostat() { Name = "Living Room Thermostat", EnergyConsumption = 12.3, Temperature = 22.5 };
            SmartLight kitchenLight = new SmartLight() { Name = "Kitchen Light", EnergyConsumption = 4.2, Brightness = 80 };

            // 2. Adding the devices to the SmartHome collection
            SmartHome home = new SmartHome();
            home.AddDevice(readingLight);
            home.AddDevice(livingRoomThermostat);
            home.AddDevice(kitchenLight);

            // 3. Using the iteratos to display all devices
            Console.WriteLine("All devices in the smart home:");
            foreach (SmartDevice device in home)
            {
                Console.WriteLine($"Device: {device.Name}, Energy: {device.EnergyConsumption}");
            }

            // 4. Using comparers to sort the devices
            Console.WriteLine("\nDevices sorted by name:");
            var devicesByName = home.OrderBy(d => d.Name).ToList();
            foreach (var device in devicesByName)
            {
                Console.WriteLine($"Device: {device.Name}, Energy: {device.EnergyConsumption}");
            }

            // Sort by energy consumption
            Console.WriteLine("\nDevices sorted by energy consumption:");
            var devicesByEnergy = home.OrderBy(d => d.EnergyConsumption).ToList();
            foreach (var device in devicesByEnergy)
            {
                Console.WriteLine($"Device: {device.Name}, Energy: {device.EnergyConsumption}");
            }

            // 5. Using reflection to display fields and their names
            Console.WriteLine("\nReflection info for SmartLight:");
            Console.WriteLine(home.DisplayFields("SmartHome.SmartLight", "Brightness"));

            Console.WriteLine("\nReflection info for SmartThermostat:");
            Console.WriteLine(home.DisplayFields("SmartHome.SmartThermostat", "Temperature"));
        }
    }
}
