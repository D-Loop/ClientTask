using ElMessage;
using ElMessage.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;

namespace ClientTask.Models
{
    public class ProcessData
    {
        public string Name {  get; set; }
        public int Id {  get; set; }
        public long MemoryUsed{  get; set; }
        public string Description { get; set; }
        public TimeSpan ProcessTime {  get; set; }
        public ulong WorkingSetSize { get; set; }
    }
}
