using Microsoft.AspNetCore.Mvc;
using ComputerBuilderMvcApp.Models;
using System.Collections.Generic;

namespace ComputerBuilderMvcApp.ViewModels
{
    public class ComputerViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public List<Component> StandardComponents { get; set; } = new List<Component>();
        public List<Component> AvailableComponents { get; set; } = new List<Component>();
    }
}