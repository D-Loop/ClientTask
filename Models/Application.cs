using ElMessage;
using ElMessage.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;

namespace ClientTask.Models
{
    public class Application
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public double MemoryUsed { get; set; }
        public double CPUUsed { get; set; }
        public ulong ReadOperations { get; set; }
        public ulong WriteOperations { get; set; }
    }
}
