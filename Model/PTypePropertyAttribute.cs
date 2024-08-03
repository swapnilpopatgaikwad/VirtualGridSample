namespace VirtualGridSample.Model
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PTypePropertyAttribute : Attribute
    {
        public PTypePropertyType PType { get; set; }
    }
}
