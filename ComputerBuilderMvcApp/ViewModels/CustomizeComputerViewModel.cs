using Microsoft.AspNetCore.Mvc;
using ComputerBuilderMvcApp.Models;
using System.Collections.Generic;

namespace ComputerBuilderMvcApp.ViewModels
{
    public class CustomizeComputerViewModel
    {
        public int ComputerId { get; set; }
        public string ComputerName { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public List<Component> AvailableComponents { get; set; } = new List<Component>();
        public List<Component> SelectedComponents { get; set; } = new List<Component>();
    }
}