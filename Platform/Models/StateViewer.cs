using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform.Models
{
    public class StateViewer : IEquatable<StateViewer>
    {
        public Converters.StateConverter.StateType Type { get; set; }

        public bool Equals(StateViewer other)
        {
            return this.Type == other.Type;
        }
    }
}
