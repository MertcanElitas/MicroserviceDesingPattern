using System;
using System.Reflection.Metadata.Ecma335;

namespace EventSourceUI.Dtos
{
    public class ChangeProductNameDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}