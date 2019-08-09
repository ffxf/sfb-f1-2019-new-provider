using SimFeedback.telemetry;

namespace F12019New
{
    public class F12019NewTelemetryValue : AbstractTelemetryValue
    {
        public F12019NewTelemetryValue(string name, object value) : base()
        {
            Name = name;
            Value = value;
        }
        public override object Value { get; set; }
    }
}